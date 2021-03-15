using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RedScare
{
    internal static partial class CompTickPatch
    {
        [HarmonyPatch(typeof(Pawn_EquipmentTracker), nameof(Pawn_EquipmentTracker.EquipmentTrackerTick))]
        private static class CompTickPatch_Patch
        {
            public static void Prefix(Pawn_EquipmentTracker __instance)
            {
                var list = __instance.AllEquipmentListForReading;
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].Tick();
                }
            }
        }

    }
    public class CompProperties_RadialEffect : CompProperties
    {
        public CompProperties_RadialEffect()
        {
            this.compClass = typeof(CompRadialEffect);
        }

        public int radius = 1;
        public float severity = 1f;
        public float tickInterval = 1000;
        public bool notOnlyAffectColonists = false;
        public HediffDef hediff;
    }
    public class CompRadialEffect : ThingComp
    {
        public CompProperties_RadialEffect Props
        {
            get
            {
                return (CompProperties_RadialEffect)this.props;
            }
        }
        public override void CompTick()
        {

            Log.Message(string.Format("CompTickRare"), false);

            if (Find.TickManager.TicksGame - lastTick > Props.tickInterval * GenDate.TicksPerHour)
            {
                lastTick = Find.TickManager.TicksGame;
                var pawn = parent.ParentHolder.ParentHolder as Pawn;
                Log.Message($"CompTickRare2 {parent?.GetType().Name} {parent?.ParentHolder?.GetType().Name} {parent?.ParentHolder?.ParentHolder?.GetType().Name}", false);
                if (pawn != null && pawn.Map != null)
                {
                    List<Pawn> list = this.Props.notOnlyAffectColonists ? pawn.Map.mapPawns.AllPawns : pawn.Map.mapPawns.FreeColonists;
                    Log.Message($"CompTickRare3 {list.Count}", false);
                    foreach (Pawn pawn2 in list)
                    {
                        bool flag3 = !pawn2.Dead && !pawn2.Downed && pawn.Position.InHorDistOf(pawn2.Position, (float)this.Props.radius);
                        if (flag3)
                        {
                            Log.Message(string.Format("Adding Boastforme hediff to: {0}", pawn2.Name), false);
                            pawn2.health.AddHediff(Props.hediff, null, null, null);
                        }
                    }
                }
            }
        }
        public int lastTick = 0;
    }
}
