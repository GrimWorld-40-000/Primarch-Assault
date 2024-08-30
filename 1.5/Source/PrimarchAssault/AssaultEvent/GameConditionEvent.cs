using System;
using RimWorld;
using Verse;

namespace RimworldModding.AssaultEvent
{
    public class GameConditionEventProperties : AssaultEventActionProperties
    {
        public int tickDuration;
        public GameConditionDef condition;
        public override Type AssaultEventClass() => typeof(GameConditionEvent);
    }
    
    public class GameConditionEvent: AssaultEventAction
    {
        private GameConditionEventProperties Props => (GameConditionEventProperties) props;
        public override void Apply(Map map)
        {
            map.gameConditionManager.RegisterCondition(GameConditionMaker.MakeCondition(Props.condition, Props.tickDuration));
        }
    }
}