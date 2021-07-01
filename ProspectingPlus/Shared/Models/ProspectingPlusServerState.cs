using System.Collections.Generic;

namespace ProspectingPlus.Shared.Models
{
    public class ProspectingPlusServerState : ModDataBase
    {
        public override string FileName => "prospectingplus.serverstate.json";

        public List<ProPickChunkReport> Reports { get; set; }

        public override void Default()
        {
            Reports = new List<ProPickChunkReport>();
        }
    }
}