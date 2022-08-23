using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Excel.Integration.Data.Helper
{
    public interface ICustomIdentity : IIdentity
    {
        int UserId { get; set; }
        string Name { get; set; }
        bool IsAuthenticated { get; set; }
        string UserName { get; set; }
        string Email { get; set; }
        string Phone { get; set; }
        string ProfilImage { get; set; }
        string Login { get; set; }
        string UserType { get; set; }
        string CariKod { get; set; }
        string CariUnvan { get; set; }
        string OzelKod { get; set; }
        string YetkiKodu { get; set; }
        string FirmaNo { get; set; }
        string AmbarNo { get; set; }
        string AmbarAdi { get; set; }

    }
}

