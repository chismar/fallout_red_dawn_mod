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
	}
}