namespace RGLNR_Interface.Services
{
    public interface IActiveDirectoryService
    {
        List<string> GetUserTargetGroups(string sAMAccountName);
    }
}
