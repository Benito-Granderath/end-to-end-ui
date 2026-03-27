namespace RGLNR_Interface.Models
{
    public class TenantDefinition
    {
        public int MandantId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;

        public static readonly IReadOnlyList<TenantDefinition> All = new List<TenantDefinition>
        {
            new() { MandantId = 100, GroupName = "DL_End2End_Mandant100", CompanyName = "Wünsche Handelsgesellschaft mbH & Co. KG" },
            new() { MandantId = 200, GroupName = "DL_End2End_Mandant200", CompanyName = "Dario GmbH & Co. KG" },
            new() { MandantId = 300, GroupName = "DL_End2End_Mandant300", CompanyName = "Monz Handelsgesellschaft International mbH" },
            new() { MandantId = 310, GroupName = "DL_End2End_Mandant310", CompanyName = "Monz International Verwaltungs GmbH" },
            new() { MandantId = 400, GroupName = "DL_End2End_Mandant400", CompanyName = "Wünsche Fashion GmbH & Co. KG" },
            new() { MandantId = 420, GroupName = "DL_End2End_Mandant420", CompanyName = "Duo Fashion GmbH" },
            new() { MandantId = 430, GroupName = "DL_End2End_Mandant430", CompanyName = "Flexxtrade GmbH & Co. KG" },
            new() { MandantId = 510, GroupName = "DL_End2End_Mandant510", CompanyName = "Globaltronics GmbH & Co. KG" },
            new() { MandantId = 575, GroupName = "DL_End2End_Mandant575", CompanyName = "Müller-Licht International GmbH" },
        };
    }
}
