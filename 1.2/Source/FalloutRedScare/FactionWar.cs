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
    public class FactionWar : IExposable
    {
        public Faction faction;
        public TotalWarDef def;
        public float points;
        public int curScapeGoatCooldownTicks;
        public int curBaseIncidentsCooldown;
        public bool CanSpawnScapeGoatQuest => Find.TickManager.TicksGame >= curScapeGoatCooldownTicks && faction.HostileTo(Faction.OfPlayer);
        public IEnumerable<Settlement> FactionBases => Find.WorldObjects.SettlementBases.Where(x => x.Faction == faction);
        public bool CanSpawnBases => Find.TickManager.TicksGame >= curBaseIncidentsCooldown && points > (FactionBases.Count() + 1) * this.def.powerPointsPerBase;
        public bool CanAbandonBase => Find.TickManager.TicksGame >= curBaseIncidentsCooldown && points < FactionBases.Count() * this.def.powerPointsPerBase;
        public FactionWar()
        {

        }

        public FactionWar(Faction faction, TotalWarDef def)
        {
            this.faction = faction;
            this.def = def;
            //this.points = FactionBases.Count() * this.def.powerPointsPerBase;
        }
        public void ExposeData()
        {
            Scribe_Defs.Look(ref def, nameof(def));
            Scribe_Values.Look(ref points, nameof(points));
            Scribe_Values.Look(ref curScapeGoatCooldownTicks, nameof(curScapeGoatCooldownTicks));
            Scribe_Values.Look(ref curBaseIncidentsCooldown, nameof(curBaseIncidentsCooldown));
            Scribe_References.Look(ref faction, nameof(faction));
        }

        public void Tick()
        {
            if (Find.TickManager.TicksGame % 60 == 0)
            {
                //Log.Message($"Faction: {faction}, points: {points}, def: {def}, curBaseIncidentsCooldown: {curBaseIncidentsCooldown}, curScapeGoatCooldownTicks: {curScapeGoatCooldownTicks}" +
                //    $", CanSpawnBases: {CanSpawnBases}, CanAbandonBase: {CanAbandonBase}");
            }
            if (CanSpawnScapeGoatQuest)
            {
                Quest quest = QuestUtility.GenerateQuestAndMakeAvailable(this.def.scapegoatQuestScript, 0);
                QuestUtility.SendLetterQuestAvailable(quest);
                curScapeGoatCooldownTicks = (int)(Find.TickManager.TicksGame + (GenDate.TicksPerDay * this.def.scapegoatCooldownDays.RandomInRange));
            }
            if (CanSpawnBases)
            {
                var parms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.Misc, Find.World);
                parms.faction = this.faction;
                var randomSpawnIncident = this.def.factionBaseSpawnIncidents.RandomElement();
                if (randomSpawnIncident.Worker.CanFireNow(parms, true) && randomSpawnIncident.Worker.TryExecute(parms))
                {
                    curBaseIncidentsCooldown = Find.TickManager.TicksGame + (int)(GenDate.TicksPerDay * this.def.baseIncidentsCooldownDays.RandomInRange);
                }
            }
            else if (CanAbandonBase)
            {
                var parms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.Misc, Find.World);
                parms.faction = this.faction;
                var abandonBaseIncident = this.def.factionBaseAbandonIncident;
                if (abandonBaseIncident.Worker.CanFireNow(parms, true) && abandonBaseIncident.Worker.TryExecute(parms))
                {
                    curBaseIncidentsCooldown = Find.TickManager.TicksGame + (int)(GenDate.TicksPerDay * this.def.baseIncidentsCooldownDays.RandomInRange);
                }
            }

            this.points += this.def.powerPointPassiveGainPerDay / GenDate.TicksPerDay;
            
        }
    }
}