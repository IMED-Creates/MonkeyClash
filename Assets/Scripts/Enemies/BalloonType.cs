using TowerArena.Cards;
using UnityEngine;

namespace TowerArena.Enemies
{
    [CreateAssetMenu(menuName = "TowerArena/Balloons/Balloon Type", fileName = "BalloonType")]
    public class BalloonType : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private Color color = Color.white;
        [SerializeField] private float maxHealth = 10f;
        [SerializeField] private float movementSpeed = 1.5f;
        [SerializeField] private int rewardCurrency = 5;
        [SerializeField] private BalloonTraits traits;
        [SerializeField] private GameObject balloonPrefab;

        public string Id => id;
        public string DisplayName => displayName;
        public Color Color => color;
        public float MaxHealth => maxHealth;
        public float MovementSpeed => movementSpeed;
        public int RewardCurrency => rewardCurrency;
        public BalloonTraits Traits => traits;
        public GameObject BalloonPrefab => balloonPrefab;
    }
}
