using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;

namespace RGLNR_Interface.Services
{
    public class ActiveDirectoryService
    {
        public string GetUserSID(string username)
        {
            using (var context = new PrincipalContext(ContextType.Domain))
            {
                var userPrincipal = UserPrincipal.FindByIdentity(context, username);
                if (userPrincipal != null)
                {
                    var directoryEntry = userPrincipal.GetUnderlyingObject() as DirectoryEntry;
                    if (directoryEntry != null && directoryEntry.Properties.Contains("objectSid"))
                    {
                        var sid = new SecurityIdentifier((byte[])directoryEntry.Properties["objectSid"].Value, 0);
                        return sid.ToString();
                    }
                }
            }
            return "SID not found";
        }
    }
}
