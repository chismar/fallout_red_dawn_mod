using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace FalloutRedScare
{
	public class AirSupportSettingsTargeted
	{
		public ThingDef shuttleDef;
		public ThingDef shuttleSkyfallerIncoming;
		public List<PawnGroupMaker> pawnGroupMakers;
		public IntRange points;
		public float radius;
	}
	public class CallAirSupportTargeted : FRS_ScriptedTitlePermitWorker<AirSupportSettingsTargeted>
	{
        private Faction calledFaction;
        private bool free;
        private Pawn caller;
        private Map map;

        public static AcceptanceReport ShuttleCanLandHere(LocalTargetInfo target, Map map)
		{
			TaggedString t = "CannotCallShuttle".Translate() + ": ";
			if (!target.IsValid)
			{
				return new AcceptanceReport(t + "MessageTransportPodsDestinationIsInvalid".Translate().CapitalizeFirst());
			}
			foreach (IntVec3 cell in GenAdj.OccupiedRect(target.Cell, Rot4.North, ThingDefOf.Shuttle.size))
			{
				string reportFromCell = GetReportFromCell(cell, map);
				if (reportFromCell != null)
				{
					return new AcceptanceReport(t + reportFromCell);
				}
			}
			string reportFromCell2 = GetReportFromCell(target.Cell + CompShuttle.DropoffSpotOffset, map);
			if (reportFromCell2 != null)
			{
				return new AcceptanceReport(t + reportFromCell2);
			}
			return AcceptanceReport.WasAccepted;
		}
		private static string GetReportFromCell(IntVec3 cell, Map map)
		{
			if (!cell.InBounds(map))
			{
				return "OutOfBounds".Translate().CapitalizeFirst();
			}
			if (cell.Fogged(map))
			{
				return "ShuttleCannotLand_Fogged".Translate().CapitalizeFirst();
			}
			if (!cell.Walkable(map))
			{
				return "ShuttleCannotLand_Unwalkable".Translate().CapitalizeFirst();
			}
			RoofDef roof = cell.GetRoof(map);
			if (roof != null && (roof.isNatural || roof.isThickRoof))
			{
				return "MessageTransportPodsDestinationIsInvalid".Translate().CapitalizeFirst();
			}
			List<Thing> thingList = cell.GetThingList(map);
			for (int i = 0; i < thingList.Count; i++)
			{
				Thing thing = thingList[i];
				if (thing is IActiveDropPod || thing is Skyfaller || thing.def.category == ThingCategory.Item || thing.def.category == ThingCategory.Building)
				{
					return "BlockedBy".Translate(thing).CapitalizeFirst();
				}
				PlantProperties plant = thing.def.plant;
				if (plant != null && plant.IsTree)
				{
					return "BlockedBy".Translate(thing).CapitalizeFirst();
				}
			}
			return null;
		}
		private static List<IntVec3> tempSourceList = new List<IntVec3>();
		public bool CanHitTarget(LocalTargetInfo target)
		{
			if (!GenSight.LineOfSight(this.caller.Position, target.Cell, this.map, true, null, 0, 0))
			{
				bool flag = false;
				ShootLeanUtility.LeanShootingSourcesFromTo(this.caller.Position, target.Cell, this.map, tempSourceList);
				for (int i = 0; i < tempSourceList.Count; i++)
				{
					if (GenSight.LineOfSight(tempSourceList[i], target.Cell, this.map, true, null, 0, 0))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}
		public bool ValidateTarget(LocalTargetInfo target)
		{
			if (!CanHitTarget(target))
			{
				if (target.IsValid)
				{
					Messages.Message(this.def.LabelCap + ": " + "AbilityCannotHitTarget".Translate(), MessageTypeDefOf.RejectInput, true);
				}
				return false;
			}
			AcceptanceReport acceptanceReport = ShuttleCanLandHere(target, this.map);
			if (!acceptanceReport.Accepted)
			{
				Messages.Message(acceptanceReport.Reason, new LookTargets(target.Cell, this.map), MessageTypeDefOf.RejectInput, false);
			}
			return acceptanceReport.Accepted;
		}
		private void BeginCallShuttle(Pawn caller, Map map, Faction faction, bool free)
		{
			this.caller = caller;
			this.map = map;
			this.calledFaction = faction;
			this.free = free; 
		}

        public void DrawHighlight(LocalTargetInfo target)
		{
			if (caller == null)
				return;
			GenDraw.DrawRadiusRing(this.caller.Position, workerSettings.radius, Color.white, null);
			DrawShuttleGhost(target, this.map); 
		}
		public void DrawShuttleGhost(LocalTargetInfo target, Map map)
		{
			Color ghostCol = ShuttleCanLandHere(target, map).Accepted ? Designator_Place.CanPlaceColor : Designator_Place.CannotPlaceColor;
			GhostDrawer.DrawGhostThing_NewTmp(target.Cell, Rot4.North, workerSettings.shuttleDef, workerSettings.shuttleDef.graphic, ghostCol, AltitudeLayer.Blueprint, null, true);
			Vector3 position = (target.Cell + IntVec3.South * 2).ToVector3ShiftedWithAltitude(AltitudeLayer.Blueprint);
			Graphics.DrawMesh(MeshPool.plane10, position, Quaternion.identity, GenDraw.InteractionCellMaterial, 0);
		}

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
					BeginCallShuttle(pawn, map, faction, free);
					Find.Targeter.BeginTargeting(ForLoc(pawn, workerSettings.radius), delegate (LocalTargetInfo x)
					{
						CallShuttle(x.Cell);
					}, DrawHighlight, ValidateTarget, pawn);
				};
				
			}
			yield return new FloatMenuOption(description, action, faction.def.FactionIcon, faction.Color);
		}
		private void CallShuttle(IntVec3 landingCell)
        {
			if (this.workerSettings.pawnGroupMakers.TryRandomElementByWeight(x => x.commonality, out PawnGroupMaker pawnGroupMaker))
			{
				PawnGroupMakerParms parms = new PawnGroupMakerParms();
				parms.groupKind = PawnGroupKindDefOf.Combat;
				parms.tile = map.Tile;
				parms.points = this.workerSettings.points.RandomInRange;
				parms.faction = calledFaction;
				parms.generateFightersOnly = true;
				CallShuttle(pawnGroupMaker, parms, caller, map, calledFaction, landingCell);
			}
		}


        private void CallShuttle(PawnGroupMaker pawnGroupMaker, PawnGroupMakerParms parms, Pawn pawn, Map map, Faction faction, IntVec3 landingCell)
		{
			if (!faction.HostileTo(Faction.OfPlayer))
			{
				var pawns = pawnGroupMaker.GeneratePawns(parms).ToList();
				Arrive(pawns, faction, map, this.workerSettings.shuttleDef, this.workerSettings.shuttleSkyfallerIncoming, landingCell);
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
		public void Arrive(List<Pawn> pawns, Faction faction, Map map, ThingDef shuttleDef, ThingDef shuttleIncomingDef, IntVec3 landingCell)
		{
			IntVec3 dropCenter = landingCell;
			LordMaker.MakeNewLord(faction, MakeLordJob(dropCenter, faction, map), map, pawns);
			var shuttle = ThingMaker.MakeThing(shuttleDef, null);
			var comp = shuttle.TryGetComp<CompShuttle>();
			var compTransporter = ThingCompUtility.TryGetComp<CompTransporter>(shuttle);
			foreach (Pawn pawn in pawns)
			{
				compTransporter.innerContainer.TryAdd(pawn, 1);
			}
			GenPlace.TryPlaceThing(SkyfallerMaker.MakeSkyfaller(shuttleIncomingDef, shuttle), dropCenter, map, ThingPlaceMode.Near, null, null, default(Rot4));
			comp.requiredColonistCount = 0;
			comp.missionShuttleTarget = map.Parent;
			comp.missionShuttleHome = null;
			comp.dropEverythingOnArrival = true;
			comp.requiredPawns = pawns;
			comp.hideControls = true;
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
			BeginCallShuttle(caster, map, caster.Faction, free);
			CallShuttle(CellFinder.RandomClosewalkCellNear(caster.Position, map, 10));
		}
    }
}
