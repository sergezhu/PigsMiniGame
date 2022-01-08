using System;

namespace Core.Spawn
{
    [Serializable]
    public struct SpawnCellRecord
    {
        private GridCell _cell;
        private EntitySpawner _spawner;

        public SpawnCellRecord(GridCell cell, EntitySpawner spawner)
        {
            _cell = cell;
            _spawner = spawner;
        }

        public GridCell Cell => _cell;
        public EntitySpawner Spawner => _spawner;
    }
}