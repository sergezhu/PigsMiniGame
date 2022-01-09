using System;
using System.Collections.Generic;
using Core.Spawn;
using UnityEngine;

namespace Core.SO
{
    [CreateAssetMenu(fileName = "EarnScoresSettings", menuName = "Earn Scores Settings", order = 0)]
    public class EarnScoresSettings : ScriptableObject
    {
        [Serializable]
        public class EarnScoresSettingsRecord
        {
            [SerializeField]
            private EntitySpawner.EntityType _entityType;
            [SerializeField]
            private int _earnScores;

            public EntitySpawner.EntityType EntityType => _entityType;
            public int EarnScores => _earnScores;
        }

        [SerializeField]
        private List<EarnScoresSettingsRecord> _entityTypeScores;

        public IEnumerable<EarnScoresSettingsRecord> EntityTypeScores => _entityTypeScores;
    }
}