using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;


namespace FalloutRedScare
{

    internal static class Raid_Patches
    {
        [HarmonyPatch]
        private static class FactionForCombatGroup_Patch
        {
            [HarmonyTargetMethod]
            public static MethodBase TargetMethod() =>
                typeof(PawnGroupMakerUtility).GetNestedTypes(AccessTools.all)
                                          .First(t => t.GetMethods(AccessTools.all).Any(mi => mi.Name.Contains(nameof(PawnGroupMakerUtility.TryGetRandomFactionForCombatPawnGroup)) &&
                                                                                              mi.GetParameters()[0].ParameterType == typeof(Faction))).GetMethods(AccessTools.all)
                                          .First(mi => mi.ReturnType == typeof(bool));

            [HarmonyPrefix]
            public static bool Prefix(Faction f, ref bool __result)
            {
                if (f != null && Find.World.GetComponent<WorldComponent_TotalWar>().factions.TryGetValue(f, out var fw))
                    if (fw.points <= fw.def.powerPointsPerBase)
                    {
                        Log.Message($"FactionForCombatGroup_Patch false");
                        __result = false;
                        return false;
                    }

                Log.Message($"FactionForCombatGroup_Patch true");
                return true;
            }
        }

        [HarmonyPatch(typeof(IncidentWorker_RaidFriendly), "TryResolveRaidFaction")]
        private static class RaidFriendlyResolveFaction_Patch
        {
            [HarmonyPrefix]
            public static void Prefix(ref IncidentParms parms)
            {
                RaidEnemyResolveFaction_Patch.Prefix(ref parms);
            }

            [HarmonyPostfix]
            public static void Postfix(ref IncidentParms parms)
            { 
                RaidEnemyResolveFaction_Patch.Postfix(ref parms);
            }
        }
        [HarmonyPatch(typeof(IncidentWorker_RaidEnemy), "TryResolveRaidFaction")]
        private static class RaidEnemyResolveFaction_Patch
        {
            [HarmonyPrefix]
            public static void Prefix(ref IncidentParms parms)
            {
                if (Settings.prUsesWealthForRaids)
                    return;
                if (parms.faction is null)
                    return;
                if (parms.faction != null && Find.World.GetComponent<WorldComponent_TotalWar>().factions.TryGetValue(parms.faction, out var fw))
                {
                    var points = fw.FactionBases.Count * fw.def.raidPointsPerBase;
                    if (points == 0)
                    {
                        Log.Message($"RaidEnemyResolveFaction_Patch null");
                        parms.faction = null;
                    }

                }
                Log.Message($"RaidEnemyResolveFaction_Patch Prefix ");
            }

            [HarmonyPostfix]
            public static void Postfix(ref IncidentParms parms)
            {
                if (Settings.prUsesWealthForRaids)
                    return;
                if (parms.faction != null && Find.World.GetComponent<WorldComponent_TotalWar>().factions.TryGetValue(parms.faction, out var fw))
                {
                    var points = fw.FactionBases.Count * fw.def.raidPointsPerBase;
                    if (points > 0)
                    {
                        Log.Message($"RaidEnemyResolveFaction_Patch {fw.points}");
                        parms.points = points;
                    }
                }
                Log.Message($"RaidEnemyResolveFaction_Patch Postfix {parms.faction?.Name}");
            }
        }

        [HarmonyPatch(typeof(PawnGroupMaker), nameof(PawnGroupMaker.CanGenerateFrom))]
        private static class GetRandomPawnGroupMaker_Patch
        {
            [HarmonyPostfix]
            public static void Postfix(PawnGroupMaker __instance, PawnGroupMakerParms parms, ref bool __result)
            {
                if (Settings.prUsesWealthForRaids)
                    return;
                Log.Message($"GetRandomPawnGroupMaker_Patch {__instance is PawnGroupMakerPR}");
                if (__instance is PawnGroupMakerPR pgmm)
                    __result &= pgmm.CanGenerate(parms);
            }
        }
    }
}