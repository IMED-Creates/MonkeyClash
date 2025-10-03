using UnityEngine;

namespace TowerArena.Economy
{
    [System.Serializable]
    public class ResourcePool
    {
        [SerializeField] private int current;
        [SerializeField] private int max;
        [SerializeField] private float regenRatePerSecond;

        public ResourcePool(int current, int max, float regenRatePerSecond)
        {
            this.current = Mathf.Clamp(current, 0, max);
            this.max = Mathf.Max(0, max);
            this.regenRatePerSecond = Mathf.Max(0, regenRatePerSecond);
        }

        public int Current => current;
        public int Max => max;
        public float RegenRate => regenRatePerSecond;

        public bool Has(int amount) => current >= amount;

        public void Add(int amount)
        {
            current = Mathf.Clamp(current + Mathf.Max(0, amount), 0, max);
        }

        public void Spend(int amount)
        {
            amount = Mathf.Max(0, amount);
            if (current < amount)
            {
                throw new System.InvalidOperationException("Not enough resources");
            }
            current -= amount;
        }

        public void Regenerate(float deltaTime)
        {
            if (regenRatePerSecond <= 0)
            {
                return;
            }
            Add(Mathf.FloorToInt(regenRatePerSecond * deltaTime));
        }

        public void SetMax(int newMax)
        {
            max = Mathf.Max(0, newMax);
            current = Mathf.Clamp(current, 0, max);
        }
    }
}
