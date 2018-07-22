using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Services.AuthService.Models;

namespace Web.ViewModels.Home
{
  public class IndexModel
  {
    public IEnumerable<Cabinet> Cabinets { get; set; }
  }
}