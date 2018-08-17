
using Repositories;
using Services.AuthService.Models;
using System;
using System.Data.Entity;
using System.Linq;
using Services.TempDbService.Exceptions;
using Services.UserSerivce.Exceptions;

namespace Services.TempDbService
{
    public class TempDbService : ITempDbService
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
                user.Updated = DateTime.Now; ;
                user.Type = (int)type;
            }

            _dbContext.SaveChanges();
        }

        public UserLogin GetUserLogin(string username)
        {
            var result = _dbContext.UserLogins.SingleOrDefault(t => t.Username == username);

            if (result == null) return null;
            // validate login user is timeout
            if (result.Updated != null)
            {
                if (result.Updated.AddMinutes(15) < DateTime.Now)
                {
                    return null;
                }
            }

            result.Updated = DateTime.Now;
            _dbContext.Entry(result).State = EntityState.Modified;
            _dbContext.SaveChanges();
            return result;
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
