using System;
using ProspectingPlus.GUI;
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
        private readonly WorldMapManager _wmm;

        private ProspectingPlusGuiDialog _dialog;

        public Action OnOverlayToggled { get; set; }
        public Action<ProPickChunkReport> OnChunkReportReceived { get; set; }

        public ProspectingPlusClient(ICoreClientAPI api)
        {
            _api = api;
            var mapManager = _api.ModLoader.GetModSystem<WorldMapManager>();
            mapManager.RegisterMapLayer<ProspectingOverlayLayer>(ModConstants.MapLayerName);

            _wmm = api.ModLoader.GetModSystem<WorldMapManager>();

            _api.Event.BlockTexturesLoaded += () =>
            {
                _api.Input.SetHotKeyHandler("worldmapdialog", _ =>
                {
                    _wmm.ToggleMap(EnumDialogType.Dialog);
                    ShowDialog();
                    _wmm.worldMapDlg.Focus();
                    return true;
                });
            };

            _api.Input.RegisterHotKey(ModConstants.OverlayHotkeyCode, "Toggle Prospecting Plus Overlay", GlKeys.Comma);
            _api.Input.SetHotKeyHandler(ModConstants.OverlayHotkeyCode, _ =>
            {
                OnOverlayToggled?.Invoke();
                return true;
            });

            _chan = _api.Network.RegisterChannelAndTypes()
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

        private bool ShowDialog()
        {
            _dialog ??= new ProspectingPlusGuiDialog(_api);

            if (_dialog != null && _dialog.IsOpened())
            {
                _dialog.TryClose();
                return true;
            }

            _dialog.TryOpen();
            return true;
        }
    }
}