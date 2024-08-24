using LudeonTK;

namespace RimworldModding
{
    public static class DebugActions
    {

        [DebugAction("Primarch Assault", "Hasten revenge assaults", allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void StartPhaseTwos()
        {
            GameComponent_ChallengeManager.Instance.StartAllPhaseTwos();
        }
    }
}