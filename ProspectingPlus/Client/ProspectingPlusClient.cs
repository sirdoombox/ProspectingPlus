using System;
using System.Linq;
using ProspectingPlus.Client.GUI;
using ProspectingPlus.Shared.Constants;
using ProspectingPlus.Shared.Extensions;
using ProspectingPlus.Shared.Models;
using ProspectingPlus.Shared.Packets;
using ProspectingPlus.Shared.Utils;
using Vintagestory.API.Client;
using Vintagestory.API.Config;
using Vintagestory.GameContent;

namespace ProspectingPlus.Client
{
    public class ProspectingPlusClient
    {
        private readonly ICoreClientAPI _api;

        private ProspectingPlusGuiDialog _dialog;
        public ProspectingPlusClientState ClientState { get; }

        public Action OnOverlayToggled { get; set; }
        public Action<ProPickChunkReport> OnChunkReportReceived { get; set; }

        public ProspectingPlusClient(ICoreClientAPI api)
        {
            _api = api;
            var mapManager = _api.ModLoader.GetModSystem<WorldMapManager>();
            mapManager.RegisterMapLayer<ProspectingOverlayLayer>(ModConstants.MapLayerName);

            ClientState = ModDataUtil.GetOrCreateDefault<ProspectingPlusClientState>(_api.World.SavegameIdentifier);

            _api.Event.LeftWorld += () => ClientState.WriteToDisk(_api.World.SavegameIdentifier);

            _api.Event.BlockTexturesLoaded += () =>
            {
                _api.Input.SetHotKeyHandler("worldmapdialog", _ =>
                {
                    mapManager.ToggleMap(EnumDialogType.Dialog);
                    ShowDialog();
                    mapManager.worldMapDlg.Focus();
                    return true;
                });
            };

            _api.Input.RegisterHotKey(ModConstants.OverlayHotkeyCode, "Toggle Prospecting Plus Overlay", GlKeys.Comma);
            _api.Input.SetHotKeyHandler(ModConstants.OverlayHotkeyCode, _ =>
            {
                OnOverlayToggled?.Invoke();
                return true;
            });

            _api.Network.RegisterChannelAndTypes()
                .SetMessageHandler<ChunkReportPacket>(report =>
                {
                    var reconstructed = new ProPickChunkReport(report);
                    OnChunkReportReceived?.Invoke(reconstructed);
                    if(ClientState.TextReportsEnabled 
                       && reconstructed.PlayerUID == _api.World.Player.PlayerUID
                       && !report.IsInitPacket)
                        PrintTextReport(reconstructed);
                        
                })
                .SetMessageHandler<OreList>(oreList =>
                {
                    _dialog ??= new ProspectingPlusGuiDialog(_api);
                    _dialog.SendOreList(oreList);
                });
        }

        private void ShowDialog()
        {
            _dialog ??= new ProspectingPlusGuiDialog(_api);

            if (_dialog != null && _dialog.IsOpened())
            {
                _dialog.TryClose();
                return;
            }

            _dialog.TryOpen();
        }

        private void PrintTextReport(ProPickChunkReport report)
        {
            foreach (var oreRep in report.OreReports.OrderByDescending(x => x.Density).ThenByDescending(x => x.Ppt))
            {
                _api.ShowChatMessage($"{Lang.Get(oreRep.Density.ToLangKey())} {Lang.Get(oreRep.OreKey)} - {oreRep.Ppt:0.##}‰");
            }
            _api.ShowChatMessage("");
        }
    }
}