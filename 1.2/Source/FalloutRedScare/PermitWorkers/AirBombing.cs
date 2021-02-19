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
		public ThingDef skyfallerBomber;
		public List<ThingDef> shells;
		public int shellsCount = 1;
		public FloatRange bombingRunAroundTargetMeters;
	}
	public class AirBombing : FRS_ScriptedTitlePermitWorker<AirBombingSettings>
	{
        public override float CombatScore(Pawn caster, Map map, FactionPermit permit, out List<LocalTargetInfo> targets)
        {
			targets = null;
			var hostiles = caster.Map.attackTargetsCache.GetPotentialTargetsFor(caster).Select(x => x.Thing);
			hostiles = hostiles.Where(x => !x.Position.Roofed(map) && x.Position.DistanceTo(caster.Position) > 10);
			if (hostiles.Any())
            {
				targets = new List<LocalTargetInfo> { hostiles.First() };
				return 1f;
            }
			return 0f;
		}

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
						DoBombing(pawn, map, x);
					}, null, null);
				};
			}
			yield return new FloatMenuOption(description, action, faction.def.FactionIcon, faction.Color);
		}

		public void DoBombing(Pawn pawn, Map map, LocalTargetInfo x)
        {
			var sk = SkyfallerMaker.MakeSkyfaller(workerSettings.skyfallerBomber);
			var dir = (x.Cell - pawn.Position).ToVector3();
			dir.x = -dir.x;
			var angle = Vector3.SignedAngle(dir, Vector3.back, Vector3.up);
			GenPlace.TryPlaceThing(sk, x.Cell, map, ThingPlaceMode.Near, null, null, default);
			Find.LetterStack.ReceiveLetter("Bombing", "Bombing", LetterDefOf.NegativeEvent, new TargetInfo(x.Cell, map));
			var compBomber = sk as ShuttleBomber;
			compBomber.faction = pawn.Faction; 
			compBomber.shells = workerSettings.shells;
			compBomber.shellsCount = workerSettings.shellsCount;
			compBomber.bombingRunAroundTargetMeters = workerSettings.bombingRunAroundTargetMeters;
			compBomber.targetMap = map;
			compBomber.angle = angle;
		}

        public override void DoPermitCast(Pawn caster, Map map, List<LocalTargetInfo> targets)
        {
            base.DoPermitCast(caster, map, targets);
			DoBombing(caster, map, targets.First());
		}
    }
}
