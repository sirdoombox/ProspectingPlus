using ProspectingPlus.Shared.Packets;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace ProspectingPlus.Shared.Extensions
{
    public static class NetworkExtensions
    {
        private static TResult RegisterChannelAndTypes<T, TResult>(this T api)
            where T : INetworkAPI where TResult : INetworkChannel
        {
            return (TResult) api.RegisterChannel(nameof(ProspectingPlus))
                .RegisterMessageType<ChunkReportPacket>()
                .RegisterMessageType<OreList>();
        }

        public static IServerNetworkChannel RegisterChannelAndTypes(this IServerNetworkAPI api)
        {
            return RegisterChannelAndTypes<IServerNetworkAPI, IServerNetworkChannel>(api);
        }

        public static IClientNetworkChannel RegisterChannelAndTypes(this IClientNetworkAPI api)
        {
            return RegisterChannelAndTypes<IClientNetworkAPI, IClientNetworkChannel>(api);
        }
    }
}