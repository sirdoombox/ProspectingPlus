using ProspectingPlus.Shared.Enums;
using Vintagestory.API.MathTools;

namespace ProspectingPlus.Shared.Extensions
{
    public static class OreDensityExtensions
    {
        private static readonly OreDensity[] densities =
        {
            OreDensity.VeryPoor,
            OreDensity.Poor,
            OreDensity.Decent,
            OreDensity.High,
            OreDensity.VeryHigh,
            OreDensity.UltraHigh
        };


        public static string ToLangKey(this OreDensity oreDensity)
        {
            return oreDensity is OreDensity.Miniscule
                ? "Miniscule Amounts Of"
                : $"propick-density-{oreDensity.ToString().ToLower()}";
        }

        public static OreDensity ToDensity(this double totalFactor)
        {
            return densities[(int) GameMath.Clamp(totalFactor * 7.5, 0.0, 5.0)];
        }
    }
}