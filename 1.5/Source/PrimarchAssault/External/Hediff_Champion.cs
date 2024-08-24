using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimworldModding;
using Verse;
using Verse.AI;

namespace PrimarchAssault.External
{
    public class Hediff_Champion: Hediff
    {
        public List<ChampionStage> Stages = new List<ChampionStage>();
        public ThingDef DroppedThing;
        public ChallengeDef PhaseTwoToQueue;




        private List<ChampionStage> _tmpStagesToRemove = new List<ChampionStage>();
        public void SetupHediff(ThingDef droppedThing, List<ChampionStage> stages, ChallengeDef phaseTwoToQueue = null)
        {
            DroppedThing = droppedThing;
            Stages = stages;
            PhaseTwoToQueue = phaseTwoToQueue;
        }

        public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
        {
            base.Notify_PawnDied(dinfo, culprit);

            if (DroppedThing != null) GenSpawn.Spawn(DroppedThing, pawn.Position, pawn.Corpse.Map);

            if (PhaseTwoToQueue != null)
            {
                GameComponent_ChallengeManager.Instance.StartPhaseTwo(PhaseTwoToQueue);
                Find.LetterStack.ReceiveLetter("GWPA.EscapedTitle".Translate(), "GWPA.Escaped".Translate(PhaseTwoToQueue.championName), LetterDefOf.ThreatSmall);
            }
            else
            {
                Find.LetterStack.ReceiveLetter("GWPA.GivenUpTitle".Translate(), "GWPA.GivenUp".Translate(), LetterDefOf.PositiveEvent);
            }
            
            //Get rid of the corpse
            EffecterDefOf.Skip_EntryNoDelay.Spawn(pawn.Corpse, pawn.Corpse.MapHeld).Cleanup();
            pawn.Corpse.DeSpawn();
            
            GameComponent_ChallengeManager.Instance.RemoveActiveChampion(pawn.thingIDNumber);
        }

        public override void Tick()
        {
            base.Tick();
            if (!pawn.IsHashIntervalTick(100)) return;
            float percent = GetChampionStage(out float apparelValue, out float healthValue);
            
            //GameComponent_ChallengeManager.Instance.HealthBar.UpdateIfWilling(pawn.thingIDNumber, apparelValue, healthValue);
            
            _tmpStagesToRemove.Clear();
            foreach (var stage in Stages.Where(stage => stage.stage > percent))
            {
                stage.ApplyToPawn(pawn);
                _tmpStagesToRemove.Add(stage);
            }

            Stages.RemoveAll(stage => _tmpStagesToRemove.Contains(stage));
        }


        private float GetChampionStage(out float apparelValue, out float healthValue)
        {
            apparelValue = (float)pawn.apparel.WornApparel.Select(apparel => apparel.HitPoints / (double)apparel.MaxHitPoints).Average();
            healthValue = pawn.health.summaryHealth.SummaryHealthPercent;
            
            return Math.Min(apparelValue, healthValue);
        }
    }

}