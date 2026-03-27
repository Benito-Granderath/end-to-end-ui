using Microsoft.Extensions.Options;
using RGLNR_Interface.Models;

namespace RGLNR_Interface.Services
{
    public class LocalTestingContext : ILocalTestingContext
    {
        private readonly IWebHostEnvironment _environment;
        private readonly LocalTestingOptions _options;

        public LocalTestingContext(IWebHostEnvironment environment, IOptions<LocalTestingOptions> options)
        {
            _environment = environment;
            _options = options.Value;
        }

        public bool IsEnabled => _environment.IsDevelopment() && _options.Enabled;

        public string UserName => _options.UserName;

        public IReadOnlyList<int> Permissions => _options.Permissions;
    }
}
