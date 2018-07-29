
using Repositories;
using Services.AuthService.Models;
using System;
using System.Linq;

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

        public void RemoveUserLogin(string username)
        {
            var userLogin = _dbContext.UserLogins.SingleOrDefault(t => t.Username == username);
            if (userLogin != null)
            {
                _dbContext.UserLogins.Remove(userLogin);
                _dbContext.SaveChanges();
            }
        }

        public void UpdateCabinet(string username, string cabinetId)
        {
            UserLogin userLogin = GetUserLogin(username);
            if (userLogin != null)
            {
                userLogin.CurrentCabinetId = cabinetId;
                _dbContext.SaveChanges();
            }
        }
    }
}
