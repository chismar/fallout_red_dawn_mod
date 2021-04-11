using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RedScare
{
    internal class JobGiver_AIPermit : ThinkNode_JobGiver
    {
        struct Score
        {
            public FRS_TitlePermitWorker worker;
            public List<LocalTargetInfo> targets;
            public float score;

            public Score(float score, FRS_TitlePermitWorker worker, List<LocalTargetInfo> targets) : this()
            {
                this.score = score;
                this.worker = worker;
                this.targets = targets;
            }
        }
        Dictionary<FactionPermit, Score> scores = new Dictionary<FactionPermit, Score>();
        Dictionary<FactionPermit, FRS_TitlePermitWorker> permits = new Dictionary<FactionPermit, FRS_TitlePermitWorker>();
        protected override Job TryGiveJob(Pawn pawn)
        {
            if (TotalWarUtils.TryGetFactionWarData(pawn.Faction, out var fw))
            {
                if (!fw.CanUsePermit())
                    return null;
            }
            permits.Clear();
            foreach (var permit in pawn.royalty.AllFactionPermits)
            {
                if (!permit.OnCooldown && permit.Permit.Worker is FRS_TitlePermitWorker worker)
                {
                    permits[permit] = worker;
                }
            }
            if (permits.Any())
            {
                scores.Clear();
                foreach (var permit in permits)
                {
                    var score = permit.Value.CombatScore(pawn, pawn.Map, permit.Key, out List<LocalTargetInfo> targets);
                    if (score > 0f)
                    {
                        scores[permit.Key] = new Score(score, permit.Value, targets);
                    }
                }
                if (scores.Any())
                {
                    var chosenPermit = scores.RandomElementByWeight(x => x.Value.score);
                    chosenPermit.Value.worker.DoPermitCast(pawn, pawn.Map, chosenPermit.Value.targets);
                    chosenPermit.Key.Notify_Used();
                    fw?.UsePermit();
                    Log.Message(pawn + " - " + pawn.kindDef + " is using " + chosenPermit.Key.Permit);
                }
            }
            return null;
        }
    }
}