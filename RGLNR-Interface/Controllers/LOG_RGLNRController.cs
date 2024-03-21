using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RGLNR_Interface.Models;
using System.Data;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using RGLNR_Interface.Services;


namespace RGLNR_Interface.Controllers
{
    public class LOG_RGLNRController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ActiveDirectoryService _adService;
        public LOG_RGLNRController(IConfiguration configuration, ILogger<LOG_RGLNRController> logger, ActiveDirectoryService adService)
        {
            _configuration = configuration;
            _adService = adService;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var adService = new ActiveDirectoryService();
                var userSID = adService.GetUserSID(User.Identity.Name);
                ViewBag.UserSID = $"Nutzer authentifiziert mit sID {userSID}";
            }
            else
            {
                ViewBag.UserSID = "Nutzer ist nicht authentifiziert.";
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LoadData()
        {
            using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
                var start = int.Parse(HttpContext.Request.Form["start"].FirstOrDefault() ?? "0");
                var length = int.Parse(HttpContext.Request.Form["length"].FirstOrDefault() ?? "0");
                var searchValue = HttpContext.Request.Form["search[value]"].FirstOrDefault();

                bool minRGLNRParsed = int.TryParse(HttpContext.Request.Form["minRGLNR"].FirstOrDefault(), out int minRGLNR);
                bool maxRGLNRParsed = int.TryParse(HttpContext.Request.Form["maxRGLNR"].FirstOrDefault(), out int maxRGLNR);
                var pasteInvoices = HttpContext.Request.Form["pasteInvoices"].FirstOrDefault();
                var startDateStr = HttpContext.Request.Form["startDate"].FirstOrDefault();
                var endDateStr = HttpContext.Request.Form["endDate"].FirstOrDefault();
                var companyPrefix = HttpContext.Request.Form["companyPrefix"].FirstOrDefault();
                var startDateFälligStr = HttpContext.Request.Form["faelligStart"].FirstOrDefault();
                var endDateFälligStr = HttpContext.Request.Form["faelligEnd"].FirstOrDefault();
                var startDateBestätigungStr = HttpContext.Request.Form["bestaetigungStart"].FirstOrDefault();
                var endDateBestätigungStr = HttpContext.Request.Form["bestaetigungEnd"].FirstOrDefault();
                var invoiceList = !string.IsNullOrEmpty(pasteInvoices)
                    ? pasteInvoices.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(invoice => invoice.Trim()).ToArray()
                    : null;

                DateTime? parsedStartDate = null, parsedEndDate = null;

                if (DateTime.TryParseExact(startDateStr, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var startDate))
                {
                    parsedStartDate = startDate;
                }

                if (DateTime.TryParseExact(endDateStr, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var endDate))
                {
                    parsedEndDate = endDate;
                }
                DateTime? parsedStartDateFällig = null, parsedEndDateFällig = null;

                if (DateTime.TryParseExact(startDateFälligStr, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var faelligStart))
                {
                    parsedStartDateFällig = faelligStart;
                }

                if (DateTime.TryParseExact(endDateFälligStr, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var faelligEnd))
                {
                    parsedEndDateFällig = faelligEnd;
                }
                DateTime? parsedStartDateBestätigung = null, parsedEndDateBestätigung = null;

                if (DateTime.TryParseExact(startDateBestätigungStr, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var bestaetigungStart))
                {
                    parsedStartDateBestätigung = bestaetigungStart;
                }

                if (DateTime.TryParseExact(endDateBestätigungStr, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var bestaetigungEnd))
                {
                    parsedEndDateBestätigung = bestaetigungEnd;
                }


                string baseQuery = @"FROM [wsmb].[dbo].[EndToEnd_BEN] WHERE 1=1";

                if (parsedStartDate.HasValue && parsedEndDate.HasValue)
                {
                    baseQuery += " AND Datum BETWEEN @startDate AND @endDate";
                }
                if (minRGLNRParsed)
                {
                    baseQuery += " AND RGLNR >= @minRGLNR";
                }
                if (maxRGLNRParsed)
                {
                    baseQuery += " AND RGLNR <= @maxRGLNR";
                }
                if (invoiceList != null && invoiceList.Length > 0)
                {
                    baseQuery += " AND Rechnung IN @invoiceList"; 
                }
                if (parsedStartDateFällig.HasValue && parsedEndDateFällig.HasValue)
                {
                    baseQuery += " AND Fällig BETWEEN @faelligStart AND @faelligEnd";
                }
                if (parsedStartDateBestätigung.HasValue && parsedEndDateBestätigung.HasValue)
                {
                    baseQuery += " AND log_date BETWEEN @bestaetigungStart AND @bestaetigungEnd";
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    baseQuery += " AND (CAST(RGLNR AS VARCHAR) LIKE @searchValue OR Rechnung LIKE @searchValue)";
                }

                if (!string.IsNullOrEmpty(companyPrefix))
                {
                    baseQuery += " AND DataAreaId = @companyPrefix";
                }

                string dataQuery = "SELECT [RGLNR], [Rechnung], [Datum], [Fällig], [log_date], [Rechnungsbetrag], [EDI Status] AS EDIStatus, [profile_name] " +
                                   baseQuery +
                                   " ORDER BY log_date DESC OFFSET @start ROWS FETCH NEXT @length ROWS ONLY";

                string filteredCountQuery = "SELECT COUNT(*) " + baseQuery;

                db.Open();

                var parameters = new
                {
                    searchValue = $"%{searchValue}%",
                    minRGLNR = minRGLNRParsed ? minRGLNR : (object)DBNull.Value,
                    maxRGLNR = maxRGLNRParsed ? maxRGLNR : (object)DBNull.Value,
                    startDate = parsedStartDate,
                    endDate = parsedEndDate,
                    faelligStart = parsedStartDateFällig,
                    faelligEnd = parsedEndDateFällig,
                    bestaetigungStart = parsedStartDateBestätigung,
                    bestaetigungEnd = parsedEndDateBestätigung,
                    companyPrefix,
                    invoiceList,
                    start,
                    length
                };

                var data = await db.QueryAsync<LOG_RGLNR_Model>(dataQuery, parameters, commandTimeout: 120); 

                var recordsFiltered = await db.ExecuteScalarAsync<int>(filteredCountQuery, parameters);

                var recordsTotal = await db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM [wsmb].[dbo].[EndToEnd_BEN]");

                db.Close();

                return Json(new { draw = draw, recordsFiltered = recordsFiltered, recordsTotal = recordsTotal, data = data });
            }
        }


    }
}
