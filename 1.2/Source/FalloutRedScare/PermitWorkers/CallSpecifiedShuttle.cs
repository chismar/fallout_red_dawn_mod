using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace FalloutRedScare
{
	public class CallSpecifiedShuttleSettings
	{
		public QuestScriptDef shuttleQuestDef;
		public ThingDef shuttleDef;
	}
	public class CallSpecifiedShuttle : RoyalTitlePermitWorker_CallShuttle
	{

        public override void DrawHighlight(LocalTargetInfo target)
        {
			var prevS = ThingDefOf.Shuttle;
			ThingDefOf.Shuttle = ((this.def as FRSRoyalTitlePermitDef).workerSettings as CallSpecifiedShuttleSettings).shuttleDef;
			base.DrawHighlight(target);
			ThingDefOf.Shuttle = prevS;
		}
    }
}
