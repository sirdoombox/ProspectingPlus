using System.Linq;
using System.Reflection;
using HarmonyLib;
using ProspectingPlus.Patches;
using ProspectingPlus.Shared.Extensions;
using ProspectingPlus.Shared.Models;
using ProspectingPlus.Shared.Packets;
using ProspectingPlus.Shared.Utils;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using Vintagestory.ServerMods;

namespace ProspectingPlus.Server
{
    public class ProspectingPlusServer
    {
        private readonly ICoreServerAPI _api;
        private readonly IServerNetworkChannel _chan;
        private readonly ProspectingPlusServerState _serverState;
        private readonly OreList _oreList;

        // TODO: Support the groups system.

        public ProspectingPlusServer(ICoreServerAPI api)
        {
            _api = api;
            var assets = _api.Assets.GetMany<DepositVariant[]>(_api.World.Logger, "worldgen/deposits/");
            _oreList = new OreList(assets.SelectMany(x => x.Value)
                .Select(x => $"ore-{x.Code}")
                .Distinct()
                .Where(x => x != Lang.Get(x))
                .ToList());
            _chan = _api.Network.RegisterChannelAndTypes();
            api.Event.PlayerNowPlaying += OnPlayerReady;
            api.Event.GameWorldSave += OnGameWorldSave;

            _serverState = ModDataUtil.GetOrCreateDefault<ProspectingPlusServerState>(_api.World.SavegameIdentifier);

            var harmony = new Harmony("prospectingplus.patches");
            harmony.PatchAll(Assembly.GetAssembly(typeof(ProspectingPlusServer)));
            ProspectingPickPatch.OnChunkReportGenerated += OnChunkReportGenerated;
        }

        private void OnChunkReportGenerated(ProPickChunkReport report)
        {
            if (_serverState.Reports.Any(x => x.ChunkX == report.ChunkX && x.ChunkZ == report.ChunkZ))
                return; // discard any reports for existing chunks.
            _serverState.Reports.Add(report);
            _chan.BroadcastPacket(new ChunkReportPacket(report));
        }

        private void OnGameWorldSave()
        {
            _serverState.WriteToDisk(_api.World.SavegameIdentifier);
        }

        private void OnPlayerReady(IServerPlayer joinedPlayer)
        {
            _chan.SendPacket(_oreList, joinedPlayer);
            foreach (var report in _serverState.Reports)
                _chan.SendPacket(new ChunkReportPacket(report), joinedPlayer);
        }
    }
}