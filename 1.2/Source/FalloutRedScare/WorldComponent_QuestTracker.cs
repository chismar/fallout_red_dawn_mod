using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RedScare
{
    public class WorldComponent_QuestTracker : WorldComponent
    {
        public static WorldComponent_QuestTracker Instance;
        public Dictionary<Quest, Faction> storedQuests = new Dictionary<Quest, Faction>();

        public ThingDef vanillaShuttle;
        public ThingDef vanillaShuttleIncoming;
        public ThingDef vanillaShuttleLeaving;
        public ThingDef vanillaShuttleCrashing;
        public ThingDef vanillaShuttleCrashed;
        public WorldComponent_QuestTracker(World world) : base(world)
        {
            Init();
        }

        public override void FinalizeInit()
        {
            base.FinalizeInit();
            Init();
        }

        void Init()
        {
            Instance = this;
            if (storedQuests is null)
            {
                storedQuests = new Dictionary<Quest, Faction>();
            }
            if (vanillaShuttle is null)
            {
                vanillaShuttle = ThingDefOf.Shuttle;
                vanillaShuttleIncoming = ThingDefOf.ShuttleIncoming;
                vanillaShuttleLeaving = ThingDefOf.ShuttleLeaving;
                vanillaShuttleCrashing = ThingDefOf.ShuttleCrashing;
                vanillaShuttleCrashed = ThingDefOf.ShuttleCrashed;
            }
        }

        public bool NeedToReplaceShuttle(Quest quest, out FactionModExtension factionModExtension)
        {
            if (storedQuests.TryGetValue(quest, out var faction))
            {
                factionModExtension = faction.def.GetModExtension<FactionModExtension>();
                return factionModExtension != null;
            }
            factionModExtension = null;
            return false;
        }

        public void ReplaceShuttle(FactionModExtension factionModExtension)
        {
            if (factionModExtension.customShuttle != null)
            {
                ThingDefOf.Shuttle = factionModExtension.customShuttle;
            }
            if (factionModExtension.customShuttleIncoming != null)
            {
                ThingDefOf.ShuttleIncoming = factionModExtension.customShuttleIncoming;
            }
            if (factionModExtension.customShuttleLeaving != null)
            {
                ThingDefOf.ShuttleLeaving = factionModExtension.customShuttleIncoming;
            }
            if (factionModExtension.customShuttleCrashing != null)
            {
                ThingDefOf.ShuttleCrashing = factionModExtension.customShuttleCrashing;
            }
            if (factionModExtension.customShuttleCrashed != null)
            {
                ThingDefOf.ShuttleCrashed = factionModExtension.customShuttleCrashed;
            }
        }

        public void RestoreShuttle()
        {
            ThingDefOf.Shuttle = vanillaShuttle;
            ThingDefOf.ShuttleIncoming = vanillaShuttleIncoming;
            ThingDefOf.ShuttleLeaving = vanillaShuttleLeaving;
            ThingDefOf.ShuttleCrashing = vanillaShuttleCrashing;
            ThingDefOf.ShuttleCrashed = vanillaShuttleCrashed;
        }
        public override void ExposeData()
        {
            Scribe_Collections.Look(ref storedQuests, "storedQuests", LookMode.Reference, LookMode.Reference, ref questKeys, ref factionValues);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                Init();
            }
        }

        private List<Quest> questKeys;
        private List<Faction> factionValues;
    }
}