using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
