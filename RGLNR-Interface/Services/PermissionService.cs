using Dapper;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using RGLNR_Interface.Models;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;

namespace RGLNR_Interface.Services
{
    [Authorize]
    public class PermissionService
    {
        private readonly ActiveDirectorySearch _adSearch;

        public PermissionService(ActiveDirectorySearch adSearch)
        {
            _adSearch = adSearch;
        }

        public async Task<IEnumerable<UserPermission>> GetUserPermissionsAsync(string username)
        {
            List<string> groupNames = _adSearch.GetUserTargetGroupsParallel(username);

            List<UserPermission> userPermissions = new List<UserPermission>();

            foreach (var groupName in groupNames)
            {
                if (GroupPermissionMappings.ContainsKey(groupName))
                {
                    userPermissions.Add(GroupPermissionMappings[groupName]);
                }
            }
            return userPermissions;
        }

        private static Dictionary<string, UserPermission> GroupPermissionMappings = new Dictionary<string, UserPermission>
    {
        { "DL_End2End_Mandant510", new UserPermission { permissionID = 5, description = 510 } },
        { "DL_End2End_Mandant575", new UserPermission { permissionID = 6, description = 575 } },
        { "DL_End2End_Mandant430", new UserPermission { permissionID = 9, description = 430 } },
        { "DL_End2End_Mandant400", new UserPermission { permissionID = 3, description = 400 } },
        { "DL_End2End_Mandant310", new UserPermission { permissionID = 8, description = 310 } },
        { "DL_End2End_Mandant300", new UserPermission { permissionID = 7, description = 300 } },
        { "DL_End2End_Mandant200", new UserPermission { permissionID = 2, description = 200 } },
        { "DL_End2End_Mandant100", new UserPermission { permissionID = 1, description = 100 } },
        { "DL_End2End_Mandant420", new UserPermission { permissionID = 4, description = 420 } }
    };
    }



}
