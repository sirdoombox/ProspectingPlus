using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ProspectingPlus.Shared.Packets;

namespace ProspectingPlus.Shared.Models
{
    public class ProPickChunkReport
    {
        public IReadOnlyList<ProPickOreReport> OreReports { get; }
        public string PlayerName { get; }
        public string PlayerUID { get; }
        public int ChunkX { get; }
        public int ChunkZ { get; }

        public ProPickChunkReport(ChunkReportPacket packet)
        {
            PlayerUID = packet.PlayerUID;
            PlayerName = packet.PlayerName;
            ChunkX = packet.ChunkX;
            ChunkZ = packet.ChunkZ;
            if (!(packet.OreKeys is null))
                OreReports = packet.OreKeys
                    .Select((t, i) => new ProPickOreReport(t, packet.Ppts[i], packet.Densities[i])).ToList();
            else
                OreReports = new List<ProPickOreReport>();
        }

        [JsonConstructor]
        public ProPickChunkReport(List<ProPickOreReport> oreReports, string playerUID, int chunkX, int chunkZ,
            string playerName)
        {
            OreReports = oreReports;
            PlayerUID = playerUID;
            ChunkX = chunkX;
            ChunkZ = chunkZ;
            PlayerName = playerName;
        }
    }
}