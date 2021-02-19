using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace FalloutRedScare
{
    public class Settings : ModSettings
    {
        public static bool overridePowerpoints;
        public static bool overrideSpawnrange;
        public static int powerpoints;
        public static float spawnRangeMax = 100;
        public static FloatRange spawnRange;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref powerpoints, nameof(powerpoints));
            Scribe_Values.Look(ref spawnRange, nameof(spawnRange));
            Scribe_Values.Look(ref overrideSpawnrange, nameof(overrideSpawnrange));
            Scribe_Values.Look(ref overridePowerpoints, nameof(overridePowerpoints));
            base.ExposeData();
        }
    }

    public class FRSMod : Mod
    {
        /// <summary>
        /// A reference to our settings.
        /// </summary>
        Settings settings;

        /// <summary>
        /// A mandatory constructor which resolves the reference to our settings.
        /// </summary>
        /// <param name="content"></param>
        public FRSMod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<Settings>();
        }

        /// <summary>
        /// The (optional) GUI part to set your settings.
        /// </summary>
        /// <param name="inRect">A Unity Rect with the size of the settings window.</param>
        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.CheckboxLabeled(nameof(Settings.overridePowerpoints), ref Settings.overridePowerpoints, nameof(Settings.overridePowerpoints));
            listingStandard.Label($"powerpoints {Settings.powerpoints}");
            Settings.powerpoints = (int)listingStandard.Slider(Settings.powerpoints, 0, 10000);
            listingStandard.CheckboxLabeled(nameof(Settings.overridePowerpoints), ref Settings.overrideSpawnrange, nameof(Settings.overrideSpawnrange));
            listingStandard.Label($"max draw {Settings.spawnRangeMax} min {Settings.spawnRange.min} max {Settings.spawnRange.max}");
            Settings.spawnRangeMax = listingStandard.Slider(Settings.spawnRangeMax, 0.1f, 1000f);
            Settings.spawnRange.min = listingStandard.Slider(Settings.spawnRange.min, 0.1f, Settings.spawnRangeMax);
            Settings.spawnRange.max = listingStandard.Slider(Settings.spawnRange.max, 0.1f, Settings.spawnRangeMax);
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        /// <summary>
        /// Override SettingsCategory to show up in the list of settings.
        /// Using .Translate() is optional, but does allow for localisation.
        /// </summary>
        /// <returns>The (translated) mod name.</returns>
        public override string SettingsCategory()
        {
            return "RedScare".Translate();
        }
    }
}
