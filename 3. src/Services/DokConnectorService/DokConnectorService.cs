using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DokCapture.ServicenNetFramework.DokConnectorService;
using DokCapture.ServicenNetFramework.DokConnectorService.Models;
using Services.AuthService.Models;
using Dokmee.Dms.Connector.Advanced;
using Dokmee.Dms.Connector.Advanced.Core.Data;
using Dokmee.Dms.Connector.Advanced.Extension;
using Services.AuthService.Models;

namespace Services.AuthService
{
  public class DokConnectorService : IDokConnectorService
  {
    private DmsConnector _dmsConnector;
    private ConnectorModel _connectorModel;
    private Web.WebCache.WebCacheService _webCache;

    public DokConnectorService(Web.WebCache.WebCacheService webCache)
    {
      _webCache = webCache;
    }

    private ConnectorModel ConnectorVm
    {
      get
      {
        if (_connectorModel == null)
        {
          _connectorModel = new ConnectorModel();
          ConnectorVm.SelectedConnectorType = ConnectorVm.ConnectorTypes.First();
        }
        return _connectorModel;
      }
    }

    public Task<DokmeeLoginResult> Login(string username, string password, ConnectorType type)
    {
      DokmeeLoginResult result = new DokmeeLoginResult();

      try
      {
        ConnectorVm.IsProgressVisible = Visibility.Visible;
        // initialize connector
        DokmeeApplication dApp = DokmeeApplication.DokmeeDMS;
        if (type == ConnectorType.DMS)
        {
          ConnectionInfo connInfo = new ConnectionInfo();
          connInfo.ServerName = ConnectorVm.Server;
          connInfo.UserID = "sa";
          connInfo.Password = "123456";

          // register connection
          dApp = DokmeeApplication.DokmeeDMS;
          _dmsConnector = new DmsConnector(dApp);
          _dmsConnector.RegisterConnection<ConnectionInfo>(connInfo);
        }
        else if (type == ConnectorType.WEB)
        {
          // register connection
          dApp = DokmeeApplication.DokmeeWeb;
          _dmsConnector = new DmsConnector(dApp);
          _dmsConnector.RegisterConnection<string>(ConnectorVm.HostUrl);
        }
        else if (type == ConnectorType.CLOUD)
        {
          // register connection
          dApp = DokmeeApplication.DokmeeCloud;
          _dmsConnector = new DmsConnector(dApp);
          _dmsConnector.RegisterConnection<string>("https://www.dokmeecloud.com");
        }

        var loginResult = _dmsConnector.Login(new LogonInfo { Username = username, Password = password });

        result.Cabinets = loginResult.DokmeeCabinets;
        result.UserInfo = loginResult.CurrentUser;
        result.CabinetResult = loginResult;
        //_webCache.SetUserDokmeeCabinets(loginResult.CurrentUser.UserName, loginResult.DokmeeCabinets);
        //_webCache.UserInfo = loginResult.CurrentUser;
      }
      catch (Exception ex)
      {
        result.Result = SignInResult.Fail;
        result.Message = ex.Message;
      }
     
      
      return Task.FromResult(result);
    }

    public Task<IEnumerable<DokmeeCabinet>> GetUserCabinet(string username)
    {
      return Task.FromResult(_webCache.GetUserDokmeeCabinet(username));
    }

    public void LogOut()
    {
      _dmsConnector?.Logout();
    }
  }
}
