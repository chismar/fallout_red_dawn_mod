using HarmonyLib;
using RimWorld;
using Verse;


namespace FalloutRedScare
{

    internal static partial class Apparel_Patches
    {
        [HarmonyPatch(typeof(RoyalTitlePermitWorker_CallShuttle))]
		[HarmonyPatch(nameof(RoyalTitlePermitWorker_CallShuttle.OrderForceTarget))]
		public static class RoyalTitlePermitWorker_CallShuttleFix
        {
            static QuestScriptDef shuttleQuest;
            [HarmonyPrefix]
            public static void Prefix(RoyalTitlePermitWorker_CallShuttle __instance)
            {
                if(__instance is CallSpecifiedShuttle s)
                {
                    shuttleQuest = QuestScriptDefOf.Permit_CallShuttle;
                    QuestScriptDefOf.Permit_CallShuttle = ((s.def as FRSRoyalTitlePermitDef).workerSettings as CallSpecifiedShuttleSettings).shuttleQuestDef;
                }

            }

            [HarmonyPostfix]
            public static void Postfix(RoyalTitlePermitWorker_CallShuttle __instance)
            {
                if (__instance is CallSpecifiedShuttle s)
                {
                    QuestScriptDefOf.Permit_CallShuttle = shuttleQuest;
                }
            }
        }

	}
}