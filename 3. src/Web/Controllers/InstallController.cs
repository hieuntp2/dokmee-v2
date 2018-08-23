using Repositories;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers
{
	public class InstallController : Controller
	{
		private static string appSettingName = "appSettings";
		private static string DbEntityName = "DokmeeTempEntities";
		private static string DbTempName = "DokmeeTemp";
		// GET: Config
		public ActionResult Index()
		{
			ConfigModel model = new ConfigModel()
			{
				DokmeeCloudUrl = ReadConfigValue(appSettingName, nameof(model.DokmeeCloudUrl)),
				DbPassword = ReadConfigValue(appSettingName, nameof(model.DbPassword)),
				DbUsername = ReadConfigValue(appSettingName, nameof(model.DbUsername)),
				DocumentStatusIndex = ReadConfigValue(appSettingName, nameof(model.DocumentStatusIndex)),
				DocumentStatusIndexValue = ReadConfigValue(appSettingName, nameof(model.DocumentStatusIndexValue)),
				DokmeeDmsHostUrl = ReadConfigValue(appSettingName, nameof(model.DokmeeDmsHostUrl)),
				SQLServerName = ReadConfigValue(appSettingName, nameof(model.SQLServerName)),
				TempFolder = ReadConfigValue(appSettingName, nameof(model.TempFolder)),
			};

			return View(model);
		}

		[HttpPost]
		public ActionResult Index(ConfigModel model)
		{
			try
			{
				BuildConnectionString(model);

				UpdateConfig(appSettingName, nameof(model.DokmeeCloudUrl), model.DokmeeCloudUrl);
				UpdateConfig(appSettingName, nameof(model.DbPassword), model.DbPassword);
				UpdateConfig(appSettingName, nameof(model.DbUsername), model.DbUsername);
				UpdateConfig(appSettingName, nameof(model.DocumentStatusIndex), model.DocumentStatusIndex);
				UpdateConfig(appSettingName, nameof(model.DocumentStatusIndexValue), model.DocumentStatusIndexValue);
				UpdateConfig(appSettingName, nameof(model.DokmeeDmsHostUrl), model.DokmeeDmsHostUrl);
				UpdateConfig(appSettingName, nameof(model.SQLServerName), model.SQLServerName);
				UpdateConfig(appSettingName, nameof(model.TempFolder), model.TempFolder);
				UpdateConfig(appSettingName, nameof(model.TempFolder), model.TempFolder);
				model.IsSuccess = true;
				model.UpdateMessage = "Update successfully!";
				UpdateConfig(appSettingName, "FirstTime", "false");

			}
			catch (Exception ex)
			{
				model.IsSuccess = false;
				model.UpdateMessage = ex.Message;
			}

			return View(model);
		}

		private void UpdateConfig(string section, string name, string value)
		{
			Configuration objConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
			AppSettingsSection objAppsettings = (AppSettingsSection)objConfig.GetSection(section);
			//Edit
			if (objAppsettings != null)
			{
				if (objAppsettings.Settings[name] != null)
				{
					objAppsettings.Settings[name].Value = value;
				}
				else
				{
					objAppsettings.Settings.Add(new KeyValueConfigurationElement(name, value));
				}
				objConfig.Save();
			}
		}

		private string ReadConfigValue(string section, string name)
		{
			Configuration objConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
			AppSettingsSection objAppsettings = (AppSettingsSection)objConfig.GetSection(section);

			return objAppsettings.Settings[name].Value;
		}

		private void BuildConnectionString(ConfigModel model)
		{
			string connectionString = $"data source={model.SQLServerName};persist security info=True;initial catalog={DbTempName};user id={model.DbUsername};password={model.DbPassword};MultipleActiveResultSets=True;App=EntityFramework";

			Configuration objConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
			ConnectionStringsSection sec = (ConnectionStringsSection)objConfig.GetSection("connectionStrings");
			sec.ConnectionStrings[DbEntityName].ConnectionString = connectionString;
			objConfig.Save();
			try
			{
				DokmeeTempEntities dbContext = new DokmeeTempEntities();
				var allUsers = dbContext.UserLogins.ToList();
				ApplicationDbContext applicationDbContext = new ApplicationDbContext();
				applicationDbContext.Users.ToList();
				applicationDbContext.Roles.ToList();
			}
			catch
			{
				throw new Exception("Fail to connect database");
			}

		}

		private bool TestConnectionString(string connect)
		{
			try
			{
				SqlConnection connection = new SqlConnection(connect);
				connection.Open();
				if ((connection.State == ConnectionState.Open))
				{
					connection.Close();
					return true;
				}
				else
				{
					return false;
				}
			}
			catch
			{
				throw;
			}
		}
	}
}