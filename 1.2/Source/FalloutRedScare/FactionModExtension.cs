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
    public class FactionModExtension : DefModExtension
    {
        public ThingDef customShuttle;
        public ThingDef customShuttleIncoming;
        public ThingDef customShuttleLeaving;
        public ThingDef customShuttleCrashing;
        public ThingDef customShuttleCrashed;
    }
}