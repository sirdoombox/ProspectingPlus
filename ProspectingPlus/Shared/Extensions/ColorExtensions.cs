using System.Drawing;
using Vintagestory.API.MathTools;

namespace ProspectingPlus.Shared.Extensions
{
    public static class ColorExtensions
    {
        public static int ToVsArgb(this Color color)
        {
            return ColorUtil.ToRgba(color.A, color.B, color.G, color.R);
        }
    }
}