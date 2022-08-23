using Excel.Integration.Data.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityObjects;

namespace Excel.Integration.Data.Tiger
{
    public class App
    {
        private static List<UnityApplication> _instances = null;

        private static UnityApplication Instance(int firmNo)
        {
            UnityApplication app = null;
            if (_instances == null || !_instances.Any())
            {
                _instances = new List<UnityApplication>();
                app = new UnityApplication();
                _instances.Add(app);
            }
            else
            {
                var first = _instances.FirstOrDefault(a => a.CurrentFirm == firmNo);
                if (first == null)
                {
                    app = new UnityApplication();
                    _instances.Add(app);
                }
                else
                {
                    app = first;
                }
            }

            return app;
        }

        protected UnityApplication TigerInstance(int firmNo)
        {

            string userName = "LOGO.USERNAME".GetAppSetting();

            if (string.IsNullOrEmpty(userName))
            {
                throw new Exception("Lisans sorunu.Lütfen ilgili birim ile görüşün.Ms= 1");
            }

            string password = "LOGO.PASSWORD".GetAppSetting();

            if (string.IsNullOrEmpty(password))
            {
                throw new Exception("Lisans sorunu .Lütfen ilgili birim ile görüşün..Ms= 2");
            }
            try
            {
                var app = Instance(firmNo);
                try
                {
                    if (app.Connected)
                    {
                        // app.Disconnect();
                    }

                }
                catch (COMException ex)
                {
                    app = Instance(firmNo);
                }

                if (!app.LoggedIn)
                {
                    var vry = app.Login(userName, password, firmNo);
                    if (!vry)
                    {
                        throw new Exception("Lisans sorunu .Lütfen ilgili birim ile görüşün..Ms= 3");
                    }
                    else
                    {

                    }
                }

                return app;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
