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
        protected override Job TryGiveJob(Pawn pawn)
        {
            var permits = new Dictionary<FactionPermit, FRS_TitlePermitWorker>();
            foreach (var permit in pawn.royalty.AllFactionPermits)
            {
                if (!permit.OnCooldown && permit.Permit.Worker is FRS_TitlePermitWorker worker)
                {
                    permits[permit] = worker;
                }
            }
            if (permits.Any())
            {
                var scores = new Dictionary<FactionPermit, Tuple<float, FRS_TitlePermitWorker, List<LocalTargetInfo>>>();
                foreach (var permit in permits)
                {
                    var score = permit.Value.CombatScore(pawn, pawn.Map, permit.Key, out List<LocalTargetInfo> targets);
                    if (score > 0f)
                    {
                        scores[permit.Key] = new Tuple<float, FRS_TitlePermitWorker, List<LocalTargetInfo>>(score, permit.Value, targets);
                    }
                }
                if (scores.Any())
                {
                    var chosenPermit = scores.RandomElementByWeight(x => x.Value.Item1);
                    chosenPermit.Value.Item2.DoPermitCast(pawn, pawn.Map, chosenPermit.Value.Item3);
                    chosenPermit.Key.Notify_Used();
                    Log.Message(pawn + " - " + pawn.kindDef + " is using " + chosenPermit.Key.Permit);
                }
            }
            return null;
        }
    }
}