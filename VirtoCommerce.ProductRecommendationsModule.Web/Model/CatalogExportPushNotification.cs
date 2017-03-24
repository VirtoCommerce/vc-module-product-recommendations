﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
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

        [JsonProperty("downloadUrl")]
        public string DownloadUrl { get; set; }

        [JsonProperty("finished")]
        public DateTime? Finished { get; set; }

        [JsonProperty("totalCount")]
        public long TotalCount { get; set; }

        [JsonProperty("processedCount")]
        public long ProcessedCount { get; set; }

        [JsonProperty("errorCount")]
        public long ErrorCount
        {
            get
            {
                return Errors != null ? Errors.Count() : 0;
            }
        }

        [JsonProperty("errors")]
        public ICollection<string> Errors { get; set; }
    }
}