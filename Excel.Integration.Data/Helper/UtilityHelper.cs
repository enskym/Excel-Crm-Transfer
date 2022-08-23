using Excel.Integration.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace Excel.Integration.Data.Helper
{
    public static class UtilityHelper
    {
        /// <summary>
        /// it returns url query value.
        /// </summary>
        /// <returns></returns>
        public static string SeoUrlControl(this string title, bool? isUpper = null)
        {
            if (title.ToControl()) return "";

            title = title.ToUpper().SetTurkishCharacterToEnglish();

            title = title.Replace('.', '-');
            // remove entities
            title = Regex.Replace(title, @"&\w+;", "");
            // remove anything that is not letters, numbers, dash, or space
            title = Regex.Replace(title, @"[^A-Za-z0-9\-\s]", "");
            // remove any leading or trailing spaces left over
            title = title.Trim();
            // replace spaces with single dash
            title = Regex.Replace(title, @"\s+", "-");
            // if we end up with multiple dashes, collapse to single dash            
            title = Regex.Replace(title, @"\-{2,}", "-");

            //   title = title.Replace(" ", "-");

            // make it all lower case
            title = title.ToLower();
            // if it's too long, clip it
            if (title.Length > 80)
                title = title.Substring(0, 79);

            // remove trailing dash, if there is one
            // if (title.EndsWith("-"))
            //   title = title.Substring(0, title.Length - 1);

            if (isUpper.HasValue && isUpper.Value)
                return title.ToUpper();

            return title.ToLower().SetTurkishCharacterToEnglish();
        }


        public static bool IsValidEmail(this string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                    RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    var domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }


        public static string SetTurkishCharacterToEnglish(this string title)
        {
            if (title.ToControl()) return string.Empty;

            const string strTr = "ığüşöçĞÜŞİÖÇ";
            const string strEn = "igusocGUSIOC";
            for (var i = 0; i < strEn.ToArray().Length; i++)
            {
                title = title.Replace(strTr.ToArray()[i], strEn.ToArray()[i]);
            }

            return title;
        }

        public static string Connection
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["DataContext"].ConnectionString;
            }
        }
        public static string LogoConnection
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["LogoDbContext"].ConnectionString;
            }
        }

        public static string GetAppSetting(this string name)
        {
            return WebConfigurationManager.AppSettings[name];
        }


        /// <summary>
        /// Telefon no () + - işaretleri kaldırır. Saf haline çevirir.
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public static string ClearPhoneFormat(this string phoneNumber)
        {
            if (phoneNumber.ToControl())
                return null;

            return phoneNumber.Replace(@"(", string.Empty)
              .Replace(@")", string.Empty)
              .Replace(@"+", string.Empty)
              .Replace(@"-", string.Empty)
              .Replace(" ", string.Empty)
              .Trim();
        }
        public static string AppSettingsVal(this string val)
        {
            try
            {
                return ConfigurationManager.AppSettings[val];
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// it returns name of enum value description.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string GetDescriptionString(this Enum val)
        {
            try
            {
                var attributes =
                    (DescriptionAttribute[])
                        val.GetType().GetField(val.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
                return attributes.Length > 0 ? attributes[0].Description : PascalCaseToPrettyString(val.ToString());
            }
            catch (Exception)
            {
                return PascalCaseToPrettyString(val.ToString());
            }
        }
        public static string PascalCaseToPrettyString(this string s)
        {
            return Regex.Replace(s, @"(\B[A-Z]|[0-9]+)", " $1");
        }
        public static string RandomString(int size)
        {
            var builder = new StringBuilder();
            var random = new Random();
            for (var i = 0; i < size; i++)
            {
                var ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }
        public static int Random8Number()
        {
            var rnd = new Random();
            var myRandomNo = rnd.Next(10000000, 99999999);
            return myRandomNo;
        }

        public static string Right(this string value, int length)
        {
            return value.Substring(value.Length - length);
        }

        /// <summary>
        /// Generates random string value . As you wanted length. Max length is 255 !;
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string KeyGenerator(byte length)
        {
            var value1 = Random8Number();

            var key = string.Format("{0}", value1);

            using (var md5Hasher = MD5.Create())
            {
                var data = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(key));
                var result = data.Aggregate(string.Empty, (current, b) => current + b.ToString());
                return result.Substring(0, length);
            }
        }
        public static string KeyGeneratorRandom(byte length)
        {
            var value1 = Random8Number();

            var key = string.Format("{0}", value1);

            return key.Substring(0, length);
        }

        /// <summary>
        /// String is null or empty control . it returns bool.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool ToControl(this string text)
        {
            return string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text);
        }

        public static string ToUpperCase(this string text)
        {
            if (text.ToControl()) return string.Empty;
            return text.ToUpper();
        }

        public static string ToLowerCase(this string text)
        {
            if (text.ToControl()) return string.Empty;
            return text.ToLower();
        }
        public static decimal MathRound(this decimal val, int num)
        {
            return Math.Round(val, num, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// params objects controls .it returns bool. 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="equalsTo"></param>
        /// <returns></returns>
        public static bool ToControl(this object value, params object[] equalsTo)
        {
            return equalsTo.Any(x => x.Equals(value));
        }
        /// <summary>
        /// object value converts to byte .
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte ToByte(this object value)
        {
            return (byte)value;
        }
        /// <summary>
        /// object value converts to int.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToInt(this object value)
        {
            return (int)value;
        }
        /// <summary>
        /// object converter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T ConvertTo<T>(this object obj)
        {
            return (T)Convert.ChangeType(obj, typeof(T));
        }
        /// <summary>
        /// Guid value converts to upper string.
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static string ToGuidUpper(this Guid guid, string prefix)
        {
            return prefix.ToControl() ? guid.ToString().ToUpper() : string.Format("{0}{1}", prefix, guid.ToString().ToUpper());
        }

        /// <summary>
        /// Params long values calculates and return total numbers as long type.
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        public static long TotalParams(params long[] numbers)
        {
            return numbers.Aggregate<long, long>(0, (current, number) => current + number);
        }
        /// <summary>
        /// Decimal değeri virgülden sonra istenen basamağa kadar keser.Yuvarlama yapmaz.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetDecimalFormat(this decimal val, int length)
        {
            var d = val.ToString(CultureInfo.InvariantCulture);
            char split;
            if (d.Contains(','))
                split = ',';
            else if (d.Contains('.'))
                split = '.';
            else
                return d;

            var splitt = d.Split(split);
            var f = splitt[1].ToArray();
            var size = length;
            if (f.Length <= length)
                return d;
            var result = string.Empty;
            for (var i = 0; i < size; i++)
            {
                result = result + f[i];
            }
            return string.Format("{0}{1}{2}", splitt[0], split, result);
        }
        /// <summary>
        /// Virgulden sonra sayıyı keser.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static decimal GetDecimalCut(this decimal val, int length)
        {
            return Math.Round(val, length);
        }

        public static string ClientIp
        {
            get
            {
                var ip = HttpContext.Current.Request.Headers["Client-IPS"];
                if (!String.IsNullOrEmpty(ip))
                    return ip;
                return HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
        }

        /// <summary>
        /// Datetime string formats return  dd/MM/yyyy HH:mm
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToStringFormatDate(this DateTime date)
        {
            return date.ToString("dd'/'MM'/'yyyy HH:mm");
        }
        /// <summary>
        /// Date time string formats return dd/MM/yyy 
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToStringFormatDateShort(this DateTime date)
        {
            return date.ToString("dd'/'MM'/'yyyy");
        }

        public static string ToStringFormatDateShort(this DateTime date, char sp)
        {
            return date.ToString("dd'" + sp + "'MM'" + sp + "'yyyy");
        }

        /// <summary>
        /// 2 parameter show property shows one.
        /// </summary>
        /// <param name="param"></param>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static T ShowProperty<T>(this bool param, object first, object second)
        {
            if (param)
            {
                if (first == null)
                    return default(T);
                return (T)first;
            }

            if (second == null)
                return default(T);
            return (T)second;
        }

        public static string SeoLinkParam(string title, string id)
        {
            return title.SeoUrlControl(false) + "-" + id;
        }
        public static string SeoLinkParam(string title, int id)
        {
            return title.SeoUrlControl(false) + "-" + id;
        }

        /// <summary>
        /// Verilen uzunluğa göre string keser ve sonuna  ...(3 Nokta) bırakır.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string SubStringCut(this string title, int length)
        {
            if (title.ToControl())
                return title;

            return title.Length > length ? title.Substring(0, length) + "..." : title;
        }

        /// <summary>
        /// Verilen uzunluğa göre değeri keser.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string StringCut(this string title, int length)
        {
            if (title.ToControl())
                return title;

            return title.Length > length ? title.Substring(0, length) : title;
        }

        /// <summary>
        /// String değeri ilk harfi büyük gerisini küçük harf yapar .28.02.2018 Menzeher 
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public static string ToTitleCase(this string title)
        {
            if (title.ToControl()) return title;
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(title.ToLowerCase());
        }

        public static string TitleCaseString(this string s)
        {
            if (s.ToControl()) return s;

            String[] words = s.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length == 0) continue;

                Char firstChar = Char.ToUpper(words[i][0]);
                String rest = "";
                if (words[i].Length > 1)
                {
                    rest = words[i].Substring(1).ToLower();
                }
                words[i] = firstChar + rest;
            }

            return String.Join(" ", words);
        }

        public static string Layout
        {
            get { return "~/Views/Shared/_Layout.cshtml"; }
        }



        //public static List<MonthListDto> GetMextMonthListForPlan()
        //{

        //    var result = new List<MonthListDto>();
        //    var today = DateTime.Today;

        //    var nextmonth = today.AddMonths(1);

        //    var aylar = GetMonths();

        //    var add = new MonthListDto
        //    {
        //        Year = today.Year,
        //        Month = today.Month,
        //        Title = today.Year + " " + aylar.FirstOrDefault(a => a.id == today.Month.ToString()).text
        //    };
        //    result.Add(add);

        //    var add2 = new MonthListDto
        //    {
        //        Year = nextmonth.Year,
        //        Month = nextmonth.Month,
        //        Title = nextmonth.Year + " " + aylar.FirstOrDefault(a => a.id == nextmonth.Month.ToString()).text
        //    };

        //    result.Add(add2);


        //    return result;

        //}
        public static string UserTypeDesc(this string userType)
        {
            if (userType == "1")
            {
                return "YÖNETİCİ";
            }
            else if (userType == "2")
            {
                return "BACK OFİS ÇALIŞANI";
            }
            else if (userType == "3")
            {
                return "SATIŞ ELEMANI";
            }

            return "";
        }

        public static string PlanStatusColor(this string status)
        {
            if (status == "0")
            {
                return "warning";
            }
            else if (status == "1")
            {
                return "info";
            }
            else if (status == "2")
            {
                return "danger";
            }
            else if (status == "3")
            {
                return "success";
            }

            return "";
        }

        public static string PlanStatusDesc(this string status)
        {
            if (status == "0")
            {
                return "Onay Bekliyor";
            }
            else if (status == "1")
            {
                return "Onaylandı";
            }
            else if (status == "2")
            {
                return "Görüşme İptal Edildi";
            }
            else if (status == "3")
            {
                return "Görüşme Tamamlandı";
            }

            return "";
        }

        public static string DayDesc(DayOfWeek day)
        {
            if (day == DayOfWeek.Monday)
            {
                return "Pazartesi";
            }
            else if (day == DayOfWeek.Tuesday)
            {
                return "Salı";
            }
            else if (day == DayOfWeek.Wednesday)
            {
                return "Çarşamba";
            }
            else if (day == DayOfWeek.Thursday)
            {
                return "Perşembe";
            }
            else if (day == DayOfWeek.Friday)
            {
                return "Cuma";
            }
            else if (day == DayOfWeek.Saturday)
            {
                return "Cumartesi";
            }
            else if (day == DayOfWeek.Sunday)
            {
                return "Pazar";
            }

            return "";
        }
        //public static string GetRazorViewAsString(this object model, string filePath)
        //{
        //    var st = new StringWriter();
        //    var context = new HttpContextWrapper(HttpContext.Current);
        //    var routeData = new RouteData();
        //    var controllerContext = new ControllerContext(new RequestContext(context, routeData), new BaseController());
        //    var razor = new RazorView(controllerContext, filePath, null, false, null);
        //    razor.Render(new ViewContext(controllerContext, razor, new ViewDataDictionary(model), new TempDataDictionary(), st), st);
        //    return st.ToString();
        //}

        public static DataTable ToDataTable<TEntity>(this object[,,] dataTableColumns, TEntity[] data)
        {
            var result = new DataTable();
            result.Columns.Add(new DataColumn("#", typeof(int)));
            for (var i = 0; i < dataTableColumns.GetLength(0); i++)
            {
                result.Columns.Add(new DataColumn((string)dataTableColumns[i, 0, 0], (Type)dataTableColumns[i, 0, 1]));
            }
            var properties = TypeDescriptor.GetProperties(typeof(TEntity));
            for (int i = 0; i < data.Length; i++)
            {
                var dataRow = result.NewRow();
                dataRow["#"] = i + 1;
                for (var y = 0; y < dataTableColumns.GetLength(0); y++)
                {
                    var info = dataTableColumns[y, 0, 2] as string[];
                    if (info != null)
                    {
                        var columnInfo = info;
                        var o = properties[columnInfo[0]].GetValue(data[i]);
                        var value = o != null && (bool)o;
                        dataRow[(string)dataTableColumns[y, 0, 0]] = value ? columnInfo[1] : columnInfo[2];
                    }
                    else
                    {
                        var value = properties[(string)dataTableColumns[y, 0, 2]].GetValue(data[i]);
                        dataRow[(string)dataTableColumns[y, 0, 0]] = value ?? DBNull.Value;
                    }
                }
                result.Rows.Add(dataRow);
            }

            return result;
        }

        public static void ExportToExcel(this DataTable source, string workSheetName, string fileName)
        {
            //using (var excelPackage = new ExcelPackage())
            //{
            //    var excelWorksheet = excelPackage.Workbook.Worksheets.Add(workSheetName);
            //    excelWorksheet.Cells["A1"].LoadFromDataTable(source, true);

            //    HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //    HttpContext.Current.Response.AddHeader("content-disposition", string.Concat("attachment;  filename = ", fileName, ".xlsx"));
            //    HttpContext.Current.Response.BinaryWrite(excelPackage.GetAsByteArray());
            //    HttpContext.Current.Response.End();
            //    HttpContext.Current.Response.Flush();

            //}
        }

        public static DataTable ToDataTable<TEntity>(this IList<TEntity> data)
        {
            var dataTable = new DataTable();
            var properties = TypeDescriptor.GetProperties(typeof(TEntity));

            foreach (PropertyDescriptor prop in properties)
                dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);

            foreach (TEntity item in data)
            {
                var dataRow = dataTable.NewRow();
                foreach (PropertyDescriptor propertyDescriptor in properties)
                    dataRow[propertyDescriptor.Name] = propertyDescriptor.GetValue(item) ?? DBNull.Value;
                dataTable.Rows.Add(dataRow);
            }
            return dataTable;
        }

        public static DataTable ToDataTable<TEntity>(this IList<string> headNames, TEntity[] data)
        {
            var result = new DataTable();
            var properties = TypeDescriptor.GetProperties(typeof(TEntity));

            foreach (PropertyDescriptor prop in properties)
                result.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);

            foreach (var item in data)
            {
                var dataRow = result.NewRow();

                foreach (PropertyDescriptor propertyDescriptor in properties)
                    dataRow[propertyDescriptor.Name] = propertyDescriptor.GetValue(item) ?? DBNull.Value;

                result.Rows.Add(dataRow);
            }

            for (var p = 0; p < result.Columns.Count; p++)
            {
                result.Columns[p].ColumnName = headNames[p];
            }

            return result;
        }


        public static string TarihCevirXml(this string deger)
        {
            if (deger.ToControl())
            {
                return null;
            }

            if (deger.Contains("/"))
            {
                var arr = deger.Split('/').ToList();

                return arr[1] + "." + arr[0] + "." + "20" + arr[2];
            }

            return null;

        }
        public static DateTime ParseDate(this string date)
        {
            if (date.Contains(" "))
            {
                var arr = date.Split(' ')[0].Trim().Split('.');

                if (arr.Length == 3)
                {
                    var day = arr[1];
                    if (day.StartsWith("0"))
                    {
                        day = day.ToArray()[1].ToString();
                    }

                    return new DateTime(int.Parse("20" + arr[2]), int.Parse(arr[0]), int.Parse(day));
                }

                return new DateTime(int.Parse(arr[2]), int.Parse(arr[1]), int.Parse(arr[0]));
            }
            else
            {
                var arr = date.Trim().Split('.');

                if (arr.Length == 3)
                {
                    var day = arr[0];
                    if (day.StartsWith("0"))
                    {
                        day = day.ToArray()[1].ToString();
                    }

                    return new DateTime(int.Parse(arr[2]), int.Parse(arr[1]), int.Parse(day));
                }

                return new DateTime(int.Parse(arr[2]), int.Parse(arr[1]), int.Parse(arr[0]));
            }


        }






    }

}
