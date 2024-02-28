using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;

namespace RGLNR_Interface.Services 
{
    public class ActiveDirectoryService
    {
        public string GetUserDepartment()
        {
            var userPrincipalName = WindowsIdentity.GetCurrent().Name;
            using (var context = new PrincipalContext(ContextType.Domain))
            {
                var userPrincipal = UserPrincipal.FindByIdentity(context, userPrincipalName);
                if (userPrincipal != null)
                {
                    var directoryEntry = userPrincipal.GetUnderlyingObject() as DirectoryEntry;
                    if (directoryEntry != null && directoryEntry.Properties.Contains("Department"))
                    {
                        
                        return directoryEntry.Properties["Department"].Value.ToString();
                    }
                }
            }
            return "Fehler: Attribut nicht gefunden";
        }
    }
}
