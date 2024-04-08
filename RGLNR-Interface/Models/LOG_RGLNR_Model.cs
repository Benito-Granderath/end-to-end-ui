namespace RGLNR_Interface.Models
{
    public class LOG_RGLNR_Model
    {
        public string RGLNR { get; set; }
        public string Rechnung { get; set; }
        public DateTime Datum { get; set; }
        public DateTime Fällig { get; set; }
        public DateTime entry_date { get; set; }
        public decimal Rechnungsbetrag { get; set; }
        public string EDIStatus {get; set; }
        public string profile_name { get; set; }
        public decimal job_nr { get; set; }
        public string status {  get; set; }

    }
}
