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
	public class TotalWarDef : Def
	{
		public FloatRange scapegoatCooldownDays;
		public QuestScriptDef scapegoatQuestScript;
		public IntRange baseIncidentsCooldown;
		public float powerPointsPerBase;
		public float powerPointsToUpgrade;
		public FactionDef factionDef;
		public List<IncidentDef> factionBaseSpawnIncidents;
		public IncidentDef factionBaseAbandonIncident;
		
		public float powerPointsLossPerPawnCombatPowerRatio;
		public float powerPointPassigeGainPerDay;
	}
}