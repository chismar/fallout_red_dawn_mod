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
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;

namespace FalloutRedScare
{
	public class CompProperties_SpawnerPawnFromDamage : CompProperties_SpawnerPawn
    {
		public bool onlySingleLord;
    }
	public class CompSpawnerPawnFromDamage : ThingComp
	{
		private bool pawnsSpawned;

		public int nextPawnSpawnTick = -1;

		public int pawnsLeftToSpawn = -1;

		public List<Pawn> spawnedPawns = new List<Pawn>();

		public bool aggressive = true;

		public bool canSpawnPawns = true;

		private PawnKindDef chosenKind;

		private CompCanBeDormant dormancyCompCached;

		private CompProperties_SpawnerPawnFromDamage Props => (CompProperties_SpawnerPawnFromDamage)props;

		public Lord Lord => !Props.onlySingleLord ? FindLordToJoin(parent, Props.lordJob, Props.shouldJoinParentLord) : null;

		private float SpawnedPawnsPoints
		{
			get
			{
				FilterOutUnspawnedPawns();
				float num = 0f;
				for (int i = 0; i < spawnedPawns.Count; i++)
				{
					num += spawnedPawns[i].kindDef.combatPower;
				}
				return num;
			}
		}

		public bool Active
		{
			get
			{
				if (pawnsLeftToSpawn == 0)
				{
					return false;
				}
				return !Dormant;
			}
		}

		public CompCanBeDormant DormancyComp => dormancyCompCached ?? (dormancyCompCached = parent.TryGetComp<CompCanBeDormant>());

		public bool Dormant
		{
			get
			{
				if (DormancyComp != null)
				{
					return !DormancyComp.Awake;
				}
				return false;
			}
		}

		public override void Initialize(CompProperties props)
		{
			base.Initialize(props);
			if (chosenKind == null)
			{
				chosenKind = RandomPawnKindDef();
			}
			if (Props.maxPawnsToSpawn != IntRange.zero)
			{
				pawnsLeftToSpawn = Props.maxPawnsToSpawn.RandomInRange;
			}
		}

		public static Lord FindLordToJoin(Thing spawner, Type lordJobType, bool shouldTryJoinParentLord, Func<Thing, List<Pawn>> spawnedPawnSelector = null)
		{
			if (spawner.Spawned)
			{
				if (shouldTryJoinParentLord)
				{
					Lord lord = (spawner as Building)?.GetLord();
					if (lord != null)
					{
						return lord;
					}
				}
				if (spawnedPawnSelector == null)
				{
					spawnedPawnSelector = ((Thing s) => s.TryGetComp<CompSpawnerPawn>()?.spawnedPawns);
				}
				Predicate<Pawn> hasJob = delegate (Pawn x)
				{
					Lord lord2 = x.GetLord();
					return lord2 != null && lord2.LordJob.GetType() == lordJobType;
				};
				Pawn foundPawn = null;
				RegionTraverser.BreadthFirstTraverse(spawner.GetRegion(), (Region from, Region to) => true, delegate (Region r)
				{
					List<Thing> list = r.ListerThings.ThingsOfDef(spawner.def);
					for (int i = 0; i < list.Count; i++)
					{
						if (list[i].Faction == spawner.Faction)
						{
							List<Pawn> list2 = spawnedPawnSelector(list[i]);
							if (list2 != null)
							{
								foundPawn = list2.Find(hasJob);
							}
							if (foundPawn != null)
							{
								return true;
							}
						}
					}
					return false;
				}, 40);
				if (foundPawn != null)
				{
					return foundPawn.GetLord();
				}
			}
			return null;
		}

		public Lord CreateNewLord(Thing byThing, bool aggressive, float defendRadius, Type lordJobType)
		{
			if (!CellFinder.TryFindRandomCellNear(byThing.PositionHeld, curMap, 5, (IntVec3 c) => c.Standable(curMap) 
				&& curMap.reachability.CanReach(c, byThing.PositionHeld, PathEndMode.OnCell, TraverseParms.For(TraverseMode.PassDoors)), out IntVec3 result))
			{
				Log.Error("Found no place for mechanoids to defend " + byThing);
				result = IntVec3.Invalid;
			}
			LordJob lordJob = null;
			if (lordJobType == typeof(LordJob_DefendPoint))
            {
				lordJob = new LordJob_DefendPoint(result);
            }
			else
            {
				lordJob = Activator.CreateInstance(lordJobType, new object[] { result }) as LordJob;
			}
			return LordMaker.MakeNewLord(byThing.Faction, lordJob, curMap);
		}

