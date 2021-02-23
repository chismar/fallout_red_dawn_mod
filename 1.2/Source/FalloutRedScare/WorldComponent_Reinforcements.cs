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
    public class WorldComponent_Reinforcements : WorldComponent
    {
        public List<Reinforcements> reinforcements = new List<Reinforcements>();
        public WorldComponent_Reinforcements(World world) : base(world)
        {

        }

        public override void FinalizeInit()
        {
            base.FinalizeInit();
            foreach (var reinforcementsDef in DefDatabase<ReinforcementsDef>.AllDefs)
            {
                reinforcements.Add(new Reinforcements(reinforcementsDef));
            }
        }

        public override void WorldComponentTick()
        {
            base.WorldComponentTick();
            foreach (var reinforcement in reinforcements)
            {
                reinforcement.Tick();
            }
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref reinforcements, "reinforcements", LookMode.Deep);
        }
    }
}