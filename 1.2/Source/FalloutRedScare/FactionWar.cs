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
    public class FactionWar : IExposable
    {

        public int nextPermitUsageTick;

        public bool CanUsePermit()
        {
            return Find.TickManager.TicksGame >= nextPermitUsageTick;
        }
        public void UsePermit()
        {
            nextPermitUsageTick = Find.TickManager.TicksGame + (int)(GenDate.TicksPerDay * this.def.permitCooldownDays.RandomInRange);
        }
        public Faction faction;
        public TotalWarDef def;
        public float points;
        public int curScapeGoatCooldownTicks;
        public int curProductionQuotaCooldownTicks;
        public int curBaseIncidentsCooldown;
        public int cooldownForIntroQuest;
        public bool CanSpawnScapeGoatQuest => Find.TickManager.TicksGame >= curScapeGoatCooldownTicks && faction.HostileTo(Faction.OfPlayer) &&
            !Find.QuestManager.QuestsListForReading.Any(x => x.root != def.scapegoatQuestScript);
        public bool CanSpawnProductionQuota => Find.TickManager.TicksGame >= curProductionQuotaCooldownTicks && faction.RelationWith(Faction.OfPlayer).kind == FactionRelationKind.Ally && !Find.QuestManager.QuestsListForReading.Any(x => x.root == def.productionQuota);
        public List<Settlement> FactionBases = new List<Settlement>();
        public IEnumerable<Settlement> FactionBasesCollect => Find.WorldObjects.SettlementBases.Where(x => x.Faction == faction);
        public bool CanSpawnBases => Find.TickManager.TicksGame >= curBaseIncidentsCooldown && points > (FactionBases.Count + 1) * this.def.powerPointsPerBase;
        public bool CanAbandonBase => Find.TickManager.TicksGame >= curBaseIncidentsCooldown && points < FactionBases.Count * this.def.powerPointsPerBase;
        public FactionWar()
        {

        }

        public FactionWar(Faction faction, TotalWarDef def)
        {
            this.faction = faction;
            this.def = def;
            this.points = FactionBasesCollect.Count() * this.def.powerPointsPerBase + this.def.powerPointsPerBase * 0.1f;
        }
        public void ExposeData()
        {
            Scribe_Defs.Look(ref def, nameof(def));
            Scribe_Values.Look(ref points, nameof(points));
            Scribe_Values.Look(ref curScapeGoatCooldownTicks, nameof(curScapeGoatCooldownTicks));
            Scribe_Values.Look(ref curProductionQuotaCooldownTicks, nameof(curProductionQuotaCooldownTicks));
            Scribe_Values.Look(ref curBaseIncidentsCooldown, nameof(curBaseIncidentsCooldown));
            Scribe_Values.Look(ref nextPermitUsageTick, nameof(nextPermitUsageTick));
            Scribe_Values.Look(ref cooldownForIntroQuest, nameof(cooldownForIntroQuest));
            Scribe_References.Look(ref faction, nameof(faction));

        }

        public void Tick()
        {
            if (Find.TickManager.TicksGame % 60 == 0)
            {
                if (cooldownForIntroQuest == 0)
                    cooldownForIntroQuest = Find.TickManager.TicksGame + (int)(GenDate.TicksPerDay * def.introDelayFromStart.RandomInRange);
                else if (cooldownForIntroQuest > 0)
                    if (Find.TickManager.TicksGame > cooldownForIntroQuest)
                    {
                        Quest quest = QuestUtility.GenerateQuestAndMakeAvailable(this.def.introScript, 0);
                        QuestUtility.SendLetterQuestAvailable(quest);
                        cooldownForIntroQuest = -1;
                    }
                FactionBases.Clear();
                FactionBases.AddRange(FactionBasesCollect);
                Log.Message($"Faction: {faction}, points: {points}, def: {def}, curBaseIncidentsCooldown: {curBaseIncidentsCooldown}, curScapeGoatCooldownTicks: {curScapeGoatCooldownTicks}" +
                    $", CanSpawnBases: {CanSpawnBases}, CanAbandonBase: {CanAbandonBase}");
                if (CanSpawnScapeGoatQuest)
                {
                    Quest quest = QuestUtility.GenerateQuestAndMakeAvailable(this.def.scapegoatQuestScript, 0);
                    QuestUtility.SendLetterQuestAvailable(quest);
                    curScapeGoatCooldownTicks = (int)(Find.TickManager.TicksGame + (GenDate.TicksPerDay * this.def.scapegoatCooldownDays.RandomInRange));
                }
                if (CanSpawnProductionQuota)
                {
                    Quest quest = QuestUtility.GenerateQuestAndMakeAvailable(this.def.productionQuota, 0);
                    QuestUtility.SendLetterQuestAvailable(quest);
                    curProductionQuotaCooldownTicks = (int)(Find.TickManager.TicksGame + (GenDate.TicksPerDay * this.def.productionQuotaCooldownDays.RandomInRange));
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
                FactionBases.Clear();
                FactionBases.AddRange(FactionBasesCollect);
            }
            this.points += this.def.powerPointPassiveGainPerDay / GenDate.TicksPerDay;

        }
    }
}