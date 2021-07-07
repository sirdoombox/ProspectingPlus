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
        private readonly LoadedTexture _texture;

        private readonly Vec3d _worldPos;
        private Vec2f _viewPos = new Vec2f();

        public ProPickChunkReport Report { get; }

        public ProspectingMapComponent(
            ICoreClientAPI api,
            ProPickChunkReport chunkReport,
            LoadedTexture loadedTexture) : base(api)
        {
            _texture = loadedTexture;
            Report = chunkReport;
            _chunkSize = api.World.BlockAccessor.ChunkSize;
            _worldPos = new Vec3d(Report.ChunkX * _chunkSize, 0, Report.ChunkZ * _chunkSize);

            var sb = new StringBuilder();
            sb.AppendLine(
                $"-- {Lang.Get("prospectingplus:map-reportfrom", Report.PlayerName)} --");
            foreach (var oreRep in Report.OreReports.OrderByDescending(x => x.Density).ThenByDescending(x => x.Ppt))
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
            if (chunkX == Report.ChunkX && chunkZ == Report.ChunkZ)
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