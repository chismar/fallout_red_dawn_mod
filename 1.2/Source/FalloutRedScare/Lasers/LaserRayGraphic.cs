using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RedScare.Lasers
{
	public class LaserRayGraphic : Thing
	{

		private int ticks;
		private Vector3 a;
		private Vector3 b;
		public Matrix4x4 drawingMatrix = default(Matrix4x4);
		private Material materialBeam;
		private Mesh mesh;
		private LaserRayDef Def => base.def as LaserRayDef;


		public float Opacity => (float)Math.Sin(Math.Pow(1.0 - 1.0 * (double)this.ticks / (double)this.Def.lifetime, (double)this.Def.impulse) * Mathf.PI);

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref this.ticks, "ticks", 0, false);
			Scribe_Values.Look(ref this.a, "a", default, false);
			Scribe_Values.Look(ref this.b, "b", default, false);
		}

		public override void Tick()
		{
			bool flag;
			if (this.Def != null)
			{
				int num = this.ticks;
				this.ticks = num + 1;
				flag = (num > this.Def.lifetime);
			}
			else
			{
				flag = true;
			}
			bool flag2 = flag;
			if (flag2)
			{
				this.Destroy(0);
			}
		}

		
		public void Setup(Thing launcher, Vector3 origin, Vector3 destination)
		{
			this.a = origin;
			this.b = destination;
		}

		public void SetupDrawing()
		{
			bool flag = this.mesh != null;
			if (!flag)
			{
				this.materialBeam = this.Def.graphicData.Graphic.MatSingle;
				float beamWidth = this.Def.beamWidth;
				Quaternion q = Quaternion.LookRotation(this.b - this.a);
				float magnitude = (this.b - this.a).magnitude;
				Vector3 s = new Vector3(beamWidth, 1f, magnitude);
				Vector3 pos = (this.a + this.b) / 2f;
				this.drawingMatrix.SetTRS(pos, q, s);
				float num = 1f * (float)this.materialBeam.mainTexture.width / (float)this.materialBeam.mainTexture.height;
				float num2 = (this.Def.seam < 0f) ? num : this.Def.seam;
				float num3 = beamWidth / num / 2f * num2;
				float sv = (magnitude <= num3 * 2f) ? 0.5f : (num3 * 2f / magnitude);
				this.mesh = LaserRayMaker.Mesh(num2, sv);
			}
		}

		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);
			bool flag = this.Def == null || this.Def.decorations == null || respawningAfterLoad;
			if (!flag)
			{
				foreach (var laserBeamDecoration in this.Def.decorations)
				{
					float num = laserBeamDecoration.spacing * this.Def.beamWidth;
					float d = laserBeamDecoration.initialOffset * this.Def.beamWidth;
					Vector3 normalized = (this.b - this.a).normalized;
					float num2 = Vector3Utility.AngleFlat(this.b - this.a);
					Vector3 vector = normalized * num;
					Vector3 exactPosition = this.a + vector * 0.5f + normalized * d;
					float num3 = (this.b - this.a).magnitude - num;
					int num4 = 0;
					while (num3 > 0f)
					{
						var moteLaserDectoration = ThingMaker.MakeThing(laserBeamDecoration.mote, null) as LaserMote;
						bool flag2 = moteLaserDectoration == null;
						if (flag2)
						{
							break;
						}
						moteLaserDectoration.ray = this;
						moteLaserDectoration.airTimeLeft = (float)this.Def.lifetime;
						moteLaserDectoration.Scale = this.Def.beamWidth;
						moteLaserDectoration.exactRotation = num2;
						moteLaserDectoration.exactPosition = exactPosition;
						moteLaserDectoration.SetVelocity(num2, laserBeamDecoration.speed);
						moteLaserDectoration.baseSpeed = laserBeamDecoration.speed;
						moteLaserDectoration.speedJitter = laserBeamDecoration.speedJitter;
						moteLaserDectoration.speedJitterOffset = laserBeamDecoration.speedJitterOffset * (float)num4;
						GenSpawn.Spawn(moteLaserDectoration, IntVec3Utility.ToIntVec3(this.a), map, 0);
						exactPosition += vector;
						num3 -= num;
						num4++;
					}
				}
			}
		}

		public override void Draw()
		{
			this.SetupDrawing();
			float opacity = this.Opacity;
			bool flag = this.Def.graphicData.graphicClass == typeof(Graphic_Flicker);
			if (flag)
			{
				bool flag2 = !Find.TickManager.Paused && Find.TickManager.TicksGame % this.Def.flickerFrameTime == 0;
				if (flag2)
				{
					this.materialBeam = this.Def.graphicData.Graphic.MatSingle;
				}
			}
			Graphics.DrawMesh(this.mesh, this.drawingMatrix, FadedMaterialPool.FadedVersionOf(this.materialBeam, opacity), 0);
		}

	}
}
