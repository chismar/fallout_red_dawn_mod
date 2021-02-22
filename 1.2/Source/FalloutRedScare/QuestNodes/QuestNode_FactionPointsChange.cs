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
    public class QuestNode_FactionPointsChange : QuestNode
    {
		[NoTranslate]
		public SlateRef<string> inSignal;

		public SlateRef<Faction> faction;

		public SlateRef<float> points;

		protected override bool TestRunInt(Slate slate)
		{
			return true;
		}

		protected override void RunInt()
		{
			Slate slate = QuestGen.slate;
			QuestPart_FactionPointsChange questPart_FactionPointsChange = new QuestPart_FactionPointsChange();
			questPart_FactionPointsChange.faction = faction.GetValue(slate);
			questPart_FactionPointsChange.points = points.GetValue(slate);
			questPart_FactionPointsChange.inSignal = (QuestGenUtility.HardcodedSignalWithQuestID(inSignal.GetValue(slate)) ?? slate.Get<string>("inSignal"));
			questPart_FactionPointsChange.signalListenMode = QuestPart.SignalListenMode.OngoingOnly;
			QuestGen.quest.AddPart(questPart_FactionPointsChange);
		}
	}
}