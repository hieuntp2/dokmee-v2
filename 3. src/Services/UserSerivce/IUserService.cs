using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.UserSerivce
{
    public interface IUserService
    {
        bool IsAuthenticate();

        string GetUserId();
        void UpdateCabinet(string cabinetId);
        string GetCurrentCabinetId();
    }
}
