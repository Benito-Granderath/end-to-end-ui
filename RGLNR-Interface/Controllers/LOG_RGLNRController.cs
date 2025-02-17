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
using Microsoft.IdentityModel.Tokens;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Buffers;


namespace RGLNR_Interface.Controllers
{
    public class LOG_RGLNRController : Controller
    {
        private readonly PermissionService _permissionService;
        private readonly IConfiguration _configuration;
        private readonly ActiveDirectorySearch _adService;
        public LOG_RGLNRController(IConfiguration configuration, ILogger<LOG_RGLNRController> logger, ActiveDirectorySearch adService)
        {
            _permissionService = new PermissionService(new ActiveDirectorySearch());
            _configuration = configuration;
            _adService = adService;
        }
        public async Task<IActionResult> Index()
        {

            if (User.Identity.IsAuthenticated)
            {
                string sAMAccountName = GetSamAccountName(User.Identity.Name);
                ViewBag.UserName = sAMAccountName;


                var permissions = await _permissionService.GetUserPermissionsAsync(sAMAccountName);

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
        [HttpPost]
        private string GetSamAccountName(string identityName)
        {
            if (!string.IsNullOrEmpty(identityName))
            {
                var parts = identityName.Split('\\');
                return parts.Length > 1 ? parts[1] : identityName;
            }

            return identityName;
        }
        public async Task<IActionResult> LoadData()
        {
            using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
                var start = int.Parse(HttpContext.Request.Form["start"].FirstOrDefault() ?? "0");
                var length = int.Parse(HttpContext.Request.Form["length"].FirstOrDefault() ?? "0");
                var orderColumnIndex = HttpContext.Request.Form["order[0][column]"].FirstOrDefault();
                var orderDir = HttpContext.Request.Form["order[0][dir]"].FirstOrDefault();
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
                var searchField = HttpContext.Request.Form["searchField"].FirstOrDefault();
                var searchValueRaw = HttpContext.Request.Form["searchValue"].FirstOrDefault();
                string searchValue = null;
                DateTime? parsedStartDate = null, parsedEndDate = null;
                List<int> userPermissionsDataAreaIds = new List<int>();
                string[] columnMapping = new string[] {
                    "DESTINATIONTYPE",
                    "Debitorenkonto",
                    "RGLNR",
                    "Rechnung",
                    "Datum",
                    "Fällig",
                    "entry_date",
                    "Rechnungsbetrag",
                    "Materialanforderung",
                    "ihrzeichen",
                    "createdby",
                    "CREATEDDATETIME",
                    "job_nr",
                    "profile_name",
                    "status",
                };

                if (companyPrefixParsed && companyPrefix == -1)
                {
                    string username = User.Identity.Name;
                    var permissions = await _permissionService.GetUserPermissionsAsync(username);

                    userPermissionsDataAreaIds = permissions.Select(p => p.description).ToList();
                }
                else if (companyPrefix > 0)
                {
                    userPermissionsDataAreaIds.Add(companyPrefix);
                }

                if (DateTime.TryParseExact(startDateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var startDate))
                {
                    parsedStartDate = startDate;
                }

                if (DateTime.TryParseExact(endDateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var endDate))
                {
                    parsedEndDate = endDate;
                }
                DateTime? parsedStartDateFällig = null, parsedEndDateFällig = null;

                if (DateTime.TryParseExact(startDateFälligStr, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var faelligStart))
                {
                    parsedStartDateFällig = faelligStart;
                }

                if (DateTime.TryParseExact(endDateFälligStr, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var faelligEnd))
                {
                    parsedEndDateFällig = faelligEnd;
                }
                DateTime? parsedStartDateBestätigung = null, parsedEndDateBestätigung = null;

                if (DateTime.TryParseExact(startDateBestätigungStr, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var bestaetigungStart))
                {
                    parsedStartDateBestätigung = bestaetigungStart;
                }

                if (DateTime.TryParseExact(endDateBestätigungStr, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var bestaetigungEnd))
                {
                    parsedEndDateBestätigung = bestaetigungEnd;
                }

                string baseQuery = @"FROM [wsmb].[dbo].[END_TO_END_STAGING] WHERE 1=1";

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
                
                if (!string.IsNullOrEmpty(searchValueRaw))
                {
                    if (searchValueRaw.Trim().ToLower() == "none")
                    {
                        searchValue = "none";
                    }
                    else if (searchValueRaw.Contains("*"))
                    {
                        searchValue = searchValueRaw.Replace('*', '%');
                    }
                    else
                    {
                        searchValue = searchValueRaw;
                    }
                }
                else
                {
                    searchField = null;
                }
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
                            break;

                        case "searchziel":
                            baseQuery += searchValue == "none"
                                ? " AND (Method IS NULL)"
                                : " AND (DESTINATIONTYPE LIKE @searchziel OR Method LIKE @searchziel)";
                            break;

                        case "searchrglnr":
                            baseQuery += searchValue == "none"
                                ? " AND RGLNR IS NULL"
                                : " AND RGLNR LIKE @searchRGLNR";
                            break;
                        case "searchdebitor":
                            baseQuery += searchValue == "none"
                                ? " AND Debitorenkonto IS NULL"
                                : " AND Debitorenkonto LIKE @searchdebitor";
                            break;

                        case "searchinvoice":
                            baseQuery += searchValue == "none"
                                ? " AND Rechnung IS NULL"
                                : " AND Rechnung LIKE @searchinvoice";
                            break;
                        case "searchdebitorrequest":
                            baseQuery += searchValue == "none"
                                ? " AND Materialanforderung IS NULL"
                                : " AND Materialanforderung LIKE @searchdebitorrequest";
                            break;

                        case "searchdebitorreference":
                            baseQuery += searchValue == "none"
                                ? " AND ihrzeichen IS NULL"
                                : " AND ihrzeichen LIKE @searchdebitorreference";
                            break;

                        case "searchcreatedby":
                            baseQuery += searchValue == "none"
                                ? " AND createdby IS NULL"
                                : " AND createdby LIKE @searchcreatedby";
                            break;

                        case "searchjobnr":
                            baseQuery += searchValue == "none"
                                ? " AND job_nr IS NULL"
                                : " AND job_nr LIKE @searchjobnr";
                            break;

                        case "searchlobsterprofile":
                            baseQuery += searchValue == "none"
                                ? " AND profile_name IS NULL"
                                : " AND profile_name LIKE @searchlobsterprofile";
                            break;

                        case "searchlobsterstatus":
                            baseQuery += searchValue == "none"
                                ? " AND status IS NULL"
                                : " AND status LIKE @searchlobsterstatus";
                            break;

                        default:
                            break;
                    }
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

                if (userPermissionsDataAreaIds.Any())
                {
                    var dataAreaIds = string.Join(", ", userPermissionsDataAreaIds);
                    baseQuery += $" AND AXRK_DataAreaId IN ({@dataAreaIds})";
                }
                string dataQuery;

                if (int.TryParse(orderColumnIndex, out int columnIndex) && columnIndex >= 0 && columnIndex < columnMapping.Length)
                {
                    var orderColumn = columnMapping[columnIndex];
                    var orderDirection = orderDir == "desc" ? "DESC" : "ASC";

                    dataQuery = $@"
                    SELECT [DESTINATIONTYPE], [RGLNR], [Rechnung], [Datum], [Fällig], [entry_date],
                           [Rechnungsbetrag], [Materialanforderung], [ihrzeichen], [createdby], [job_nr],
                           [profile_name], [status], [CUSTOMPORT], [PRINTER], [EMAILFROM], [EMAILTO], [CREATEDDATETIME], [HOS], [Debitorenkonto]
                    {baseQuery}
                    ORDER BY {orderColumn} {orderDirection} 
                    OFFSET @start ROWS FETCH NEXT @length ROWS ONLY";
                }
                else
                {
                    dataQuery = $@"
                    SELECT [DESTINATIONTYPE], [RGLNR], [Rechnung], [Datum], [Fällig], [entry_date],
                           [Rechnungsbetrag], [Materialanforderung], [ihrzeichen], [createdby], [job_nr],
                           [profile_name], [status], [CUSTOMPORT], [PRINTER], [EMAILFROM], [EMAILTO], [CREATEDDATETIME], [HOS], [Debitorenkonto]
                    {baseQuery}
                    ORDER BY RGLNR ASC  -- Default order by RGLNR ASC
                    OFFSET @start ROWS FETCH NEXT @length ROWS ONLY";
                }

                string filteredCountQuery = "SELECT COUNT(*) " + baseQuery;

                db.Open();
                var parameters = new
                {
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
                    length,
                    orderColumnIndex,
                    orderDir,
                    searchValue,
                    searchziel = searchField == "searchziel" ? searchValue : (object)DBNull.Value,
                    searchRGLNR = searchField == "searchrglnr" ? searchValue : (object)DBNull.Value,
                    searchdebitor = searchField == "searchdebitor" ? searchValue : (object)DBNull.Value,
                    searchinvoice = searchField == "searchinvoice" ? searchValue : (object)DBNull.Value,
                    searchdebitorrequest = searchField == "searchdebitorrequest" ? searchValue : (object)DBNull.Value,
                    searchdebitorreference = searchField == "searchdebitorreference" ? searchValue : (object)DBNull.Value,
                    searchcreatedby = searchField == "searchcreatedby" ? searchValue : (object)DBNull.Value,
                    searchjobnr = searchField == "searchjobnr" ? searchValue : (object)DBNull.Value,
                    searchlobsterprofile = searchField == "searchlobsterprofile" ? searchValue : (object)DBNull.Value,
                    searchlobsterstatus = searchField == "searchlobsterstatus" ? searchValue : (object)DBNull.Value
                };

                var data = await db.QueryAsync<LOG_RGLNR_Model>(dataQuery, parameters, commandTimeout: 120);

                var recordsFiltered = await db.ExecuteScalarAsync<int>(filteredCountQuery, parameters);

                var recordsTotal = await db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM [wsmb].[dbo].[END_TO_END_STAGING]");
                db.Close();

                return Json(new { draw, recordsFiltered, recordsTotal, data });
            }

        }

        [HttpPost]
        public IActionResult GetInvoiceDetails(string invoiceId, DateTime? entry_date)
        {
            string query = "SELECT * FROM [END_TO_END_STAGING] WHERE Rechnung = @InvoiceId ORDER BY CASE WHEN entry_date = @entry_date THEN 0 ELSE 1 END, entry_date";
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var parameters = new { invoiceId, entry_date };
                var results = connection.Query<LOG_RGLNR_Model>(query, parameters).ToList();

                return Json(results);
            }
        }
    }
}