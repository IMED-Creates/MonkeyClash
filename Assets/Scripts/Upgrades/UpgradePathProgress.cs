namespace TowerArena.Upgrades
{
    public class UpgradePathProgress
    {
        public string PathId { get; }
        public int CurrentTier { get; private set; }

        public UpgradePathProgress(string pathId)
        {
            PathId = pathId;
            CurrentTier = 0;
        }

        public bool CanAdvanceToTier(int nextTier)
        {
            if (nextTier <= CurrentTier)
            {
                return false;
            }

            return nextTier == CurrentTier + 1;
        }

        public void ApplyTier(UpgradeTierDefinition tier)
        {
            CurrentTier = tier.Tier;
        }
    }
}
