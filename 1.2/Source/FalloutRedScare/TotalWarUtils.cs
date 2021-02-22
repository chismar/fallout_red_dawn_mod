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
    public static class TotalWarUtils
    {
        public static bool TryGetFactionWarData(Faction faction, out FactionWar factionWar)
        {
            var warComp = Find.World.GetComponent<WorldComponent_TotalWar>();
            if (warComp.factions.TryGetValue(faction, out FactionWar tmp_factionWar)) 
            {
                factionWar = tmp_factionWar;
                return true;
            }
            factionWar = null;
            return false;
        }
    }
}