using ProspectingPlus.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.API.Client;

namespace ProspectingPlus.ModSystem
{
    public class ProspectingPlusSystem : Vintagestory.API.Common.ModSystem
    {
        public override bool ShouldLoad(EnumAppSide forSide) => true;

        public override void StartServerSide(ICoreServerAPI api)
        {
        }

        public override void StartClientSide(ICoreClientAPI api)
        {
            new ProspectingPlusClient(api);
        }
    }
}
