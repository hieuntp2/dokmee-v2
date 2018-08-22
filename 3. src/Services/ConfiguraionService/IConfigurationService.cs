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
		string DocumentStatusIndex { get; }
		List<string> DocumentStatusIndexValue { get; }
        void UpdateConfig(string section, string name, string value);
        bool IsFirstTime { get; }
    }
}
