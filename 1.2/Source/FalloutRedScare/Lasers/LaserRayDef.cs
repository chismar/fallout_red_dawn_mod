using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RedScare.Lasers
{
	public class LaserRayDef : ThingDef
	{
		public float capSize = 1f;
		public float capOverlap = 0.0171875f;
		public int lifetime = 30;
		public int flickerFrameTime = 5;
		public float impulse = 4f;
		public float beamWidth = 1f;
		public float seam = -1f;
		public List<LaserBeamDecoration> decorations;
		public EffecterDef explosionEffect;
		public EffecterDef hitLivingEffect;
		public ThingDef beamGraphic;
		public float shieldDamageMultiplier = 0.5f;
	}
	
	public class LaserBeamDecoration
	{
		public ThingDef mote;
		public float spacing = 1f;
		public float initialOffset = 0f;
		public float speed = 1f;
		public float speedJitter;
		public float speedJitterOffset;
	}
}
