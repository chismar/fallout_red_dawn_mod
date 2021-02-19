using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace FalloutRedScare
{
    public class PawnGroupMakerPR : PawnGroupMaker
    {
        public int minimumPower = 0;
        public int maximumPower = int.MaxValue;

        public bool CanGenerate(PawnGroupMakerParms parms)
        {
            var wctw = Find.World.GetComponent<WorldComponent_TotalWar>();
            if (!wctw.factions.TryGetValue(parms.faction, out var fw))
                return true;
            var points = (int)(fw.FactionBases.Count * fw.def.raidPointsPerBase);
            return points >= minimumPower && points < maximumPower;
        }
    }
}
