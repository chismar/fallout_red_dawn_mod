using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace FalloutRedScare
{
    [StaticConstructorOnStartup]
    internal static class HarmonyInit
    {
        static HarmonyInit()
        {
            new Harmony("Chismar.FalloutRedScare").PatchAll();
            
        }
    }

    [HarmonyPatch(typeof(Pawn), "Kill")]
    internal static class Patch_Pawn_Kill
    {
        private static bool Prefix(Pawn __instance)
        {
            var faction = __instance.Faction;
            if (faction != null && faction != Faction.OfPlayer && TotalWarUtils.TryGetFactionWarData(faction, out FactionWar factionWar))
            {
                factionWar.points -= __instance.kindDef.combatPower * factionWar.def.powerPointsLossPerPawnCombatPowerRatio;
            }
            return true;
        }
    }
}
