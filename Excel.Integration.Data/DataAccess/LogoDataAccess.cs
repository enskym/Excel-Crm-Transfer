using Dapper;
using Excel.Integration.Data.Helper;
using Excel.Integration.Data.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excel.Integration.Data.DataAccess
{
    public class LogoDataAccess
    {
        public static SevkiyatDto SevkiyatAdresiSorgu(string cariKodu, string adresKodu)
        {
            try
            {
                using (var db = new SqlConnection(UtilityHelper.Connection))
                {
                    var sql = "SELECT TOP 1 * FROM [dbo].[View_CariSevkAdresler] where [CARI_KODU] = @cariKodu AND  [ADRES_KODU] = @adresKodu ";
                    return db.Query<SevkiyatDto>(sql, new { cariKodu,adresKodu }).FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                return null;
            }

        }
    }
}
