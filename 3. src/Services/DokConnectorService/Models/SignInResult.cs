using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dms.Contracts;
using Dokmee.Dms.Connector.Advanced.Core.Data;

namespace DokCapture.ServicenNetFramework.DokConnectorService.Models
{
  public class DokmeeLoginResult
  {
    public DokmeeCabinetResult CabinetResult { get; set; }
    public SignInResult Result { get; set; }
    public IEnumerable<DokmeeCabinet> Cabinets { get; set; }
    public UserInfo UserInfo { get; set; }
    public string Message { get; set; }
  }
  public enum SignInResult
  {
    Success,
    Fail
  }
}
