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
        public override float CombatScore(Pawn caster, Map map, FactionPermit permit, out List<LocalTargetInfo> targets)
        {
            targets = null;
            var candidates = caster.Map.mapPawns.AllPawns.Where(x => x.RaceProps.Humanlike && x.Faction != caster.Faction).OrderBy(x => x.Position.DistanceTo(caster.Position)).ToList();
            if (candidates.Any())
            {
                var firstPawn = candidates.First();
                candidates.Remove(firstPawn);
                candidates = candidates.Where(x => x.Position.DistanceTo(firstPawn.Position) <= workerSettings.radiusToSwitch).ToList();
                var cells = GenRadial.RadialCellsAround(firstPawn.Position, workerSettings.radiusToSwitch, true);

                Predicate<IntVec3> validator = delegate (IntVec3 c)
                {
                    foreach (var pawn in candidates)
                    {
                        if (c.DistanceTo(pawn.Position) > workerSettings.radiusToSwitch)
                        {
                            return false;
                        }
                    }
                    return true;
                };
                var targetCell = cells.First(x => validator(x));
                Log.Message("targetCell: " + targetCell);
                targets = new List<LocalTargetInfo> { targetCell };
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
            var pawnsToSwitchSide = caster.Map.mapPawns.AllPawns.Where(x => x.RaceProps.Humanlike 
            && x.Faction != caster.Faction && x.Position.DistanceTo(cell) <= workerSettings.radiusToSwitch).ToList();
            Log.Message($"targetCell: {cell}, pawnsToSwitchSide: {pawnsToSwitchSide.Count}");
            if (caster.Faction != Faction.OfPlayer)
            {
                var playerPawns = pawnsToSwitchSide.Where(x => x.Faction == Faction.OfPlayer);
                foreach (var pawn in playerPawns)
                {
                    pawn.needs.mood.thoughts.memories.TryGainMemory(workerSettings.giveThoughtToPlayerPawns);
                    Log.Message($"DoSwitchSides: giving a thought {workerSettings.giveThoughtToPlayerPawns} to {pawn}");
                }
                pawnsToSwitchSide.RemoveAll(x => playerPawns.Contains(x));
            }
            var lord = caster.GetLord();
            foreach (var pawn in pawnsToSwitchSide)
            {
                pawn.SetFaction(Find.FactionManager.FirstFactionOfDef(def.faction));
                Log.Message($"DoSwitchSides: switching pawn's {pawn} faction to {Find.FactionManager.FirstFactionOfDef(def.faction)}");

                if (lord != null)
                {
                    lord.AddPawn(pawn);
                }
            }
        }

        public override void DoPermitCast(Pawn caster, Map map, List<LocalTargetInfo> targets)
        {
            base.DoPermitCast(caster, map, targets);
            DoSwitchSides(caster, targets.First().Cell);
        }
    }
}