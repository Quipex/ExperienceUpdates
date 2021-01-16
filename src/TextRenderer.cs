using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using System;
using System.Collections.Generic;

namespace ExperienceUpdates
{
    internal class TextRenderer
    {
        private const int MINESHAFT_LEVEL_PIXELS = 75;
        private const int EXP_BARS_INTERVAL = 40;
        private const int TEXT_WIDTH = 50;

        internal static void Render(Dictionary<int, SparklingText> textsToSkill, int skillsNumber)
        {
            if (textsToSkill.Count == 0) return;

            var offsetY = CalculateYOffset(skillsNumber);
            var offsetX = CalculateXOffset();
            foreach (var textToSkill in textsToSkill)
            {
                textToSkill.Value.draw(Game1.spriteBatch, new Vector2(offsetX, offsetY + textToSkill.Key * EXP_BARS_INTERVAL));
            }
        }

        private static int CalculateXOffset()
        {
            var offsetX = ModEntry.Config.X;
            if (offsetX < 0)
            {
                var rightestXPixel = Game1.viewport.Width - Math.Abs(offsetX);
                return rightestXPixel - TEXT_WIDTH;
            }
            else
            {
                return offsetX;
            }
        }

        private static int CalculateYOffset(int skills)
        {
            var offsetY = ModEntry.Config.Y;
            if (offsetY < 0)
            {
                var lowestYPixel = Game1.viewport.Height - Math.Abs(offsetY);
                return lowestYPixel - skills * EXP_BARS_INTERVAL;
            }

            if (Game1.player.currentLocation != null && Game1.player.currentLocation is MineShaft)
            {
                offsetY += MINESHAFT_LEVEL_PIXELS;
            }

            return offsetY;
        }
    }
}
