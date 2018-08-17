using System.Collections.Generic;

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
		string CustomerStatusIndex { get; }
		List<string> CustomerStatusIndexValue { get; }
	}
}
