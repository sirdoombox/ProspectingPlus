using System;
using System.Collections.Generic;
using System.Linq;
using ProspectingPlus.Shared.Enums;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

namespace ProspectingPlus.Shared.Extensions
{
    public static class OreDensityExtensions
    {
        private static readonly OreDensity[] Densities =
        {
            OreDensity.VeryPoor,
            OreDensity.Poor,
            OreDensity.Decent,
            OreDensity.High,
            OreDensity.VeryHigh,
            OreDensity.UltraHigh
        };


        public static string ToLangKey(this OreDensity oreDensity, bool sentenceLike = true)
        {
            return oreDensity is OreDensity.Miniscule
                ? sentenceLike ? "Miniscule Amounts Of" : "Miniscule Amounts"
                : $"propick-density-{oreDensity.ToString().ToLower()}";
        }

        public static OreDensity ToDensity(this double totalFactor)
        {
            return Densities[(int) GameMath.Clamp(totalFactor * 7.5, 0.0, 5.0)];
        }

        public static Dictionary<string, string> GetOreDensityStrings()
        {
            var list = Densities.ToList();
            list.Insert(0, OreDensity.Miniscule);
            return list.ToDictionary(x => x.ToString(), y => Lang.Get(y.ToLangKey(false)));
        }

        public static OreDensity ToDensityEnum(this string densityString)
        {
            return (OreDensity) Enum.Parse(typeof(OreDensity), densityString);
        }
    }
}