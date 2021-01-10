using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using System;
using System.Collections.Generic;

namespace ExperienceUpdates
{
    class TextRenderer
    {
        private const int MINESHAFT_LEVEL_PIXELS = 75;
        private const int EXP_BARS_INTERVAL = 40;
        private const int TEXT_WIDTH = 50;

        internal void Render(Dictionary<int, SparklingText> textsToSkill, int skillsNumber)
        {
            if (textsToSkill.Count != 0)
            {
                int offsetY = CalculateYOffset(skillsNumber);
                int offsetX = CalculateXOffset();
                foreach (var textToSkill in textsToSkill)
                {
                    textToSkill.Value.draw(Game1.spriteBatch, new Vector2(offsetX, offsetY + textToSkill.Key * EXP_BARS_INTERVAL));
                }
            }
        }

        private int CalculateXOffset()
        {
            int offsetX = ModEntry.Config.X;
            if (offsetX < 0)
            {
                int rightestXPixel = Game1.viewport.Width - Math.Abs(offsetX);
                return rightestXPixel - TEXT_WIDTH;
            }
            else
            {
                return offsetX;
            }
        }

        private int CalculateYOffset(int skills)
        {
            int offsetY = ModEntry.Config.Y;
            if (offsetY < 0)
            {
                int lowestYPixel = Game1.viewport.Height - Math.Abs(offsetY);
                return lowestYPixel - skills * EXP_BARS_INTERVAL;
            }
            else
            {
                if (Game1.player.currentLocation != null && Game1.player.currentLocation is MineShaft)
                {
                    offsetY += MINESHAFT_LEVEL_PIXELS;
                }
                return offsetY;
            }
        }
    }
}
