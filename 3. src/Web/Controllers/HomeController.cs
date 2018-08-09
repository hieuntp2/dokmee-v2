using AutoMapper;
using DokCapture.ServicenNetFramework.Auth;
using Dokmee.Dms.Advanced.WebAccess.Data;
using Dokmee.Dms.Connector.Advanced.Core.Data;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Services.AuthService;
using Services.AuthService.Models;
using Services.ConfiguraionService;
using Services.SessionHelperService;
using Services.TempDbService.Exceptions;
using Services.UserSerivce;
using Services.UserSerivce.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Web.Models;
using Web.ViewModels.Home;

namespace Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        private readonly IDokmeeService _dokmeeService;
        private readonly IMapper _mapper;
        private readonly ISessionHelperService _sessionHelperService;
        private readonly IUserService _userService;
        private readonly IConfigurationService _configurationService;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(HomeController));

        public HomeController(IDokmeeService dokmeeService, IMapper mapper, ISessionHelperService sessionHelperService, IUserService userService, IConfigurationService configurationService)
        {
            _dokmeeService = dokmeeService;
            _mapper = mapper;
            _sessionHelperService = sessionHelperService;
            _userService = userService;
            _configurationService = configurationService;
        }

        public ActionResult Index()
        {

            try
            {
                throw new Exception("Some message");
                IndexModel model = new IndexModel();
                IEnumerable<DokmeeCabinet> dokmeeCabinets = _dokmeeService.GetCurrentUserCabinet(User.Identity.GetUserId());
                model.Cabinets = _mapper.Map<IEnumerable<Cabinet>>(dokmeeCabinets);
                return View(model);
            }
            catch (InvalideUsernameException ex)
            {
                return RedirectToAction("Logoff", "Account");
            }
            catch (InvalidePasswordException ex)
            {
                return RedirectToAction("Logoff", "Account");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Redirect after user login. To store username/password to use later request
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="loginType"></param>
        /// <returns></returns>
        public ActionResult AfterMyActionResult(string username, string password, ConnectorType loginType)
        {
            _sessionHelperService.Username = username;
            _sessionHelperService.Password = password;
            _sessionHelperService.ConnectorType = loginType;
            return RedirectToAction("Index");
        }

        // dont remove cabinetId, this is save selected cabinet to temp db
        public ActionResult CabinetDetail()
        {
            string cabinetId = _userService.GetCurrentCabinetId();
            string username = User.Identity.GetUserId();
            IEnumerable<DmsNode> cabinetContent = _dokmeeService.GetCabinetContent(cabinetId, username);
            IEnumerable<Node> nodes = _mapper.Map<IEnumerable<Node>>(cabinetContent);
            ViewBag.cabinetId = cabinetId;
            return View(nodes);
        }

        public ActionResult UpdateCabinet(string cabinetId)
        {
            // update user select cabinet
            _userService.UpdateCabinet(cabinetId);
            return RedirectToAction("Search");
        }

        public ActionResult Details(string dmstype, string name)
        {
            string cabinetId = _userService.GetCurrentCabinetId();
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(dmstype)
                 && !string.IsNullOrEmpty(cabinetId))
            {
                var isFolder = dmstype.ToUpper().Trim() == "FOLDER" ? true : false;
                string username = User.Identity.GetUserId();

                var dokIndexs = _dokmeeService.GetCabinetIndexes(username, cabinetId).ToList();

                IEnumerable<DokmeeFilesystem> dokmeeFilesystems = _dokmeeService.GetDokmeeFilesystems(username, name, isFolder, cabinetId);

                DetailModel model = new DetailModel()
                {
                    FolderName = name,
                    CabinetId = cabinetId,
                    TableTitles = _mapper.Map<List<DocumentIndex>>(dokIndexs),
                    DocumentItems = _mapper.Map<List<DocumentItem>>(dokmeeFilesystems).Where(t => !t.IsInRecycleBin).ToList()
                };

                model.TableTitles = model.TableTitles.OrderBy(t => t.Order).ToList();
                ViewBag.cabinetId = cabinetId;
                return View(model);
            }
            else return View();
        }

        public async Task<ActionResult> Folder(string id, bool isRoot)
        {
            string username = User.Identity.GetUserId();
            var contents = await _dokmeeService.GetFolderContent(username, id, isRoot);
            IEnumerable<Node> nodes = _mapper.Map<IEnumerable<Node>>(contents);
            return View(nodes);
        }

        [HttpPost]
        public ActionResult UpdateStatus(Dictionary<object, object> args)
        {
            if (args != null && args.ContainsKey("CustomerStatus"))
            {
                string username = User.Identity.GetUserId();
                _dokmeeService.UpdateIndex(username, args);
            }
            return Json(new { });
        }

        //[HttpPost]
        public JsonResult Preview(string id)
        {
            string cabinetId = _userService.GetCurrentCabinetId();

            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(cabinetId))
            {
                string username = User.Identity.GetUserId();
                string url = _dokmeeService.Preview(username, id, cabinetId);
                return Json(new
                {
                    isError = false,
                    url = url
                });
            }
            return Json(new
            {
                isError = true
            });
        }

        [HttpGet]
        public ActionResult Search()
        {
            string username = _userService.GetUserId();
            string cabinetId = _userService.GetCurrentCabinetId();
            var dokIndexs = _dokmeeService.GetCabinetIndexes(username, cabinetId);

            SearchModel model = new SearchModel
            {
                CabinetId = cabinetId,
                TableTitles = _mapper.Map<List<DocumentIndex>>(dokIndexs)
            };

            model.TableTitles = model.TableTitles.OrderBy(t => t.Order).ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult Search(SearchModel model)
        {
            string username = _userService.GetUserId();
            string cabinetId = _userService.GetCurrentCabinetId();

            List<DocumentIndex> conditions =
              model.TableTitles.Where(t => !string.IsNullOrWhiteSpace(t.ValueString)).ToList();

            if (!conditions.Any())
            {
                return View(model);
            }

            // search first condition
            var firstCondition = conditions.First();

            SearchFieldType searchFieldType;
            switch (firstCondition.Type)
            {
                case IndexValueType.Float:
                case IndexValueType.Integer:
                    searchFieldType = SearchFieldType.NumberIndex;
                    break;
                case IndexValueType.DateTime:
                    searchFieldType = SearchFieldType.DateIndex;
                    break;
                default:
                    searchFieldType = SearchFieldType.TextIndex;
                    break;
            }
            var dokResult = _dokmeeService.Search(username, firstCondition.Title, firstCondition.ValueString, cabinetId, searchFieldType).ToList();

            //remove file in Recycle bin
            dokResult = dokResult?.Where(x => x.FullPath != null && !x.FullPath.ToUpper().Contains("RECYCLE BIN")).ToList();

            bool haveDocumentStausNew = model.TableTitles.Any(t =>
              t.Title.ToUpper().Equals("DOCUMENT STATUS") && t.ValueString.Equals(EDocumentStatus.New.ToString()));
            if (haveDocumentStausNew)
            {
                DocumentIndex docStatus = model.TableTitles.Single(t => t.Title.ToUpper().Equals("DOCUMENT STATUS"));
                dokResult.AddRange(_dokmeeService.Search(username, docStatus.Title, string.Empty, cabinetId, searchFieldType).ToList());
            }
            model.DocumentItems = _mapper.Map<List<DocumentItem>>(dokResult).Where(t => !t.IsInRecycleBin).ToList();

            // sorting index result
            foreach (var doc in model.DocumentItems)
            {
                doc.Indexs = doc.Indexs.OrderBy(t => t.Order).ToList();
            }
            model.DocumentItems = model.DocumentItems.DistinctBy(x => x.Id).ToList();

            return View(model);
        }

        [HttpPost]
        public ActionResult Complete(Dictionary<object, object> args)
        {
            if (args != null && args.ContainsKey("NodeId"))
            {
                string cabinetId = _userService.GetCurrentCabinetId();
                string username = User.Identity.GetUserId();
                var tempFolder = _configurationService.TempFolder;
                _dokmeeService.Complete(username, args, tempFolder, cabinetId);
            }
            return Json(new { });
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            log.Error(filterContext.Exception);
            if (filterContext.Exception is CabinetNotSelectedException)
            {
                // Switch to an error view
                filterContext.Result = RedirectToAction("Index");   
                filterContext.ExceptionHandled = true;
                return;
            }

            if (filterContext.Exception is UserNotFoundInTempDbException)
            {
                // Switch to an error view
                filterContext.Result = RedirectToAction("LogOff", "Account");
                filterContext.ExceptionHandled = true;
                return;
            }

            filterContext.ExceptionHandled = true;

            filterContext.Result = this.RedirectToAction("Exception", "Home"); // Redirect to error page.

            base.OnException(filterContext);
        }

        public ActionResult Exception()
        {
            return View();
        }
    }
}