using System.Collections.Generic;
using Dms.Contracts;
using Dokmee.Dms.Connector.Advanced.Core.Data;
using Services.AuthService.Models;

namespace Services.WebCacheService
{
  public interface IWebCacheService
  {
    IEnumerable<DokmeeCabinet> Cabinets { get; set; }
    UserInfo UserInfo { get; set; }

    IEnumerable<DokmeeCabinet> GetUserDokmeeCabinet(string username);

    bool SetUserDokmeeCabinets(string username, IEnumerable<DokmeeCabinet> cabinets);

    bool ClearUserDokmeeCabinets(string username);
  }
}
