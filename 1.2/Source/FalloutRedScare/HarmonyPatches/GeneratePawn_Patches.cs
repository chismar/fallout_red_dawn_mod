using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;


namespace RedScare
{

    internal static class GeneratePawn_Patches
    {
        [HarmonyPatch(typeof(PawnGenerator), "GenerateBodyType_NewTemp")]
        private static class GenerateBodyType_NewTemp_Patch
        {
            [HarmonyPostfix]
            public static void Postfix(Pawn __0, PawnGenerationRequest __1)
            {
                if(__1.KindDef is RestrictedBodyPawnKindDef rbp)
                {
                    __0.story.bodyType = rbp.restrictBodyTypesTo[UnityEngine.Random.Range(0, rbp.restrictBodyTypesTo.Count)];
                }
            }
        }
    }
}