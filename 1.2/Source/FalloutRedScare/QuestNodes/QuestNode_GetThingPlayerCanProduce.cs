using RimWorld;
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
    public class QuestNode_GetThingPlayerCanProduce : QuestNode
    {
        [NoTranslate]
        public SlateRef<string> storeProductionItemDefAs;

        [NoTranslate]
        public SlateRef<string> storeProductionItemStuffAs;

        [NoTranslate]
        public SlateRef<string> storeProductionItemCountAs;

        [NoTranslate]
        public SlateRef<string> storeProductionItemLabelAs;

        [NoTranslate]
        public SlateRef<FloatRange> totalMarketValuePerPlayerWealth;

        [NoTranslate]
        public SlateRef<List<ThingDef>> thingDefCandidates;

        [NoTranslate]
        public SlateRef<List<ThingDef>> thingStuffCandidates;

        private static List<Pair<ThingStuffPair, int>> tmpCandidates = new List<Pair<ThingStuffPair, int>>();

        protected override bool TestRunInt(Slate slate)
        {
            return DoWork(slate);
        }

        protected override void RunInt()
        {
            DoWork(QuestGen.slate);
        }

        private bool DoWork(Slate slate)
        {
            Map map = slate.Get<Map>("map");
            if (map == null)
            {
                return false;
            }

            tmpCandidates.Clear();
            var totalWealth = map.wealthWatcher.WealthTotal;
            foreach (var candidate in thingDefCandidates.GetValue(slate))
            {
                var goalMarketValue = totalWealth * totalMarketValuePerPlayerWealth.GetValue(slate).RandomInRange;
                var stuffCandidate = GetStuffFor(candidate, slate);
                var thingMarketValue = StatWorker_MarketValue.CalculatedBaseMarketValue(candidate, stuffCandidate);
                var goalThingCount = (int)(goalMarketValue / thingMarketValue);
                Log.Message($"candidate: {candidate} - totalWealth: {totalWealth} - goalMarketValue: {goalMarketValue} - goalThingCount: {goalThingCount} " +
                    $"- stuffCandidate: {stuffCandidate} - thingMarketValue: {thingMarketValue}");
                tmpCandidates.Add(new Pair<ThingStuffPair, int>(new ThingStuffPair(candidate, stuffCandidate), goalThingCount));
            }
            if (!tmpCandidates.Any())
            {
                return false;
            }
            Pair<ThingStuffPair, int> pair = tmpCandidates.RandomElement();
            int num6 = pair.Second;
            num6 = Mathf.Max(num6, 1);
            slate.Set(storeProductionItemDefAs.GetValue(slate), pair.First.thing);
            slate.Set(storeProductionItemStuffAs.GetValue(slate), pair.First.stuff);
            slate.Set(storeProductionItemCountAs.GetValue(slate), num6);
            string value3 = storeProductionItemLabelAs.GetValue(slate);
            if (!string.IsNullOrEmpty(value3))
            {
                slate.Set(value3, GenLabel.ThingLabel(pair.First.thing, pair.First.stuff, num6));
            }
            return true;
        }

        public ThingDef GetStuffFor(ThingDef candidate, Slate slate)
        {
            var stuffCandidates = GenStuff.AllowedStuffsFor(candidate);
            if (stuffCandidates.Any())
            {
                if (thingStuffCandidates.GetValue(slate) != null)
                {
                    stuffCandidates = stuffCandidates.Where(x => thingStuffCandidates.GetValue(slate).Contains(x));
                    if (stuffCandidates.Any())
                    {
                        return stuffCandidates.RandomElement();
                    }
                }
                else
                {
                    return stuffCandidates.RandomElement();
                }
            }
            return null;
        }
    }
}