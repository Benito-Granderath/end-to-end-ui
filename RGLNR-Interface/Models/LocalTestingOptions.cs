namespace RGLNR_Interface.Models
{
    public class LocalTestingOptions
    {
        public const string SectionName = "LocalTesting";

        public bool Enabled { get; set; }

        public string UserName { get; set; } = "WUENSCHE\\local.tester";

        public int[] Permissions { get; set; } = Array.Empty<int>();
    }
}
