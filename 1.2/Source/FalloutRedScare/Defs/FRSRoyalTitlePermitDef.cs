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
	public class FRS_TitlePermitWorker : RoyalTitlePermitWorker
	{
		public new FRSRoyalTitlePermitDef def => base.def as FRSRoyalTitlePermitDef;
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
	}
}