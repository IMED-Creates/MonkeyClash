using UnityEngine;

namespace TowerArena.Units
{
    [System.Serializable]
    public struct ParagonStats
    {
        [SerializeField] private float damage;
        [SerializeField] private float attackRate;
        [SerializeField] private float range;
        [SerializeField] private int pierce;
        [SerializeField] private bool canDetectCamo;

        public float Damage => damage;
        public float AttackRate => attackRate;
        public float Range => range;
        public int Pierce => pierce;
        public bool CanDetectCamo => canDetectCamo;
    }
}
