using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using RimWorld;
using RimworldModding.AssaultEvent;
using Verse;

namespace PrimarchAssault.External
{
    public class AssaultEventDef: Def
    {
        public List<AssaultEventAction> Actions;
        public List<AssaultEventActionProperties> actionProperties;
        //Whether to trigger all actions, or select a random one
        public bool triggerAllActions = true;
        
        
        
        public override void PostLoad()
        {
            base.PostLoad();
            InitializeComps();
        }

        
        
        public void InitializeComps()
        {
            Actions = new List<AssaultEventAction>();
            if (actionProperties.NullOrEmpty()) return;
            for (int index = 0; index < actionProperties.Count; ++index)
            {
                AssaultEventAction action = null;
                try
                {
                    action = (AssaultEventAction) Activator.CreateInstance(actionProperties[index].AssaultEventClass());
                    action.parent = this;
                    Actions.Add(action);
                    action.Initialize(actionProperties[index]);
                }
                catch (Exception ex)
                {
                    Log.Error("Could not instantiate or initialize an assault action: " + ex);
                    Actions.Remove(action);
                }
            }
        }

        public void FireAllComps(Map map)
        {
            if (triggerAllActions)
            {
                foreach (AssaultEventAction assaultEventAction in Actions)
                {
                    assaultEventAction.Apply(map);
                }
            }
            else
            {
                Actions.RandomElement().Apply(map);
            }
        }
    }
}