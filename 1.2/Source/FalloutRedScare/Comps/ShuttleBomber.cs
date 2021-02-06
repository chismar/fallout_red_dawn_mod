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
	/*public class CompProperties_ShuttleBomber : CompProperties
    {
		public CompProperties_ShuttleBomber()
        {
			this.compClass = typeof(CompShuttleBomber);
        }
	}*/
	public class ShuttleBomber : Skyfaller
	{
		public List<ThingDef> shells;

		public Map targetMap;
        public override Vector3 DrawPos => SkyfallerDrawPosUtility.DrawPos_ConstantSpeed(this.TrueCenter(), this.ticksToImpact, this.angle, this.def.skyfaller.speed);
        public IntVec3 GetPos()
        {
            return DrawPos.ToIntVec3();
        }
        bool reversed;
        public override void Tick()
        {
            if(this.ticksToImpact == 1)
            {
                reversed = true;
                this.angle = angle+180;
            }
            if (reversed)
                ticksToImpact++;
            if (ticksToImpact == 220)
                ticksToImpact = 0;
                base.Tick();
            if(reversed && ticksToImpact != 0)
                ticksToImpact++;
            
            if (Find.TickManager.TicksGame % 20 == 0)
			{
				var curCell = GetPos();
				if (curCell.InBounds(targetMap))
                {
                    Projectile projectile = (Projectile)GenSpawn.Spawn(shells.RandomElement(), curCell, targetMap, WipeMode.Vanish);
                    projectile.Launch(null, curCell.ToVector3ShiftedWithAltitude(AltitudeLayer.Projectile), curCell, curCell, ProjectileHitFlags.All, null, null);
                }
                
			}
		}

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref shells, "shells", LookMode.Def);
			Scribe_References.Look(ref targetMap, "targetMap");
        }
    }
}