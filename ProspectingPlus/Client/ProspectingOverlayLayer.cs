using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProspectingPlus.ModSystem;
using ProspectingPlus.Shared.Constants;
using ProspectingPlus.Shared.Enums;
using ProspectingPlus.Shared.Extensions;
using ProspectingPlus.Shared.Models;
using Vintagestory.API.Client;
using Vintagestory.GameContent;

namespace ProspectingPlus.Client
{
    public class ProspectingOverlayLayer : MapLayer
    {
        public override string Title => nameof(ProspectingOverlayLayer);
        public override EnumMapAppSide DataSide => EnumMapAppSide.Client;

        private Dictionary<OreDensity, LoadedTexture> _textureMap;
        private readonly ProspectingPlusClient _client;
        private readonly ICoreClientAPI _api;
        private readonly int _chunkSize;
        private readonly List<ProPickChunkReport> _chunkReports;
        private readonly IWorldMapManager _mapManager;
        private readonly List<ProspectingMapComponent> _components = new List<ProspectingMapComponent>();
        private bool _isOverlayEnabled;

        public ProspectingOverlayLayer(ICoreClientAPI api, IWorldMapManager mapSink) : base(api, mapSink)
        {
            _api = api;
            _mapManager = mapSink;
            _chunkSize = api.World.BlockAccessor.ChunkSize;
            _chunkReports = new List<ProPickChunkReport>();
            _client = api.ModLoader.GetModSystem<ProspectingPlusSystem>().Client;
            _client.OnOverlayToggled += OnOverlayToggled;
            _client.OnChunkReportReceived += OnChunkReportReceived;
            GenerateOverlayTextureMap();
        }

        private void OnChunkReportReceived(ProPickChunkReport report)
        {
            _chunkReports.Add(report);
            var tex = _textureMap[report.OreReports.OrderByDescending(x => x.Density).First().Density];
            _components.Add(new ProspectingMapComponent(_api, report, tex));
        }

        private void OnOverlayToggled() =>
            _isOverlayEnabled = !_isOverlayEnabled;


        private void GenerateOverlayTextureMap()
        {
            _textureMap = new Dictionary<OreDensity, LoadedTexture>();
            foreach (var mappedColor in ModConstants.DensityColorMap)
            {
                var tex = new LoadedTexture(_api, 0, _chunkSize, _chunkSize);
                var arr = Enumerable.Repeat(mappedColor.Value.ToVsArgb(), _chunkSize * _chunkSize).ToArray();
                _api.Render.LoadOrUpdateTextureFromRgba(arr, false, 0, ref tex);
                _api.Render.BindTexture2d(tex.TextureId);
                _textureMap.Add(mappedColor.Key, tex);
            }
        }

        public override void OnMouseMoveClient(MouseEvent args, GuiElementMap mapElem, StringBuilder hoverText)
        {
            if (!_isOverlayEnabled) return;
            foreach (var comp in _components)
                comp.OnMouseMove(args, mapElem, hoverText);
        }

        public override void Render(GuiElementMap mapElem, float dt)
        {
            if (!_isOverlayEnabled) return;
            foreach (var comp in _components)
                comp.Render(mapElem, dt);
        }

        public override void Dispose()
        {
            foreach (var comp in _components)
                comp.Dispose();
            base.Dispose();
        }
    }
}