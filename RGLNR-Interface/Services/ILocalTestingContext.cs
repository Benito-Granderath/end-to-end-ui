namespace RGLNR_Interface.Services
{
    public interface ILocalTestingContext
    {
        bool IsEnabled { get; }

        string UserName { get; }

        IReadOnlyList<int> Permissions { get; }
    }
}
