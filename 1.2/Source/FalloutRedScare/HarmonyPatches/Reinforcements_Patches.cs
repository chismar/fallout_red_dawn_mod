using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;


namespace RedScare
{

    internal static class Reinforcements_Patches
    {
        [HarmonyPatch(typeof(PawnGroupMakerUtility), nameof(PawnGroupMakerUtility.MaxPawnCost))]
        private static class RaidEnemyResolveFaction_Patch
        {
            [HarmonyPrefix]
            public static bool Prefix(Faction __0, float __1, ref float __result)
            {
                if(__0.def.maxPawnCostPerTotalPointsCurve == null)
                {
                    __result = __1;
                    return false;
                }
                return true;
            }
        }
    }
}