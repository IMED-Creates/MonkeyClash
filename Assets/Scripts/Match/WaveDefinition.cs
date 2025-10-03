using System.Collections.Generic;
using UnityEngine;

namespace TowerArena.Match
{
    [CreateAssetMenu(menuName = "TowerArena/Match/Wave", fileName = "WaveDefinition")]
    public class WaveDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private float startTime;
        [SerializeField] private List<WaveEntry> entries;

        public string Id => id;
        public float StartTime => Mathf.Max(0f, startTime);
        public IReadOnlyList<WaveEntry> Entries => entries;
    }
}
