using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RedScare
{
    public class SpatialHashMapComp : MapComponent
    {
        public static Dictionary<Map, SpatialHash> HashMapPerMap = new Dictionary<Map, SpatialHash>();
        public SpatialHash SpatialHash;
        public SpatialHashMapComp(Map map) : base(map)
        {
            SpatialHash = new SpatialHash(map.Size.x, map.Size.z, 20);
            HashMapPerMap[map] = SpatialHash;
        }

        public override void MapRemoved()
        {
            HashMapPerMap.Remove(map);
        }
    }
    [HarmonyPatch(typeof(Thing), nameof(Thing.Position), MethodType.Setter)]
    public static class PawnSpatialHasherSetPos
    {
        public static void Prefix(Thing __instance, IntVec3 value)
        {
            if (!(__instance is Pawn))
                return;
            if (value == __instance.Position)
                return;
            if (!__instance.Spawned)
                return;
            var map = __instance.Map;
            if (SpatialHashMapComp.HashMapPerMap.TryGetValue(map, out var cmp))
            {
                cmp.UpdateValue(__instance);
            }
        }
    }
    [HarmonyPatch(typeof(Pawn), nameof(Pawn.DeSpawn))]
    public static class PawnSpatialHasherDespawn
    {
        public static void Prefix(Pawn __instance)
        {
            var map = __instance.Map;
            if (SpatialHashMapComp.HashMapPerMap.TryGetValue(map, out var cmp))
            {
                cmp.Remove(__instance);
            }
        }
    }
    public class SpatialHash
    {
        Dictionary<Thing, IntVec2> thingToCell = new Dictionary<Thing, IntVec2>();
        HashSet<Thing>[,] spatialHash;
        readonly int cellCountX;
        readonly int cellCountZ;
        readonly int sizeX;
        readonly int sizeZ;
        readonly int cellSize;
        public SpatialHash(int sizeX, int sizeZ, int cellSize)
        {
            this.sizeX = sizeX;
            this.sizeZ = sizeZ;
            this.cellSize = cellSize;
            cellCountX = sizeX / cellSize + 1;
            cellCountZ = sizeZ / cellSize + 1;
            spatialHash = new HashSet<Thing>[cellCountX, cellCountZ];
        }


        public void UpdateValue(Thing thing)
        {
            var newPos = new IntVec2(thing.Position.x / cellSize, thing.Position.z / cellSize);
            var newCell = spatialHash[newPos.x, newPos.z];
            if (newCell == null || !newCell.Contains(thing))
            {
                if (thingToCell.Remove(thing, out var pos))
                {
                    spatialHash[pos.x, pos.z].Remove(thing);
                }
                if (newCell == null)
                {
                    newCell = new HashSet<Thing>();
                    spatialHash[newPos.x, newPos.z] = newCell;
                }
                newCell.Add(thing);
                thingToCell[thing] = newPos;
            }

        }

        public void Remove(Thing thing)
        {
            if (thingToCell.Remove(thing, out var pos))
                spatialHash[pos.x, pos.z].Remove(thing);
        }


        public void GetAllAround(IntVec2 pos, int radius, IList<Thing> things)
        {
            var sqrRadius = radius * radius;
            var radiusInCells = radius / cellSize;
            var centralCell = new IntVec2(pos.x / cellSize, pos.z / cellSize);
            for (int x = -radiusInCells; x <= radiusInCells; x++)
                for (int z = -radiusInCells; z <= radiusInCells; z++)
                {
                    var cellX = centralCell.x + x;
                    var cellZ = centralCell.z + z;
                    if (cellX < 0 || cellZ < 0)
                        continue;
                    var cell = spatialHash[cellX, cellZ];
                    if (cell == null)
                        continue;
                    foreach (var thing in cell)
                    {
                        var delta = (thing.Position.ToIntVec2 - pos);
                        if ((delta.x * delta.x + delta.z * delta.z <= sqrRadius))
                            things.Add(thing);
                    }
                }
        }

    }
}
