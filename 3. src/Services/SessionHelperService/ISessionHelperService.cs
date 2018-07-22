using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.AuthService.Models;

namespace Services.SessionHelperService
{
    public interface ISessionHelperService
    {
        string Username { get; set; }
        string Password { get; set; }
        ConnectorType ConnectorType { get; set; }
        
    }
}
