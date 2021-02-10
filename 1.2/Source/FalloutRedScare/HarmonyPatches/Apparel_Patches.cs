using System.Linq;
using System.Reflection;
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

    }
}