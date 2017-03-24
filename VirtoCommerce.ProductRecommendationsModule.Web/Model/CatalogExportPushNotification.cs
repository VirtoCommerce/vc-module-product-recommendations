using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.PushNotifications;

namespace VirtoCommerce.ProductRecommendationsModule.Web.Model
{
    public class CatalogExportPushNotification : PushNotification
    {
        public CatalogExportPushNotification(string creator) : base(creator)
        {
            NotifyType = "CatalogCvsExportPrepatedForRecommendations";
            Errors = new List<string>();
        }

        public string DownloadUrl { get; set; }

        public DateTime? Finished { get; set; }

        public long TotalCount { get; set; }

        public long ProcessedCount { get; set; }

        public long ErrorCount
        {
            get
            {
                return Errors != null ? Errors.Count() : 0;
            }
        }

        public ICollection<string> Errors { get; set; }
    }
}