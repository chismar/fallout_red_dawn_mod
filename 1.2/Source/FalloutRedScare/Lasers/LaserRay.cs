
using System;
using RimWorld;
using UnityEngine;
using Verse;
namespace RedScare.Lasers
{
    internal interface IDrawnWeaponWithRotation
    {
        float RotationOffset { get; set; }
    }
    public static class ThingExtensions
    {
        // Token: 0x0600005F RID: 95 RVA: 0x0000440C File Offset: 0x0000260C
        public static bool IsShielded(this Thing thing)
        {
            return (thing as Pawn).IsShielded();
        }

        // Token: 0x06000060 RID: 96 RVA: 0x0000442C File Offset: 0x0000262C
        public static bool IsShielded(this Pawn pawn)
        {
            bool flag = pawn == null || pawn.apparel == null;
            bool result;
            if (flag)
            {
                result = false;
            }
            else
            {
                DamageInfo damageInfo = new DamageInfo(DamageDefOf.Bomb, 0f, 0f, -1f, null, null, null, 0, null);
                foreach (Apparel apparel in pawn.apparel.WornApparel)
                {
                    bool flag2 = apparel.CheckPreAbsorbDamage(damageInfo);
                    if (flag2)
                    {
                        return true;
                    }
                }
                result = false;
            }
            return result;
        }
    }
    public class LaserRay : Bullet
    {
        private LaserRayDef Def => base.def as LaserRayDef;

        public override void Draw()
        {
        }

        private void TriggerEffect(EffecterDef effect, Vector3 position)
        {
            this.TriggerEffect(effect, IntVec3.FromVector3(position));
        }

        private void TriggerEffect(EffecterDef effect, IntVec3 dest)
        {
            bool flag = effect == null;
            if (!flag)
            {
                var targetInfo = new TargetInfo(dest, base.Map, false);
                Effecter effecter = effect.Spawn();
                effecter.Trigger(targetInfo, targetInfo);
                effecter.Cleanup();
            }
        }

        private void SpawnBeam(Vector3 a, Vector3 b)
        {
            var laserBeamGraphic = ThingMaker.MakeThing(this.Def.beamGraphic, null) as LaserRayGraphic;
            bool flag = laserBeamGraphic == null;
            if (!flag)
            {
                laserBeamGraphic.def = this.Def;
                laserBeamGraphic.Setup(this.launcher, a, b);
                GenSpawn.Spawn(laserBeamGraphic, IntVec3Utility.ToIntVec3(this.origin), base.Map, 0);
            }
        }

        // Token: 0x06000039 RID: 57 RVA: 0x00002E0C File Offset: 0x0000100C
        private void SpawnBeamReflections(Vector3 a, Vector3 b, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Vector3 normalized = (b - a).normalized;
                Vector3 b2 = b - Vector3Utility.RotatedBy(normalized, Rand.Range(-22.5f, 22.5f)) * Rand.Range(1f, 4f);
                this.SpawnBeam(b, b2);
            }
        }

        // Token: 0x0600003A RID: 58 RVA: 0x00002E78 File Offset: 0x00001078
        protected override void Impact(Thing hitThing)
        {
            var isShielded = hitThing.IsShielded();
            var laserGunDef = this.equipmentDef as LaserRayGunDef;
            Vector3 normalized = (this.destination - this.origin).normalized;
            normalized.y = 0f;
            Vector3 vector = this.origin + normalized * ((laserGunDef == null) ? 0.9f : laserGunDef.barrelLength);
            Vector3 vector2 = isShielded ? (GenThing.TrueCenter(hitThing) - Vector3Utility.RotatedBy(normalized, Rand.Range(-22.5f, 22.5f)) * 0.8f) : this.destination;
            vector.y = (vector2.y = this.Def.Altitude);
            this.SpawnBeam(vector, vector2);

            Pawn pawn = this.launcher as Pawn;
            IDrawnWeaponWithRotation drawnWeaponWithRotation = null;
            bool flag4 = pawn != null && pawn.equipment != null;
            if (flag4)
            {
                drawnWeaponWithRotation = (pawn.equipment.Primary as IDrawnWeaponWithRotation);
            }
            if (drawnWeaponWithRotation != null)
            {
                float num = Vector3Utility.AngleFlat(vector2 - vector) - Vector3Utility.AngleFlat(this.intendedTarget.CenterVector3 - vector);
                drawnWeaponWithRotation.RotationOffset = (num + 180f) % 360f - 180f;
            }
            if (hitThing == null)
            {
                this.TriggerEffect(this.Def.explosionEffect, this.destination);
            }
            else
            {
                if (hitThing is Pawn && isShielded)
                {
                    this.weaponDamageMultiplier *= this.Def.shieldDamageMultiplier;
                    this.SpawnBeamReflections(vector, vector2, 5);
                }
                this.TriggerEffect(this.Def.explosionEffect, this.ExactPosition);
            }
            base.Impact(hitThing);
        }
    }


}
