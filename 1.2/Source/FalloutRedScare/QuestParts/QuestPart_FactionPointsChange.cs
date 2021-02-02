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
	public class QuestPart_FactionPointsChange : QuestPart
	{
		public string inSignal;

		public Faction faction;

		public float points;
		public override IEnumerable<Faction> InvolvedFactions
		{
			get
			{
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
				if (TotalWarUtils.TryGetFactionWarData(faction, out FactionWar factionWar))
                {
					factionWar.points += points;
                }
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref inSignal, "inSignal");
			Scribe_References.Look(ref faction, "faction");
			Scribe_Values.Look(ref points, "points");
		}
	}
}