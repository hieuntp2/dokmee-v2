using System.Collections.Generic;
using Services.AuthService.Models;
using System.Threading.Tasks;
using DokCapture.ServicenNetFramework.Auth.Models;
using Dokmee.Dms.Connector.Advanced.Core.Data;

namespace DokCapture.ServicenNetFramework.Auth
{
  public interface IDokmeeService
  {
   // DokUser Login(string username, string password, ConnectorType type);

    Task<SignInResult> Login(string username, string password, ConnectorType type);

      IEnumerable<DokmeeCabinet> GetCurrentUserCabinet(string username);
      IEnumerable<DmsNode> GetCabinetContent(string cabinetId, string username);
  }
}
