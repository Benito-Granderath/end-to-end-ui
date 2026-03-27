using RGLNR_Interface.Models;

namespace RGLNR_Interface.Services
{
    public interface IStagingDataService
    {
        Task<(IEnumerable<LOG_RGLNR_Model> Data, int FilteredCount, int TotalCount)>
            GetPagedDataAsync(LoadDataRequest request, List<int> permittedDataAreaIds);

        Task<IEnumerable<LOG_RGLNR_Model>> GetInvoiceDetailsAsync(string invoiceId, DateTime? entryDate);
    }
}
