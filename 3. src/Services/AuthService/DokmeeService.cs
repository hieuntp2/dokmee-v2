using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DokCapture.ServicenNetFramework.Auth;
using DokCapture.ServicenNetFramework.Auth.Models;
using Services.AuthService.Models;
using Dokmee.Dms.Connector.Advanced;
using Dokmee.Dms.Connector.Advanced.Core.Data;
using Dokmee.Dms.Connector.Advanced.Extension;
using Repositories;
using Services.SessionHelperService;
using Services.TempDbService;
using Dokmee.Dms.Advanced.WebAccess.Data;
using System.Reflection;
using System.Diagnostics;
using Services.UserSerivce;

namespace Services.AuthService
{
    public class DokmeeService : IDokmeeService
    {
        private ISessionHelperService _sessionHelperService;
        private DmsConnector _dmsConnector;
        private ConnectorModel _connectorModel;
        private ITempDbService _tempDbService;
        private IUserService _userService;

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

        public DokmeeService(ISessionHelperService sessionHelperService, ITempDbService tempDbService, IUserService userService)
        {
            _sessionHelperService = sessionHelperService;
            _tempDbService = tempDbService;
            _userService = userService;
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
                throw new Exception("User login is not save to database.");
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

            //UserLogin user = _tempDbService.GetUserLogin(username);
            //if (_dmsConnector == null)
            //{
            //    CreateConnector(user.Username, user.Password, (ConnectorType)user.Type);
            //}

            DmsConnectorProperty.RegisterCabinet(new Guid(cabinetId));
            IEnumerable<DmsNode> dmsNodes = DmsConnectorProperty.GetFsNodesByName();
            return dmsNodes;
        }

        public Task<IEnumerable<DmsNode>> GetFolderContent(string username, string id, bool isRoot)
        {
            //UserLogin user = _tempDbService.GetUserLogin(username);
            //if (_dmsConnector == null)
            //{
            //    CreateConnector(user.Username, user.Password, (ConnectorType)user.Type);
            //}
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
            //UserLogin user = _tempDbService.GetUserLogin(username);

            //if (_dmsConnector == null)
            //{
            //    CreateConnector(user.Username, user.Password, (ConnectorType)user.Type);
            //}
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
            var cabinetId = args["CabinetId"].ToString();
            Guid idTemp = Guid.Empty;
            //if (_dmsConnector == null)
            //{
            //    if (!string.IsNullOrEmpty(cabinetId) && Guid.TryParse(cabinetId, out idTemp))
            //    {
            //        CreateConnector(user.Username, user.Password, (ConnectorType)user.Type);
            //        _dmsConnector.RegisterCabinet(idTemp);
            //    }
            //}

            if (!string.IsNullOrEmpty(cabinetId) && Guid.TryParse(cabinetId, out idTemp))
            {
                DmsConnectorProperty.RegisterCabinet(idTemp);
            }
            
            var status = args["CustomerStatus"].ToString().Split(';');
            if (status.Length > 0)
            {
                foreach (var item in status)
                {
                    var info = item.Split(':');
                    if (info.Length == 2)
                    {
                        var nodeId = info[0].Trim();
                        var customerStatus = info[1].Trim();
                        Guid id = Guid.Empty;
                        if (!string.IsNullOrEmpty(nodeId) && Guid.TryParse(nodeId, out id))
                        {
                            var fileSystems = DmsConnectorProperty.Search(SearchFieldType.ByNodeID, nodeId).DmsFilesystem;
                            if (fileSystems != null && fileSystems.Any())
                            {
                                var file = fileSystems.First();
                                var dokmeeIndexInfos = file.IndexFieldPairCollection;
                                if (dokmeeIndexInfos != null && dokmeeIndexInfos.Any())
                                {
                                    var statusIndex = dokmeeIndexInfos.FirstOrDefault(x => x.IndexName.ToUpper() == "DOCUMENT STATUS");
                                    if (statusIndex != null)
                                    {
                                        statusIndex.IndexValue = customerStatus;
                                        IEnumerable<DokmeeIndex> dokmeeIndexes = dokmeeIndexInfos.Select(x => new DokmeeIndex
                                        {
                                            DokmeeIndexID = x.IndexFieldGuid,
                                            Name = x.IndexName,
                                            Value = x.IndexValue,
                                            SortOrder = x.SortOrder,
                                            CabinetID = idTemp
                                        });
                                        DmsConnectorProperty.UpdateIndex(id, dokmeeIndexes);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Preview(string username, string id, string cabinetId)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("username is null or empty");
            }
            //UserLogin user = _tempDbService.GetUserLogin(username);
            //if (_dmsConnector == null)
            //{
            //    CreateConnector(user.Username, user.Password, (ConnectorType)user.Type);
            //    _dmsConnector.RegisterCabinet(new Guid(cabinetId));
            //}

            DmsConnectorProperty.RegisterCabinet(new Guid(cabinetId));
            var config = Assembly.GetExecutingAssembly().Location;
            Process.Start(DmsConnectorProperty.ViewFile(id), config);
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

		#region private methods

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
                    ServerName = ConnectorVm.Server,
                    UserID = "sa",
                    Password = "123456"
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
                _dmsConnector.RegisterConnection<string>(ConnectorVm.HostUrl);
            }
            else if (type == ConnectorType.CLOUD)
            {
                // register connection
                dApp = DokmeeApplication.DokmeeCloud;
                _dmsConnector = new DmsConnector(dApp);
                _dmsConnector.RegisterConnection<string>("https://www.dokmeecloud.com");
            }

            var loginResult = _dmsConnector.Login(new LogonInfo
            {
                Username = username,
                Password = password
            });

            return loginResult;
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
