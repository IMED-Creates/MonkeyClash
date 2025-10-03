using UnityEngine;

namespace TowerArena.Match
{
    [System.Serializable]
    public class LaneDefinition
    {
        [SerializeField] private string id;
        [SerializeField] private Transform[] pathPoints;

        public string Id => id;
        public Transform[] PathPoints => pathPoints;
    }
}
