using System.Collections.Generic;
using RimWorld;
using Verse;
using AbilityDef = RimWorld.AbilityDef;

namespace RimworldModding
{
    public abstract class ChampionStage
    {
        public float stage;

        public abstract void ApplyToPawn(Pawn pawn);
    }
    
    public class ChampionAbilityStage: ChampionStage
    {
        public List<AbilityDef> abilitiesToGain;
        public override void ApplyToPawn(Pawn pawn)
        {
            foreach (AbilityDef ability in abilitiesToGain)
            {
                pawn.abilities.GainAbility(ability);
            }
        }
    }
    
    public class ChampionHediffStage: ChampionStage
    {
        public List<HediffDef> hediffsToGain;
        public override void ApplyToPawn(Pawn pawn)
        {
            foreach (HediffDef hediffDef in hediffsToGain)
            {
                pawn.health.AddHediff(hediffDef);
            }
        }
    }
}