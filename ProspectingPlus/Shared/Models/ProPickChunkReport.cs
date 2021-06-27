using System.Collections.Generic;
using System.Linq;
using ProspectingPlus.Shared.Packets;
using Vintagestory.API.Common;

namespace ProspectingPlus.Shared.Models
{
    public class ProPickChunkReport
    {
        public IReadOnlyList<ProPickOreReport> OreReports { get; }
        public string ReportByPlayerUID { get; }
        public int ChunkX { get; }
        public int ChunkZ { get; }

        public ProPickChunkReport(IReadOnlyList<ProPickOreReport> oreReports,
            IPlayer reportByPlayer,
            int chunkX,
            int chunkZ)
        {
            OreReports = oreReports;
            ReportByPlayerUID = reportByPlayer.PlayerUID;
            ChunkX = chunkX;
            ChunkZ = chunkZ;
        }

        public ProPickChunkReport(ChunkReportPacket packet, ICoreAPI api)
        {
            ReportByPlayerUID = api.World.AllOnlinePlayers.First(x => x.PlayerUID == packet.PlayerUID).PlayerUID;
            ChunkX = packet.ChunkX;
            ChunkZ = packet.ChunkZ;
            OreReports = packet.OreKeys
                .Select((t, i) => new ProPickOreReport(t, packet.Ppts[i], packet.Densities[i])).ToList();
        }
    }
}