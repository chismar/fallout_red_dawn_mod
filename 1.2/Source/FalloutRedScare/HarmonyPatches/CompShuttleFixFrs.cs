using HarmonyLib;
using RimWorld;
using Verse;


namespace RedScare
{

    internal static partial class Apparel_Patches
    {
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