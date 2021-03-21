using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;


namespace RedScare
{

    internal static partial class Apparel_Patches
    {
        [HarmonyPatch(typeof(ApparelUtility), nameof(ApparelUtility.HasPartsToWear))]
        private static class FactionForCombatGroup_Patch
        {
            public static bool Prefix(Pawn __0, ThingDef __1, ref bool __result)
            {
                for (int i = 0; i < __1.comps.Count; i++)
                {
                    if (__1.comps[i] is CompProperties_ApparelComp_RestrictBodyType restrict)
                    {
                        if (restrict.restrictBodyTypes != null)
                            for (int j = 0; j < restrict.restrictBodyTypes.Count; j++)
                            {
                                if (__0.story.bodyType == restrict.restrictBodyTypes[j])
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


    internal static partial class Weapons_Patches
    {
        [HarmonyPatch(typeof(PawnRenderer), nameof(PawnRenderer.DrawEquipmentAiming))]
        private static class FactionForCombatGroup_Patch
        {
            public static bool Prefix(Thing eq, Vector3 drawLoc, float aimAngle)
            {
                float num = aimAngle - 90f;
                Mesh mesh;
                if (aimAngle > 20f && aimAngle < 160f)
                {
                    mesh = MeshPool.plane10;
                    num += eq.def.equippedAngleOffset;
                }
                else if (aimAngle > 200f && aimAngle < 340f)
                {
                    mesh = MeshPool.plane10Flip;
                    num -= 180f;
                    num -= eq.def.equippedAngleOffset;
                }
                else
                {
                    mesh = MeshPool.plane10;
                    num += eq.def.equippedAngleOffset;
                }
                num %= 360f;
                Graphic_StackCount graphic_StackCount = eq.Graphic as Graphic_StackCount;
                Material matSingle;
                if (graphic_StackCount != null)
                {
                    matSingle = graphic_StackCount.SubGraphicForStackCount(1, eq.def).MatSingle;
                }
                else
                {
                    matSingle = eq.Graphic.MatSingle;
                }
                Graphics.DrawMesh(mesh, Matrix4x4.TRS(drawLoc, Quaternion.AngleAxis(num, Vector3.up), new Vector3(eq.Graphic.drawSize.x, 1, eq.Graphic.drawSize.y)), matSingle, 0);
                return false;
            }
        }

    }
}