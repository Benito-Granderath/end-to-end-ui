using Dapper;
using Microsoft.Data.SqlClient;
using RGLNR_Interface.Models;

namespace RGLNR_Interface.Services
{
    public class StagingDataService : IStagingDataService
    {
        private readonly IConfiguration _configuration;

        public StagingDataService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<(IEnumerable<LOG_RGLNR_Model> Data, int FilteredCount, int TotalCount)>
            GetPagedDataAsync(LoadDataRequest request, List<int> permittedDataAreaIds)
        {
            var (dataQuery, filteredCountQuery, totalCountQuery, parameters) = StagingQueryBuilder.Build(request, permittedDataAreaIds);

            using var db = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await db.OpenAsync();

            var data = await db.QueryAsync<LOG_RGLNR_Model>(dataQuery, parameters, commandTimeout: 120);
            var recordsFiltered = await db.ExecuteScalarAsync<int>(filteredCountQuery, parameters);
            var recordsTotal = await db.ExecuteScalarAsync<int>(totalCountQuery, parameters);

            return (data, recordsFiltered, recordsTotal);
        }

        public async Task<IEnumerable<LOG_RGLNR_Model>> GetInvoiceDetailsAsync(string invoiceId, DateTime? entryDate)
        {
            string query = StagingQueryBuilder.BuildInvoiceDetailQuery();

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var parameters = new { InvoiceId = invoiceId, entry_date = entryDate };
            var results = await connection.QueryAsync<LOG_RGLNR_Model>(query, parameters);

            return results;
        }
    }
}
