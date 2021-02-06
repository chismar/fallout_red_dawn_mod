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
	public class AirBombingSettings
	{
		public string airBombingCommandKey;
		public float shootingLineAngle;
		public ThingDef shuttleBomberDef;
		public ThingDef skyfallerBomber;
		public List<ThingDef> shells;
	}
	public class AirBombing : FRS_ScriptedTitlePermitWorker<AirBombingSettings>
	{
		public override IEnumerable<FloatMenuOption> GetRoyalAidOptions(Map map, Pawn pawn, Faction faction)
		{
			if (AidDisabled(map, pawn, faction, out string reason))
			{
				yield return new FloatMenuOption(def.LabelCap + ": " + reason, null);
				yield break;
			}
			Action action = null;
			string description = def.LabelCap + " (" + workerSettings.airBombingCommandKey.Translate() + "): ";
			if (FillAidOption(pawn, faction, ref description, out bool free))
			{
				action = delegate
				{
					Find.Targeter.BeginTargeting(ForLoc(), delegate (LocalTargetInfo x)
					{
						var shuttle = ThingMaker.MakeThing(workerSettings.shuttleBomberDef, null);
						var comp = shuttle.TryGetComp<CompShuttle>();
						var compTransporter = ThingCompUtility.TryGetComp<CompTransporter>(shuttle);
						var startingCell = CellRect.WholeMap(map).EdgeCells.OrderBy(cell => cell.DistanceTo(x.Cell)).FirstOrDefault();
						GenPlace.TryPlaceThing(SkyfallerMaker.MakeSkyfaller(workerSettings.skyfallerBomber, shuttle), startingCell, map, ThingPlaceMode.Near, null, null, default(Rot4));
						comp.requiredColonistCount = 0;
						comp.missionShuttleTarget = map.Parent;
						comp.missionShuttleHome = null;
						var compBomber = shuttle.TryGetComp<CompShuttleBomber>();
						compBomber.shells = workerSettings.shells;
						compBomber.targetMap = map;
					}, null, null);
				};
			}
			yield return new FloatMenuOption(description, action, faction.def.FactionIcon, faction.Color);
		}
	}
}
