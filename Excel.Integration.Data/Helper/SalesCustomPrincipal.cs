using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace Excel.Integration.Data.Helper
{
    public class SalesCustomPrincipal : IPrincipal
    {
        public bool IsInRole(string role)
        {
            return true;
        }

        public IIdentity Identity { get; private set; }

        public string UserName { get { return ((ICustomIdentity)Identity).UserName; } }
        public string ProfilImage { get { return ((ICustomIdentity)Identity).ProfilImage; } }
        public string Login { get { return ((ICustomIdentity)Identity).Login; } }
        public string Phone { get { return ((ICustomIdentity)Identity).Phone; } }
        public string Email { get { return ((ICustomIdentity)Identity).Email; } }

        public int UserId { get { return ((ICustomIdentity)Identity).UserId; } }
        public string Name { get { return ((ICustomIdentity)Identity).Name; } }
        public string UserType { get { return ((ICustomIdentity)Identity).UserType; } }
        public bool IsAuthenticated { get { return ((ICustomIdentity)Identity).IsAuthenticated; } }
        public string CariKod { get { return ((ICustomIdentity)Identity).CariKod; } }
        public string CariUnvan { get { return ((ICustomIdentity)Identity).CariUnvan; } }
        public string FirmaNo { get { return ((ICustomIdentity)Identity).FirmaNo; } }
        public string OzelKod { get { return ((ICustomIdentity)Identity).OzelKod; } }
        public string YetkiKodu { get { return ((ICustomIdentity)Identity).YetkiKodu; } }
        public string Bolges { get { return ((ICustomIdentity)Identity).YetkiKodu; } }
        public string AmbarNo { get { return ((ICustomIdentity)Identity).AmbarNo; } }

        public SalesCustomPrincipal(ICustomIdentity identity)
        {
            Identity = identity;
        }

        public static ICustomIdentity FromJson(string cookieString)
        {
            //IdentityRepresentation serializedIdentity = null;
            //using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(cookieString)))
            //{
            //    DataContractJsonSerializer jsonSerializer =
            //        new DataContractJsonSerializer(typeof(IdentityRepresentation));
            //    serializedIdentity = jsonSerializer.ReadObject(stream) as IdentityRepresentation;
            //}

            var fromJob = JsonConvert.DeserializeObject<SalesCustomIdendity>(cookieString);

            ICustomIdentity identity = new SalesCustomIdendity
            {
                IsAuthenticated = true,
                Name = fromJob.Name,
                UserId = fromJob.UserId,
                UserName = fromJob.UserName,
                UserType = fromJob.UserType,
                Email = fromJob.Email,
                Login = fromJob.Login,
                Phone = fromJob.Phone,
                ProfilImage = fromJob.ProfilImage,
                FirmaNo = fromJob.FirmaNo,
                CariKod = fromJob.CariKod,
                CariUnvan = fromJob.CariUnvan,
                OzelKod = fromJob.OzelKod,
                YetkiKodu = fromJob.YetkiKodu,
                Bolges = fromJob.Bolges,
                AmbarAdi = fromJob.AmbarAdi,
                AmbarNo = fromJob.AmbarNo
            };

            return identity;
        }

        private static SalesCustomPrincipal LoginCookie(string cookieString)
        {
            try
            {
                if (string.IsNullOrEmpty(cookieString))
                    return null;

                ICustomIdentity identity = SalesCustomPrincipal.FromJson(cookieString);

                var sessionUser = new SalesCustomPrincipal(identity);

                if (identity.IsAuthenticated)
                    HttpContext.Current.User = sessionUser;
                else
                    HttpContext.Current.User = sessionUser;

                return sessionUser;

            }

            catch (Exception exception)
            {
                return null;
            }
        }

        public static SalesCustomPrincipal FromCookie()
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie != null && cookie.Value != null)
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
                var newTicket = FormsAuthentication.RenewTicketIfOld(ticket);
                if (newTicket.Expiration != ticket.Expiration)
                {
                    string encryptedTicket = FormsAuthentication.Encrypt(newTicket);

                    cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                    cookie.Path = FormsAuthentication.FormsCookiePath;
                    HttpContext.Current.Response.Cookies.Add(cookie);
                }

                var result = LoginCookie(ticket.UserData);
                return result;
            }

            return null;
        }
    }
    public class SalesCustomIdendity : ICustomIdentity
    {
        public string Name { get; set; }
        public string AuthenticationType { get; set; }

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string ProfilImage { get; set; }
        public string Login { get; set; }
        public string UserType { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string CariKod { get; set; }
        public string CariUnvan { get; set; }
        public string OzelKod { get; set; }
        public string YetkiKodu { get; set; }
        public string FirmaNo { get; set; }
        public string AmbarNo { get; set; }
        public string AmbarAdi { get; set; }
        public string Bolges { get; set; }


    }
    public class SessionHelper
    {
        public static bool IsLogin => HttpContext.Current.User != null;

        public static SalesCustomPrincipal User => SalesCustomPrincipal.FromCookie();

        public static void LogOut()
        {
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.RemoveAll();
            HttpContext.Current.Session.Abandon();

            HttpCookie cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie != null)
            {
                FormsAuthentication.SignOut();
                HttpContext.Current.Request.Cookies.Remove(FormsAuthentication.FormsCookieName);
            }

            HttpContext.Current.User = null;
            HttpContext.Current.Session["User"] = null;
        }

        public static void Login(string userName, string password)
        {
            bool rememberMe = true;

            HttpContext.Current.Session["User"] = new { Name = userName };
            HttpContext.Current.Session.Timeout = 360;

            var customIdentity = new SalesCustomIdendity
            {
                Name = userName,
                IsAuthenticated = true,
                AuthenticationType = "Custom",
                UserId = 0,
                UserName = userName,

                ProfilImage = "",
            };

            HttpContext.Current.User = new SalesCustomPrincipal(customIdentity);

            var customIdentityJson = JsonConvert.SerializeObject(customIdentity);

            var formsAuthenticationTicket = new FormsAuthenticationTicket(
                1,
                customIdentity.Name,
                DateTime.Now,
                DateTime.Now.AddHours(6),
                rememberMe,
                customIdentityJson,
                FormsAuthentication.FormsCookiePath);

            var encryptedTicket = FormsAuthentication.Encrypt(formsAuthenticationTicket);

            var httpCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
            {
                Path = FormsAuthentication.FormsCookiePath
            };

            if (rememberMe)
                httpCookie.Expires = DateTime.Now.AddYears(3);

            HttpContext.Current.Response.Cookies.Add(httpCookie);
        }
    }
}
