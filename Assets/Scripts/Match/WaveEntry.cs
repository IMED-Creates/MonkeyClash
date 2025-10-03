using TowerArena.Enemies;
using UnityEngine;

namespace TowerArena.Match
{
    [System.Serializable]
    public class WaveEntry
    {
        [SerializeField] private BalloonType balloonType;
        [SerializeField] private int quantity = 10;
        [SerializeField] private float spawnInterval = 0.5f;
        [SerializeField] private int laneIndex = 0;

        public BalloonType BalloonType => balloonType;
        public int Quantity => Mathf.Max(0, quantity);
        public float SpawnInterval => Mathf.Max(0.01f, spawnInterval);
        public int LaneIndex => Mathf.Max(0, laneIndex);
    }
}
