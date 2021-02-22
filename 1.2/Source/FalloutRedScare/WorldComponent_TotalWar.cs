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

        }

        public override void FinalizeInit()
        {
            base.FinalizeInit();
            foreach (var totalWarDef in DefDatabase<TotalWarDef>.AllDefs)
            {
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
            factions[faction] = new FactionWar(faction, totalWarDef);
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
            base.ExposeData();
            Scribe_Collections.Look(ref factions, "factions", LookMode.Reference, LookMode.Deep, ref factionKeys, ref factionWarValues);
        }

        private List<Faction> factionKeys;
        private List<FactionWar> factionWarValues;
    }
}