using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace RedScare
{
    [HarmonyPatch(typeof(JobGiver_TakeCombatEnhancingDrug), "TryGiveJob")]
    internal static class TryGiveJob_Patch
    {
        public static void Postfix(Pawn pawn)
        {
            if (pawn.Faction != Faction.OfPlayer)
            {
                var verbs = pawn.VerbTracker.AllVerbs.OfType<ICanBeCastedByAI>();
                if (verbs.Any())
                {
                    var castableVerbs = verbs.Where(x => x.CanBeUsed());
                    if (castableVerbs.Any())
                    {
                        var chosenVerb = verbs.RandomElementByWeight(x => x.GetWeight());
                        chosenVerb.TryUseDecideTarget();
                    }
                }
            }
        }
    }
    [HarmonyPatch(typeof(Pawn), "TryGetAttackVerb")]
    public static class Patch_TryGetAttackVerb
    {
        public static void Postfix(Pawn __instance, Thing target, bool allowManualCastWeapons = false)
        {
            if (__instance.Faction != Faction.OfPlayer)
            {
                var verbs = __instance.VerbTracker.AllVerbs.OfType<ICanBeCastedByAI>();
                if (verbs.Any())
                {
                    var castableVerbs = verbs.Where(x => x.CanBeUsed());
                    if (castableVerbs.Any())
                    {
                        var chosenVerb = verbs.RandomElementByWeight(x => x.GetWeight());
                        chosenVerb.UseOn(target);
                    }
                }
            }
        }
    }
}