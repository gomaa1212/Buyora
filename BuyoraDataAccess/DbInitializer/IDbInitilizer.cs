using System;
using System.Collections.Generic;
using System.Text;

namespace BuyoraDataAccess.DbInitializer
{
    public interface IDbInitilizer
    {
        Task InitilizeAsync();
    }
}
