using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using ProspectingPlus.ModSystem;
using ProspectingPlus.Shared.Extensions;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;
using MapDB = ProspectingPlus.Shared.Utils.MapDB;

namespace ProspectingPlus.Client
{
    public class GridOverlayLayer : MapLayer
    {
        private readonly ICoreClientAPI _api;
        private readonly ProspectingPlusClient _client;
        private readonly int _chunkSize;
        private readonly Dictionary<Vec2i, GridMapComponent> _toRender;
        private LoadedTexture _gridTexture;

        public override string Title => nameof(GridOverlayLayer);
        public override EnumMapAppSide DataSide => EnumMapAppSide.Client;

        public GridOverlayLayer(ICoreClientAPI api, IWorldMapManager mapSink) : base(api, mapSink)
        {
            _api = api;
            _client = api.ModLoader.GetModSystem<ProspectingPlusSystem>().Client;
            var mapDb = new MapDB(api.Logger);
            var err = string.Empty;
            mapDb.OpenOrCreate(GetMapDbFilePath(), ref err, false, false, false);
            _chunkSize = api.World.BlockAccessor.ChunkSize;
            GenerateGridChunkTexture(_client.ClientState.GridOpacityPercent);
            var chunks = mapDb.GetExploredMapChunks();
            _toRender = chunks.ToDictionary(x => x, y => new GridMapComponent(_api, y, _gridTexture));
            _api.Event.ChunkDirty += OnChunkDirty;
        }

        private void OnChunkDirty(Vec3i chunkCoord, IWorldChunk chunk, EnumChunkDirtyReason reason)
        {
            var coord = new Vec2i(chunkCoord.X, chunkCoord.Z);
            if (reason == EnumChunkDirtyReason.NewlyLoaded && !_toRender.ContainsKey(coord))
                _toRender.Add(coord, new GridMapComponent(_api, coord, _gridTexture));
        }

        private void GenerateGridChunkTexture(int opacityPercent)
        {
            var alpha = (int) (255 * (opacityPercent / 100f));
            var color = Color.FromArgb(alpha,0,0,0).ToVsArgb();
            var tex = new LoadedTexture(_api, 0, _chunkSize, _chunkSize);
            var arr = GenerateGridArrWithColor(color);
            _api.Render.LoadOrUpdateTextureFromRgba(arr, false, 0, ref tex);
            _api.Render.BindTexture2d(tex.TextureId);
            _gridTexture = tex;
        }
        
        public void ModifyAlpha(int opacityPercent)
        {
            var alpha = (int) (255 * (opacityPercent / 100f));
            var color = Color.FromArgb(alpha,0,0,0).ToVsArgb();
            var arr = GenerateGridArrWithColor(color);
            _api.Render.LoadOrUpdateTextureFromRgba(arr, false, 0, ref _gridTexture);
        }

        public override void Render(GuiElementMap mapElem, float dt)
        {
            if (!_client.ClientState.GridEnabled
                || !_client.ClientState.OverlayEnabled)
                return;
            foreach (var component in _toRender)
                component.Value.Render(mapElem, dt);
        }

        private string GetMapDbFilePath()
        {
            var str = Path.Combine(GamePaths.DataPath, "Maps");
            GamePaths.EnsurePathExists(str);
            return Path.Combine(str, api.World.SavegameIdentifier + ".db");
        }

        private int[] GenerateGridArrWithColor(int color)
        {
            const int thickness = 1;
            var arr = new int[_chunkSize * _chunkSize];
            for (var x = 0; x < _chunkSize; x++)
            for (var y = 0; y < _chunkSize; y++)
            {
                if (x < thickness || x > _chunkSize - 1 - thickness)
                    arr[y * _chunkSize + x] = color;
                else if (y < thickness || y > _chunkSize - 1 - thickness)
                    arr[y * _chunkSize + x] = color;
            }

            return arr;
        }

    }
}