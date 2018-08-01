using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.TempDbService.Exceptions
{
  public class UserNotFoundInTempDbException : Exception
  {
    public UserNotFoundInTempDbException()
    {
    }

    public UserNotFoundInTempDbException(string message)
      : base(message)
    {
    }

    public UserNotFoundInTempDbException(string message, Exception inner)
      : base(message, inner)
    {
    }
  }
}
