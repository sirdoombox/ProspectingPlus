using System.Collections.Generic;
using System.Drawing;
using ProspectingPlus.Shared.Enums;

namespace ProspectingPlus.Shared.Constants
{
    public static class ModConstants
    {
        public const string MapLayerName = "ProspectingPlus";
        public const string OverlayHotkeyCode = "toggleProspectingOverlay";
        public const string DataFileName = "prospectingplus.reports.json";

        public static readonly IReadOnlyDictionary<OreDensity, Color> DensityColorMap =
            new Dictionary<OreDensity, Color>
            {
                {OreDensity.Unknown, Color.FromArgb(145, 255, 255, 255)},
                {OreDensity.Miniscule, Color.FromArgb(145, 0, 255, 17)},
                {OreDensity.VeryPoor, Color.FromArgb(145, 133, 230, 0)},
                {OreDensity.Poor, Color.FromArgb(145, 180, 203, 0)},
                {OreDensity.Decent, Color.FromArgb(145, 213, 174, 0)},
                {OreDensity.High, Color.FromArgb(145, 235, 141, 0)},
                {OreDensity.VeryHigh, Color.FromArgb(145, 249, 105, 0)},
                {OreDensity.UltraHigh, Color.FromArgb(145, 253, 62, 2)}
            };
    }
}