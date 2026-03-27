using Microsoft.AspNetCore.Mvc;
using RGLNR_Interface.Models;
using RGLNR_Interface.Services;

namespace RGLNR_Interface.Controllers
{
    public class LOG_RGLNRController : Controller
    {
        private readonly IPermissionService _permissionService;
        private readonly IStagingDataService _stagingDataService;
        private readonly ILocalTestingContext _localTestingContext;

        public LOG_RGLNRController(IPermissionService permissionService, IStagingDataService stagingDataService, ILocalTestingContext localTestingContext)
        {
            _permissionService = permissionService;
            _stagingDataService = stagingDataService;
            _localTestingContext = localTestingContext;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            string sAMAccountName = GetSamAccountName(GetEffectiveUserName());
            ViewBag.UserName = sAMAccountName;

            var permissions = await GetEffectivePermissionsAsync(sAMAccountName);

            if (permissions == null || !permissions.Any())
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            return View(permissions);
        }

        [HttpPost]
        public async Task<IActionResult> LoadData()
        {
            var form = HttpContext.Request.Form;

            var request = new LoadDataRequest
            {
                Draw = form["draw"].FirstOrDefault(),
                Start = int.Parse(form["start"].FirstOrDefault() ?? "0"),
                Length = int.Parse(form["length"].FirstOrDefault() ?? "0"),
                OrderColumnIndex = form["order[0][column]"].FirstOrDefault(),
                OrderDir = form["order[0][dir]"].FirstOrDefault(),
                MinRGLNR = form["minRGLNR"].FirstOrDefault(),
                MaxRGLNR = form["maxRGLNR"].FirstOrDefault(),
                PasteInvoices = form["pasteInvoices"].FirstOrDefault(),
                StartDate = form["startDate"].FirstOrDefault(),
                EndDate = form["endDate"].FirstOrDefault(),
                CompanyPrefix = form["companyPrefix"].FirstOrDefault(),
                FaelligStart = form["faelligStart"].FirstOrDefault(),
                FaelligEnd = form["faelligEnd"].FirstOrDefault(),
                BestaetigungStart = form["bestaetigungStart"].FirstOrDefault(),
                BestaetigungEnd = form["bestaetigungEnd"].FirstOrDefault(),
                SearchField = form["searchField"].FirstOrDefault(),
                SearchValue = form["searchValue"].FirstOrDefault()
            };

            var permittedIds = await GetPermittedDataAreaIds(request.CompanyPrefix);
            var (data, recordsFiltered, recordsTotal) = await _stagingDataService.GetPagedDataAsync(request, permittedIds);

            return Json(new { draw = request.Draw, recordsFiltered, recordsTotal, data });
        }

        [HttpPost]
        public async Task<IActionResult> GetInvoiceDetails(string invoiceId, DateTime? entry_date)
        {
            var results = await _stagingDataService.GetInvoiceDetailsAsync(invoiceId, entry_date);
            return Json(results);
        }

        private async Task<List<int>> GetPermittedDataAreaIds(string? companyPrefix)
        {
            string username = GetSamAccountName(GetEffectiveUserName());
            var permissions = await GetEffectivePermissionsAsync(username);
            var allowedMandantIds = permissions.Select(p => p.MandantId).Distinct().ToList();

            if (!allowedMandantIds.Any())
            {
                return new List<int>();
            }

            if (int.TryParse(companyPrefix, out int prefix) && prefix == -1)
            {
                return allowedMandantIds;
            }

            if (int.TryParse(companyPrefix, out int specificId) && specificId > 0)
            {
                return allowedMandantIds.Contains(specificId)
                    ? new List<int> { specificId }
                    : new List<int>();
            }

            return allowedMandantIds;
        }

        private string GetEffectiveUserName()
        {
            if (_localTestingContext.IsEnabled)
            {
                return _localTestingContext.UserName;
            }

            return User.Identity?.Name ?? string.Empty;
        }

        private async Task<IEnumerable<UserPermission>> GetEffectivePermissionsAsync(string username)
        {
            if (_localTestingContext.IsEnabled)
            {
                return _localTestingContext.Permissions.Select((mandantId, index) => new UserPermission
                {
                    PermissionId = index + 1,
                    MandantId = mandantId
                });
            }

            return await _permissionService.GetUserPermissionsAsync(username);
        }

        private static string GetSamAccountName(string identityName)
        {
            if (!string.IsNullOrEmpty(identityName))
            {
                var parts = identityName.Split('\\');
                return parts.Length > 1 ? parts[1] : identityName;
            }

            return identityName;
        }
    }
}
