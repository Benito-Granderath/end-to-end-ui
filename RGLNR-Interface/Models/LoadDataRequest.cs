namespace RGLNR_Interface.Models
{
    public class LoadDataRequest
    {
        public string? Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public string? MinRGLNR { get; set; }
        public string? MaxRGLNR { get; set; }
        public string? PasteInvoices { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? CompanyPrefix { get; set; }
        public string? FaelligStart { get; set; }
        public string? FaelligEnd { get; set; }
        public string? BestaetigungStart { get; set; }
        public string? BestaetigungEnd { get; set; }
        public string? SearchField { get; set; }
        public string? SearchValue { get; set; }

        // DataTables sorting parameters (sent as order[0][column] and order[0][dir])
        public string? OrderColumnIndex { get; set; }
        public string? OrderDir { get; set; }
    }
}
