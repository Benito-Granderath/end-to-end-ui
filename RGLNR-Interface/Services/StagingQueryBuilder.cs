using System.Globalization;
using Dapper;
using RGLNR_Interface.Models;

namespace RGLNR_Interface.Services
{
    public static class StagingQueryBuilder
    {
        private static readonly string[] ColumnMapping =
        {
            "DESTINATIONTYPE", "Debitorenkonto", "RGLNR", "Rechnung", "Datum",
            "Fällig", "entry_date", "Rechnungsbetrag", "Materialanforderung",
            "ihrzeichen", "createdby", "CREATEDDATETIME", "job_nr", "profile_name", "status"
        };

        private const string SelectColumns = @"[DESTINATIONTYPE], [Method], [source], [RGLNR], [Rechnung], [Datum], [Fällig], [entry_date],
                           [Rechnungsbetrag], [Materialanforderung], [ihrzeichen], [createdby], [job_nr],
                           [profile_name], [status], [CUSTOMPORT], [PRINTER], [EMAILFROM], [EMAILTO], [CREATEDDATETIME], [HOS], [Debitorenkonto]";

        private const string InvoiceDetailColumns = @"[DESTINATIONTYPE], [Method], [source], [RGLNR], [Rechnung], [Datum], [Fällig], [entry_date],
                           [Rechnungsbetrag], [Materialanforderung], [ihrzeichen], [createdby], [job_nr],
                           [profile_name], [status], [CUSTOMPORT], [PRINTER], [EMAILFROM], [EMAILTO], [CREATEDDATETIME], [HOS], [Debitorenkonto]";

        public static (string DataQuery, string FilteredCountQuery, string TotalCountQuery, DynamicParameters Parameters)
            Build(LoadDataRequest request, List<int> permittedDataAreaIds)
        {
            var parameters = new DynamicParameters();
            string totalCountBaseQuery = @"FROM [wsmb].[dbo].[END_TO_END_STAGING] WHERE 1=1";
            string baseQuery = totalCountBaseQuery;

            // Parse dates
            DateTime? parsedStartDate = ParseDate(request.StartDate);
            DateTime? parsedEndDate = ParseDate(request.EndDate);
            DateTime? parsedFaelligStart = ParseDate(request.FaelligStart);
            DateTime? parsedFaelligEnd = ParseDate(request.FaelligEnd);
            DateTime? parsedBestaetigungStart = ParseDate(request.BestaetigungStart);
            DateTime? parsedBestaetigungEnd = ParseDate(request.BestaetigungEnd);

            // Parse RGLNR range
            bool minRGLNRParsed = int.TryParse(request.MinRGLNR, out int minRGLNR);
            bool maxRGLNRParsed = int.TryParse(request.MaxRGLNR, out int maxRGLNR);

            // Date range filter
            if (parsedStartDate.HasValue && parsedEndDate.HasValue)
            {
                baseQuery += " AND Datum BETWEEN @startDate AND @endDate";
                parameters.Add("startDate", parsedStartDate);
                parameters.Add("endDate", parsedEndDate);
            }

            // RGLNR range
            if (minRGLNRParsed)
            {
                baseQuery += " AND RGLNR >= @minRGLNR";
                parameters.Add("minRGLNR", minRGLNR);
            }
            if (maxRGLNRParsed)
            {
                baseQuery += " AND RGLNR <= @maxRGLNR";
                parameters.Add("maxRGLNR", maxRGLNR);
            }

            // Search value normalization
            string? searchValue = null;
            string? searchField = request.SearchField;

            if (!string.IsNullOrEmpty(request.SearchValue))
            {
                if (request.SearchValue.Trim().ToLower() == "none")
                {
                    searchValue = "none";
                }
                else if (request.SearchValue.Contains("*"))
                {
                    searchValue = request.SearchValue.Replace('*', '%');
                }
                else
                {
                    searchValue = request.SearchValue;
                }
            }
            else
            {
                searchField = null;
            }

            // Field-specific search
            if (!string.IsNullOrEmpty(searchField))
            {
                switch (searchField)
                {
                    case "searchallcategories":
                        baseQuery += @" AND (
                            (DESTINATIONTYPE LIKE @searchValue OR @searchValue IS NULL)
                            OR (Method LIKE @searchValue OR @searchValue IS NULL)
                            OR (RGLNR LIKE @searchValue OR @searchValue IS NULL)
                            OR (Debitorenkonto LIKE @searchValue OR @searchValue IS NULL)
                            OR (Rechnung LIKE @searchValue OR @searchValue IS NULL)
                            OR (Materialanforderung LIKE @searchValue OR @searchValue IS NULL)
                            OR (ihrzeichen LIKE @searchValue OR @searchValue IS NULL)
                            OR (createdby LIKE @searchValue OR @searchValue IS NULL)
                            OR (job_nr LIKE @searchValue OR @searchValue IS NULL)
                            OR (profile_name LIKE @searchValue OR @searchValue IS NULL)
                            OR (status LIKE @searchValue OR @searchValue IS NULL)
                        )";
                        parameters.Add("searchValue", searchValue);
                        break;

                    case "searchziel":
                        baseQuery += searchValue == "none"
                            ? " AND (Method IS NULL)"
                            : " AND (DESTINATIONTYPE LIKE @searchziel OR Method LIKE @searchziel)";
                        parameters.Add("searchziel", searchValue);
                        break;

                    case "searchrglnr":
                        baseQuery += searchValue == "none" ? " AND RGLNR IS NULL" : " AND RGLNR LIKE @searchRGLNR";
                        parameters.Add("searchRGLNR", searchValue);
                        break;

                    case "searchdebitor":
                        baseQuery += searchValue == "none" ? " AND Debitorenkonto IS NULL" : " AND Debitorenkonto LIKE @searchdebitor";
                        parameters.Add("searchdebitor", searchValue);
                        break;

                    case "searchinvoice":
                        baseQuery += searchValue == "none" ? " AND Rechnung IS NULL" : " AND Rechnung LIKE @searchinvoice";
                        parameters.Add("searchinvoice", searchValue);
                        break;

                    case "searchdebitorrequest":
                        baseQuery += searchValue == "none" ? " AND Materialanforderung IS NULL" : " AND Materialanforderung LIKE @searchdebitorrequest";
                        parameters.Add("searchdebitorrequest", searchValue);
                        break;

                    case "searchdebitorreference":
                        baseQuery += searchValue == "none" ? " AND ihrzeichen IS NULL" : " AND ihrzeichen LIKE @searchdebitorreference";
                        parameters.Add("searchdebitorreference", searchValue);
                        break;

                    case "searchcreatedby":
                        baseQuery += searchValue == "none" ? " AND createdby IS NULL" : " AND createdby LIKE @searchcreatedby";
                        parameters.Add("searchcreatedby", searchValue);
                        break;

                    case "searchjobnr":
                        baseQuery += searchValue == "none" ? " AND job_nr IS NULL" : " AND job_nr LIKE @searchjobnr";
                        parameters.Add("searchjobnr", searchValue);
                        break;

                    case "searchlobsterprofile":
                        baseQuery += searchValue == "none" ? " AND profile_name IS NULL" : " AND profile_name LIKE @searchlobsterprofile";
                        parameters.Add("searchlobsterprofile", searchValue);
                        break;

                    case "searchlobsterstatus":
                        baseQuery += searchValue == "none" ? " AND status IS NULL" : " AND status LIKE @searchlobsterstatus";
                        parameters.Add("searchlobsterstatus", searchValue);
                        break;
                }
            }

