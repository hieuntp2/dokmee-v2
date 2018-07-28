using System.Collections.Generic;
using Services.AuthService.Models;
using System.Threading.Tasks;
using DokCapture.ServicenNetFramework.Auth.Models;
using Dokmee.Dms.Connector.Advanced.Core.Data;
using System;
using Dokmee.Dms.Advanced.WebAccess.Data;

namespace DokCapture.ServicenNetFramework.Auth
{
	public interface IDokmeeService
	{
		Task<SignInResult> Login(string username, string password, ConnectorType type);
		IEnumerable<DokmeeCabinet> GetCurrentUserCabinet(string username);
		IEnumerable<DmsNode> GetCabinetContent(string cabinetId, string username);
		Task<IEnumerable<DmsNode>> GetFolderContent(string username, string id, bool isRoot);
		IEnumerable<DokmeeFilesystem> GetDokmeeFilesystems(string username, string name, bool isFolder, string cabinetId);
		void UpdateIndex(string username, Dictionary<object, object> args);
		void Preview(string username, string id, string cabinetId);

		/// <summary>
		/// Search File System by Index
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="searchValue"></param>
		/// <param name="cabinetId"></param>
		/// <returns></returns>
		IEnumerable<DokmeeFilesystem> Search(string username, string fieldName, string searchValue, string cabinetId, SearchFieldType indexFieldType);

		/// <summary>
		/// Get all Cabinet Indexes
		/// </summary>
		/// <param name="username"></param>
		/// <param name="cabinetId"></param>
		/// <returns></returns>
		IEnumerable<DokmeeIndex> GetCabinetIndexes(string username, string cabinetId);

		/// <summary>
		/// Move files to Temp Folder
		/// </summary>
		/// <param name="username"></param>
		/// <param name="args"></param>
		/// <param name="tempFolder"></param>
		void Complete(string username, Dictionary<object, object> args, string tempFolder, string cabinetId);
	}
}
