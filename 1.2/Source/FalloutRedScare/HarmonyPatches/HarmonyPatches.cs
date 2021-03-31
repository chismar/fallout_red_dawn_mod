using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RedScare
{
    [StaticConstructorOnStartup]
    internal static class HarmonyInit
    {
        static HarmonyInit()
        {
            Debug.Log("PATCH RED SCARE");
            new Harmony("Chismar.RedScare").PatchAll();
            
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
