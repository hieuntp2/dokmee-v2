using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.ViewModels.Home;

namespace Web.ViewModels.Elements
{
    public class DocumentItemListModel
    {
        public IEnumerable<DocumentIndex> TableTitles { get; set; } = new List<DocumentIndex>();
        public IEnumerable<DocumentItem> DocumentItems { get; set; } = new List<DocumentItem>();

        public string CabinetId { get; set; } = string.Empty;
    }
}