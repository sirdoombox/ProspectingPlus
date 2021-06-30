using System.Collections.Generic;
using ProtoBuf;

namespace ProspectingPlus.Shared.Packets
{
    [ProtoContract]
    public class OreList
    {
        [ProtoMember(1)] public List<string> OreCodes;

        public OreList(List<string> oreCodes)
        {
            OreCodes = oreCodes;
        }

        public OreList()
        {
        }
    }
}