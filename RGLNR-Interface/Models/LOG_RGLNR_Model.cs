namespace RGLNR_Interface.Models
{
    public class LOG_RGLNR_Model
    {
        public string RGLNR { get; set; }
        public string Rechnung { get; set; }
        public DateTime Datum { get; set; }
        public DateTime Fällig { get; set; }
        public DateTime? entry_date { get; set; }
        public DateTime? transfer_date { get; set; }
        public decimal Rechnungsbetrag { get; set; }
        public string profile_name { get; set; }
        public decimal job_nr { get; set; }
        public string status { get; set; }
        public string Materialanforderung { get; set; }
        public string ihrzeichen { get; set; } 
        public string DestinationType { get; set; }
        public string createdby { get; set; }
        public string CUSTOMPORT { get; set; }
        public string PRINTER { get; set; }
        public string EMAILFROM { get; set; }
        public string EMAILTO { get; set; }
        public string CREATEDDATETIME { get; set; }
        public string HOS { get; set; }
        public string Debitorenkonto { get; set; }

    }
}
