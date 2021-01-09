﻿using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using System.Collections.Generic;
using System.Linq;

namespace ExperienceUpdates
{
    class ExperienceCalculator
    {
        public static readonly int[] expNeededForLevel = new int[] { 100, 380, 770, 1300, 2150, 3300, 4800, 6900, 10000, 15000 };
        private readonly int[] lastExp = new int[6];
        private readonly Dictionary<int, SparklingText> textsToSkill = new Dictionary<int, SparklingText>();
        private readonly IMonitor monitor;

        public ExperienceCalculator(IMonitor monitor)
        {
            this.monitor = monitor;
        }

        internal Dictionary<int, SparklingText> GetUpdatableTexts()
        {
            UpdateState();
            return textsToSkill;
        }

        internal void Reset()
        {
            Game1.player.experiencePoints.CopyTo(lastExp, 0);
        }

        private void UpdateState()
        {
            var newExp = Game1.player.experiencePoints;
            if (InitExp(newExp)) return;

            UpdateTexts();
            CheckForSkillUpdates(newExp);

        }

        private bool InitExp(Netcode.NetArray<int, Netcode.NetInt> newExp)
        {
            if (lastExp == null)
            {
                newExp.CopyTo(lastExp, 0);
                return true;
            }
            return false;
        }

        private void UpdateTexts()
        {
            foreach (var textToSkill in textsToSkill.ToList())
            {
                if (textToSkill.Value.update(Game1.currentGameTime))
                {
                    textsToSkill.Remove(textToSkill.Key);
                }
            }
        }

        private void CheckForSkillUpdates(Netcode.NetArray<int, Netcode.NetInt> newExp)
        {
            for (int skillIndex = 0; skillIndex < newExp.Length; skillIndex++)
            {
                int diff = newExp[skillIndex] - lastExp[skillIndex];
                if (diff != 0)
                {
                    HandleSkillGain(skillIndex, diff, newExp[skillIndex]);
                    lastExp[skillIndex] = newExp[skillIndex];
                }
            }
        }

        private void HandleSkillGain(int skill, int gained, int total)
        {
            LogGainedExp(skill, gained, total);
            AddTextToRender(skill, gained);
        }

        private void LogGainedExp(int skill, int gained, int total)
        {
            int leftTillNext = 0;
            foreach (int neededExp in expNeededForLevel)
            {
                if (total < neededExp)
                {
                    leftTillNext = neededExp - total;
                    break;
                }
            }
            var nextLevelText = leftTillNext > 0 ? $"{leftTillNext} more for next level" : "max level";
            monitor.Log($"Gained +{gained} for {(ExperienceType)skill} ({nextLevelText})", LogLevel.Info);
        }

        private void AddTextToRender(int skill, int gained)
        {
            if (Game1.activeClickableMenu != null) return;

            textsToSkill.Remove(skill);
            textsToSkill.Add(skill, new SparklingText(Game1.smallFont, "+" + gained,
                SkillColorHelper.GetSkillColor(skill), SkillColorHelper.GetSkillColor(skill), millisecondsDuration: 4000));
        }
    }
}
