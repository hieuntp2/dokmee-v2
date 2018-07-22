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
	}
}