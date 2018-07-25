using System;
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

    public ActionResult CabinetDetail(string cabinetId)
    {
      string username = User.Identity.GetUserId();
      IEnumerable<DmsNode> cabinetContent = _dokmeeService.GetCabinetContent(cabinetId, username);
      IEnumerable<Node> nodes = _mapper.Map<IEnumerable<Node>>(cabinetContent);
      ViewBag.cabinetId = cabinetId;
      return View(nodes);
    }

    public ActionResult Details(string cabinetId, string dmstype, string name)
    {
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
    public ActionResult Search(string cabinetId)
    {
      string username = _userService.GetUserId();
      //  var dokIndexs = _dokmeeService.GetCabinetIndexes(username, cabinetId);

      SearchModel model = new SearchModel()
      {
        CabinetId = cabinetId
      };

      model.TableTitles.Add(new DocumentIndex()
      {
        Id = "0",
        Order = 1,
        Title = "Customer ID",
        Type = IndexValueType.Integer
      });
      model.TableTitles.Add(new DocumentIndex()
      {
        Id = "1",
        Order = 2,
        Title = "Customer Name",
        Type = IndexValueType.String
      });
      model.TableTitles.Add(new DocumentIndex()
      {
        Id = "2",
        Order = 3,
        Title = "SIGN UP",
        Type = IndexValueType.DateTime
      });


      model.DocumentItems.Add(new DocumentItem()
      {
        Id = "aa1",
        Title = "Forder 1",
        Indexs = new List<DocumentIndex>(),
        IsFolder = true,
        IsRoot = true,
        Type = "folder"
      });
      model.DocumentItems.Add(new DocumentItem()
      {
        Id = "aa2",
        Title = "Forder 2",
        Indexs = new List<DocumentIndex>(),
        IsFolder = true,
        IsRoot = true,
        Type = "folder"
      });
      model.DocumentItems.Add(new DocumentItem()
      {
        Id = "aa3",
        Title = "Forder 3",
        Indexs = new List<DocumentIndex>(),
        IsFolder = true,
        IsRoot = true,
        Type = "folder"
      });

      model.DocumentItems.Add(new DocumentItem()
      {
        Id = "bb1",
        Title = "file 1",
        Indexs = new List<DocumentIndex>(),
        IsFolder = false,
        IsRoot = false,
        Type = "pdf"
      });
      model.DocumentItems.Add(new DocumentItem()
      {
        Id = "bb1",
        Title = "file 2",
        Indexs = new List<DocumentIndex>(),
        IsFolder = false,
        IsRoot = false,
        Type = "pdf"
      });
      model.DocumentItems.Add(new DocumentItem()
      {
        Id = "bb3",
        Title = "file 3",
        Indexs = new List<DocumentIndex>(),
        IsFolder = false,
        IsRoot = false,
        Type = "png"
      });
      return View(model);
    }

    [HttpPost]
    public ActionResult Search(SearchModel model)
    {
      string username = _userService.GetUserId();

      //  var dokIndexs = _dokmeeService.GetCabinetIndexes(username, cabinetId);

     

      model.TableTitles.Add(new DocumentIndex()
      {
        Order = 1,
        Title = "Customer ID",
        Type = IndexValueType.Integer
      });
      model.TableTitles.Add(new DocumentIndex()
      {
        Order = 2,
        Title = "Customer Name",
        Type = IndexValueType.String
      });
      model.TableTitles.Add(new DocumentIndex()
      {
        Order = 3,
        Title = "SIGN UP",
        Type = IndexValueType.DateTime
      });


      model.DocumentItems.Add(new DocumentItem()
      {
        Id = "aa1",
        Title = "Forder 1",
        Indexs = new List<DocumentIndex>(),
        IsFolder = true,
        IsRoot = true,
        Type = "folder"
      });
      model.DocumentItems.Add(new DocumentItem()
      {
        Id = "aa2",
        Title = "Forder 2",
        Indexs = new List<DocumentIndex>(),
        IsFolder = true,
        IsRoot = true,
        Type = "folder"
      });
      model.DocumentItems.Add(new DocumentItem()
      {
        Id = "aa3",
        Title = "Forder 3",
        Indexs = new List<DocumentIndex>(),
        IsFolder = true,
        IsRoot = true,
        Type = "folder"
      });

      model.DocumentItems.Add(new DocumentItem()
      {
        Id = "bb1",
        Title = "file 1",
        Indexs = new List<DocumentIndex>(),
        IsFolder = false,
        IsRoot = false,
        Type = "pdf"
      });
      model.DocumentItems.Add(new DocumentItem()
      {
        Id = "bb1",
        Title = "file 2",
        Indexs = new List<DocumentIndex>(),
        IsFolder = false,
        IsRoot = false,
        Type = "pdf"
      });
      model.DocumentItems.Add(new DocumentItem()
      {
        Id = "bb3",
        Title = "file 3",
        Indexs = new List<DocumentIndex>(),
        IsFolder = false,
        IsRoot = false,
        Type = "png"
      });
      return View(model);
    }
  }
}