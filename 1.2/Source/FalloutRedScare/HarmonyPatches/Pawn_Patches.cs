using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;


namespace RedScare
{

	[HarmonyPatch(typeof(Pawn), "SetFaction")]
	public static class Patch_SetFaction
	{
		private static void Prefix(Pawn __instance, Faction newFaction, out bool __state)
        {
			if (newFaction != __instance.Faction)
            {
				__state = true;
            }
			else
            {
				__state = false;
			}
        }

		private static void Postfix(Pawn __instance, bool __state)
		{
			if (__state && __instance?.Faction == Faction.OfPlayer && __instance.RaceProps.Humanlike)
			{
				var extension = Find.Storyteller.def.GetModExtension<RedStorytellerExtension>();
				if (extension != null)
                {
					foreach (var option in extension.options)
                    {
						if (option.playerFaction == Faction.OfPlayer.def)
                        {
							var otherFaction = Find.FactionManager.FirstFactionOfDef(option.otherFaction);
							if (otherFaction != null)
                            {
								if (option.messageReasonKey.NullOrEmpty())
                                {
									otherFaction.TryAffectGoodwillWith(Faction.OfPlayer, option.goodwillGainPerRecrution);
								}
								else
                                {
									otherFaction.TryAffectGoodwillWith(Faction.OfPlayer, option.goodwillGainPerRecrution, true, true,
										option.messageReasonKey.Translate(otherFaction.Named("OTHERFACTION"), __instance.Named("RECRUITE")));
                                }
                            }
							if (option.thoughtOnRecrutionForEveryColonists != null)
                            {
								foreach (Pawn otherPawn in PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonistsAndPrisoners)
								{
									if (__instance != otherPawn && otherPawn.IsColonist && otherPawn.needs.mood != null)
									{
										otherPawn.needs.mood.thoughts.memories.TryGainMemory(option.thoughtOnRecrutionForEveryColonists);
									}
								}
							}
							if (option.traitsToGive.Any())
							{
								var list = option.traitsToGive.Where(x => DefDatabase<TraitDef>.GetNamedSilentFail(x) != null).ToList().ListFullCopy();
								while (list.Any())
								{
									var traitDefName = list.RandomElement();
									var traitDef = DefDatabase<TraitDef>.GetNamed(traitDefName); 
									int degree = RandomTraitDegree(traitDef);

									if (TraitIsAllowed(__instance, traitDef, degree))
									{
										Trait trait = new Trait(traitDef, degree);
										__instance.story.traits.GainTrait(trait);
										break;
									}
									else
									{
										list.Remove(traitDefName);
									}
								}
							}

							if (option.goodIncidentsToSpawn.Any())
                            {
								var list = option.goodIncidentsToSpawn.Where(x => DefDatabase<IncidentDef>.GetNamedSilentFail(x) != null).ToList().ListFullCopy();
								while (list.Any())
                                {
									var incidentDefName = list.RandomElement();
									var incidentDef = DefDatabase<IncidentDef>.GetNamed(incidentDefName);
									var target = __instance.Map != null ? __instance.Map : Find.AnyPlayerHomeMap;
									var parms = StorytellerUtility.DefaultParmsNow(incidentDef.category, target);
									if (incidentDef.Worker.CanFireNow(parms, true) && incidentDef.Worker.TryExecute(parms))
									{
										break;
									}
									else
									{
										list.Remove(incidentDefName);
									}
								}
							}
                        }
                    }
                }
			}
		}
		private static int RandomTraitDegree(TraitDef traitDef)
		{
			if (traitDef.degreeDatas.Count == 1)
			{
				return traitDef.degreeDatas[0].degree;
			}
			return traitDef.degreeDatas.RandomElementByWeight((TraitDegreeData dd) => dd.commonality).degree;
		}
		private static bool TraitIsAllowed(Pawn pawn, TraitDef newTraitDef, int degree)
		{
			if (pawn.story.traits.HasTrait(newTraitDef) || (pawn.kindDef.disallowedTraits != null && pawn.kindDef.disallowedTraits.Contains(newTraitDef))
				|| (pawn.kindDef.requiredWorkTags != 0 && (newTraitDef.disabledWorkTags & pawn.kindDef.requiredWorkTags) != 0) || (pawn.Faction != null && Faction.OfPlayerSilentFail != null
				&& pawn.Faction.HostileTo(Faction.OfPlayer) && !newTraitDef.allowOnHostileSpawn) || pawn.story.traits.allTraits.Any((Trait tr) => newTraitDef.ConflictsWith(tr))
				|| (newTraitDef.requiredWorkTypes != null && pawn.OneOfWorkTypesIsDisabled(newTraitDef.requiredWorkTypes)) || pawn.WorkTagIsDisabled(newTraitDef.requiredWorkTags)
				|| (newTraitDef.forcedPassions != null && pawn.workSettings != null && newTraitDef.forcedPassions.Any((SkillDef p) =>
				p.IsDisabled(pawn.story.DisabledWorkTagsBackstoryAndTraits, pawn.GetDisabledWorkTypes(permanentOnly: true))))
				|| pawn.story.childhood.DisallowsTrait(newTraitDef, degree) && (pawn.story.adulthood == null || pawn.story.adulthood.DisallowsTrait(newTraitDef, degree)))
			{
				return false;
			}
			return true;
		}
	}
}