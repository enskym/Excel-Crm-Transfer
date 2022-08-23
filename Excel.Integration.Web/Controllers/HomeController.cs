using Excel.Integration.Data.Helper;
using Excel.Integration.Data.Models;
using Excel.Integration.Data.Tiger;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Excel.Integration.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();

        }

        public ActionResult Start()
        {
            ViewBag.Model = new TranspetExcelDto();
            return View();

        }

        [HttpPost]
        public ActionResult FileRead(HttpPostedFileBase file)
        {
            if (file == null)
            {
                ViewBag.Error = "Dosya seçimi zorunludur.";
                return View("ExcelDealer");
            }

            try
            {
                var fileName = "ExcelFile";

                FileIoHelper.AddSyncFile(file, fileName);

                var readedList = FileIoHelper.ReadFile(fileName);
                var listt = readedList.Select(row => new TranspetExcelDto
                {
                    Tarih = row.Field<string>("Tarih"),
                    Plaka1 = row.Field<string>("Plaka1"),
                    Plaka2 = row.Field<string>("Plaka2"),
                    Adi = row.Field<string>("Adi"),
                    Birim = row.Field<string>("Birim"),
                    CariKodu = row.Field<string>("CariKodu"),
                    CariUnvan = row.Field<string>("CariUnvan"),
                    Miktar = row.Field<string>("Miktar"),
                    SatirAciklama = row.Field<string>("SatirAciklama"),
                    SevkAdres = row.Field<string>("SevkAdres"),
                    SevkAdresKodu = row.Field<string>("SevkAdresKodu"),
                    SevkIlce = row.Field<string>("SevkIlce"),
                    SevkPostaKodu = row.Field<string>("SevkPostaKodu"),
                    SevkSehir = row.Field<string>("SevkSehir"),
                    Soyadi = row.Field<string>("Soyadi"),
                    TasimaTipi = row.Field<string>("TasimaTipi"),
                    TasiyiciKodu = row.Field<string>("TasiyiciKodu"),
                    TcKimlik = row.Field<string>("TcKimlik"),
                    Telefon = row.Field<string>("Telefon"),
                    TeslimSekli = row.Field<string>("TeslimSekli"),
                    BelgeNo=row.Field<string>("BelgeNo"),
                    DokumanIzlemeNo=row.Field<string>("DokumanIzlemeNo"),
                    MalzemeKodu=row.Field<string>("MalzemeKodu")


                }).ToList();

                ViewBag.Model = listt;

            }
            catch (Exception e)
            {
                ViewBag.Error = "Yükleme esnasında hata oluştu.Lütfen exceli kontrol ediniz. Err = " + e.Message;
            }

            return View("Start");
        }




        [HttpPost]
        public JsonResult CreaateInvoice(TranspetExcelDto satir)
        {
            var sonuc = new LogoStoreService().CreateDispatch(satir);
            return Json(sonuc, JsonRequestBehavior.AllowGet);
        }
    }

}