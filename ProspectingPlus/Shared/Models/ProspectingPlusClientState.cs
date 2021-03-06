using System.Collections.Generic;
using ProspectingPlus.Shared.Enums;

namespace ProspectingPlus.Shared.Models
{
    public class ProspectingPlusClientState : ModDataBase
    {
        public override string FileName => "prospectingplus.clientstate.json";

        public List<string> EnabledOreKeys { get; set; }
        public bool OverlayEnabled { get; set; }
        public bool TextReportsEnabled { get; set; }
        public OreDensity SelectedMinimumDensity { get; set; }
        public int OverlayOpacityPercent { get; set; }
        public bool GridEnabled { get; set; }
        public int GridOpacityPercent { get; set; }

        public override void Default()
        {
            EnabledOreKeys = null;
            OverlayEnabled = true;
            GridEnabled = true;
            GridOpacityPercent = 45;
            TextReportsEnabled = false;
            SelectedMinimumDensity = OreDensity.Miniscule;
            OverlayOpacityPercent = 45;
        }
    }
}