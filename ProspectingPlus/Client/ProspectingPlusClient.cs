using System.Reflection;
using HarmonyLib;
using ProspectingPlus.Patches;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace ProspectingPlus.Client
{
    public class ProspectingPlusClient
    {
        private readonly ICoreClientAPI _api;
        
        public ProspectingPlusClient(ICoreClientAPI api)
        {
            _api = api;
            new Harmony("com.prospectingplus.patches").PatchAll(Assembly.GetExecutingAssembly());
            ProspectingPickPatch.OnChunkReportGenerated += chunkReport =>
            {
                foreach (var ore in chunkReport.OreReports)
                    chunkReport.ReportByPlayer.SendMessage(GlobalConstants.InfoLogChatGroup, $"{ore.OreKey} | {ore.Density} | {ore.Ppt}",
                        EnumChatType.Notification);
            };
        }
    }
}