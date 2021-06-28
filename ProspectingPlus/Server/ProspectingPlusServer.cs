using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using ProspectingPlus.Shared.Constants;
using ProspectingPlus.Shared.Extensions;
using ProspectingPlus.Shared.Models;
using ProspectingPlus.Shared.Packets;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

namespace ProspectingPlus.Server
{
    public class ProspectingPlusServer
    {
        private readonly ICoreServerAPI _api;
        private readonly IServerNetworkChannel _chan;
        private readonly List<ProPickChunkReport> _reports = new List<ProPickChunkReport>();
        private readonly string _filePath;

        // TODO: Support the groups system.

        public ProspectingPlusServer(ICoreServerAPI api)
        {
            _api = api;
            _chan = _api.Network.RegisterChannelAndTypes()
                .SetMessageHandler<ChunkReportPacket>(ChunkReportReceived);
            api.Event.PlayerNowPlaying += OnPlayerReady;
            api.Event.GameWorldSave += OnGameWorldSave;
            _filePath = CreateDirs();
            if (File.Exists(_filePath))
                _reports = JsonConvert.DeserializeObject<List<ProPickChunkReport>>(File.ReadAllText(_filePath));
        }

        private string CreateDirs()
        {
            var dirPath = Path.Combine(GamePaths.DataPath, "ModData");
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
            dirPath = Path.Combine(dirPath, _api.World.SavegameIdentifier);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
            return Path.Combine(dirPath, ModConstants.DataFileName);
        }

        private void OnGameWorldSave()
        {
            File.WriteAllText(_filePath, JsonConvert.SerializeObject(_reports));
        }

        private void OnPlayerReady(IServerPlayer joinedPlayer)
        {
            foreach (var report in _reports)
                _chan.SendPacket(new ChunkReportPacket(report), joinedPlayer);
        }

        private void ChunkReportReceived(IServerPlayer _, ChunkReportPacket reportPacket)
        {
            var report = new ProPickChunkReport(reportPacket);
            if (_reports.Any(x => x.ChunkX == report.ChunkX && x.ChunkZ == report.ChunkZ))
                return; // discard any reports for existing chunks.
            _reports.Add(report);
            _chan.BroadcastPacket(reportPacket);
        }
    }
}