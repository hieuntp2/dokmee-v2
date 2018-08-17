using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Services.ConfiguraionService
{
	public class ConfigurationService : IConfigurationService
	{
		public string DbUsername => ConfigurationManager.AppSettings["DbUsername"];
		public string DbPassword => ConfigurationManager.AppSettings["DbPassword"];
		public string SQLServerName => ConfigurationManager.AppSettings["SQLServerName"];
		public string TempFolder => ConfigurationManager.AppSettings["TempFolder"];
		public string DokmeeCloudUrl => ConfigurationManager.AppSettings["DokmeeCloudUrl"];
		public string DokmeeDmsHostUrl => ConfigurationManager.AppSettings["DokmeeDmsHostUrl"];
		public string CustomerStatusIndex => ConfigurationManager.AppSettings["CustomerStatusIndex"];
		public List<string> CustomerStatusIndexValue
		{
			get
			{
				List<string> status = new List<string>();
				var value = ConfigurationManager.AppSettings["CustomerStatusIndexValue"];
				if (!string.IsNullOrEmpty(value))
				{
					status = value.Split(',').ToList();
				}
				return status;
			}
		}
	}
}
