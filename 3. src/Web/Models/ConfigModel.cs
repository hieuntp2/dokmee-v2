using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class ConfigModel
    {
        [DisplayName("Complete folder")]
        [Required]
        public string TempFolder { get; set; }

        [DisplayName("Database Username")]
        [Required]
        public string DbUsername { get; set; }

        [DisplayName("Database password")]
        [Required]
        public string DbPassword { get; set; }

        [DisplayName("SQL Server name")]
        [Required]
        public string SQLServerName { get; set; }

        [DisplayName("Dokmee Cloud Url")]
        [Required]
        public string DokmeeCloudUrl { get; set; }

        [DisplayName("Dokmee DMS Host Url")]
        [Required]
        public string DokmeeDmsHostUrl { get; set; }

        [DisplayName("Document Status Index")]
        public string DocumentStatusIndex { get; set; }

        [DisplayName("Document Status select list")]
        public string DocumentStatusIndexValue { get; set; }

        public static string DEFAULT_TempFolder { get; set; }
        public static string DEFAULT_DbUsername { get; set; }
        public static string DEFAULT_DbPassword { get; set; }

        public static string DEFAULT_SQLServerName { get; set; }

        public static string DEFAULT_DokmeeCloudUrl { get; set; }

        public static string DEFAULT_DokmeeDmsHostUrl { get; set; }
        public static string DEFAULT_DocumentStatusIndex { get; set; }
        public static string DEFAULT_DocumentStatusIndexValue { get; set; }
        public string UpdateMessage { get; internal set; }
        public bool IsSuccess { get; internal set; }
    }
}