		private void SpawnInitialPawns(Map map)
		{
			for (int i = 0; i < Props.initialPawnsCount; i++)
			{
				if (!TrySpawnPawn(map, out Pawn _))
				{
					break;
				}
			}
			SpawnPawnsUntilPoints(map, Props.initialPawnsPoints);
		}

		public void SpawnPawnsUntilPoints(Map map, float points)
		{
			int num = 0;
			while (SpawnedPawnsPoints < points)
			{
				num++;
				if (num > 1000)
				{
					Log.Error("Too many iterations.");
					break;
				}
				if (!TrySpawnPawn(map, out Pawn _))
				{
					break;
				}
			}
		}
		private void FilterOutUnspawnedPawns()
		{
			for (int num = spawnedPawns.Count - 1; num >= 0; num--)
			{
				if (!spawnedPawns[num].Spawned)
				{
					spawnedPawns.RemoveAt(num);
				}
			}
		}

		private PawnKindDef RandomPawnKindDef()
		{
			float curPoints = SpawnedPawnsPoints;
			IEnumerable<PawnKindDef> source = Props.spawnablePawnKinds;
			if (Props.maxSpawnedPawnsPoints > -1f)
			{
				source = source.Where((PawnKindDef x) => curPoints + x.combatPower <= Props.maxSpawnedPawnsPoints);
			}
			if (source.TryRandomElement(out PawnKindDef result))
			{
				return result;
			}
			return null;
		}

		private Map curMap;
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
			curMap = this.parent.MapHeld;
        }
        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
			this.SpawnInitialPawns(curMap);
            base.PostDestroy(mode, previousMap);
		}
		public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
        {
            base.PostPreApplyDamage(dinfo, out absorbed);
			if (!pawnsSpawned && dinfo.Def.ExternalViolenceFor(parent) && dinfo.Amount >= (float)parent.HitPoints)
            {
				this.SpawnInitialPawns(curMap);
				pawnsSpawned = true;
            }
        }

        private bool TrySpawnPawn(Map map, out Pawn pawn)
		{
			if (!canSpawnPawns)
			{
				pawn = null;
				return false;
			}
			if (!Props.chooseSingleTypeToSpawn)
			{
				chosenKind = RandomPawnKindDef();
			}
			if (chosenKind == null)
			{
				pawn = null;
				return false;
			}
			int index = chosenKind.lifeStages.Count - 1;
			pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(chosenKind, parent.Faction, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, newborn: false, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, null, 1f, null, null, null, null, null, chosenKind.race.race.lifeStageAges[index].minAge));
			spawnedPawns.Add(pawn);
			GenSpawn.Spawn(pawn, parent.PositionHeld, map);
			Lord lord = Lord;
			if (lord == null)
			{
				lord = CreateNewLord(parent, aggressive, Props.defendRadius, Props.lordJob);
			}
			lord.AddPawn(pawn);
			if (Props.spawnSound != null)
			{
				Props.spawnSound.PlayOneShot(parent);
			}
			if (pawnsLeftToSpawn > 0)
			{
				pawnsLeftToSpawn--;
			}
			SendMessage();
			return true;
		}
		public void SendMessage()
		{
			if (!Props.spawnMessageKey.NullOrEmpty() && MessagesRepeatAvoider.MessageShowAllowed(Props.spawnMessageKey, 0.1f))
			{
				Messages.Message(Props.spawnMessageKey.Translate(), parent, MessageTypeDefOf.NegativeEvent);
			}
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref nextPawnSpawnTick, "nextPawnSpawnTick", 0);
			Scribe_Values.Look(ref pawnsLeftToSpawn, "pawnsLeftToSpawn", -1);
			Scribe_Collections.Look(ref spawnedPawns, "spawnedPawns", LookMode.Reference);
			Scribe_Values.Look(ref aggressive, "aggressive", defaultValue: false);
			Scribe_Values.Look(ref canSpawnPawns, "canSpawnPawns", defaultValue: true);
			Scribe_Defs.Look(ref chosenKind, "chosenKind");
			Scribe_Values.Look(ref pawnsSpawned, "pawnsSpawned");
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				spawnedPawns.RemoveAll((Pawn x) => x == null);
				if (pawnsLeftToSpawn == -1 && Props.maxPawnsToSpawn != IntRange.zero)
				{
					pawnsLeftToSpawn = Props.maxPawnsToSpawn.RandomInRange;
				}
			}
		}
	}
}