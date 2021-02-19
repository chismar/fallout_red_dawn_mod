using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI.Group;

namespace FalloutRedScare
{
	public class AirSupportSettings
	{
		public ThingDef shuttleDef;
		public ThingDef shuttleSkyfallerIncoming;
		public List<PawnGroupMaker> pawnGroupMakers;
		public IntRange points;
	}
	public class CallAirSupport : FRS_ScriptedTitlePermitWorker<AirSupportSettings>
	{
		public override IEnumerable<FloatMenuOption> GetRoyalAidOptions(Map map, Pawn pawn, Faction faction)
		{
			if (AidDisabled(map, pawn, faction, out string reason))
			{
				yield return new FloatMenuOption(def.LabelCap + ": " + reason, null);
				yield break;
			}
			Action action = null;
			string description = def.LabelCap + " (" + "FRS.CommandSquadShuttle".Translate() + "): ";
			if (FillAidOption(pawn, faction, ref description, out bool free))
			{
				action = delegate
                {
                    CallShuttleWithParams(map, pawn, faction);
                };
				
			}
			yield return new FloatMenuOption(description, action, faction.def.FactionIcon, faction.Color);
		}

        private void CallShuttleWithParams(Map map, Pawn pawn, Faction faction)
        {
            if (this.workerSettings.pawnGroupMakers.TryRandomElementByWeight(x => x.commonality, out PawnGroupMaker pawnGroupMaker))
            {
                PawnGroupMakerParms parms = new PawnGroupMakerParms();
                parms.groupKind = PawnGroupKindDefOf.Combat;
                parms.tile = map.Tile;
                parms.points = this.workerSettings.points.RandomInRange;
                parms.faction = faction;
                parms.generateFightersOnly = true;
                CallShuttle(pawnGroupMaker, parms, pawn, map, faction);
            }
        }

        private void CallShuttle(PawnGroupMaker pawnGroupMaker, PawnGroupMakerParms parms, Pawn pawn, Map map, Faction faction)
		{
			if (!faction.HostileTo(Faction.OfPlayer))
			{
				var pawns = pawnGroupMaker.GeneratePawns(parms).ToList();
				Arrive(pawns, faction, map, this.workerSettings.shuttleDef, this.workerSettings.shuttleSkyfallerIncoming);
			}
		}
		protected LordJob MakeLordJob(IntVec3 dropCenter, Faction faction, Map map)
		{
			if (faction.HostileTo(Faction.OfPlayer))
			{
				return new LordJob_AssaultColony(faction);
			}
			RCellFinder.TryFindRandomSpotJustOutsideColony(dropCenter, map, out IntVec3 result);
			return new LordJob_AssistColony(faction, result);
		}
		public void Arrive(List<Pawn> pawns, Faction faction, Map map, ThingDef shuttleDef, ThingDef shuttleIncomingDef)
		{
			while (pawns.Any())
			{
				var group = pawns.Take(8);
				pawns = pawns.Skip(group.Count()).ToList();
				IntVec3 dropCenter = DropCellFinder.RandomDropSpot(map);
				LordMaker.MakeNewLord(faction, MakeLordJob(dropCenter, faction, map), map, group);
				var shuttle = ThingMaker.MakeThing(shuttleDef, null);
				var comp = shuttle.TryGetComp<CompShuttle>();
				var compTransporter = ThingCompUtility.TryGetComp<CompTransporter>(shuttle);
				foreach (Pawn pawn in group)
				{
					compTransporter.innerContainer.TryAdd(pawn, 1);
				}
				GenPlace.TryPlaceThing(SkyfallerMaker.MakeSkyfaller(shuttleIncomingDef, shuttle), dropCenter, map, ThingPlaceMode.Near, null, null, default(Rot4));
				comp.requiredColonistCount = 0;
				comp.missionShuttleTarget = map.Parent;
				comp.missionShuttleHome = null;
				comp.dropEverythingOnArrival = true;
				comp.requiredPawns = group.ToList();
				comp.hideControls = true;
			}
		}

        public override float CombatScore(Pawn caster, Map map, FactionPermit permit, out List<LocalTargetInfo> targets)
        {
			targets = null;
			var hostiles = caster.Map.attackTargetsCache.GetPotentialTargetsFor(caster).Select(x => x.Thing);
			if (hostiles.Any())
            {
				return 1f;
            }
			return 0f;
		}

        public override void DoPermitCast(Pawn caster, Map map, List<LocalTargetInfo> targets)
        {
            base.DoPermitCast(caster, map, targets);
			CallShuttleWithParams(map, caster, caster.Faction);
		}
    }
}
