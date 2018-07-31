using Dokmee.Dms.Connector.Advanced.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.ViewModels.Home
{
	public class DetailModel
	{
		public IEnumerable<DokmeeFilesystem> dokmeeFilesystems { get; set; }
	    public List<DocumentIndex> TableTitles { get; set; } = new List<DocumentIndex>();
	    public List<DocumentItem> DocumentItems { get; set; } = new List<DocumentItem>();
	    public string CabinetId { get; set; }
	    public string FolderName { get; set; }
	}
}