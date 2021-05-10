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
            this.compClass = typeof(CompHediffActivation_Reloadable);
        }
    }

    public abstract class CompHediffActivation_Reloadable : CompReloadable
	{
        public CompProperties_HediffActivationReloadable Props => this.props as CompProperties_HediffActivationReloadable;

        private int ticksUntilExpired;

        [HarmonyPatch(typeof(CompReloadable), "UsedOnce")]
        internal static class Patch_UsedOnce
        {
            private static void Postfix(CompReloadable __instance)
            {
                if (__instance is CompHediffActivation_Reloadable compHediffActivationReloadable)
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

        public abstract float GetWeightAI();
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref ticksUntilExpired, "ticksUntilExpired");
        }
    }

    public class CompStealthActivation_Reloadable : CompHediffActivation_Reloadable
    {
        public override float GetWeightAI()
        {
            var pawn = this.Wearer;
            var hediff = pawn.health.hediffSet.GetFirstHediffOfDef(Props.hediffDef);
            if (hediff != null)
            {
                return 0f;
            }
            List<IAttackTarget> potentialTargetsFor = this.Wearer.Map.attackTargetsCache.GetPotentialTargetsFor(this.Wearer);
            for (int i = 0; i < potentialTargetsFor.Count; i++)
            {
                if (GenHostility.IsActiveThreatTo(potentialTargetsFor[i], this.Wearer.Faction))
                {
                    if (potentialTargetsFor[i].Thing.Position.DistanceTo(this.Wearer.Position) <= 40f)
                    {
                        return 1f;
                    }
                }
            }
            return 0f;
        }
    }
}