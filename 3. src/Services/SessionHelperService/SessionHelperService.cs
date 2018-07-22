using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;
using Services.AuthService.Models;

namespace Services.SessionHelperService
{
    public class SessionHelperService : ISessionHelperService
    {
        private HttpSessionState _sessionState;

        public SessionHelperService(HttpSessionState sessionState)
        {
            this._sessionState = sessionState;
        }

        public string Username
        {
            get => this._sessionState["Username"] as string;
            set => _sessionState["Username"] = value;
        }

        public string Password
        {
            get => this._sessionState["Password"] as string;
            set => _sessionState["Password"] = value;
        }
        public ConnectorType ConnectorType
        {
            get
            {
                if (_sessionState["ConnectorType"] == null)
                {
                    return ConnectorType.DMS;
                }

                return (ConnectorType)_sessionState["ConnectorType"];
            }
            set => _sessionState["ConnectorType"] = value;
        }
    }
}
