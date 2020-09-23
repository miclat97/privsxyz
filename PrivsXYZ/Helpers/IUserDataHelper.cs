using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrivsXYZ.Helpers
{
    public interface IUserDataHelper
    {
        Tuple<string, string, string> GetUserData();
    }
}
