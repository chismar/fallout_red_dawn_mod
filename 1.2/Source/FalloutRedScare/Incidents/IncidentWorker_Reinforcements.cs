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
	public class IncidentWorker_Reinforcements : IncidentWorker
	{
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            var idef = def as ReinforcementsIncidentDef;
            Map map = (Map)parms.target;
            var ls = DropCellFinder.GetBestShuttleLandingSpot(map, parms.faction, out _);
            if(!ls.IsValid)
            {
                Messages.Message("No available landing zone for reinforcements", new LookTargets(), MessageTypeDefOf.RejectInput, false);
            }
            else
            {
                AcceptanceReport acceptanceReport = CallAirSupportTargeted.ShuttleCanLandHere(ls, map);
                if (!acceptanceReport.Accepted)
                {
                    Messages.Message(acceptanceReport.Reason, new LookTargets(ls, map), MessageTypeDefOf.RejectInput, false);
                }
                else
                {
                    if(Settings.reinforcementsPowerPoints)
                    {
                        idef.airSupportSettings.points = new IntRange(Settings.powerpoints, Settings.powerpoints);
                    }
                    CallAirSupportTargeted.CallShuttle(idef.airSupportSettings,
                        new CallAirSupportTargeted.AirSupportShuttleSettings() { calledFaction = Faction.OfPlayer, map = map }, ls);
                }
            }
            return true;
        }

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            var ls = DropCellFinder.GetBestShuttleLandingSpot(map, parms.faction, out _);
            if (!ls.IsValid)
            {
                Messages.Message("No available landing zone for reinforcements", new LookTargets(), MessageTypeDefOf.RejectInput, false);
            }
            return ls.IsValid;
        }

    }
}