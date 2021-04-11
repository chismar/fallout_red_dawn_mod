using RimWorld;
using System.Collections.Generic;
using Verse;

namespace RedScare
{
    public class TotalWarDef : Def
	{
		public FloatRange scapegoatCooldownDays;
		public FloatRange productionQuotaCooldownDays;
		public QuestScriptDef scapegoatQuestScript;
		public FloatRange baseIncidentsCooldownDays;
		public int raidPointsPerBase;
		public float powerPointsPerBase;
		public float powerPointsToUpgrade;
		public FactionDef factionDef;
		public QuestScriptDef productionQuota;
		public List<IncidentDef> factionBaseSpawnIncidents;
		public IncidentDef factionBaseAbandonIncident;
		public FloatRange permitCooldownDays;
		public FloatRange introDelayFromStart;
		public QuestScriptDef introScript;
		
		public float powerPointsLossPerPawnCombatPowerRatio;
		public float powerPointPassiveGainPerDay;
	}
}