using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dokmee.Dms.Advanced.WebAccess.Data;

namespace Web.ViewModels.Home
{
    public class SearchModel
    {
        public string Key { get; set; }
        public List<DocumentIndex> TableTitles { get; set; } = new List<DocumentIndex>();

        public List<DocumentItem> DocumentItems { get; set; } = new List<DocumentItem>();
        public string CabinetId { get; set; }
    }

    public class DocumentIndex
    {
        public string Id { get; set; } = "0";
        public string Title { get; set; } = string.Empty;
        public int Order { get; set; } = 0;
        public object Value { get; set; }

        public IndexValueType Type { get; set; } = IndexValueType.String;
    }

    public class DocumentItem
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public bool IsFolder { get; set; } = false;
        public bool IsRoot { get; set; } = false;

        public List<DocumentIndex> Indexs { get; set; } = new List<DocumentIndex>();
        public string DisplayFileSize { get; set; }
        public string FullPath { get; set; }
        public string ParentFsGuid { get; set; }
    }
}