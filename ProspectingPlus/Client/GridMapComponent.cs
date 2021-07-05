using Vintagestory.API.Client;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace ProspectingPlus.Client
{
    public class GridMapComponent : MapComponent
    {
        private readonly Vec2i _chunkPos;
        private readonly LoadedTexture _gridTexture;
        private readonly int _chunkSize;

        private Vec2f _viewPos = new Vec2f();
        private readonly Vec3d _worldPos;
        
        public GridMapComponent(ICoreClientAPI capi, Vec2i chunkPos, LoadedTexture gridTexture) : base(capi)
        {
            _chunkSize = capi.World.BlockAccessor.ChunkSize;
            _chunkPos = chunkPos;
            _gridTexture = gridTexture;
            _worldPos = new Vec3d(_chunkPos.X * _chunkSize, 0, chunkPos.Y * _chunkSize);
        }

        public override void Render(GuiElementMap map, float dt)
        {
            map.TranslateWorldPosToViewPos(_worldPos, ref _viewPos);
            capi.Render.Render2DTexture(
                _gridTexture.TextureId,
                (int)(map.Bounds.renderX + _viewPos.X),
                (int)(map.Bounds.renderY + _viewPos.Y),
                (int)(_gridTexture.Width * map.ZoomLevel),
                (int)(_gridTexture.Height * map.ZoomLevel));
        }
    }
}