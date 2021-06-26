using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace ProspectingPlus.Client
{
    public class ProspectingOverlayLayer : MapLayer
    {
        public ProspectingOverlayLayer(ICoreAPI api, IWorldMapManager mapSink) : base(api, mapSink)
        {
            
        }

        public override string Title => "ProspectingOverlay";
        public override EnumMapAppSide DataSide { get; }
    }
}
