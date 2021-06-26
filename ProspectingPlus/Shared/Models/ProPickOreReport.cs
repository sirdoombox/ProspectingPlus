using ProspectingPlus.Shared.Enums;

namespace ProspectingPlus.Shared.Models
{
    public struct ProPickOreReport
    {
        public OreDensity Density { get; }
        public string OreKey { get; }
        public double Ppt { get; }

        public ProPickOreReport(string oreKey, double ppt, OreDensity density)
        {
            Density = density;
            OreKey = oreKey;
            Ppt = ppt;
        }
    }
}