using Newtonsoft.Json;

namespace ProspectingPlus.Shared.Models
{
    public abstract class ModDataBase
    {
        [JsonIgnore]
        public abstract string FileName { get; }
        public abstract void Default();
    }
}