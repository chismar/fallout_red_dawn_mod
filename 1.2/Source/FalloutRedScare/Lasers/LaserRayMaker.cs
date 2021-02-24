using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RedScare.Lasers
{
	public static class LaserRayMaker
	{
		// Token: 0x0600005B RID: 91 RVA: 0x0000402C File Offset: 0x0000222C
		public static Mesh Mesh(float st, float sv)
		{
			bool flag = st < 0f;
			if (flag)
			{
				st = 0f;
			}
			bool flag2 = st > 0.5f;
			if (flag2)
			{
				st = 0.5f;
			}
			bool flag3 = sv < 0f;
			if (flag3)
			{
				sv = 0f;
			}
			bool flag4 = sv > 0.5f;
			if (flag4)
			{
				sv = 0.5f;
			}
			int num = (int)(st / 0.5f * (float)textureSeamPrecision);
			int num2 = (int)(sv / 0.5f * (float)geometrySeamPrecision);
			int key = num2 + (textureSeamPrecision + 1) * geometrySeamPrecision;
			Mesh mesh;
			bool flag5 = cachedMeshes.TryGetValue(key, out mesh);
			Mesh result;
			if (flag5)
			{
				result = mesh;
			}
			else
			{
				st = 0.5f * (float)num / (float)textureSeamPrecision;
				sv = 0.5f * (float)num2 / (float)geometrySeamPrecision;
				float y = 1f - st;
				float num3 = 0.5f - sv;
				Vector3[] vertices = new Vector3[]
				{
					new Vector3(-0.5f, 0f, -0.5f),
					new Vector3(-0.5f, 0f, -num3),
					new Vector3(0.5f, 0f, -num3),
					new Vector3(0.5f, 0f, -0.5f),
					new Vector3(-0.5f, 0f, num3),
					new Vector3(0.5f, 0f, num3),
					new Vector3(-0.5f, 0f, 0.5f),
					new Vector3(0.5f, 0f, 0.5f)
				};
				Vector2[] uv = new Vector2[]
				{
					new Vector2(0f, 0f),
					new Vector2(0f, st),
					new Vector2(1f, st),
					new Vector2(1f, 0f),
					new Vector2(0f, y),
					new Vector2(1f, y),
					new Vector2(0f, 1f),
					new Vector2(1f, 1f)
				};
				int[] triangles = new int[]
				{
					0,
					1,
					2,
					0,
					2,
					3,
					1,
					4,
					5,
					1,
					5,
					2,
					4,
					6,
					7,
					4,
					7,
					5
				};
				mesh = new Mesh();
				mesh.name = "NewLaserMesh()";
				mesh.vertices = vertices;
				mesh.uv = uv;
				mesh.SetTriangles(triangles, 0);
				mesh.RecalculateNormals();
				mesh.RecalculateBounds();
				cachedMeshes[key] = mesh;
				result = mesh;
			}
			return result;
		}

		// Token: 0x0400004B RID: 75
		private static int textureSeamPrecision = 256;

		// Token: 0x0400004C RID: 76
		private static int geometrySeamPrecision = 512;

		// Token: 0x0400004D RID: 77
		private static Dictionary<int, Mesh> cachedMeshes = new Dictionary<int, Mesh>();
	}
}
