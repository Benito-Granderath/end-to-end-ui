using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using RGLNR_Interface.Models;
using System.Data;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Globalization;
using System;

namespace RGLNR_Interface.Controllers
{
    public class LOG_RGLNRController : Controller
    {
        private readonly IConfiguration _configuration;

        public LOG_RGLNRController(IConfiguration configuration, ILogger<LOG_RGLNRController> logger)
        {
            _configuration = configuration;
        }


        public IActionResult Index()
        {
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
                var startDateStr = HttpContext.Request.Form["startDate"].FirstOrDefault();
                var endDateStr = HttpContext.Request.Form["endDate"].FirstOrDefault();
                var companyPrefix = HttpContext.Request.Form["companyPrefix"].FirstOrDefault();


                DateTime? parsedStartDate = null, parsedEndDate = null;

                if (DateTime.TryParseExact(startDateStr, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var startDate))
                {
                    parsedStartDate = startDate;
                }

                if (DateTime.TryParseExact(endDateStr, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var endDate))
                {
                    parsedEndDate = endDate;
                }


                string baseQuery = @"FROM [wsmb].[dbo].[LOG_RGLNR] WHERE RGLNR != 0";

                if (parsedStartDate.HasValue && parsedEndDate.HasValue)
                {
                    baseQuery += " AND ABGDATUMZEIT BETWEEN @startDate AND @endDate";
                }
                if (minRGLNRParsed)
                {
                    baseQuery += " AND RGLNR >= @minRGLNR";
                }
                if (maxRGLNRParsed)
                {
                    baseQuery += " AND RGLNR <= @maxRGLNR";
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    baseQuery += " AND (CAST(RGLNR AS VARCHAR) LIKE @searchValue OR AXRGNR LIKE @searchValue)";
                }

                if (!string.IsNullOrEmpty(companyPrefix))
                {
                    if (companyPrefix == "AR")
                    {
                        baseQuery += " AND AXRGNR LIKE @companyPrefix + '%'";
                    }
                    else
                    {
                        baseQuery += " AND AXRGNR LIKE @companyPrefix + '%'";
                    }
                }

                string dataQuery = "SELECT [RGLNR], [AXRGNR], [ABGDATUMZEIT], [AKTIONSNUMMER], [SENDERGLN], [SENDERNAME], [EMPFGLN], [EMPFNAME], [RGBETRAG] " +
                                   baseQuery +
                                   " ORDER BY RGLNR OFFSET @start ROWS FETCH NEXT @length ROWS ONLY";

                string filteredCountQuery = "SELECT COUNT(*) " + baseQuery;

                db.Open();

                var parameters = new
                {
                    searchValue = $"%{searchValue}%",
                    minRGLNR = minRGLNRParsed ? minRGLNR : (object)DBNull.Value,
                    maxRGLNR = maxRGLNRParsed ? maxRGLNR : (object)DBNull.Value,
                    startDate = parsedStartDate,
                    endDate = parsedEndDate,
                    companyPrefix,
                    start,
                    length
                };

                var data = await db.QueryAsync<LOG_RGLNR_Model>(dataQuery, parameters);

                var recordsFiltered = await db.ExecuteScalarAsync<int>(filteredCountQuery, parameters);

                var recordsTotal = await db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM [wsmb].[dbo].[LOG_RGLNR] WHERE RGLNR != 0");

                db.Close();

                return Json(new { draw = draw, recordsFiltered = recordsFiltered, recordsTotal = recordsTotal, data = data });
            }
        }


    }
}
