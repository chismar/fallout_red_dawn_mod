using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace RedScare
{
	public class Verb_UseStealthCompReloadable : Verb, ICanBeCastedByAI
	{
		public CompHediffActivationReloadable Comp => base.ReloadableCompSource as CompHediffActivationReloadable;
		public bool CanBeUsed()
        {
			CompReloadable comp = base.ReloadableCompSource;
			if (comp != null && comp.CanBeUsed)
			{
				return Comp.GetWeightAI() > 0;
			}
			return false;
		}

        public float GetWeight()
        {
			return Comp.GetWeightAI();
		}

        public void TryUseDecideTarget()
        {
			if (this.verbProps.targetable)
            {
				this.currentTarget = FindAttackTarget(CasterPawn);
			}
			else if (!this.verbProps.violent)
            {
				this.currentTarget = CasterPawn;
            }
			if (this.currentTarget.IsValid)
            {
				TryCastShot();
            }
		}

		protected Thing FindAttackTarget(Pawn pawn)
		{
			TargetScanFlags targetScanFlags = TargetScanFlags.NeedLOSToPawns | TargetScanFlags.NeedReachableIfCantHitFromMyPos | TargetScanFlags.NeedThreat | TargetScanFlags.NeedAutoTargetable;
			if (VerbIsIncendiary())
			{
				targetScanFlags |= TargetScanFlags.NeedNonBurning;
			}
			return (Thing)AttackTargetFinder.BestAttackTarget(pawn, targetScanFlags, null, 0f, this.verbProps.range, IntVec3.Invalid, 999999f);
		}

		private bool VerbIsIncendiary()
		{
			if (EquipmentCompSource != null)
			{
				List<Verb> allVerbs = EquipmentCompSource.AllVerbs;
				for (int i = 0; i < allVerbs.Count; i++)
				{
					if (allVerbs[i].verbProps.isPrimary)
					{
						return allVerbs[i].IsIncendiary();
					}
				}
			}
			return false;
		}


		public void UseOn(Thing target)
        {
			this.currentTarget = target;
            TryCastShot();
        }
		protected override bool TryCastShot()
		{
			CompReloadable comp = base.ReloadableCompSource;
			if (comp != null && comp.CanBeUsed)
			{
				comp.UsedOnce();
			}
			return true;
		}
	}
}
