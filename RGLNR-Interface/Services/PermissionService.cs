using Dapper;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using RGLNR_Interface.Models;
using Microsoft.AspNetCore.Authorization;

namespace RGLNR_Interface.Services
{
    [Authorize]
    public class PermissionService
    {
        private readonly string _connectionString;

        public PermissionService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<UserPermission>> GetUserPermissionsAsync(string userSID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var parameters = new { userSID = userSID };
                var userPermissions = await connection.QueryAsync<UserPermission>(
                    "GetUserPermissions",
                    param: parameters,
                    commandType: System.Data.CommandType.StoredProcedure);
                return userPermissions;
            }
        }
    }


}
