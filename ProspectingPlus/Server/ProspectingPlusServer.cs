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
        
        public ProspectingPlusServer(ICoreServerAPI api)
        {
            _api = api;
            _chan = _api.Network.RegisterChannelAndTypes()
                .SetMessageHandler<ChunkReportPacket>(ChunkReportReceived);
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