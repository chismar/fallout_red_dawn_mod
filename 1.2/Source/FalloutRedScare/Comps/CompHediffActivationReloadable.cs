using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Noise;

namespace RedScare
{
    public class CompProperties_HediffActivationReloadable : CompProperties_Reloadable
    {
        public int hediffDurationTicks;
        public HediffDef hediffDef;
        public CompProperties_HediffActivationReloadable()
        {
            this.compClass = typeof(CompHediffActivationReloadable);
        }
    }

    public class CompHediffActivationReloadable : CompReloadable
	{
        public CompProperties_HediffActivationReloadable Props => this.props as CompProperties_HediffActivationReloadable;

        private int ticksUntilExpired;

        [HarmonyPatch(typeof(CompReloadable), "UsedOnce")]
        internal static class Patch_UsedOnce
        {
            private static void Postfix(CompReloadable __instance)
            {
                if (__instance is CompHediffActivationReloadable compHediffActivationReloadable)
                {
                    compHediffActivationReloadable.PostUsedOnce();
                }
            }
        }

        private void PostUsedOnce()
        {
            var hediff = this.Wearer.health.hediffSet.GetFirstHediffOfDef(Props.hediffDef);
            if (hediff is null)
            {
                this.Wearer.health.AddHediff(Props.hediffDef);
                ticksUntilExpired = Props.hediffDurationTicks;
            }
        }

        public override void CompTick()
        {
            base.CompTick();
            ticksUntilExpired--;
            if (ticksUntilExpired <= 0)
            {
                var hediff = this.Wearer.health.hediffSet.GetFirstHediffOfDef(Props.hediffDef);
                if (hediff != null)
                {
                    this.Wearer.health.RemoveHediff(hediff);
                }
            }
        }

        public float GetWeightAI()
        {
            var pawn = this.Wearer;
            var hediff = pawn.health.hediffSet.GetFirstHediffOfDef(Props.hediffDef);
            if (hediff is null)
            {
                return 0f;
            }
            var comp = hediff.TryGetComp<HediffComp_AIActivable>();
            return comp.GetWeightAI();
        }
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref ticksUntilExpired, "ticksUntilExpired");
        }
    }
}