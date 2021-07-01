using System;
using System.IO;
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
        public ProspectingPlusClientState ClientState { get; private set; }

        private ProspectingPlusGuiDialog _dialog;

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
                    OnChunkReportReceived?.Invoke(new ProPickChunkReport(report));
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
    }
}