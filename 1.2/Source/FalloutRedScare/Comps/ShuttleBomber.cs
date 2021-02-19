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
        public int shellsCount;
        public FloatRange bombingRunAroundTargetMeters;
        public Faction faction;
        public override Vector3 DrawPos => SkyfallerDrawPosUtility.DrawPos_ConstantSpeed(this.TrueCenter(), ticksToImpactDraw == -1 ? ticksToImpact : ticksToImpactDraw, reversed ? this.angle + 180 : this.angle, this.def.skyfaller.speed);
        public IntVec3 GetPos()
        {
            return DrawPos.ToIntVec3();
        }
        bool reversed;
        int ticksToImpactDraw = -1;
        int bombsDropped = 0;
        Vector3 lastBombDrop = default;
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
            var distanceToCenterSqr = (DrawPos - this.TrueCenter()).sqrMagnitude;
            bool throwBomb = false;
            if(!reversed)
            {
                if (distanceToCenterSqr < bombingRunAroundTargetMeters.min * bombingRunAroundTargetMeters.min)
                    throwBomb = true;
            }
            else
            {
                if (distanceToCenterSqr < bombingRunAroundTargetMeters.max * bombingRunAroundTargetMeters.max)
                    throwBomb = true;
            }
            if (lastBombDrop == default)
                lastBombDrop = this.DrawPos;
            if (throwBomb)
            {
                var distanceToLastBombdrop = (DrawPos - lastBombDrop).sqrMagnitude;
                var shellPerMeter = (bombingRunAroundTargetMeters.Span / shellsCount);
                if (distanceToLastBombdrop < shellPerMeter * shellPerMeter)
                {
                    throwBomb = false;
                }
                if (bombsDropped >= shellsCount)
                    throwBomb = false;
            }
            if (throwBomb)
			{
                lastBombDrop = DrawPos;
                bombsDropped++;
				var curCell = GetPos();
				if (curCell.InBounds(targetMap))
                {
                    Projectile projectile = (Projectile)GenSpawn.Spawn(shells.RandomElement(), curCell, targetMap, WipeMode.Vanish);
                    var c = projectile.TryGetComp<CompSpawnerPawnFromDamage>();
                    if (c != null)
                    {
                        c.faction = faction;
                    }
                    projectile.Launch(null, curCell.ToVector3ShiftedWithAltitude(AltitudeLayer.Projectile), curCell, curCell, ProjectileHitFlags.All, null, null);
                }
                
			}
		}

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref shells, "shells", LookMode.Def);
            Scribe_References.Look(ref faction, "faction");
            Scribe_References.Look(ref targetMap, "targetMap");
        }
    }
}