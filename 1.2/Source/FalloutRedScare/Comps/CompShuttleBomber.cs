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
using Verse.Sound;

namespace FalloutRedScare
{
	public class CompProperties_ShuttleBomber : CompProperties
    {
		public CompProperties_ShuttleBomber()
        {
			this.compClass = typeof(CompShuttleBomber);
        }
	}
	public class CompShuttleBomber : ThingComp
	{
		public List<ThingDef> shells;

		public Map targetMap;

        public IntVec3 GetPos()
        {
            if (this.parent.holdingOwner.Owner is Thing thing)
            {
                return thing.DrawPos.ToIntVec3();
            }
            return this.parent.Position;
        }
        public override void CompTick()
        {
			if (Find.TickManager.TicksGame % 60 == 0)
			{
				var curCell = GetPos();
				if (!curCell.InBounds(targetMap))
                {
					curCell = CellRect.WholeMap(targetMap).EdgeCells.ToList().OrderBy(x => x.DistanceTo(curCell)).FirstOrDefault();
                }
				Projectile projectile = (Projectile)GenSpawn.Spawn(shells.RandomElement(), curCell, targetMap, WipeMode.Vanish);
				projectile.Launch(null, curCell.ToVector3ShiftedWithAltitude(AltitudeLayer.Projectile), this.parent.Position, this.parent.Position, ProjectileHitFlags.All, null, null);
			}
		}

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Collections.Look(ref shells, "shells", LookMode.Def);
			Scribe_References.Look(ref targetMap, "targetMap");
        }
    }
}