using RGLNR_Interface.Models;

namespace RGLNR_Interface.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IActiveDirectoryService _adSearch;

        private static readonly Dictionary<string, UserPermission> GroupPermissionMappings =
            TenantDefinition.All
                .Select((t, index) => new { t.GroupName, Permission = new UserPermission { PermissionId = index + 1, MandantId = t.MandantId } })
                .ToDictionary(x => x.GroupName, x => x.Permission);

        public PermissionService(IActiveDirectoryService adSearch)
        {
            _adSearch = adSearch;
        }

        public Task<IEnumerable<UserPermission>> GetUserPermissionsAsync(string username)
        {
            List<string> groupNames = _adSearch.GetUserTargetGroups(username);

            List<UserPermission> userPermissions = new List<UserPermission>();

            foreach (var groupName in groupNames)
            {
                if (GroupPermissionMappings.TryGetValue(groupName, out var permission))
                {
                    userPermissions.Add(permission);
                }
            }

            return Task.FromResult<IEnumerable<UserPermission>>(userPermissions);
        }
    }
}
