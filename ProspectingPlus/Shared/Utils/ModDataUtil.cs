using System.IO;
using Newtonsoft.Json;
using ProspectingPlus.Shared.Models;
using Vintagestory.API.Config;

namespace ProspectingPlus.Shared.Utils
{
    public static class ModDataUtil
    {
        public static T GetOrCreateDefault<T>(string saveId) where T : ModDataBase, new()
        {
            var dirPath = GetDirCreateIfNotExist(saveId);
            var dataObj = new T();
            var filePath = Path.Combine(dirPath, dataObj.FileName);
            if (!File.Exists(filePath))
                dataObj.Default();
            else
                dataObj = JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));
            return dataObj;
        }

        public static void WriteToDisk<T>(this T obj, string saveId) where T : ModDataBase, new()
        {
            var serialised = JsonConvert.SerializeObject(obj);
            var dirPath = GetDirCreateIfNotExist(saveId);
            var filePath = Path.Combine(dirPath, obj.FileName);
            File.WriteAllText(filePath, serialised);
        }

        private static string GetDirCreateIfNotExist(string saveId)
        {
            var dirPath = Path.Combine(GamePaths.DataPath, "ModData");
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
            dirPath = Path.Combine(dirPath, saveId);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
            return dirPath;
        }
    }
}