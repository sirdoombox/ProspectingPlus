using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace ProspectingPlus.Shared.Utils
{
    public class MapDB : SQLiteDBConnection
    {
        private SQLiteCommand _getAllMapChunksCommand;
        
        public MapDB(ILogger logger) : base(logger)
        {
            
        }

        public override void OnOpened()
        {
            _getAllMapChunksCommand = sqliteConn.CreateCommand();
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

        public override void Close()
        {
            _getAllMapChunksCommand?.Dispose();
            base.Close();
        }

        public override void Dispose()
        {
            _getAllMapChunksCommand?.Dispose();
            base.Dispose();
        }
    }
}