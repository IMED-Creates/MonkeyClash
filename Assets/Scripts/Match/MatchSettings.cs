using UnityEngine;

namespace TowerArena.Match
{
    [CreateAssetMenu(menuName = "TowerArena/Match/Match Settings", fileName = "MatchSettings")]
    public class MatchSettings : ScriptableObject
    {
        [Header("Player Resources")]
        [SerializeField] private int startingElixir = 5;
        [SerializeField] private int maxElixir = 10;
        [SerializeField] private float elixirRegenRate = 1.5f;
        [SerializeField] private int startingCurrency = 250;
        [SerializeField] private int currencyCap = 9999;
        [SerializeField] private int endpointHealth = 20;

        [Header("Match Flow")]
        [SerializeField] private float timeBeforeFirstWave = 5f;
        [SerializeField] private float timeBetweenWaves = 20f;

        public int StartingElixir => startingElixir;
        public int MaxElixir => maxElixir;
        public float ElixirRegenRate => elixirRegenRate;
        public int StartingCurrency => startingCurrency;
        public int CurrencyCap => currencyCap;
        public int EndpointHealth => endpointHealth;
        public float TimeBeforeFirstWave => timeBeforeFirstWave;
        public float TimeBetweenWaves => timeBetweenWaves;
    }
}
