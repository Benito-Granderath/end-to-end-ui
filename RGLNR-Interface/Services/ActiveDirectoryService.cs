using Microsoft.AspNetCore.Authorization;
using System.Collections.Concurrent;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;

namespace RGLNR_Interface.Services
{
    public class ActiveDirectorySearch
    {
        public List<string> GetUserTargetGroupsParallel(string sAMAccountName)
        {
            List<string> groupsUserIsMemberOf = new List<string>();

            List<string> targetGroupNames = new List<string>
        {
            "DL_End2End_Mandant510",
            "DL_End2End_Mandant575",
            "DL_End2End_Mandant430",
            "DL_End2End_Mandant400",
            "DL_End2End_Mandant310",
            "DL_End2End_Mandant300",
            "DL_End2End_Mandant200",
            "DL_End2End_Mandant100",
            "DL_End2End_Mandant420"
        };

            var targetGroupSIDs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            using (PrincipalContext context = new PrincipalContext(ContextType.Domain, "wuensche-group.local"))
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
