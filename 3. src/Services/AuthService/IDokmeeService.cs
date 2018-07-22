using System.Collections.Generic;
using Services.AuthService.Models;
using System.Threading.Tasks;
using DokCapture.ServicenNetFramework.Auth.Models;
using Dokmee.Dms.Connector.Advanced.Core.Data;
using System;

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
	}
}
