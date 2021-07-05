using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using MonoMod.Utils;
using ProspectingPlus.Shared.Extensions;
using ProspectingPlus.Shared.Utils;
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
        private readonly int _chunkSize;

        private Dictionary<Vec2i, GridMapComponent> _toRender = new Dictionary<Vec2i, GridMapComponent>();
        private LoadedTexture _gridTexture;
        private MapDB _mapDb;

        public override string Title => nameof(GridOverlayLayer);
        public override EnumMapAppSide DataSide => EnumMapAppSide.Client;

        public GridOverlayLayer(ICoreClientAPI api, IWorldMapManager mapSink) : base(api, mapSink)
        {
            _api = api;
            _mapDb = new MapDB(api.Logger);
            var err = string.Empty;
            _mapDb.OpenOrCreate(GetMapDbFilePath(), ref err, false, false, false);
            _chunkSize = api.World.BlockAccessor.ChunkSize;
            GenerateGridChunkTexture();
            var chunks = _mapDb.GetExploredMapChunks();
            _toRender = chunks.ToDictionary(x => x, y => new GridMapComponent(_api, y, _gridTexture));
            _api.Event.ChunkDirty += OnChunkDirty;
        }

        private void OnChunkDirty(Vec3i chunkCoord, IWorldChunk chunk, EnumChunkDirtyReason reason)
        {
            var coord = new Vec2i(chunkCoord.X, chunkCoord.Z);
            if(reason == EnumChunkDirtyReason.NewlyLoaded && !_toRender.ContainsKey(coord))
                _toRender.Add(coord, new GridMapComponent(_api, coord, _gridTexture));
        }

        private void GenerateGridChunkTexture()
        {
            const int tempThick = 1;
            var tex = new LoadedTexture(_api, 0, _chunkSize, _chunkSize);
            var arr = new int[_chunkSize * _chunkSize];
            for (var x = 0; x < _chunkSize; x++)
            for (var y = 0; y < _chunkSize; y++)
            {
                if (x < tempThick || x > _chunkSize - 1 - tempThick)
                    arr[y * _chunkSize + x] = Color.FromArgb(75, 0, 0, 0).ToVsArgb();
                else if (y < tempThick || y > _chunkSize - 1 - tempThick)
                    arr[y * _chunkSize + x] = Color.FromArgb(75, 0, 0, 0).ToVsArgb();
            }

            _api.Render.LoadOrUpdateTextureFromRgba(arr, false, 0, ref tex);
            _api.Render.BindTexture2d(tex.TextureId);
            _gridTexture = tex;
        }

        public override void Render(GuiElementMap mapElem, float dt)
        {
            foreach (var component in _toRender)
                component.Value.Render(mapElem, dt);
        }

        public override void OnTick(float dt)
        {
            
        }
        
        public string GetMapDbFilePath()
        {
            var str = Path.Combine(GamePaths.DataPath, "Maps");
            GamePaths.EnsurePathExists(str);
            return Path.Combine(str, api.World.SavegameIdentifier + ".db");
        }
    }
}