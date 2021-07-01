using System.Collections.Generic;
using ProspectingPlus.Shared.Enums;

namespace ProspectingPlus.Shared.Models
{
    public class ProspectingPlusClientState : ModDataBase
    {
        public override string FileName => "prospectingplus.clientstate.json";

        public List<string> EnabledOreKeys { get; set; }
        public bool OverlayEnabled { get; set; }
        public bool TextPrintoutEnabled { get; set; }
        public OreDensity SelectedMinimumDensity { get; set; }
        
        public override void Default()
        {
            EnabledOreKeys = new List<string>();
            OverlayEnabled = true;
            TextPrintoutEnabled = false;
            SelectedMinimumDensity = OreDensity.Miniscule;
        }
    }
}