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
        public override Vector3 DrawPos => SkyfallerDrawPosUtility.DrawPos_ConstantSpeed(this.TrueCenter(), ticksToImpactDraw == -1 ? ticksToImpact : ticksToImpactDraw, reversed ? this.angle + 180 : this.angle, this.def.skyfaller.speed);
        public IntVec3 GetPos()
        {
            return DrawPos.ToIntVec3();
        }
        bool reversed;
        int midTick;
        int ticksToImpactDraw = -1;
        public override void Tick()
        {
            if (ticksToImpactDraw == -1 && this.ticksToImpact != 0)
                ticksToImpactDraw = this.ticksToImpact;
            var tickPre = ticksToImpact;
            base.Tick();
            if(ticksToImpactDraw != -1)
            {
                var delta = (ticksToImpact - tickPre) * 2;
                ticksToImpactDraw += reversed ? -delta : delta;
                if (ticksToImpactDraw <= 0)
                {
                    ticksToImpactDraw = 0;
                    reversed = true;
                }
            }
                
                            
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