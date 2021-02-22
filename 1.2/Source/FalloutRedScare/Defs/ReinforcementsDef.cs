using RimWorld;
using System.Collections.Generic;
using Verse;

namespace RedScare
{
    public class ReinforcementsDef : Def
	{
		public List<FactionDef> reinforceOnlyIfPlayerIsOfFaction;
		public FloatRange reinforcementCooldownDays;
		public List<IncidentDef> reinforcementIncidents;
	}
}