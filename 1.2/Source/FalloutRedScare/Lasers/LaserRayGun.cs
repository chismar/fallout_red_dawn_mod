using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RedScare.Lasers
{
	public class LaserRayGunDef : ThingDef
	{
		public static LaserRayGunDef defaultObj = new LaserRayGunDef();
		public float barrelLength = 0.9f;
	}
	public class LaserRayGun : ThingWithComps, IDrawnWeaponWithRotation
	{

		private int ticksPreviously = 0;
		private float rotationSpeed = 0f;
		private float rotationOffset = 0f;
		public float RotationOffset
		{
			get
			{
				int ticksGame = Find.TickManager.TicksGame;
				this.UpdateRotationOffset(ticksGame - this.ticksPreviously);
				this.ticksPreviously = ticksGame;
				return this.rotationOffset;
			}
			set
			{
				this.rotationOffset = value;
				this.rotationSpeed = 0f;
			}
		}

		public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn pawn)
		{
			foreach (var o in base.GetFloatMenuOptions(pawn))
			{
				bool flag = o != null;
				if (flag)
				{
					yield return o;
				}
			}
		}

		private void UpdateRotationOffset(int ticks)
		{
			bool flag = this.rotationOffset == 0f;
			if (!flag)
			{
				bool flag2 = ticks <= 0;
				if (!flag2)
				{
					bool flag3 = ticks > 30;
					if (flag3)
					{
						ticks = 30;
					}
					bool flag4 = this.rotationOffset > 0f;
					if (flag4)
					{
						this.rotationOffset -= this.rotationSpeed;
						bool flag5 = this.rotationOffset < 0f;
						if (flag5)
						{
							this.rotationOffset = 0f;
						}
					}
					else
					{
						bool flag6 = this.rotationOffset < 0f;
						if (flag6)
						{
							this.rotationOffset += this.rotationSpeed;
							bool flag7 = this.rotationOffset > 0f;
							if (flag7)
							{
								this.rotationOffset = 0f;
							}
						}
					}
					this.rotationSpeed += (float)ticks * 0.01f;
				}
			}
		}

	}
}
