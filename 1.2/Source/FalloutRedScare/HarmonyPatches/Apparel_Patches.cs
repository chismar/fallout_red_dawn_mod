using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;


namespace FalloutRedScare
{

    internal static class Apparel_Patches
    {
        [HarmonyPatch(typeof(ApparelUtility), nameof(ApparelUtility.HasPartsToWear))]
        private static class FactionForCombatGroup_Patch
        {
            public static bool Prefix(Pawn __0, ThingDef __1, ref bool __result)
            {
                for(int i =0; i < __1.comps.Count; i++)
                {
                    if(__1.comps[i] is CompProperties_ApparelComp_RestrictBodyType restrict)
                    {
                        if(restrict.restrictBodyTypes != null)
                        for (int j = 0; j < restrict.restrictBodyTypes.Count;j++)
                        {
                            if(__0.story.bodyType == restrict.restrictBodyTypes[j])
                                {
                                    __result = false;
                                    return false;
                                }
                        }
                    }
                }
                return true;
            }
        }

		[HarmonyPatch(typeof(CompShuttle))]
		[HarmonyPatch(nameof(CompShuttle.Send))]
		public static class CompShuttleFixFrs
        {
            static ThingDef shuttleLeaving;
            [HarmonyPrefix]
            public static void Prefix(CompShuttle __instance)
            {
                if(__instance is FRS_Shuttle s)
                {
                    shuttleLeaving = ThingDefOf.ShuttleLeaving;
                    ThingDefOf.ShuttleLeaving = (s.props as CompProperties_FRS_Shuttle).skyfallerLeaving;
                }

            }

            [HarmonyPostfix]
            public static void Postfix(CompShuttle __instance)
            {
                if (__instance is FRS_Shuttle s)
                {
                    ThingDefOf.ShuttleLeaving = shuttleLeaving;
                }
            }
        }

	}
}