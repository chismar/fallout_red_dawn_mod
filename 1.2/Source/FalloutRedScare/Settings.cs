using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RedScare
{
    public class Settings : ModSettings
    {
        public static bool prUsesWealthForRaids;
        public static bool reinforcementsPowerPoints;
        public static bool overrideReinforcementsCooldown;
        public static int powerpoints;
        public static bool overrideMaxPawns;
        public static int maxPawns;
        public static FloatRange spawnRange;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref prUsesWealthForRaids, nameof(prUsesWealthForRaids));
            Scribe_Values.Look(ref powerpoints, nameof(powerpoints));
            Scribe_Values.Look(ref spawnRange, nameof(spawnRange));
            Scribe_Values.Look(ref overrideReinforcementsCooldown, nameof(overrideReinforcementsCooldown));
            Scribe_Values.Look(ref overrideMaxPawns, nameof(overrideMaxPawns));
            Scribe_Values.Look(ref maxPawns, nameof(maxPawns));
            Scribe_Values.Look(ref reinforcementsPowerPoints, nameof(reinforcementsPowerPoints));
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
        string buffer = "";
        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);

            listingStandard.CheckboxLabeled(nameof(Settings.prUsesWealthForRaids), ref Settings.prUsesWealthForRaids, nameof(Settings.prUsesWealthForRaids));

            listingStandard.CheckboxLabeled(nameof(Settings.reinforcementsPowerPoints), ref Settings.reinforcementsPowerPoints, nameof(Settings.reinforcementsPowerPoints));
            if(Settings.reinforcementsPowerPoints)
            {
                listingStandard.Label($"Reinforcemenets power points: {Settings.powerpoints}");
                listingStandard.TextFieldNumeric(ref Settings.powerpoints, ref buffer, 0, Settings.spawnRange.max);
            }

            listingStandard.CheckboxLabeled(nameof(Settings.overrideReinforcementsCooldown), ref Settings.overrideReinforcementsCooldown, nameof(Settings.overrideReinforcementsCooldown));
            if(Settings.overrideReinforcementsCooldown)
            {
                listingStandard.Label($"Reinforcements cooldown days random range: min {Settings.spawnRange.min} max {Settings.spawnRange.max}");
                listingStandard.TextFieldNumeric(ref Settings.spawnRange.min, ref buffer, 0, Settings.spawnRange.max);
                listingStandard.TextFieldNumeric(ref Settings.spawnRange.max, ref buffer, Settings.spawnRange.min, 1000);
            }

            listingStandard.CheckboxLabeled(nameof(Settings.overrideMaxPawns), ref Settings.overrideMaxPawns, nameof(Settings.overrideMaxPawns));
            if (Settings.overrideMaxPawns)
            {
                listingStandard.Label($"Reinforcements stop when colony pawns count reach: {Settings.maxPawns}");
                listingStandard.TextFieldNumeric(ref Settings.maxPawns, ref buffer, 0, 1000);
            }

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
            return "Red Scare Framework";
        }
    }
}
