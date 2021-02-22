using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RedScare
{
    class FRS_Shuttle : CompShuttle
    {
    }
    class CompProperties_FRS_Shuttle : CompProperties_Shuttle
    {
        public ThingDef skyfallerLeaving;
        public CompProperties_FRS_Shuttle()
        {
            this.compClass = typeof(FRS_Shuttle);
        }
    }
}
