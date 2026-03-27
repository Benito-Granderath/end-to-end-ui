using RGLNR_Interface.Models;

namespace RGLNR_Interface.Services
{
    public interface IPermissionService
    {
        Task<IEnumerable<UserPermission>> GetUserPermissionsAsync(string username);
    }
}
