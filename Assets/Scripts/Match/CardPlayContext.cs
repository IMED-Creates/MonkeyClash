using TowerArena.Decks;
using TowerArena.Match;

namespace TowerArena.Cards
{
    public struct CardPlayContext
    {
        public PlayerMatchState PlayerState { get; }
        public MatchManager MatchManager { get; }
        public DeckManager DeckManager { get; }

        public CardPlayContext(PlayerMatchState playerState, MatchManager matchManager, DeckManager deckManager)
        {
            PlayerState = playerState;
            MatchManager = matchManager;
            DeckManager = deckManager;
        }
    }
}
