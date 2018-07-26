﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DokCapture.ServicenNetFramework.Auth;
using Dokmee.Dms.Advanced.WebAccess.Data;
using Dokmee.Dms.Connector.Advanced.Core.Data;
using Microsoft.AspNet.Identity;
using Services.AuthService;
using Services.AuthService.Models;
using Services.SessionHelperService;
using Services.UserSerivce;
using Web.ViewModels.Home;
namespace Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        private IDokmeeService _dokmeeService;
        private IMapper _mapper;
        private ISessionHelperService _sessionHelperService;
        private IUserService _userService;

        public HomeController(IDokmeeService dokmeeService, IMapper mapper, ISessionHelperService sessionHelperService, IUserService userService)
        {
            _dokmeeService = dokmeeService;
            _mapper = mapper;
            _sessionHelperService = sessionHelperService;
            _userService = userService;
        }

        public ActionResult Index()
        {
            try
            {
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

        public ActionResult AfterMyActionResult(string username, string password, ConnectorType loginType)
        {
            Session["abc"] = "123";
            _sessionHelperService.Username = username;
            _sessionHelperService.Password = password;
            _sessionHelperService.ConnectorType = loginType;
            return RedirectToAction("Index");
        }

        // dont remove cabinetId, this is save selected cabinet to temp db
        public ActionResult CabinetDetail(string cabinetId)
        {
            // update user select cabinet
            _userService.UpdateCabinet(cabinetId);
            string username = User.Identity.GetUserId();
            IEnumerable<DmsNode> cabinetContent = _dokmeeService.GetCabinetContent(cabinetId, username);
            IEnumerable<Node> nodes = _mapper.Map<IEnumerable<Node>>(cabinetContent);
            ViewBag.cabinetId = cabinetId;
            return View(nodes);
        }

        public ActionResult Details(string dmstype, string name)
        {
            string cabinetId = _userService.GetCurrentCabinetId();
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(dmstype)
                   && !string.IsNullOrEmpty(cabinetId))
            {
                var isFolder = dmstype.ToUpper().Trim() == "FOLDER" ? true : false;
                string username = User.Identity.GetUserId();
                DetailModel model = new DetailModel();
                IEnumerable<DokmeeFilesystem> dokmeeFilesystems = _dokmeeService.GetDokmeeFilesystems(username, name, isFolder, cabinetId);
                model.dokmeeFilesystems = dokmeeFilesystems;
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
        public ActionResult Preview(string id, string cabinetId)
        {
            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(cabinetId))
            {
                string username = User.Identity.GetUserId();
                _dokmeeService.Preview(username, id, cabinetId);
            }
            return Json(new { });
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

            return View(model);
        }

        [HttpPost]
        public ActionResult Search(SearchModel model)
        {
            string username = _userService.GetUserId();
            string cabinetId = _userService.GetCurrentCabinetId();
           // var dokIndexs = _dokmeeService.GetCabinetIndexes(username, cabinetId);
            

            foreach (var item in model.TableTitles)
            {
                if(item.Value == null) continue;
                string searchValue = (item.Value as String[])?[0]; 
                if(string.IsNullOrWhiteSpace(searchValue)) continue;
                SearchFieldType searchFieldType;
                switch (item.Type)
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
                        searchValue = (item.Value as String[])?[0];
                        break;
                }
                var dokResult = _dokmeeService.Search(username, item.Title, searchValue, cabinetId, searchFieldType);
                model.DocumentItems.AddRange(_mapper.Map<List<DocumentItem>>(dokResult));
            }

          

            return View(model);
        }
    }
}