using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Services.UserSerivce
{
    public class UserService: IUserService
    {
        private readonly IPrincipal _userPrincipal;

        public UserService(IPrincipal userPrincipal)
        {
            _userPrincipal = userPrincipal;
        }

        public bool IsAuthenticate()
        {
            return _userPrincipal.Identity.IsAuthenticated;
        }

        public string GetUserId()
        {
            return _userPrincipal.Identity.GetUserId();
        }
    }
}
