using ProspectingPlus.Shared.Enums;
using ProspectingPlus.Shared.Models;
using ProtoBuf;

namespace ProspectingPlus.Shared.Packets
{
    [ProtoContract]
    public class ChunkReportPacket
    {
        [ProtoMember(1)] public string PlayerUID;
        [ProtoMember(2)] public int ChunkX;
        [ProtoMember(3)] public int ChunkZ;
        [ProtoMember(4)] public string[] OreKeys;
        [ProtoMember(5)] public double[] Ppts;
        [ProtoMember(6)] public OreDensity[] Densities;
        [ProtoMember(7)] public string PlayerName;

        public ChunkReportPacket(ProPickChunkReport proPickChunkReport)
        {
            PlayerUID = proPickChunkReport.PlayerUID;
            ChunkX = proPickChunkReport.ChunkX;
            ChunkZ = proPickChunkReport.ChunkZ;
            var len = proPickChunkReport.OreReports.Count;
            OreKeys = new string[len];
            Ppts = new double[len];
            PlayerName = proPickChunkReport.PlayerName;
            Densities = new OreDensity[len];
            for (var i = 0; i < len; i++)
            {
                var oreRep = proPickChunkReport.OreReports[i];
                OreKeys[i] = oreRep.OreKey;
                Ppts[i] = oreRep.Ppt;
                Densities[i] = oreRep.Density;
            }
        }

        public ChunkReportPacket()
        {
        }
    }
}