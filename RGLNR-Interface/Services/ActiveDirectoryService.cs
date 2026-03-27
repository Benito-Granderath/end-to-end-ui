using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;
using RGLNR_Interface.Models;

namespace RGLNR_Interface.Services
{
    public class ActiveDirectoryService : IActiveDirectoryService
    {
        private readonly string _domain;

        public ActiveDirectoryService(IConfiguration configuration)
        {
            _domain = configuration["ActiveDirectory:Domain"] ?? "wuensche-group.local";
        }

        public List<string> GetUserTargetGroups(string sAMAccountName)
        {
            List<string> groupsUserIsMemberOf = new List<string>();

            List<string> targetGroupNames = TenantDefinition.All.Select(t => t.GroupName).ToList();

            var targetGroupSIDs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            using (PrincipalContext context = new PrincipalContext(ContextType.Domain, _domain))
            {
                foreach (var groupName in targetGroupNames)
                {
                    GroupPrincipal group = GroupPrincipal.FindByIdentity(context, groupName);
                    if (group != null)
                    {
                        targetGroupSIDs[group.Sid.Value] = groupName;
                    }
                }

                UserPrincipal user = UserPrincipal.FindByIdentity(context, sAMAccountName);
                if (user == null)
                {
                    Console.WriteLine("User does not exist");
                    return new List<string>();
                }

                DirectoryEntry userEntry = (DirectoryEntry)user.GetUnderlyingObject();

                userEntry.RefreshCache(new[] { "tokenGroups" });

                foreach (byte[] sidBytes in userEntry.Properties["tokenGroups"])
                {
                    SecurityIdentifier sid = new SecurityIdentifier(sidBytes, 0);
                    if (targetGroupSIDs.TryGetValue(sid.Value, out string groupName))
                    {
                        groupsUserIsMemberOf.Add(groupName);
                    }
                }
            }

            return groupsUserIsMemberOf;
        }
    }
}
