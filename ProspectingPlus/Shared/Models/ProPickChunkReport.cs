using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace ProspectingPlus.Shared.Models
{
    public class ProPickChunkReport
    {
        public IReadOnlyList<ProPickOreReport> OreReports { get; }
        public IServerPlayer ReportByPlayer { get; }
        public double ChunkX { get; }
        public double ChunkZ { get; }

        public ProPickChunkReport(IReadOnlyList<ProPickOreReport> oreReports, 
            IServerPlayer reportByPlayer, 
            double chunkX,
            double chunkZ)
        {
            OreReports = oreReports;
            ReportByPlayer = reportByPlayer;
            ChunkX = chunkX;
            ChunkZ = chunkZ;
        }
    }
}