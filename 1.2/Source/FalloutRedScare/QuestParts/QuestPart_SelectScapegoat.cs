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
	public class QuestPart_SelectScapegoat : QuestPart
	{
		public string inSignal;

		public Faction faction;
		public override bool RequiresAccepter => true;
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
			if (signal.tag == inSignal)
			{
				Log.Message("signal.tag: " + signal.tag);
				Pawn arg = quest.AccepterPawn;
				if (arg == null)
				{
					signal.args.TryGetArg("CHOSEN", out arg);
				}
				if (arg != null && arg.royalty != null)
				{
					arg.royalty.ReduceTitle(faction);
				}
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref inSignal, "inSignal");
			Scribe_References.Look(ref faction, "faction");
		}
	}
}