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

		protected override void RunInt()
		{
			var prevS = ThingDefOf.Shuttle;
			ThingDefOf.Shuttle = shuttle.GetValue(QuestGen.slate);
			base.RunInt();
			ThingDefOf.Shuttle = prevS;
		}
	}
}