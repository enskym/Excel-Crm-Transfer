using Excel.Integration.Data.DataAccess;
using Excel.Integration.Data.Helper;
using Excel.Integration.Data.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityObjects;

namespace Excel.Integration.Data.Tiger
{
   public class LogoStoreService :App
    {
        public ResponseMessage CreateDispatch(TranspetExcelDto model)
        {
            try
            {
                var sevkiyatadresi = LogoDataAccess.SevkiyatAdresiSorgu(model.CariKodu, model.SevkAdresKodu);

                if (sevkiyatadresi == null)
                {
                    var adresResp = CreateDeliveryAddress(model);
                    if (!adresResp.Status)
                    {
                        return adresResp;
                    }
                }

                var resp = new ResponseMessage();
                var nfi = new NumberFormatInfo
                {
                    NumberDecimalSeparator = ".",
                    NumberGroupSeparator = ""
                };



                int firmano = Convert.ToInt32("FirmNo".GetAppSetting());
                var tiger = TigerInstance(firmano);

                var now = DateTime.Now;

                object myDate = null;
                tiger.PackDate(now.Day, now.Month, now.Year, ref myDate);

                object myTime = null;
                tiger.PackTime(now.Hour, now.Minute, now.Second, ref myTime);

                UnityObjects.Data invoice = tiger.NewDataObject(UnityObjects.DataObjectType.doSalesDispatch);
                invoice.New();

                invoice.DataFields.FieldByName("TYPE").Value = 8;
                invoice.DataFields.FieldByName("NUMBER").Value = "~"; //Order.No;

                invoice.DataFields.FieldByName("DOC_DATE").Value = model.Tarih;
                invoice.DataFields.FieldByName("DATE").Value = model.Tarih;
                invoice.DataFields.FieldByName("TIME").Value = myTime;

                invoice.DataFields.FieldByName("DOC_NUMBER").Value = model.BelgeNo;
                invoice.DataFields.FieldByName("AUXIL_CODE").Value = "";
                invoice.DataFields.FieldByName("DOC_TRACK_NR").Value = model.DokumanIzlemeNo;
                invoice.DataFields.FieldByName("AUTH_CODE").Value = "";

                invoice.DataFields.FieldByName("ARP_CODE").Value = model.CariKodu;

                invoice.DataFields.FieldByName("NOTES1").Value = "TEL NO:" + model.Telefon;

                invoice.DataFields.FieldByName("PAYMENT_CODE").Value = "Logo.PayplanCode".GetAppSetting();

               // invoice.DataFields.FieldByName("SALESMAN_CODE").Value = "";// Order.SalesPersonCode;

                invoice.DataFields.FieldByName("SHIPMENT_TYPE").Value = model.TeslimSekli;
                invoice.DataFields.FieldByName("SHIPPING_AGENT").Value = model.TasiyiciKodu;


                invoice.DataFields.FieldByName("SHIPLOC_CODE").Value = model.SevkAdresKodu;

                invoice.DataFields.FieldByName("EDESPATCH").Value = "1";
                invoice.DataFields.FieldByName("EDESPATCH_PROFILEID").Value = "1";
                invoice.DataFields.FieldByName("EINVOICE").Value = "1";
                invoice.DataFields.FieldByName("EINVOICE_TYPE").Value = "7";
                invoice.DataFields.FieldByName("EINVOICE_PROFILEID").Value = "2";
                invoice.DataFields.FieldByName("DEDUCTIONPART1").Value = "2";
                invoice.DataFields.FieldByName("DEDUCTIONPART2").Value = "3";
                invoice.DataFields.FieldByName("CURRSEL_TOTALS").Value = "1";
                invoice.DataFields.FieldByName("FRG_TYP_CODE").Value = model.TasimaTipi;

                invoice.DataFields.FieldByName("EINVOICE_DRIVERNAME1").Value = model.Adi;
                invoice.DataFields.FieldByName("EINVOICE_DRIVERSURNAME1").Value = model.Soyadi;

                invoice.DataFields.FieldByName("EINVOICE_DRIVERTCKNO1").Value = model.TcKimlik;
                invoice.DataFields.FieldByName("EINVOICE_PLATENUM1").Value = model.Plaka1;
                invoice.DataFields.FieldByName("EINVOICE_CHASSISNUM1").Value = model.Plaka2 ;

                Lines detay = invoice.DataFields.FieldByName("TRANSACTIONS").Lines;

                int index = 0;


                if (detay.AppendLine())
                {
                    detay[index].FieldByName("TYPE").Value = 0; // Hizmet 4 kart 0 olacak
                    detay[index].FieldByName("MASTER_CODE").Value = model.MalzemeKodu; //"Ürün Kodu";
                    detay[index].FieldByName("QUANTITY").Value = model.Miktar.Replace(".", "");
                    detay[index].FieldByName("UNIT_CODE").Value = model.Birim;
                    detay[index].FieldByName("DESCRIPTION").Value = model.SatirAciklama;

                    // detay[index].FieldByName("DUE_DATE").Value = ((string)item.ShipmentDate).TarihCevirXml();
                }

                index += 1;

                invoice.FillAccCodes();

                if (invoice.Post() == true)
                {
                    resp.Status = true;
                    resp.Message = "Aktarım Başarılı.";
                    return resp;
                }
                else
                {
                    if (invoice.ErrorCode != 0)
                    {

                    }
                    else if (invoice.ValidateErrors.Count > 0)
                    {
                        string result = "XML ErrorList:";
                        for (int i = 0; i < invoice.ValidateErrors.Count; i++)
                        {
                            result += "(" + invoice.ValidateErrors[i].ID.ToString() + ") - " + invoice.ValidateErrors[i].Error;
                        }
                        resp.Message = result;
                    }

                    resp.Message = resp.Message + "Logo fiş aktarılamadı";
                    return resp;
                }

                //return new ResponseMessage
                //{
                //    Status = true
                //};


            }
            catch (Exception e)
            {
                return new ResponseMessage
                {
                    Message = "Logo tiger lisans sorunu .Sipariş entegrasyon sırasında hata oluştu. " + e.Message,
                    Code = e.Message + " | " + e.StackTrace
                };
            }

        }


