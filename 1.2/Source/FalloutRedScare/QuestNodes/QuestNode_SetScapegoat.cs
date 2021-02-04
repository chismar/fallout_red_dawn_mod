using RimWorld;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace FalloutRedScare
{
    public class QuestNode_SetScapegoat : QuestNode
    {
		public SlateRef<string> inSignal;
		public SlateRef<int> setGoodwill;
		public SlateRef<Faction> faction;
		public SlateRef<FactionRelationKind> factionRelation;
		public SlateRef<RoyalTitleDef> minimumTitle;
		protected override bool TestRunInt(Slate slate)
		{
			return PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonists_NoCryptosleep.Any(x => x.royalty.GetCurrentTitle(slate.Get<Faction>("askerFaction")) != null);
			/*var faction = slate.Get<Faction>("askerFaction");
			var warComp = Find.World.GetComponent<WorldComponent_TotalWar>();
			if (TotalWarUtils.TryGetFactionWarData(faction, out FactionWar factionWar) && factionWar.CanSpawnScapeGoatQuest)
			{
				return PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonists_NoCryptosleep.Any(x => x.royalty.GetCurrentTitle(faction) != null);
			}
			return false;*/
		}

		protected override void RunInt()
        {
			Slate slate = QuestGen.slate;
			QuestPart_Choice questPart_Choice = new QuestPart_Choice();

			QuestPart_SetFactionGoodwill questPart_FactionGoodwillChange = new QuestPart_SetFactionGoodwill();
			questPart_FactionGoodwillChange.faction = faction.GetValue(slate);
			questPart_FactionGoodwillChange.goodwillFixed = setGoodwill.GetValue(slate);
			questPart_FactionGoodwillChange.relationKind = factionRelation.GetValue(slate);
			questPart_FactionGoodwillChange.inSignal = QuestGenUtility.HardcodedSignalWithQuestID("Initiate");
			questPart_FactionGoodwillChange.outSignal = QuestGenUtility.HardcodedSignalWithQuestID(questPart_FactionGoodwillChange.outSignal);
			QuestGen.quest.AddPart(questPart_FactionGoodwillChange);

			QuestPart_SelectScapegoat questPart_SelectScapegoat = new QuestPart_SelectScapegoat();
			questPart_SelectScapegoat.faction = faction.GetValue(slate);
			questPart_SelectScapegoat.inSignal = QuestGenUtility.HardcodedSignalWithQuestID("Initiate");
			QuestGen.quest.AddPart(questPart_SelectScapegoat);

			QuestGen.quest.AddPart(new QuestPart_RequirementsToAcceptColonistWithTitle
			{
				minimumTitle = minimumTitle.GetValue(slate),
				faction = faction.GetValue(slate)
			});
			QuestPart_Choice.Choice choice2 = new QuestPart_Choice.Choice();
			Reward_RoyalFavor reward_Favor = new Reward_RoyalFavor();
			reward_Favor.faction = slate.Get<Faction>("askerFaction");
			reward_Favor.amount = -10;
			choice2.rewards.Add(reward_Favor);
			choice2.questParts.Add(questPart_SelectScapegoat);
			questPart_Choice.choices.Add(choice2);
			QuestGen.quest.AddPart(questPart_Choice);
		}
	}
}