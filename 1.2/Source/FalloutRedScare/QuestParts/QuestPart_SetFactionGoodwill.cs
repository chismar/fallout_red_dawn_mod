using RimWorld;
using RimWorld.Planet;
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
	public class QuestPart_SetFactionGoodwill : QuestPart
	{
		public string inSignal;

		public FactionRelationKind relationKind;

		public int goodwillFixed;

		public Faction faction;

		public bool canSendMessage = true;

		public bool canSendHostilityLetter = true;

		public string reason;

		public bool getLookTargetFromSignal = true;

		public GlobalTargetInfo lookTarget;

        public override bool RequiresAccepter => true;

        public override IEnumerable<GlobalTargetInfo> QuestLookTargets
		{
			get
			{
				foreach (GlobalTargetInfo questLookTarget in base.QuestLookTargets)
				{
					yield return questLookTarget;
				}
				yield return lookTarget;
			}
		}

		public override IEnumerable<Faction> InvolvedFactions
		{
			get
			{
				foreach (Faction involvedFaction in base.InvolvedFactions)
				{
					yield return involvedFaction;
				}
				if (faction != null)
				{
					yield return faction;
				}
			}
		}

		public override void Notify_QuestSignalReceived(Signal signal)
		{
			base.Notify_QuestSignalReceived(signal);
			Log.Message("signal.tag == inSignal && faction: " + signal.tag + " - " + faction);
			if (signal.tag == inSignal && faction != null && faction != Faction.OfPlayer)
			{
				Log.Message("signal.tag: " + signal.tag);
				LookTargets lookTargets;
				GlobalTargetInfo value = lookTarget.IsValid ? lookTarget : ((!getLookTargetFromSignal) ? GlobalTargetInfo.Invalid : ((!SignalArgsUtility.TryGetLookTargets(signal.args, "SUBJECT", out lookTargets)) ? GlobalTargetInfo.Invalid : lookTargets.TryGetPrimaryTarget()));
				FactionRelationKind playerRelationKind = faction.PlayerRelationKind;
				var relation = new FactionRelation();
				relation.goodwill = goodwillFixed;
				relation.kind = FactionRelationKind.Ally;
				relation.other = Faction.OfPlayer;
				faction.SetRelation(relation);
				TaggedString text = "";
				faction.TryAppendRelationKindChangedInfo(ref text, playerRelationKind, faction.PlayerRelationKind);
				if (!text.NullOrEmpty())
				{
					text = "\n\n" + text;
				}
				Find.SignalManager.SendSignal(new Signal("ScapegoatSelected"));
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref inSignal, "inSignal");
			Scribe_Values.Look(ref relationKind, "relationKind");
			Scribe_Values.Look(ref goodwillFixed, "goodwillFixed", 0);
			Scribe_References.Look(ref faction, "faction");
			Scribe_Values.Look(ref canSendMessage, "canSendMessage", defaultValue: true);
			Scribe_Values.Look(ref canSendHostilityLetter, "canSendHostilityLetter", defaultValue: true);
			Scribe_Values.Look(ref reason, "reason");
			Scribe_Values.Look(ref getLookTargetFromSignal, "getLookTargetFromSignal", defaultValue: true);
			Scribe_TargetInfo.Look(ref lookTarget, "lookTarget");
		}
	}
}