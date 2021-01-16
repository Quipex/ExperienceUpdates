using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using System.Collections.Generic;
using System.Linq;

namespace ExperienceUpdates
{
    class ExperienceCalculator
    {
        private static readonly int[] EXP_NEEDED_FOR_LEVEL = new int[] {100, 380, 770, 1300, 2150, 3300, 4800, 6900, 10000, 15000};
        private readonly int[] _lastExp = new int[6];
        private readonly Dictionary<int, SparklingText> _textsToSkill = new Dictionary<int, SparklingText>();
        private readonly IMonitor _monitor;
        private bool _running;

        public ExperienceCalculator(IMonitor monitor)
        {
            _monitor = monitor;
        }

        internal void Stop()
        {
            _running = false;
        }

        internal Dictionary<int, SparklingText> GetUpdatableTexts()
        {
            UpdateState();
            return _textsToSkill;
        }

        internal void Reset()
        {
            var newExp = Game1.player.experiencePoints;
            newExp.CopyTo(_lastExp, 0);
            _monitor.Log($"Reset experience {newExp}. Resuming experience update listener.");
            _running = true;
        }

        private void UpdateState()
        {
            var newExp = Game1.player.experiencePoints;
            UpdateTexts();
            if (_running)
            {
                CheckForSkillUpdates(newExp);
            }
        }

        private void UpdateTexts()
        {
            foreach (var textToSkill in _textsToSkill.ToList())
            {
                if (textToSkill.Value.update(Game1.currentGameTime))
                {
                    _textsToSkill.Remove(textToSkill.Key);
                }
            }
        }

        private void CheckForSkillUpdates(Netcode.NetArray<int, Netcode.NetInt> newExp)
        {
            for (var skillIndex = 0; skillIndex < newExp.Length; skillIndex++)
            {
                var diff = newExp[skillIndex] - _lastExp[skillIndex];
                if (diff == 0) continue;
                HandleSkillGain(skillIndex, diff, newExp[skillIndex]);
                _lastExp[skillIndex] = newExp[skillIndex];
            }
        }

        private void HandleSkillGain(int skill, int gained, int total)
        {
            LogGainedExp(skill, gained, total);
            AddTextToRender(skill, gained);
        }

        private void LogGainedExp(int skill, int gained, int total)
        {
            var leftTillNext = (from neededExp in EXP_NEEDED_FOR_LEVEL
                where total < neededExp
                select neededExp - total).FirstOrDefault();
            var nextLevelText = leftTillNext > 0 ? $"{leftTillNext} more for next level" : "max level";
            _monitor.Log($"Gained +{gained} for {(ExperienceType) skill} ({nextLevelText})", LogLevel.Debug);
        }

        private void AddTextToRender(int skill, int gained)
        {
            if (Game1.activeClickableMenu != null) return;

            _textsToSkill.Remove(skill);
            _textsToSkill.Add(skill, new SparklingText(Game1.smallFont, "+" + gained,
                SkillColorHelper.GetSkillColor(skill), SkillColorHelper.GetSkillColor(skill), millisecondsDuration: ModEntry.Config.TextDurationMS));
        }
    }
}
