using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RedScare.Lasers
{
	public class LaserMote : MoteThrown
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public override float Alpha
		{
			get
			{
				base.Speed = (float)((double)this.baseSpeed + (double)this.speedJitter * Math.Sin(3.1415926535897931 * (double)((float)Find.TickManager.TicksGame * 18f + this.speedJitterOffset) / 180.0));
				bool flag = this.ray != null;
				float result;
				if (flag)
				{
					result = this.ray.Opacity;
				}
				else
				{
					result = base.Alpha;
				}
				return result;
			}
		}

		// Token: 0x04000001 RID: 1
		public LaserRayGraphic ray;

		// Token: 0x04000002 RID: 2
		public float baseSpeed;

		// Token: 0x04000003 RID: 3
		public float speedJitter;

		// Token: 0x04000004 RID: 4
		public float speedJitterOffset;
	}
}
