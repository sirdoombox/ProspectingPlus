using System.Collections.Generic;
using System.Linq;
using ProspectingPlus.Shared.Extensions;
using ProspectingPlus.Shared.Models;
using ProspectingPlus.Shared.Packets;
using Vintagestory.API.Server;

namespace ProspectingPlus.Server
{
    public class ProspectingPlusServer
    {
        private readonly ICoreServerAPI _api;
        private readonly IServerNetworkChannel _chan;
        private readonly List<ProPickChunkReport> _reports = new List<ProPickChunkReport>();

        // TODO: Support the groups system.
        // TODO: Implement data storage and retrieval.

        public ProspectingPlusServer(ICoreServerAPI api)
        {
            _api = api;
            _chan = _api.Network.RegisterChannelAndTypes()
                .SetMessageHandler<ChunkReportPacket>(ChunkReportReceived);
            api.Event.PlayerJoin += OnPlayerJoined;
        }

        private void OnPlayerJoined(IServerPlayer joinedPlayer)
        {
            foreach (var report in _reports)
                _chan.SendPacket(report, joinedPlayer);
        }

        private void ChunkReportReceived(IServerPlayer _, ChunkReportPacket reportPacket)
        {
            var report = new ProPickChunkReport(reportPacket, _api);
            if (_reports.Any(x => x.ChunkX == report.ChunkX && x.ChunkZ == report.ChunkZ))
                return; // discard any reports for existing chunks.
            _reports.Add(report);
            _chan.BroadcastPacket(reportPacket);
        }
    }
}