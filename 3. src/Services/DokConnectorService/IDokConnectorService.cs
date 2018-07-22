using System.Collections.Generic;
using Services.AuthService.Models;
using System.Threading.Tasks;
using DokCapture.ServicenNetFramework.DokConnectorService.Models;
using Dokmee.Dms.Connector.Advanced.Core.Data;

namespace DokCapture.ServicenNetFramework.DokConnectorService
{
  public interface IDokConnectorService
  {
   // DokUser Login(string username, string password, ConnectorType type);

    Task<DokmeeLoginResult> Login(string username, string password, ConnectorType type);

    Task<IEnumerable<DokmeeCabinet>> GetUserCabinet(string username);
    void LogOut();
  }
}
