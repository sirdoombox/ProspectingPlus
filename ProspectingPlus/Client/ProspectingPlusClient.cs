using System;
using System.Reflection;
using HarmonyLib;
using ProspectingPlus.Patches;
using ProspectingPlus.Shared.Constants;
using ProspectingPlus.Shared.Extensions;
using ProspectingPlus.Shared.Models;
using ProspectingPlus.Shared.Packets;
using Vintagestory.API.Client;
using Vintagestory.GameContent;

namespace ProspectingPlus.Client
{
    public class ProspectingPlusClient
    {
        private readonly ICoreClientAPI _api;
        private readonly IClientNetworkChannel _chan;

        public Action OnOverlayToggled { get; set; }
        public Action<ProPickChunkReport> OnChunkReportReceived { get; set; }

        public ProspectingPlusClient(ICoreClientAPI api)
        {
            _api = api;

            var mapManager = _api.ModLoader.GetModSystem<WorldMapManager>();
            mapManager.RegisterMapLayer<ProspectingOverlayLayer>(ModConstants.MapLayerName);

            _api.Input.RegisterHotKey(ModConstants.HotkeyCode, "Toggle Prospecting Plus Overlay", GlKeys.Comma);
            api.Input.SetHotKeyHandler(ModConstants.HotkeyCode, _ =>
            {
                OnOverlayToggled?.Invoke();
                return true;
            });

            _chan = _api.Network.RegisterChannelAndTypes().SetMessageHandler<ChunkReportPacket>(report =>
            {
                OnChunkReportReceived?.Invoke(new ProPickChunkReport(report));
            });
            
            ProspectingPickPatch.OnChunkReportGenerated += report =>
            {
                _chan.SendPacket(new ChunkReportPacket(report));
            };
        }
    }
}