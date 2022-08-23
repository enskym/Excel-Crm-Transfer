using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excel.Integration.Data.Models
{


    public class TranspetExcelDto
    {
        public string Tarih { get; set; }
        public string Plaka1 { get; set; }
        public string Plaka2 { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public string TcKimlik { get; set; }
        public string Telefon { get; set; }
        public string Miktar { get; set; }
        public string Birim { get; set; }
        public string SatirAciklama { get; set; }
        public string CariKodu { get; set; }
        public string CariUnvan { get; set; }
        public string SevkAdresKodu { get; set; }
        public string SevkSehir { get; set; }
        public string SevkIlce { get; set; }
        public string SevkAdres { get; set; }
        public string SevkPostaKodu { get; set; }
        public string TasimaTipi { get; set; }
        public string TasiyiciKodu { get; set; }
        public string TeslimSekli { get; set; }
        public string MalzemeKodu { get; set; }
        public string DokumanIzlemeNo { get; set; }
        public string BelgeNo { get; set; }


    }
    public class SevkiyatDto
    {
        public int ADRES_LREF { get; set; }
        public int CARI_LREF { get; set; }
        public string CARI_KODU { get; set; }
        public string CARI_ADI { get; set; }
        public string ADRES_KODU { get; set; }
        public string ADRES_ADI { get; set; }
        public string ADDR1 { get; set; }
        public string ADDR2 { get; set; }
        public string TOWN { get; set; }
        public string CITY { get; set; }
        public string COUNTRY { get; set; }
        public string POSTCODE { get; set; }
        public string TELNRS1 { get; set; }
        public string TELNRS2 { get; set; }
        public string FAXNR { get; set; }
        public string BolgeKodu { get; set; }

    }

}
