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

namespace RedScare
{
	public class IncidentWorker_BaseAbandoned : IncidentWorker
	{
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Faction faction = parms.faction;
            var settlementCandidates = Find.WorldObjects.SettlementBases.Where(x => x.Faction == faction);
            if (settlementCandidates.Any())
            {
                var settlement = settlementCandidates.RandomElement();
                string letterLabel = this.def.letterLabel;
                string letterText = this.def.letterText;
                this.SendStandardLetter(letterLabel, letterText, def.letterDef, parms, settlement, faction.Name);
                settlement.Destroy();
                return true;
            }
            return false;
        }
    }
}