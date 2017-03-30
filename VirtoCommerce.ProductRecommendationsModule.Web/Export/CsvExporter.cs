using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using CsvHelper;
using CsvHelper.Configuration;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.PushNotifications;
using VirtoCommerce.Platform.Data.Common;
using VirtoCommerce.ProductRecommendationsModule.Web.Model;

namespace VirtoCommerce.ProductRecommendationsModule.Web.Export
{
    public class CsvExporter
    {
        private readonly IPushNotificationManager _pushNotifier;
        private readonly IBlobStorageProvider _blobStorageProvider;
        private readonly IBlobUrlResolver _blobUrlResolver;

        public CsvExporter(IPushNotificationManager pushNotifier,
            IBlobStorageProvider blobStorageProvider, IBlobUrlResolver blobUrlResolver)
        {
            _pushNotifier = pushNotifier;
            _blobStorageProvider = blobStorageProvider;
            _blobUrlResolver = blobUrlResolver;
        }

        [CLSCompliant(false)]
        protected void DoExport<TCsvClass, TClass>(string fileName, int chunkSize,
            string entitiesType, Func<ICollection<TCsvClass>> entityFactory, CsvClassMap<TClass> entityClassMap,
            ExportPushNotification notification)
        {
            Action<ExportImportProgressInfo> progressCallback = x =>
            {
                notification.Description = x.Description;
                notification.TotalCount = x.TotalCount;
                notification.ProcessedCount = x.ProcessedCount;
                notification.Errors = x.Errors;
                _pushNotifier.Upsert(notification);
            };
            
            var progressInfo = new ExportImportProgressInfo
            {
                Description = string.Format("Loading {0}...", entitiesType)
            };
            progressCallback(progressInfo);
            
            var updateProgress = new Action(() =>
            {
                progressInfo.Description = string.Format("{0} of {1} {2} processed", progressInfo.ProcessedCount, progressInfo.TotalCount, entitiesType);
                progressCallback(progressInfo);
            });

            var relativeUrl = "temp/" + fileName + ".zip";
            using (var blobStream = _blobStorageProvider.OpenWrite(relativeUrl))
            {
                try
                {
                    using (new Timer(state => updateProgress(), null, TimeSpan.Zero, TimeSpan.FromSeconds(1)))
                    {
                        using (var archive = new ZipArchive(blobStream, ZipArchiveMode.Create, true, new UTF8Encoding(false)))
                        {
                            var partIndex = 1;
                            using (var stream = new MemoryStream())
                            {
                                using (var streamWriter = new StreamWriter(stream, new UTF8Encoding(false), 1024, true) { AutoFlush = true })
                                {
                                    using (var csvWriter = new CsvWriter(streamWriter))
                                    {
                                        progressCallback(progressInfo);
                                        
                                        var entities = entityFactory().ToArray();

                                        csvWriter.Configuration.Delimiter = ",";
                                        csvWriter.Configuration.RegisterClassMap(entityClassMap);

                                        progressInfo.TotalCount = entities.Length;

                                        for (var index = 0; index < entities.Length; index++)
                                        {
                                            try
                                            {
                                                var previousSize = (int) stream.Length;
                                                csvWriter.WriteRecord(entities[index]);
                                                
                                                if (stream.Length > chunkSize)
                                                {
                                                    WriteEntry(archive, fileName, ref partIndex, x => x.Write(stream.GetBuffer(), 0, previousSize - 2));
                                                    stream.SetLength(0);
                                                    --index;
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                progressInfo.Errors.Add(ex.ToString());
                                                progressCallback(progressInfo);
                                            }

                                            progressInfo.ProcessedCount = index + 1;
                                        }
                                    }
                                }
                                
                                WriteEntry(archive, fileName, ref partIndex, stream.WriteTo, true);
                            }
                        }
                        updateProgress();
                    }
                    notification.DownloadUrl = _blobUrlResolver.GetAbsoluteUrl(relativeUrl);
                }
                catch (Exception ex)
                {
                    notification.Description = "Export failed";
                    notification.Errors.Add(ex.ExpandExceptionMessage());
                }
                finally
                {
                    notification.Description = "Export finished";
                    notification.Finished = DateTime.UtcNow;
                    _pushNotifier.Upsert(notification);
                }
            }
        }
        
        private void WriteEntry(ZipArchive archive, string fileName, ref int partIndex, Action<Stream> writeTo, bool last = false)
        {
            var entry = archive.CreateEntry(fileName + (last && partIndex == 1 ? "" : " - " + partIndex) + ".csv");
            using (var entryStream = entry.Open())
            {
                writeTo(entryStream);
            }
            ++partIndex;
        }
    }
}