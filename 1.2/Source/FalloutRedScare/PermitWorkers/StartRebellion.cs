using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI.Group;

namespace RedScare
{
	public class RebellionSettings
	{
		public string rebellionCommandKey;
		public string rebellionNoPrisonersAvailableKey;
		public List<ThingDef> weaponsToGivePrisoners;
	}

	public class StartRebellion : FRS_ScriptedTitlePermitWorker<RebellionSettings>
	{
		public override IEnumerable<FloatMenuOption> GetRoyalAidOptions(Map map, Pawn pawn, Faction faction)
		{
			if (AidDisabled(map, pawn, faction, out string reason))
			{
				yield return new FloatMenuOption(def.LabelCap + ": " + reason, null);
				yield break;
			}
			if (!pawn.Map.mapPawns.AllPawns.Where(x => x.IsPrisoner).Any())
            {
				yield return new FloatMenuOption(def.LabelCap + ": " + workerSettings.rebellionNoPrisonersAvailableKey.Translate(), null);
				yield break;
			}
			Action action = null;
			string description = def.LabelCap + " (" + workerSettings.rebellionCommandKey.Translate() + "): ";
			if (FillAidOption(pawn, faction, ref description, out bool free))
			{
				action = delegate
				{
					DoRebellion(pawn, pawn.Map.mapPawns.AllPawns.Where(x => x.IsPrisoner && x.guest.HostFaction != pawn.Faction).ToList());
				};
			}
			yield return new FloatMenuOption(description, action, faction.def.FactionIcon, faction.Color);
		}
		private void DoRebellion(Pawn caster, List<Pawn> prisoners)
		{
			foreach (var prisoner in prisoners)
            {
				prisoner.SetFaction(caster.Faction);
				var weaponDef = workerSettings.weaponsToGivePrisoners.RandomElement();
				var weapon = ThingMaker.MakeThing(weaponDef);
				prisoner.equipment.AddEquipment(weapon as ThingWithComps);
				Log.Message(caster + " - DoRebellion: changing faction of " + prisoner + " to " + caster.Faction);

            }
			LordMaker.MakeNewLord(caster.Faction, MakeLordJob(prisoners.RandomElement().Position, caster.Faction, caster.Map), caster.Map, prisoners);
		}

		protected LordJob MakeLordJob(IntVec3 cell, Faction faction, Map map)
		{
			if (faction.HostileTo(Faction.OfPlayer))
			{
				return new LordJob_AssaultColony(faction);
			}
			return new LordJob_AssistColony(faction, cell);
		}

		public override float CombatScore(Pawn caster, Map map, FactionPermit permit, out List<LocalTargetInfo> targets)
		{
			targets = null;
			var prisoners = map.mapPawns.AllPawns.Where(x => x.IsPrisoner && x.guest?.HostFaction != caster.Faction);
			if (prisoners.Any())
			{
				return 1f;
			}
			return 0f;
		}

        public override void DoPermitCast(Pawn caster, Map map, List<LocalTargetInfo> targets)
        {
            base.DoPermitCast(caster, map, targets);
			DoRebellion(caster, targets.Select(x => x.Thing).Cast<Pawn>().ToList());
		}
    }
}