        public ResponseMessage CreateDeliveryAddress(TranspetExcelDto address)
        {
            ResponseMessage resp = new ResponseMessage();

            try
            {
                int firmano = Convert.ToInt32("FirmNo".GetAppSetting());
                var tiger = TigerInstance(firmano);
                UnityObjects.Data newAddress = tiger.NewDataObject(DataObjectType.doArpShipLic);

                newAddress.New();

                newAddress.DataFields.FieldByName("ARP_CODE").Value = address.CariKodu;   // "Cari hesap kodu";
                newAddress.DataFields.FieldByName("CODE").Value = address.SevkAdresKodu;      // "Kod "; 

                newAddress.DataFields.FieldByName("DESCRIPTION").Value = address.SevkAdresKodu;                 // Açıklama";
                newAddress.DataFields.FieldByName("ADDRESS1").Value = address.SevkAdres;     // "Adres Alanı 1";
                newAddress.DataFields.FieldByName("ADDRESS2").Value = "";     // "Adres Alanı 1";
                newAddress.DataFields.FieldByName("DISTRICT").Value = " ";
                newAddress.DataFields.FieldByName("DISTRICT_CODE").Value = null;
                newAddress.DataFields.FieldByName("TOWN_CODE").Value = "";
                newAddress.DataFields.FieldByName("TOWN").Value = address.SevkIlce;
                newAddress.DataFields.FieldByName("CITY_CODE").Value = "";
                newAddress.DataFields.FieldByName("CITY").Value = address.SevkSehir;
                newAddress.DataFields.FieldByName("COUNTRY_CODE").Value = "TR";
                newAddress.DataFields.FieldByName("COUNTRY").Value = "TR";
                newAddress.DataFields.FieldByName("POSTAL_CODE").Value = address.SevkPostaKodu;
                newAddress.DataFields.FieldByName("TELEPHONE1").Value = "";
                newAddress.DataFields.FieldByName("TELEPHONE2").Value = "";

                ValidateErrors err = newAddress.ValidateErrors;

                if (newAddress.Post())
                {
                    resp.Code = (string)newAddress.DataFields.FieldByName("CODE").Value;
                    resp.Status = true;
                }
                else
                {
                    resp.Status = false;
                  
                    resp.Code = newAddress.ErrorCode.ToString();
                    resp.Message = "Sevk adres oluşturlamadı." + newAddress.ErrorDesc;
                    for (int i = 0; i < err.Count; i++)
                    {
                        string stra = $"{err[i].Error} - {err[i].ID};";
                        resp.Message = resp.Message + stra;
                    }
                }

                return resp;
            }
            catch (Exception ex)
            {
                resp.Status = false;
                resp.Message = "Sevk Adres oluşturma hatası .Error/Hata " + ex.Message;
                return resp;
            }
        }


    }
}
