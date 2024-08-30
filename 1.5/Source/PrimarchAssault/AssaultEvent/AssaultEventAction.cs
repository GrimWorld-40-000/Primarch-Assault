using PrimarchAssault.External;
using Verse;

namespace RimworldModding.AssaultEvent
{
    public abstract class AssaultEventAction
    {
        public AssaultEventDef parent;
        public AssaultEventActionProperties props;

        public virtual void Initialize(AssaultEventActionProperties props)
        {
            this.props = props;
        }

        public abstract void Apply(Map map);
    }
}