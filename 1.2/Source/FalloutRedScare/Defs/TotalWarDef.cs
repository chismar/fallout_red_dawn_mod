﻿using RimWorld;
using System.Collections.Generic;
using Verse;

namespace FalloutRedScare
{
    public class TotalWarDef : Def
	{
		public FloatRange scapegoatCooldownDays;
		public QuestScriptDef scapegoatQuestScript;
		public FloatRange baseIncidentsCooldownDays;
		public float powerPointsPerBase;
		public float powerPointsToUpgrade;
		public FactionDef factionDef;
		public List<IncidentDef> factionBaseSpawnIncidents;
		public IncidentDef factionBaseAbandonIncident;
		
		public float powerPointsLossPerPawnCombatPowerRatio;
		public float powerPointPassiveGainPerDay;
	}
}