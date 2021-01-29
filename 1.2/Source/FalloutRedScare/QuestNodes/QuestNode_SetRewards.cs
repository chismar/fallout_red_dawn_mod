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
    public class QuestNode_SetRewards : QuestNode
    {
		public SlateRef<IntRange> goodwillRange;
		public SlateRef<IntRange> favorRange;

		public SlateRef<Pawn> giveTo;
		public SlateRef<string> inSignal;
		protected override bool TestRunInt(Slate slate)
        {
            return true;
        }

        protected override void RunInt()
        {
			Slate slate = QuestGen.slate;

			QuestPart_Choice questPart_Choice = new QuestPart_Choice();
			QuestPart_Choice.Choice choice = new QuestPart_Choice.Choice();
			Reward_Goodwill reward_Goodwill = new Reward_Goodwill();
			reward_Goodwill.faction = slate.Get<Faction>("askerFaction");
			reward_Goodwill.amount = goodwillRange.GetValue(slate).RandomInRange;
			choice.rewards.Add(reward_Goodwill);
			questPart_Choice.choices.Add(choice);

			QuestPart_GiveRoyalFavor questPart_GiveRoyalFavor = new QuestPart_GiveRoyalFavor();
			questPart_GiveRoyalFavor.giveTo = giveTo.GetValue(slate);
			questPart_GiveRoyalFavor.giveToAccepter = true;
			questPart_GiveRoyalFavor.faction = slate.Get<Faction>("askerFaction");
			questPart_GiveRoyalFavor.amount = favorRange.GetValue(slate).RandomInRange;
			questPart_GiveRoyalFavor.inSignal = (QuestGenUtility.HardcodedSignalWithQuestID(inSignal.GetValue(slate)) ?? QuestGen.slate.Get<string>("inSignal"));
			QuestGen.quest.AddPart(questPart_GiveRoyalFavor);

			QuestPart_Choice.Choice choice2 = new QuestPart_Choice.Choice();
			Reward_RoyalFavor reward_Favor = new Reward_RoyalFavor();
			reward_Favor.faction = slate.Get<Faction>("askerFaction");
			reward_Favor.amount = questPart_GiveRoyalFavor.amount;
			choice2.rewards.Add(reward_Favor);
			questPart_Choice.choices.Add(choice2);

			QuestGen.quest.AddPart(questPart_Choice);
		}
    }
}