using System.Collections.Generic;
using System.Drawing;
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
        private readonly ICoreClientAPI _api;
        private readonly int _chunkSize;
        private readonly ProspectingPlusClient _client;
        private readonly List<ProspectingMapComponent> _components = new List<ProspectingMapComponent>();
        private bool _isOverlayEnabled = true;

        private Dictionary<OreDensity, LoadedTexture> _textureMap;

        private List<string> _oreFilter;
        private OreDensity _densityMinimum;

        public override string Title => nameof(ProspectingOverlayLayer);
        public override EnumMapAppSide DataSide => EnumMapAppSide.Client;

        public ProspectingOverlayLayer(ICoreClientAPI api, IWorldMapManager mapSink) : base(api, mapSink)
        {
            _api = api;
            _chunkSize = api.World.BlockAccessor.ChunkSize;
            _client = api.ModLoader.GetModSystem<ProspectingPlusSystem>().Client;
            _isOverlayEnabled = _client.ClientState.OverlayEnabled;
            _client.OnOverlayToggled += OnOverlayToggled;
            _client.OnChunkReportReceived += OnChunkReportReceived;
            _oreFilter = _client.ClientState.EnabledOreKeys;
            _densityMinimum = _client.ClientState.SelectedMinimumDensity;
            GenerateOverlayTextureMap();
        }

        private void OnChunkReportReceived(ProPickChunkReport report)
        {
            var tex = report.OreReports.Count <= 0
                ? _textureMap[OreDensity.Unknown]
                : _textureMap[report.OreReports.OrderByDescending(x => x.Density).First().Density];
            _components.Add(new ProspectingMapComponent(_api, report, tex));
        }

        private void OnOverlayToggled()
        {
            _isOverlayEnabled = !_isOverlayEnabled;
            _client.ClientState.OverlayEnabled = _isOverlayEnabled;
        }

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

            ModifyAlpha(_client.ClientState.OverlayOpacityPercent);
        }

        public void ModifyAlpha(int valPercent)
        {
            var alpha = (int) (255 * (valPercent / 100f));
            foreach (var mappedTex in _textureMap)
            {
                var tex = mappedTex.Value;
                var color = ModConstants.DensityColorMap[mappedTex.Key];
                var alphaAdjusted = Color.FromArgb(alpha, color);
                var arr = Enumerable.Repeat(alphaAdjusted.ToVsArgb(), _chunkSize * _chunkSize).ToArray();
                _api.Render.LoadOrUpdateTextureFromRgba(arr, false, 0, ref tex);
            }
        }

        public override void OnMouseMoveClient(MouseEvent args, GuiElementMap mapElem, StringBuilder hoverText)
        {
            if (!_isOverlayEnabled) return;
            foreach (var comp in FilteredComponents())
                comp.OnMouseMove(args, mapElem, hoverText);
        }

        public override void Render(GuiElementMap mapElem, float dt)
        {
            if (!_isOverlayEnabled) return;
            foreach (var comp in FilteredComponents())
                comp.Render(mapElem, dt);
        }

        private IEnumerable<ProspectingMapComponent> FilteredComponents()
        {
            if (_oreFilter is null)
                return _components;
            return _components
                .Where(comp =>
                    comp.Report.OreReports.Any(x =>
                        _oreFilter.Any(y => x.OreKey == y) && x.Density >= _densityMinimum));
        }

        public void FilterUpdated(List<string> keysToDisplay, OreDensity densityMinimum)
        {
            _client.ClientState.EnabledOreKeys = keysToDisplay;
            _client.ClientState.SelectedMinimumDensity = densityMinimum;
            _oreFilter = keysToDisplay;
            _densityMinimum = densityMinimum;
        }

        public override void Dispose()
        {
            foreach (var comp in _components)
                comp.Dispose();
            base.Dispose();
        }
    }
}