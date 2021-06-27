using System.Linq;
using System.Text;
using ProspectingPlus.Shared.Extensions;
using ProspectingPlus.Shared.Models;
using Vintagestory.API.Client;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace ProspectingPlus.Client
{
    public class ProspectingMapComponent : MapComponent
    {
        private readonly int _chunkSize;
        private readonly string _hoverText;
        private readonly ProPickChunkReport _report;
        private readonly LoadedTexture _texture;

        private readonly Vec3d _worldPos;
        private Vec2f _viewPos = new Vec2f();

        public ProspectingMapComponent(
            ICoreClientAPI api,
            ProPickChunkReport chunkReport,
            LoadedTexture loadedTexture) : base(api)
        {
            _texture = loadedTexture;
            _report = chunkReport;
            _chunkSize = api.World.BlockAccessor.ChunkSize;
            _worldPos = new Vec3d(_report.ChunkX * _chunkSize, 0, _report.ChunkZ * _chunkSize);

            var sb = new StringBuilder();
            sb.AppendLine(
                $"-- Prospecting Report From {_report.PlayerName} --");
            foreach (var oreRep in _report.OreReports.OrderByDescending(x => x.Density).ThenByDescending(x => x.Ppt))
                sb.AppendLine($"{Lang.Get(oreRep.Density.ToLangKey())} {Lang.Get(oreRep.OreKey)} - {oreRep.Ppt:0.##}‰");
            _hoverText = sb.ToString();
        }

        public override void OnMouseMove(MouseEvent args, GuiElementMap mapElem, StringBuilder hoverText)
        {
            var worldPos = new Vec3d();
            var mouseX = (float) (args.X - mapElem.Bounds.renderX);
            var mouseY = (float) (args.Y - mapElem.Bounds.renderY);

            mapElem.TranslateViewPosToWorldPos(new Vec2f(mouseX, mouseY), ref worldPos);

            var chunkX = (int) (worldPos.X / _chunkSize);
            var chunkZ = (int) (worldPos.Z / _chunkSize);
            if (chunkX == _report.ChunkX && chunkZ == _report.ChunkZ)
                hoverText.AppendLine($"\n{_hoverText}");
        }

        public override void Render(GuiElementMap map, float dt)
        {
            map.TranslateWorldPosToViewPos(_worldPos, ref _viewPos);
            capi.Render.Render2DTexture(
                _texture.TextureId,
                (int) (map.Bounds.renderX + _viewPos.X),
                (int) (map.Bounds.renderY + _viewPos.Y),
                (int) (_texture.Width * (double) map.ZoomLevel),
                (int) (_texture.Height * (double) map.ZoomLevel));
        }
    }
}