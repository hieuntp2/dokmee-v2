using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Services;
using Services.ConfiguraionService;
using Web.ViewModels.Home;

namespace Web.ViewModels.Elements
{
    public class DocumentItemListModel
    {
        private List<DocumentIndex> _tableTitles = new List<DocumentIndex>();

        public List<DocumentIndex> TableTitles
        {
            get => _tableTitles;
            set
            {
				var configuration = new ConfigurationService();
				_tableTitles = value;

                Conditions = value.Where(t => !string.IsNullOrWhiteSpace(t.ValueString)).ToList();
                HaveSearchValue = Conditions.Any();

                DocumentIndex statusFind = value.SingleOrDefault(t =>
                    string.Equals(t.Title.ToUpper(), configuration.CustomerStatusIndex.ToUpper()));
                HaveDocumentStatusTitle = statusFind != null;
            }
        }
        public IEnumerable<DocumentItem> DocumentItems { get; set; } = new List<DocumentItem>();

        public string CabinetId { get; set; } = string.Empty;

        public bool HaveSearchValue { get; internal set; } = false;

        public List<DocumentIndex> Conditions
        {
            get;
            internal set;
        } = new List<DocumentIndex>();

        public bool HaveDocumentStatusTitle
        {
            get;
            internal set;
        }
    }
}