using ProspectingPlus.Client;
using ProspectingPlus.Server;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace ProspectingPlus.ModSystem
{
    public class ProspectingPlusSystem : Vintagestory.API.Common.ModSystem
    {
        public ProspectingPlusClient Client { get; private set; }
        public ProspectingPlusServer Server { get; private set; }

        public override bool ShouldLoad(EnumAppSide forSide)
        {
            return true;
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            Server = new ProspectingPlusServer(api);
        }

        public override void StartClientSide(ICoreClientAPI api)
        {
            Client = new ProspectingPlusClient(api);
        }

        public override double ExecuteOrder()
        {
            return 10000d;
        }
    }
}