using TowerArena.Economy;

namespace TowerArena.Match
{
    public class PlayerMatchState
    {
        public ResourcePool Elixir { get; }
        public ResourcePool MatchCurrency { get; }
        public int MaxHealth { get; }
        public int CurrentHealth { get; private set; }
        public PlayerSide Side { get; }

        public PlayerMatchState(PlayerSide side, int startingElixir, int maxElixir, float elixirRegen, int startingCurrency, int currencyCap, int maxHealth)
        {
            Elixir = new ResourcePool(startingElixir, maxElixir, elixirRegen);
            MatchCurrency = new ResourcePool(startingCurrency, currencyCap, 0f);
            Side = side;
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
        }

        public int CurrentElixir => Elixir.Current;
        public int CurrentCurrency => MatchCurrency.Current;

        public bool IsDefeated => CurrentHealth <= 0;

        public void SpendElixir(int amount)
        {
            Elixir.Spend(amount);
        }

        public void SpendCurrency(int amount)
        {
            MatchCurrency.Spend(amount);
        }

        public void GainCurrency(int amount)
        {
            MatchCurrency.Add(amount);
        }

        public void Tick(float deltaTime)
        {
            Elixir.Regenerate(deltaTime);
        }

        public void TakeDamage(int amount)
        {
            CurrentHealth = System.Math.Max(0, CurrentHealth - System.Math.Max(0, amount));
        }

        public void Heal(int amount)
        {
            CurrentHealth = System.Math.Min(MaxHealth, CurrentHealth + System.Math.Max(0, amount));
        }
    }
}
