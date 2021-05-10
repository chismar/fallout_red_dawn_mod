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
    public class HediffCompProperties_HediffActivationAIActivable : HediffCompProperties
    {

    }
    public abstract class HediffComp_AIActivable : HediffComp
    {
        public abstract float GetWeightAI();
    }

    public class HediffCompStealthAIActivable : HediffComp_AIActivable
    {
        public override float GetWeightAI()
        {
            List<IAttackTarget> potentialTargetsFor = Pawn.Map.attackTargetsCache.GetPotentialTargetsFor(Pawn);
            for (int i = 0; i < potentialTargetsFor.Count; i++)
            {
                if (GenHostility.IsActiveThreatTo(potentialTargetsFor[i], Pawn.Faction))
                {
                    if (potentialTargetsFor[i].Thing.Position.DistanceTo(Pawn.Position) <= 40f)
                    {
                        return 1f;
                    }
                }
            }
            return 0f;
        }
    }
}