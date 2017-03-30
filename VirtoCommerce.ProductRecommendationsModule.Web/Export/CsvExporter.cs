using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using CsvHelper;
using CsvHelper.Configuration;
using VirtoCommerce.Platform.Core.ExportImport;

namespace VirtoCommerce.ProductRecommendationsModule.Web.Export
{
    public class CsvExporter
    {
        [CLSCompliant(false)]
        protected void DoExport<TCsvClass, TClass>(Stream outStream, string fileName, int chunkSize,
            string entitiesType, Func<ICollection<TCsvClass>> entityFactory, CsvClassMap<TClass> entityClassMap,
            Action<ExportImportProgressInfo> progressCallback)
        {
            // Push notification about starting process
            var progressInfo = new ExportImportProgressInfo
            {
                Description = string.Format("Loading {0}...", entitiesType)
            };
            progressCallback(progressInfo);

            // Push notification about progress
            var updateProgress = new Action(() =>
            {
                progressInfo.Description = string.Format("{0} of {1} {2} processed", progressInfo.ProcessedCount, progressInfo.TotalCount, entitiesType);
                progressCallback(progressInfo);
            });

            // Use timer instead of pushing notification every x items, because latter case freeze UI (UI receive avalanche-like flood)
            using (new Timer(state => updateProgress(), null, TimeSpan.Zero, TimeSpan.FromSeconds(1)))
            {
                // https://connect.microsoft.com/VisualStudio/feedback/details/392318/argumentexception-text-is-wrong
                // ZipPackage has a bug (exception occurs when you try to create it with FileMode.Create, FileAccess.Write params),
                // which don't allow use it to directly write to write-only streams like Azure blob
                // Use newer .NET Framework class ZipArchive instead
                using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true, new UTF8Encoding(false)))
                {
                    // We use memory stream only for current chunk and write zipped info to blob directly
                    // so we have O(n) memory usage, where n is minimum of chunk size (200 MB by default) or exported items size
                    var partIndex = 1;
                    using (var stream = new MemoryStream())
                    {
                        using (var streamWriter = new StreamWriter(stream, new UTF8Encoding(false), 1024, true) { AutoFlush = true })
                        {
                            using (var csvWriter = new CsvWriter(streamWriter))
                            {
                                //Notification
                                progressCallback(progressInfo);

                                //Load all entities to export
                                var entities = entityFactory().ToArray();

                                csvWriter.Configuration.Delimiter = ",";

                                // Why we need this?
                                // From https://joshclose.github.io/CsvHelper/: "Auto mapping will map the properties in your class in the order they appear in"
                                // But if you look at CsvHelper code, you will see that it uses Type.GetProperties method to retrieve properties and learn order of them
                                // From https://msdn.microsoft.com/en-us/library/kyaxdd3x(v=vs.110).aspx:
                                // "The GetProperties method does not return properties in a particular order, such as alphabetical or declaration order.
                                // Your code must not depend on the order in which properties are returned, because that order varies."
                                // Because we must export columns in specific order (see below), I created this map
                                csvWriter.Configuration.RegisterClassMap(entityClassMap);

                                progressInfo.TotalCount = entities.Length;

                                for (var index = 0; index < entities.Length; index++)
                                {
                                    try
                                    {
                                        var previousSize = (int) stream.Length;
                                        csvWriter.WriteRecord(entities[index]);

                                        // If stream size with last update is large than chunk size, cancel this update and write previous iteration data
                                        if (stream.Length > chunkSize)
                                        {
                                            // 2 bytes decrease is because of empty new line (\r\n symbols) in the end of file (otherwise you will get 2 symbols of last update in addition)
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
                        
                        // Write data which hasn't been written in previous chunk
                        WriteEntry(archive, fileName, ref partIndex, stream.WriteTo, true);
                    }
                }
                updateProgress();
            }
        }

        // Write file to zip archive
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