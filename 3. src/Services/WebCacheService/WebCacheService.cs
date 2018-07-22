using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using Dms.Contracts;
using Dokmee.Dms.Connector.Advanced.Core.Data;
using Services.AuthService.Models;
using Services.WebCacheService;

namespace Web.WebCache
{
  /// <summary>
  /// Manage store to session
  /// </summary>
  public class WebCacheService : IWebCacheService
  {
    private System.Web.SessionState.HttpSessionState _session;

    private IDictionary<string, IEnumerable<DokmeeCabinet>> _dokmeeCabinets;

    public IDictionary<string, IEnumerable<DokmeeCabinet>> DokmeeCabinets
    {
      get => _dokmeeCabinets ?? (_dokmeeCabinets = new ConcurrentDictionary<string, IEnumerable<DokmeeCabinet>>());
      set => _dokmeeCabinets = value;
    }
    public WebCacheService(System.Web.SessionState.HttpSessionState session)
    {
      _session = session;
    }

    private const string CabinetName = "USER_CABINETS";

    public IEnumerable<DokmeeCabinet> Cabinets
    {
      get => _session["USER_CABINETS"] as IEnumerable<DokmeeCabinet>;
      set => _session["USER_CABINETS"] = value;
    }

    private const string UserInforName = "USER_INFOR";
    public UserInfo UserInfo
    {
      get => _session[UserInforName] as UserInfo;
      set => _session[UserInforName] = value;
    }

    public IEnumerable<DokmeeCabinet> GetUserDokmeeCabinet(string username)
    {
      if (DokmeeCabinets.ContainsKey(username))
      {
        return DokmeeCabinets[username];
      }
      return new List<DokmeeCabinet>();
    }

    public bool SetUserDokmeeCabinets(string username, IEnumerable<DokmeeCabinet> cabinets)
    {
      if (DokmeeCabinets.ContainsKey(username))
      {
        DokmeeCabinets[username] = cabinets;
      }
      else
      {
        DokmeeCabinets.Add(username, cabinets);
      }

      return true;
    }

    public bool ClearUserDokmeeCabinets(string username)
    {
      return DokmeeCabinets.Remove(username);
    }
  }
}