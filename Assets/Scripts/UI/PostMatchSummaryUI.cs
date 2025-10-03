using TowerArena.Match;
using UnityEngine;
using UnityEngine.UI;

namespace TowerArena.UI
{
    public class PostMatchSummaryUI : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private Text resultText;
        [SerializeField] private Text statsText;

        private void Awake()
        {
            if (root != null)
            {
                root.SetActive(false);
            }
        }

        public void Show(PlayerSide defeatedSide, int defenderHealth, int attackerHealth, float matchDuration)
        {
            if (root != null)
            {
                root.SetActive(true);
            }

            if (resultText != null)
            {
                var winner = defeatedSide == PlayerSide.Defender ? "Attacker" : "Defender";
                resultText.text = $"{winner} Victory";
            }

            if (statsText != null)
            {
                statsText.text = $"Duration: {matchDuration:0.0}s\nDefender HP: {defenderHealth}\nAttacker HP: {attackerHealth}";
            }
        }

        public void Hide()
        {
            if (root != null)
            {
                root.SetActive(false);
            }
        }
    }
}
