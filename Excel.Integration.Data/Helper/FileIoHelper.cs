using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Excel.Integration.Data.Helper
{
    public class FileIoHelper : IDisposable
    {
        public static void SyncFile(HttpPostedFileBase httpPostedFile, string fileName, string directory, string fileHeadName)
        {
            if (httpPostedFile == null)
                return;
            if (httpPostedFile.ContentLength <= 0)
                return;
            var path = HttpContext.Current.Server.MapPath(Path.Combine(directory, fileName));
            try
            {

                RemoveFiles(fileHeadName);
                httpPostedFile.SaveAs(path);

            }
            catch (Exception exception)
            {
                throw new Exception("Add file error.", exception);
            }
        }

        private static void RemoveFiles(string file)
        {
            var fileName = string.Format("{0}.{1}", file, "xls");
            var path1 = GetProjectFilePath(fileName, "//Content//");
            var fileName2 = string.Format("{0}.{1}", file, "xlsx");
            var path2 = GetProjectFilePath(fileName2, "//Content//");
            Remove(path1);
            Remove(path2);
        }
        public static string GetProjectFilePath(string fileName, string directory)
        {
            return HttpContext.Current.Server.MapPath(Path.Combine(directory, fileName));
        }
        public static string GetCommonFileName(string fileHeadName)
        {
            var fileName = string.Format("{0}.{1}", fileHeadName, "xls");
            var path1 = GetProjectFilePath(fileName, "//Content//");
            if (File.Exists(path1))
                return path1;

            var fileName2 = string.Format("{0}.{1}", fileHeadName, "xlsx");
            var path2 = GetProjectFilePath(fileName2, "//Content//");
            return File.Exists(path2) ? path2 : null;
        }

        public static DataTable ReadSycnedExcel(bool hasHeader, string fileHeadName)
        {
            var path = GetCommonFileName(fileHeadName);

            if (path == null) return null;

            using (var pck = new ExcelPackage())
            {
                using (var stream = File.OpenRead(path))
                {
                    pck.Load(stream);
                }
                var ws = pck.Workbook.Worksheets.First();
                var tbl = new DataTable();
                foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                {
                    tbl.Columns.Add(hasHeader ? firstRowCell.Text.Trim().ToUpper() : string.Format("Column {0}", firstRowCell.Start.Column));
                }
                var startRow = hasHeader ? 2 : 1;
                for (var rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                    var row = tbl.Rows.Add();
                    foreach (var cell in wsRow.ToList())
                    {
                        row[cell.Start.Column - 1] = cell.Text.Trim();
                    }
                }

                return tbl;
            }
        }

        public static void AddSyncFile(HttpPostedFileBase fileBase, string file)
        {
            try
            {
                var fileName = string.Format("{0}{1}", file, Path.GetExtension(fileBase.FileName));
                SyncFile(fileBase, fileName, "//Content//", file);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public static List<DataRow> ReadFile(string file)
        {
            var readedList = ReadSycnedExcel(true, file).AsEnumerable().ToList();
            return readedList;
        }

        public static void Remove(string path)
        {
            try
            {
                if (!File.Exists(path))
                    return;

                File.Delete(path);

            }
            catch (Exception exception)
            {
                throw new Exception("Add file error.", exception);
            }
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }

}
