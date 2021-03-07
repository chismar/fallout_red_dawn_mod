using RimWorld;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RedScare
{
    public class QuestNode_GenerateSpecifiedShuttle : QuestNode_GenerateShuttle
    {
        public SlateRef<ThingDef> shuttle;
        public SlateRef<ThingDef> requiredDef;
        public SlateRef<ThingDef> requiredStuff;
        public SlateRef<int> requiredCount;

        protected override void RunInt()
        {
            var prevS = ThingDefOf.Shuttle;
            ThingDefOf.Shuttle = shuttle.GetValue(QuestGen.slate);
            base.RunInt();
            ThingDefOf.Shuttle = prevS;
            Slate slate = QuestGen.slate;
            if (requiredDef.GetValue(slate) != null)
            {
                var thing = QuestGen.slate.Get<Thing>(this.storeAs.GetValue(slate));
                var cs = thing.TryGetComp<CompShuttle>();
                cs.requiredItems = new List<ThingDefCount>();
                cs.requiredItems.Add(new ThingDefCount(requiredDef.GetValue(slate), requiredCount.GetValue(slate)));
            }
        }
    }
}