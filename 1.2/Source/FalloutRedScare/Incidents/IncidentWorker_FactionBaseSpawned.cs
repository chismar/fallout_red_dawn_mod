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
	public class IncidentWorker_FactionBaseSpawned : IncidentWorker
	{
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Settlement settlement = (Settlement)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Settlement);
            Faction faction = parms.faction;
            settlement.SetFaction(faction);
            settlement.Name = SettlementNameGenerator.GenerateSettlementName(settlement, parms.faction.def.settlementNameMaker);
            settlement.Tile = TileFinder.RandomSettlementTileFor(faction);
            Find.WorldObjects.Add(settlement);
            string letterLabel = this.def.letterLabel;
            string letterText = this.def.letterText;
            this.SendStandardLetter(letterLabel, letterText, def.letterDef, parms, settlement, faction.Name);
            return true;
        }
    }
}