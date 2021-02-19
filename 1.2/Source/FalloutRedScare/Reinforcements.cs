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

namespace FalloutRedScare
{
    public class Reinforcements : IExposable
    {
        public ReinforcementsDef def;
        public int curReinforcementCooldown;
        public bool CanSendReinforcements => Find.TickManager.TicksGame >= curReinforcementCooldown;
        public Reinforcements()
        {

        }

        public Reinforcements(ReinforcementsDef def)
        {
            this.def = def;
        }
        public void ExposeData()
        {
            Scribe_Defs.Look(ref def, nameof(def));
            Scribe_Values.Look(ref curReinforcementCooldown, nameof(curReinforcementCooldown));
        }
        bool disable = false;
        public void Tick()
        {
            if (disable)
                return;
            if(!(def.reinforceOnlyIfPlayerIsOfFaction?.Any(x=>x == Faction.OfPlayer.def) ?? false))
            {
                disable = true;
                return;
            }
            if (Find.TickManager.TicksGame % 60 == 0)
            {
                Log.Message($"Reinforcements {CanSendReinforcements} {Find.TickManager.TicksGame >= curReinforcementCooldown}");
            }
            if (CanSendReinforcements)
            {
                var parms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.Misc, Find.AnyPlayerHomeMap);
                parms.faction = Faction.OfPlayer;
                var randomSpawnIncident = this.def.reinforcementIncidents.RandomElement();
                if (randomSpawnIncident.Worker.CanFireNow(parms, true) && randomSpawnIncident.Worker.TryExecute(parms))
                {
                    if (Settings.overrideReinforcementsCooldown)
                    {
                        this.def.reinforcementCooldownDays = Settings.spawnRange;
                    }
                    curReinforcementCooldown = Find.TickManager.TicksGame + (int)(GenDate.TicksPerDay * this.def.reinforcementCooldownDays.RandomInRange);
                }
            }

            
        }
    }
}