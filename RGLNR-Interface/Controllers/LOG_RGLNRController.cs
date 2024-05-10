using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RGLNR_Interface.Models;
using System.Data;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using RGLNR_Interface.Services;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;


namespace RGLNR_Interface.Controllers
{
    public class LOG_RGLNRController : Controller
    {
        private readonly PermissionService _permissionService;
        private readonly IConfiguration _configuration;
        private readonly ActiveDirectoryService _adService;
        public LOG_RGLNRController(IConfiguration configuration, ILogger<LOG_RGLNRController> logger, ActiveDirectoryService adService)
        {
            _permissionService = new PermissionService(configuration.GetConnectionString("DefaultConnection"));
            _configuration = configuration;
            _adService = adService;
        }
        public async Task<IActionResult> Index()
        {

            if (User.Identity.IsAuthenticated)
            {
                string username = User.Identity.Name;
                string userSID = _adService.GetUserSID(username);
                ViewBag.UserSID = userSID;
                ViewBag.UserName = username;

                if (!string.IsNullOrEmpty(userSID))
                {
                    var permissions = await _permissionService.GetUserPermissionsAsync(userSID);

                    if (permissions == null || !permissions.Any())
                    {
                        return RedirectToAction("AccessDenied", "Home");
                    }

                    return View(permissions);
                }
                else
                {
                    return RedirectToAction("AccessDenied", "Home");
                }
            }
            else
            {
                return RedirectToAction("AccessDenied", "Home");
            }
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
                var companyPrefixParsed = int.TryParse(HttpContext.Request.Form["companyPrefix"].FirstOrDefault(), out int companyPrefix);
                var startDateFälligStr = HttpContext.Request.Form["faelligStart"].FirstOrDefault();
                var endDateFälligStr = HttpContext.Request.Form["faelligEnd"].FirstOrDefault();
                var startDateBestätigungStr = HttpContext.Request.Form["bestaetigungStart"].FirstOrDefault();
                var endDateBestätigungStr = HttpContext.Request.Form["bestaetigungEnd"].FirstOrDefault();
                var invoiceList = !string.IsNullOrEmpty(pasteInvoices)
                    ? pasteInvoices.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(invoice => invoice.Trim()).ToArray()
                    : null;
                DateTime? parsedStartDate = null, parsedEndDate = null;
                List<int> userPermissionsDataAreaIds = new List<int>();

                if (companyPrefixParsed && companyPrefix == -1)
                {
                    string username = User.Identity.Name;
                    string userSID = _adService.GetUserSID(username);
                    var permissions = await _permissionService.GetUserPermissionsAsync(userSID);

                    userPermissionsDataAreaIds = permissions.Select(p => p.description).ToList();

                }
                else if (companyPrefix > 0)
                {
                    userPermissionsDataAreaIds.Add(companyPrefix);
                }

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
                    baseQuery += " AND entry_date BETWEEN @bestaetigungStart AND @bestaetigungEnd";
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    baseQuery += " AND (CAST(RGLNR AS VARCHAR) LIKE @searchValue OR Rechnung LIKE @searchValue OR Materialanforderung LIKE @searchValue OR [Ihr Zeichen] LIKE @searchValue OR job_nr LIKE @searchValue)";
                }

                if (userPermissionsDataAreaIds.Any())
                {
                    var dataAreaIds = string.Join(", ", userPermissionsDataAreaIds);
                    baseQuery += $" AND DataAreaId IN ({@dataAreaIds})";
                }

                string dataQuery = "SELECT [RGLNR], [Rechnung], [Datum], [Fällig], [entry_date], [Rechnungsbetrag], [Materialanforderung], [Ihr Zeichen] AS IhrZeichen, [EDI Status] AS EDIStatus, [profile_name], [job_nr], [status] " +
                                   baseQuery +
                                   " ORDER BY entry_date DESC OFFSET @start ROWS FETCH NEXT @length ROWS ONLY";
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
                Debug.WriteLine(companyPrefix);
                db.Close();

                return Json(new { draw, recordsFiltered, recordsTotal, data });
            }
        }


    }
}
