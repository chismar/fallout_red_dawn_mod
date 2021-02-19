using RimWorld;
using System.Collections.Generic;
using Verse;

namespace FalloutRedScare
{
    public class ReinforcementsDef : Def
	{
		public FloatRange reinforcementCooldownDays;
		public List<IncidentDef> reinforcementIncidents;
	}
}