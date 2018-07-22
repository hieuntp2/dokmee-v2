
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.AuthService.Models;

namespace Services.TempDbService
{
    public class TempDbService: ITempDbService
    {
        // private 
        private DokmeeTempEntities _dbContext;

        public TempDbService()
        {
            _dbContext = new DokmeeTempEntities();
        }


        public void SetUser(string username, string password, ConnectorType type)
        {
            UserLogin user = GetUserLogin(username);
            if (user == null)
            {
                user = new UserLogin()
                {
                    Username = username,
                    Password = password,
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                    Type = (int)type
                };

                _dbContext.UserLogins.Add(user);
            }
            else
            {
                user.Password = password;
                user.Updated = DateTime.Now;;
                user.Type = (int) type;
            }

            _dbContext.SaveChanges();
        }

        public UserLogin GetUserLogin(string username)
        {
            return _dbContext.UserLogins.SingleOrDefault(t => t.Username == username);
        }
    }
}