            // Invoice list
            var invoiceList = !string.IsNullOrEmpty(request.PasteInvoices)
                ? request.PasteInvoices.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim()).ToArray()
                : null;

            if (invoiceList != null && invoiceList.Length > 0)
            {
                baseQuery += " AND Rechnung IN @invoiceList";
                parameters.Add("invoiceList", invoiceList);
            }

            // Additional date ranges
            if (parsedFaelligStart.HasValue && parsedFaelligEnd.HasValue)
            {
                baseQuery += " AND Fällig BETWEEN @faelligStart AND @faelligEnd";
                parameters.Add("faelligStart", parsedFaelligStart);
                parameters.Add("faelligEnd", parsedFaelligEnd);
            }
            if (parsedBestaetigungStart.HasValue && parsedBestaetigungEnd.HasValue)
            {
                baseQuery += " AND entry_date BETWEEN @bestaetigungStart AND @bestaetigungEnd";
                parameters.Add("bestaetigungStart", parsedBestaetigungStart);
                parameters.Add("bestaetigungEnd", parsedBestaetigungEnd);
            }

            // Company/tenant filter (safe parameterized IN clause)
            if (permittedDataAreaIds.Any())
            {
                baseQuery += " AND AXRK_DataAreaId IN @dataAreaIds";
                totalCountBaseQuery += " AND AXRK_DataAreaId IN @dataAreaIds";
                parameters.Add("dataAreaIds", permittedDataAreaIds);
            }
            else
            {
                baseQuery += " AND 1 = 0";
                totalCountBaseQuery += " AND 1 = 0";
            }

            // ORDER BY
            string orderColumn = "RGLNR";
            string orderDirection = "ASC";

            if (int.TryParse(request.OrderColumnIndex, out int columnIndex) && columnIndex >= 0 && columnIndex < ColumnMapping.Length)
            {
                orderColumn = ColumnMapping[columnIndex];
                orderDirection = request.OrderDir == "desc" ? "DESC" : "ASC";
            }

            parameters.Add("start", request.Start);
            parameters.Add("length", request.Length);

            string dataQuery = $@"SELECT {SelectColumns} {baseQuery}
                    ORDER BY {orderColumn} {orderDirection}
                    OFFSET @start ROWS FETCH NEXT @length ROWS ONLY";

            string filteredCountQuery = "SELECT COUNT(*) " + baseQuery;
            string totalCountQuery = "SELECT COUNT(*) " + totalCountBaseQuery;

            return (dataQuery, filteredCountQuery, totalCountQuery, parameters);
        }

        public static string BuildInvoiceDetailQuery()
        {
            return $"SELECT {InvoiceDetailColumns} FROM [wsmb].[dbo].[END_TO_END_STAGING] WHERE Rechnung = @InvoiceId ORDER BY CASE WHEN entry_date = @entry_date THEN 0 ELSE 1 END, entry_date";
        }

        private static DateTime? ParseDate(string? dateStr)
        {
            if (DateTime.TryParseExact(dateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                return date;
            return null;
        }
    }
}
