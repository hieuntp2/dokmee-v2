using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ConfiguraionService
{
    public interface IConfigurationService
    {
        string DbUsername { get; }
        string DbPassword { get; }
        string SQLServerName { get; }

        string TempFolder { get; }

        string DokmeeCloudUrl { get; }
        string DokmeeDmsHostUrl { get; }
    }
}
