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
    public class WorldComponent_TotalWar : WorldComponent
    {
        public Dictionary<Faction, FactionWar> factions = new Dictionary<Faction, FactionWar>();
        public WorldComponent_TotalWar(World world) : base(world)
        {
            Init();

        }

        public override void FinalizeInit()
        {
            base.FinalizeInit();
        }

        void Init()
        {

            foreach (var totalWarDef in DefDatabase<TotalWarDef>.AllDefs)
            {
                Log.Message($"{totalWarDef.defName}");

                foreach (var faction in Find.FactionManager.AllFactions.Where(x => !x.IsPlayer))
                {
                    if (totalWarDef.factionDef == faction.def && !factions.ContainsKey(faction))
                    {
                        RegisterFaction(faction, totalWarDef);
                    }
                }
            }
        }

        public void RegisterFaction(Faction faction, TotalWarDef totalWarDef)
        {
            if (factionKeys == null)
                factionKeys = new List<Faction>();
            if (factionWarValues == null)
                factionWarValues = new List<FactionWar>();


            factionKeys.Add(faction);
            var fw = new FactionWar(faction, totalWarDef);
            factionWarValues.Add(fw);
            factions.Add(faction, fw);
        }
        public override void WorldComponentTick()
        {
            base.WorldComponentTick();
            foreach (var factionData in factions)
            {
                factionData.Value.Tick();
            }
        }
        public override void ExposeData()
        {
            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                factions.Clear();
                factionKeys?.Clear();
                factionWarValues?.Clear();
            }
            base.ExposeData();
            foreach (var f in factions)
            {
                Log.Message($"{f.Key.Name} {f.Value.def.defName}");
            }
            Scribe_Collections.Look(ref factions, "factions", LookMode.Reference, LookMode.Deep, ref factionKeys, ref factionWarValues);
            foreach (var f in factions)
            {
                Log.Message($"{f.Key.Name} {f.Value.def.defName}");
            }
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                Init();
            }
        }

        private List<Faction> factionKeys;
        private List<FactionWar> factionWarValues;
    }
}