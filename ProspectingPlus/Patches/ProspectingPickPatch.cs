using System.Collections.Generic;
using HarmonyLib;
using ProspectingPlus.Shared.Enums;
using ProspectingPlus.Shared.Extensions;
using ProspectingPlus.Shared.Models;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using Vintagestory.ServerMods;

namespace ProspectingPlus.Patches
{
    [HarmonyPatch(typeof(ItemProspectingPick), "PrintProbeResults")]
    public class ProspectingPickPatch
    {
        private static readonly AccessTools.FieldRef<ItemProspectingPick, ICoreAPI> ApiRef =
            AccessTools.FieldRefAccess<ItemProspectingPick, ICoreAPI>("api");

        private static readonly AccessTools.FieldRef<ItemProspectingPick, ProPickWorkSpace> PpwsRef =
            AccessTools.FieldRefAccess<ItemProspectingPick, ProPickWorkSpace>("ppws");

        public static Action<ProPickChunkReport> OnChunkReportGenerated { get; set; }

        public static bool Prefix(
            ItemProspectingPick __instance,
            IWorldAccessor world,
            IServerPlayer byPlayer,
            ItemSlot itemslot,
            BlockPos pos)
        {
            var api = ApiRef(__instance);
            var ppws = PpwsRef(__instance);
            if (api.ModLoader.GetModSystem<GenDeposits>()?.Deposits == null)
                return true;
            var blockAccessor = world.BlockAccessor;
            var regionSize = blockAccessor.RegionSize;
            var mapRegion = world.BlockAccessor.GetMapRegion(pos.X / regionSize, pos.Z / regionSize);
            var num1 = pos.X % regionSize;
            var num2 = pos.Z % regionSize;
            pos = pos.Copy();
            pos.Y = world.BlockAccessor.GetTerrainMapheightAt(pos);
            var rockColumn = ppws.GetRockColumn(pos.X, pos.Z);
            var oreReports = new List<ProPickOreReport>();
            foreach (var oreMap in mapRegion.OreMaps)
            {
                var intDataMap2D = oreMap.Value;
                var innerSize = intDataMap2D.InnerSize;
                var unpaddedColorLerped = intDataMap2D.GetUnpaddedColorLerped(
                    num1 / (float) regionSize * innerSize,
                    num2 / (float) regionSize * innerSize);
                if (!ppws.depositsByCode.ContainsKey(oreMap.Key))
                {
                    oreReports.Add(new ProPickOreReport("ore-" + oreMap.Key, 1.0, OreDensity.Unknown));
                }
                else
                {
                    ppws.depositsByCode[oreMap.Key]
                        .GetPropickReading(
                            pos,
                            unpaddedColorLerped,
                            rockColumn,
                            out var ppt,
                            out var totalFactor);
                    if (totalFactor > 0.025)
                        oreReports.Add(new ProPickOreReport("ore-" + oreMap.Key, ppt, totalFactor.ToDensity()));
                    else if (totalFactor > 0.002)
                        oreReports.Add(new ProPickOreReport("ore-" + oreMap.Key, ppt, OreDensity.Miniscule));
                }
            }

            byPlayer.SendMessage(GlobalConstants.InfoLogChatGroup, "Prospecting Complete.", EnumChatType.Notification);
            var chunkSize = api.World.BlockAccessor.ChunkSize;
            OnChunkReportGenerated?.Invoke(
                new ProPickChunkReport(oreReports, byPlayer, pos.X / chunkSize, pos.Z / chunkSize));
            return false; // Skip the original method, we have no interest in reporting prospecting in the standard way.
        }
    }
}