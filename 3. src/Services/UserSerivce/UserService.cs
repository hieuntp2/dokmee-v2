using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Repositories;
using Services.TempDbService;
using Services.UserSerivce.Exceptions;

namespace Services.UserSerivce
{
  public class UserService : IUserService
  {
    private readonly IPrincipal _userPrincipal;
    private ITempDbService _tempDbService;

    public UserService(IPrincipal userPrincipal, ITempDbService tempDbService)
    {
      _userPrincipal = userPrincipal;
      _tempDbService = tempDbService;
    }

    public bool IsAuthenticate()
    {
      return _userPrincipal.Identity.IsAuthenticated;
    }

    public string GetUserId()
    {
      return _userPrincipal.Identity.GetUserId();
    }

    public void UpdateCabinet(string cabinetId)
    {
      _tempDbService.UpdateCabinet(GetUserId(), cabinetId);
    }

    public string GetCurrentCabinetId()
    {
      UserLogin userLogin = _tempDbService.GetUserLogin(GetUserId());
      if (string.IsNullOrWhiteSpace(userLogin.CurrentCabinetId))
      {
        throw new CabinetNotSelectedException("Cabinet not selceted");
      }
      return userLogin.CurrentCabinetId;
    }
  }
}
