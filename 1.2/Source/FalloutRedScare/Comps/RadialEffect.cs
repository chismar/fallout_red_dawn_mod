using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RedScare
{
    /*internal static partial class CompTickPatch
    {
        [HarmonyPatch(typeof(Pawn_EquipmentTracker), nameof(Pawn_EquipmentTracker.EquipmentTrackerTick))]
        private static class CompTickPatch_Patch
        {
            public static void Prefix(Pawn_EquipmentTracker __instance)
            {
                var list = __instance.AllEquipmentListForReading;
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].Tick();
                }
            }
        }

    }*/
    internal static partial class VerbsPatcher
    {
        [HarmonyPatch(typeof(Verb), nameof(Verb.VerbTick))]
        private static class VerbsPatcher_Patch
        {
            public static void Postfix(Verb __instance)
            {
                if (__instance is TickedVerb tv)
                    tv.OnVerbTick();
            }
        }

    }
    internal static partial class TickedVerbsNotifyDespawnPatch
    {
        [HarmonyPatch(typeof(Pawn_EquipmentTracker), nameof(Pawn_EquipmentTracker.Notify_PawnDied))]
        private static class VerbsPatcher_Patch
        {
            public static void Postfix(Pawn_EquipmentTracker __instance)
            {
                foreach (var verb in __instance.AllEquipmentVerbs)
                    if (verb is TickedVerb tv)
                        tv.NotifyPawnDied();
            }
        }

    }
    public class VerbProperties_RadialEffect : VerbProperties
    {
        public VerbProperties_RadialEffect()
        {
            this.verbClass = typeof(VerbRadialEffect);
        }

        public float severity = 1f;
        public float tickInterval = 1000;
        public bool notOnlyAffectColonists = false;
        public HediffDef hediff;
    }

    public class TickedVerb : Verb
    {
        public virtual void OnVerbTick()
        {

        }

        protected override bool TryCastShot()
        {
            return false;
        }

        public virtual void NotifyPawnDied()
        {
        }
    }
    public class VerbRadialEffect : TickedVerb
    {
        public VerbProperties_RadialEffect Props
        {
            get
            {
                return (VerbProperties_RadialEffect)this.verbProps;
            }
        }
        static List<Thing> _staticListForQuery = new List<Thing>();
        static HashSet<Hediff> _addedThisTick = new HashSet<Hediff>();
        Dictionary<Pawn, Hediff> _previousThings = new Dictionary<Pawn, Hediff>();
        public override void OnVerbTick()
        {
            if (Find.TickManager.TicksGame - lastTick > Props.tickInterval * GenDate.TicksPerHour)
            {
                lastTick = Find.TickManager.TicksGame;
                var pawn = this.CasterPawn;
                if (pawn != null && pawn.Map != null)
                {
                    if (SpatialHashMapComp.HashMapPerMap.TryGetValue(pawn.Map, out var hash))
                    {
                        hash.GetAllAround(pawn.Position.ToIntVec2, Mathf.RoundToInt(this.Props.range), _staticListForQuery);
                        foreach (var thing in _staticListForQuery)
                        {
                            var pawn2 = thing as Pawn;
                            if (pawn2 == null)
                                continue;
                            if (!pawn2.Dead && !pawn2.Downed)
                            {
                                if (!_previousThings.ContainsKey(pawn2))
                                    _addedThisTick.Add(pawn2.health.AddHediff(Props.hediff, null, null, null));
                                else
                                {
                                    _previousThings.Remove(pawn2, out var hediff);
                                    _addedThisTick.Add(hediff);
                                }
                            }
                        }
                    }
                }
                foreach (var prevThing in _previousThings)
                    prevThing.Key.health.RemoveHediff(prevThing.Value);
                _previousThings.Clear();
                foreach (var addedObject in _addedThisTick)
                    _previousThings.Add(addedObject.pawn, addedObject);
                _staticListForQuery.Clear();
                _addedThisTick.Clear();
            }
        }

        public override void NotifyPawnDied()
        {
            Notify_EquipmentLost();
        }

        public override void Notify_EquipmentLost()
        {
            foreach (var prevThing in _previousThings)
                prevThing.Key.health.RemoveHediff(prevThing.Value);
            _previousThings.Clear();
        }

        protected override bool TryCastShot()
        {
            return false;
        }

        public int lastTick = 0;
    }
}
