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

namespace RedScare
{
    public class RedStorytellerOptions
    {
        public FactionDef playerFaction;
        public FactionDef otherFaction;
        public int goodwillGainPerRecrution;
        public string messageReasonKey;
        public List<string> traitsToGive;
        public ThoughtDef thoughtOnRecrutionForEveryColonists;
        public List<string> goodIncidentsToSpawn;
    }
    public class RedStorytellerExtension : DefModExtension
    {
        public List<RedStorytellerOptions> options;
    }
}