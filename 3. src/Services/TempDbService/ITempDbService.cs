using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories;
using Services.AuthService.Models;

namespace Services.TempDbService
{
    public interface ITempDbService
    {
        void SetUser(string username, string password, ConnectorType type);
        UserLogin GetUserLogin(string username);
    }
}
