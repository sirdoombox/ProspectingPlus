using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Reflection;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace ProspectingPlus.Shared.Utils
{
    public class MapDBUtil
    {
        private SQLiteCommand _getAllMapChunksCommand;
        
        public MapDBUtil(ChunkMapLayer chunkMapLayer)
        {
            var mapDb = typeof(ChunkMapLayer)
                .GetField("mapdb", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.GetValue(chunkMapLayer);
            var conn = typeof(MapDB)
                .GetField("sqliteConn", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.GetValue(mapDb);
            _getAllMapChunksCommand = ((SQLiteConnection) conn).CreateCommand();
            _getAllMapChunksCommand.CommandText = "SELECT position FROM mappiece";
            _getAllMapChunksCommand.Prepare();
        }

        public IEnumerable<Vec2i> GetExploredMapChunks()
        {
            using var sqLiteDataReader = _getAllMapChunksCommand.ExecuteReader();
            while (sqLiteDataReader.Read())
            {
                var val = Convert.ToUInt64(sqLiteDataReader.GetValue(0));
                yield return new Vec2i((int)(val & 0x7FFFFFF), (int)((val >> 27) & 0x7FFFFFF));
            }
        }

        public void Close()
        {
            _getAllMapChunksCommand?.Dispose();
        }

        public void Dispose()
        {
            _getAllMapChunksCommand?.Dispose();
        }
    }
}