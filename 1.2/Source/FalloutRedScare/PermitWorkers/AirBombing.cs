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
						//var shuttle = ThingMaker.MakeThing(workerSettings.shuttleBomberDef, null);

						//var comp = shuttle.TryGetComp<CompShuttle>();
						// compTransporter = ThingCompUtility.TryGetComp<CompTransporter>(shuttle);
						//var startingCell = CellRect.WholeMap(map).EdgeCells.OrderBy(cell => cell.DistanceTo(x.Cell)).FirstOrDefault();
						var sk = SkyfallerMaker.MakeSkyfaller(workerSettings.skyfallerBomber);
						GenPlace.TryPlaceThing(sk, x.Cell, map, ThingPlaceMode.Near, null, null, default(Rot4));
						//comp.requiredColonistCount = 0;
						//comp.missionShuttleTarget = map.Parent;
						//comp.missionShuttleHome = null;
						var compBomber = sk as ShuttleBomber;
						compBomber.shells = workerSettings.shells;
						compBomber.targetMap = map;
						var dir = (x.Cell - pawn.Position).ToVector3();
						compBomber.angle = Vector3.SignedAngle(dir, Vector3.forward, Vector3.up);
					}, null, null);
				};
			}
			yield return new FloatMenuOption(description, action, faction.def.FactionIcon, faction.Color);
		}
	}
}
