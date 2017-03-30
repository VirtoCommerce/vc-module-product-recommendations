using System;
using System.IO;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.PushNotifications;
using VirtoCommerce.Platform.Data.Common;
using VirtoCommerce.ProductRecommendationsModule.Web.Model;

namespace VirtoCommerce.ProductRecommendationsModule.Web.Export
{
    public class Exporter
    {
        private readonly IPushNotificationManager _pushNotifier;
        private readonly IBlobStorageProvider _blobStorageProvider;
        private readonly IBlobUrlResolver _blobUrlResolver;

        public Exporter(IPushNotificationManager pushNotifier,
            IBlobStorageProvider blobStorageProvider, IBlobUrlResolver blobUrlResolver)
        {
            _pushNotifier = pushNotifier;
            _blobStorageProvider = blobStorageProvider;
            _blobUrlResolver = blobUrlResolver;
        }

        public void Export(Action<Stream, string, Action<ExportImportProgressInfo>> exporter, string fileName, ExportPushNotification notification)
        {
            Action<ExportImportProgressInfo> progressCallback = x =>
            {
                notification.Description = x.Description;
                notification.TotalCount = x.TotalCount;
                notification.ProcessedCount = x.ProcessedCount;
                notification.Errors = x.Errors;
                _pushNotifier.Upsert(notification);
            };

            var relativeUrl = "temp/" + fileName + ".zip";
            using (var stream = _blobStorageProvider.OpenWrite(relativeUrl))
            {
                try
                {
                    exporter(stream, fileName, progressCallback);
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
    }
}