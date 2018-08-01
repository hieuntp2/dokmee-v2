using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.UserSerivce.Exceptions
{
  public class CabinetNotSelectedException : Exception
  {
    public CabinetNotSelectedException()
    {
    }

    public CabinetNotSelectedException(string message)
      : base(message)
    {
    }

    public CabinetNotSelectedException(string message, Exception inner)
      : base(message, inner)
    {
    }
  }
}
