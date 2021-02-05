using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using VFE.Mechanoids;
using VFE.Mechanoids.HarmonyPatches;
using VFE.Mechanoids.Needs;

namespace FalloutRedScare
{
    [HarmonyPatch(typeof(CompMachine), nameof(CompMachine.PostSpawnSetup))]
    public static class FixPowerInMachine
    {
        public static void Prefix(ref CompMachine __instance, out float __state)
        {
            __state = (__instance.parent as Pawn).needs.TryGetNeed<Need_Power>()?.CurLevel ?? 0;
        }
        public static void Postfix(ref CompMachine __instance, float __state)
        {
            (__instance.parent as Pawn).needs.TryGetNeed<Need_Power>().CurLevel = __state;
        }
    }
    [HarmonyPatch(typeof(Need_Power), nameof(Need_Power.NeedInterval))]
    public static class NeedPowerFixNullRefOnMissingChargingPad
    {
        public static bool Prefix(ref Need_Power __instance)
        {
            if (__instance.ChargingStationComp == null)
            {
                __instance.CurLevel = __instance.MaxLevel;
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(CaravanUIUtility), nameof(CaravanUIUtility.AddPawnsSections))]
    public static class AddWidgetForLongtermDrones
    {
        public static void Postfix(TransferableOneWayWidget widget, List<TransferableOneWay> transferables)
        {
            IEnumerable<TransferableOneWay> source = from x in transferables
                                                     where x.ThingDef.category == ThingCategory.Pawn
                                                     select x;
            widget.AddSection("MechanoidsSection".Translate(), from x in source
                                                            where (((Pawn)x.AnyThing).GetComp<CompMachine>()?.Props.hoursActive ?? 0) >= 24000 
                                                            select x);
        }
    }
}
