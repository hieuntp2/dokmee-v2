using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DokCapture.ServicenNetFramework.Auth;
using Dokmee.Dms.Connector.Advanced.Core.Data;
using Microsoft.AspNet.Identity;
using Services.AuthService;
using Services.AuthService.Models;
using Services.SessionHelperService;
using Web.ViewModels.Home;
namespace Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        
        private IDokmeeService _dokmeeService;
        private IMapper _mapper;
        private ISessionHelperService _sessionHelperService;
        public HomeController(IDokmeeService dokmeeService, IMapper mapper, ISessionHelperService sessionHelperService)
        {
            _dokmeeService = dokmeeService;
            _mapper = mapper;
            _sessionHelperService = sessionHelperService;
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
            }catch (Exception ex)
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

        public ActionResult CabinetDetail(string cabinetId)
        {
            string username = User.Identity.GetUserId();
            IEnumerable<DmsNode> cabinetContent = _dokmeeService.GetCabinetContent(cabinetId, username);
            IEnumerable<Node> nodes = _mapper.Map<IEnumerable<Node>>(cabinetContent);
            ViewBag.cabinetId = cabinetId;
            return View(nodes);
        }
    }
}