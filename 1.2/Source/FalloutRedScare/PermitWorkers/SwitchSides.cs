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
	public class SwitchSidesSettings
	{
		public string switchSidesCommandKey;
		public float radiusToSwitch;
		public ThoughtDef giveThoughtToPlayerPawns;
	}
	public class SwitchSides : FRS_ScriptedTitlePermitWorker<SwitchSidesSettings>
	{
		public override IEnumerable<FloatMenuOption> GetRoyalAidOptions(Map map, Pawn pawn, Faction faction)
		{
			if (AidDisabled(map, pawn, faction, out string reason))
			{
				yield return new FloatMenuOption(def.LabelCap + ": " + reason, null);
				yield break;
			}
			Action action = null;
			string description = def.LabelCap + " (" + workerSettings.switchSidesCommandKey.Translate() + "): ";
			if (FillAidOption(pawn, faction, ref description, out bool free))
			{
				action = delegate
				{
					Find.Targeter.BeginTargeting(ForLoc(), delegate (LocalTargetInfo x)
					{
						DoSwitchSides(pawn, x.Cell);
					}, (LocalTargetInfo x) => GenDraw.DrawRadiusRing(x.Cell, workerSettings.radiusToSwitch), null, null);
				};
			}
			yield return new FloatMenuOption(description, action, faction.def.FactionIcon, faction.Color);
		}
		private void DoSwitchSides(Pawn caster, IntVec3 cell)
		{
			var pawnsToSwitchSide = caster.Map.mapPawns.AllPawns.Where(x => x.RaceProps.Humanlike && 
				x.Faction != caster.Faction && x.Position.DistanceTo(cell) <= workerSettings.radiusToSwitch).ToList();
			if (caster.Faction != Faction.OfPlayer)
            {
				var playerPawns = pawnsToSwitchSide.Where(x => x.Faction == Faction.OfPlayer);
				pawnsToSwitchSide.RemoveAll(x => playerPawns.Contains(x));
				foreach (var pawn in playerPawns)
                {
					pawn.needs.mood.thoughts.memories.TryGainMemory(workerSettings.giveThoughtToPlayerPawns);
                }
            }
			var lord = caster.GetLord();
			foreach (var pawn in pawnsToSwitchSide)
            {
				pawn.SetFaction(Find.FactionManager.FirstFactionOfDef(def.faction));
				if (lord != null)
                {
					lord.AddPawn(pawn);
                }
            }
		}
	}
}
