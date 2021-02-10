using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace FalloutRedScare
{
    class ApparelComp_RestrictBodyType : ThingComp
    {
    }
    public class CompProperties_ApparelComp_RestrictBodyType : CompProperties
    {
        public List<BodyTypeDef> restrictBodyTypes;
        public CompProperties_ApparelComp_RestrictBodyType()
        {
            this.compClass = typeof(ApparelComp_RestrictBodyType);
        }
    }
}
