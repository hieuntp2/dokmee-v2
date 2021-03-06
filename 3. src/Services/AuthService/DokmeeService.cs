﻿using DokCapture.ServicenNetFramework.Auth;
using DokCapture.ServicenNetFramework.Auth.Models;
using Dokmee.Dms.Advanced.WebAccess.Data;
using Dokmee.Dms.Connector.Advanced;
using Dokmee.Dms.Connector.Advanced.Core.Data;
using Dokmee.Dms.Connector.Advanced.Extension;
using Repositories;
using Services.AuthService.Models;
using Services.ConfiguraionService;
using Services.SessionHelperService;
using Services.TempDbService;
using Services.TempDbService.Exceptions;
using Services.UserSerivce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace Services.AuthService
{
    public class DokmeeService : IDokmeeService
    {
        private ISessionHelperService _sessionHelperService;
        private DmsConnector _dmsConnector;
        private ConnectorModel _connectorModel;
        private ITempDbService _tempDbService;
        private IUserService _userService;
        private IConfigurationService _configurationService;

        public DmsConnector DmsConnectorProperty
        {
            get
            {
                if (!_userService.IsAuthenticate())
                {
                    throw new Exception("User not authenticated");
                }
                if (_dmsConnector == null)
                {
                    var username = _userService.GetUserId();
                    var tempLogin = _tempDbService.GetUserLogin(username);
                    CreateConnector(tempLogin.Username, tempLogin.Password, (ConnectorType)tempLogin.Type);
                }
                return _dmsConnector;
            }
            set { _dmsConnector = value; }
        }

        public DokmeeService(ISessionHelperService sessionHelperService, ITempDbService tempDbService, IUserService userService, IConfigurationService configurationService)
        {
            _sessionHelperService = sessionHelperService;
            _tempDbService = tempDbService;
            _userService = userService;
            _configurationService = configurationService;
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

        public Task<SignInResult> Login(string username, string password, ConnectorType type)
        {
            DokUser user = new DokUser();
            SignInResult result = SignInResult.Success;

            if (_dmsConnector == null)
            {
                CreateConnector(username, password, type);
            }

            return Task.FromResult(result);
        }

        public IEnumerable<DokmeeCabinet> GetCurrentUserCabinet(string username)
        {
            UserLogin user = _tempDbService.GetUserLogin(username);
            if (user == null)
            {
                throw new UserNotFoundInTempDbException("User login is not save to database.");
            }

            if (_dmsConnector == null)
            {
                var cabinets = CreateConnector(user.Username, user.Password, (ConnectorType)user.Type);
                return cabinets.DokmeeCabinets;
            }

            var loginResult = _dmsConnector.Login(new LogonInfo
            {
                Username = user.Username,
                Password = user.Password
            });
            return loginResult.DokmeeCabinets;
        }

        public IEnumerable<DmsNode> GetCabinetContent(string cabinetId, string username)
        {
            if (string.IsNullOrWhiteSpace(cabinetId))
            {
                throw new ArgumentException("cabinetId is null or empty");
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("username is null or empty");
            }

            DmsConnectorProperty.RegisterCabinet(new Guid(cabinetId));
            IEnumerable<DmsNode> dmsNodes = DmsConnectorProperty.GetFsNodesByName();
            return dmsNodes;
        }

        public Task<IEnumerable<DmsNode>> GetFolderContent(string username, string id, bool isRoot)
        {
            var result = isRoot ? DmsConnectorProperty.GetFilesystem(SubjectTypes.Folder)
              : DmsConnectorProperty.GetFilesystem(SubjectTypes.Folder, id);

            return Task.FromResult(result);
        }

        public IEnumerable<DokmeeFilesystem> GetDokmeeFilesystems(string username, string name, bool isFolder, string cabinetId)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("username is null or empty");
            }

            IEnumerable<DokmeeFilesystem> results = new List<DokmeeFilesystem>();
            Guid id = Guid.Empty;
            if (!string.IsNullOrEmpty(cabinetId) && Guid.TryParse(cabinetId, out id))
            {
                DmsConnectorProperty.RegisterCabinet(id);
                if (isFolder)
                {
                    results = DmsConnectorProperty.Search(SearchFieldType.TextIndex, name, "Folder Title").DmsFilesystem;
                }
                else
                {
                    try
                    {
                        var nodes = DmsConnectorProperty.GetFsNodesByName(SubjectTypes.Document, name);
                        if (nodes != null && nodes.Any())
                        {
                            var nodeId = nodes.First()?.ID.ToString();
                            if (!string.IsNullOrEmpty(nodeId))
                            {
                                results = DmsConnectorProperty.Search(SearchFieldType.ByNodeID, nodeId).DmsFilesystem;
                            }
                        }
                    }
                    catch { }
                }
            }
            return results;
        }

        public void UpdateIndex(string username, Dictionary<object, object> args)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("username is null or empty");
            }
            UserLogin user = _tempDbService.GetUserLogin(username);
            IEnumerable<DokmeeFilesystem> results = new List<DokmeeFilesystem>();
            var cabinetIdTemp = _userService.GetCurrentCabinetId();
            Guid cabinetId = Guid.Empty;

            if (!string.IsNullOrEmpty(cabinetIdTemp) && Guid.TryParse(cabinetIdTemp, out cabinetId))
            {
                DmsConnectorProperty.RegisterCabinet(cabinetId);
            }

            var status = args["CustomerStatus"].ToString().Split(';');
            if (status.Length > 0)
            {
                foreach (var item in status)
                {
                    var info = item.Split(':');
                    if (info.Length == 2)
                    {
                        var nodeIdTemp = info[0].Trim();
                        var customerStatus = info[1].Trim();
                        Guid nodeId = Guid.Empty;
                        if (!string.IsNullOrEmpty(nodeIdTemp) && Guid.TryParse(nodeIdTemp, out nodeId))
                        {
                            UpdateCustomerStatus(cabinetId, customerStatus, nodeId);
                        }
                    }
                }
            }
        }

        public string Preview(string username, string id, string cabinetId)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("username is null or empty");
            }

            DmsConnectorProperty.RegisterCabinet(new Guid(cabinetId));
            string url = DmsConnectorProperty.ViewFile(id);
            //string machineName = HttpContext.Current.Request.Url.Host;
            string machineName = GetIPAddress();
            string localhost = "localhost";
            if (url.Contains(localhost))
            {
                url = url.Replace(localhost, machineName);
            }
            return url;
        }

        /// <summary>
        /// Search File System by Index
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="searchValue"></param>
        /// <param name="cabinetId"></param>
        /// <returns></returns>
        public IEnumerable<DokmeeFilesystem> Search(string username, string fieldName, string searchValue, string cabinetId, SearchFieldType indexFieldType)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("username is null or empty");
            }

            IEnumerable<DokmeeFilesystem> results = new List<DokmeeFilesystem>();
            Guid id = Guid.Empty;
            if (!string.IsNullOrEmpty(cabinetId) && Guid.TryParse(cabinetId, out id))
            {
                DmsConnectorProperty.RegisterCabinet(id);
                if (DateTime.TryParse(searchValue, out DateTime conditionIndexValue))
                {
                    searchValue = conditionIndexValue.ToString("yyyy/MMM/dd");
                }
                var lookupResults = DmsConnectorProperty.Search(indexFieldType, searchValue, fieldName);
                results = lookupResults.DmsFilesystem;
            }
            return results;
        }

        /// <summary>
        /// Get all Cabinet Indexes
        /// </summary>
        /// <param name="username"></param>
        /// <param name="cabinetId"></param>
        /// <returns></returns>
        public IEnumerable<DokmeeIndex> GetCabinetIndexes(string username, string cabinetId)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("username is null or empty");
            }

            IEnumerable<DokmeeIndex> results = new List<DokmeeIndex>();
            Guid id = Guid.Empty;
            if (!string.IsNullOrEmpty(cabinetId) && Guid.TryParse(cabinetId, out id))
            {
                DmsConnectorProperty.RegisterCabinet(id);
                results = DmsConnectorProperty.GetCabinetIndexInfoByID(id);
            }
            return results;
        }

        /// <summary>
        /// Move files to Temp Folder
        /// </summary>
        /// <param name="username"></param>
        /// <param name="args"></param>
        /// <param name="tempFolder"></param>
        public void Complete(string username, Dictionary<object, object> args, string tempFolder, string cabinetIdTemp)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("username is null or empty");
            }

            UserLogin user = _tempDbService.GetUserLogin(username);
            IEnumerable<DokmeeFilesystem> results = new List<DokmeeFilesystem>();
            Guid cabinetId = Guid.Empty;
            if (!string.IsNullOrEmpty(cabinetIdTemp) && Guid.TryParse(cabinetIdTemp, out cabinetId))
            {
                DmsConnectorProperty.RegisterCabinet(cabinetId);
            }

            var status = args["NodeId"].ToString().Split(';');
            if (status.Length > 0)
            {
                foreach (var noteIdTemp in status)
                {
                    Guid nodeId = Guid.Empty;
                    if (!string.IsNullOrEmpty(noteIdTemp) && Guid.TryParse(noteIdTemp, out nodeId))
                    {
                        //update status to Complete
                        var customerStatus = "Complete";
                        UpdateCustomerStatus(cabinetId, customerStatus, nodeId);
                    }

                    //if (!string.IsNullOrEmpty(noteIdTemp) && Guid.TryParse(noteIdTemp, out nodeId))
                    //{
                    //	var folders = DmsConnectorProperty.GetFsNodesByName(SubjectTypes.Folder, tempFolder);
                    //	if (folders != null && folders.ToList().Count == 1)
                    //	{
                    //		var folder = folders.First();
                    //		DmsConnectorProperty.MoveFile(noteIdTemp, folder.ID.ToString());
                    //		//update status to Complete
                    //		var customerStatus = "Complete";
                    //		UpdateCustomerStatus(cabinetId, customerStatus, nodeId);
                    //	}
                    //	else
                    //	{
                    //		throw new Exception("Cant find folder name " + tempFolder + " to move selected file!");
                    //	}
                    //}
                }
            }
        }

        #region private methods
        private void UpdateCustomerStatus(Guid cabinetId, string customerStatus, Guid nodeId)
        {
            var fileSystems = DmsConnectorProperty.Search(SearchFieldType.ByNodeID, nodeId.ToString()).DmsFilesystem;
            if (fileSystems != null && fileSystems.Any())
            {
                var file = fileSystems.First();
                var dokmeeIndexInfos = file.IndexFieldPairCollection;
                if (dokmeeIndexInfos != null && dokmeeIndexInfos.Any())
                {
                    var listIndexes = DmsConnectorProperty.GetCabinetIndexInfoByID(cabinetId);
                    var statusIndex = dokmeeIndexInfos.FirstOrDefault(x => x.IndexName.ToUpper() == _configurationService.DocumentStatusIndex.ToUpper());
                    if (statusIndex != null)
                    {
                        statusIndex.IndexValue = customerStatus;
                        IEnumerable<DokmeeIndex> dokmeeIndexes = dokmeeIndexInfos.Select(x => new DokmeeIndex
                        {
                            DokmeeIndexID = x.IndexFieldGuid,
                            Name = x.IndexName,
                            Value = x.IndexValue,
                            SortOrder = x.SortOrder,
                            CabinetID = cabinetId
                        });
                        DmsConnectorProperty.UpdateIndex(nodeId, dokmeeIndexes);
                    }
                }
            }
        }

        private DokmeeCabinetResult CreateConnector(string username, string password, ConnectorType type)
        {
            username = username ?? _sessionHelperService.Username;
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new InvalideUsernameException("Username is null");
            }
            password = password ?? _sessionHelperService.Password;
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidePasswordException("Password is null");
            }
            //// initialize connector
            DokmeeApplication dApp = DokmeeApplication.DokmeeDMS;

            if (type == ConnectorType.DMS)
            {
                ConnectionInfo connInfo = new ConnectionInfo
                {
                    ServerName = _configurationService.SQLServerName,
                    UserID = _configurationService.DbUsername,
                    Password = _configurationService.DbPassword
                };

                // register connection
                dApp = DokmeeApplication.DokmeeDMS;
                _dmsConnector = new DmsConnector(dApp);
                _dmsConnector.RegisterConnection<ConnectionInfo>(connInfo); // =>> this code cause lost session. Why???
            }
            else if (type == ConnectorType.WEB)
            {
                // register connection
                dApp = DokmeeApplication.DokmeeWeb;
                _dmsConnector = new DmsConnector(dApp);
                _dmsConnector.RegisterConnection<string>(_configurationService.DokmeeDmsHostUrl);
            }
            else if (type == ConnectorType.CLOUD)
            {
                // register connection
                dApp = DokmeeApplication.DokmeeCloud;
                _dmsConnector = new DmsConnector(dApp);
                _dmsConnector.RegisterConnection<string>(_configurationService.DokmeeCloudUrl);
            }

            var loginResult = _dmsConnector.Login(new LogonInfo
            {
                Username = username,
                Password = password
            });

            return loginResult;
        }

        public string GetIPAddress()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName()); // `Dns.Resolve()` method is deprecated.

            foreach (var ip in ipHostInfo.AddressList)
            {
                if (ValidateIPv4(ip.ToString()))
                {
                    return ip.ToString();
                }
            }
            return string.Empty;
            //IPAddress ipAddress = ipHostInfo.AddressList[0];

            //return ipAddress.ToString();
        }

        public bool ValidateIPv4(string ipString)
        {
            if (String.IsNullOrWhiteSpace(ipString))
            {
                return false;
            }

            string[] splitValues = ipString.Split('.');
            if (splitValues.Length != 4)
            {
                return false;
            }

            byte tempForParsing;

            return splitValues.All(r => byte.TryParse(r, out tempForParsing));
        }
        #endregion
    }

    public class InvalideUsernameException : ArgumentException
    {
        public InvalideUsernameException(string mesage) : base(mesage)
        {

        }
    }

    public class InvalidePasswordException : ArgumentException
    {
        public InvalidePasswordException(string mesage) : base(mesage)
        {

        }
    }
}
