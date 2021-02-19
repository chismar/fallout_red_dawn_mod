using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace FalloutRedScare
{

	public class FRSRoyalTitlePermitDef : RoyalTitlePermitDef
	{
		public object workerSettings;
	}
	public abstract class FRS_TitlePermitWorker : RoyalTitlePermitWorker
	{
		public new FRSRoyalTitlePermitDef def => base.def as FRSRoyalTitlePermitDef;
		public abstract float CombatScore(Pawn caster, Map map, FactionPermit permit, out List<LocalTargetInfo> targets);
		public abstract void DoPermitCast(Pawn caster, Map map, List<LocalTargetInfo> targets);

	}

	public class FRS_ScriptedTitlePermitWorker<T> : FRS_TitlePermitWorker where T : class
	{
		public T workerSettings => def.workerSettings as T;
		public static TargetingParameters ForLoc(Pawn caller = null, float? radius = null)
		{
			TargetingParameters targetingParameters = new TargetingParameters();
			targetingParameters.canTargetLocations = true;
			if(radius != null)
				targetingParameters.validator = (TargetInfo target) => target.Cell.DistanceTo(caller.Position) <= radius.Value;
			return targetingParameters;
		}

        public override float CombatScore(Pawn caster, Map map, FactionPermit permit, out List<LocalTargetInfo> targets)
        {
            throw new NotImplementedException();
        }

        public override void DoPermitCast(Pawn caster, Map map, List<LocalTargetInfo> targets)
        {
        }
    }
}