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

        internal void Render(Dictionary<int, SparklingText> textsToSkill, int skillsNumber, int x, int y)
        {
            if (textsToSkill.Count != 0)
            {
                int offsetY = CalculateYOffset(skillsNumber, x + ModEntry.Config.offsetX, y + ModEntry.Config.offsetY);
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

        private int CalculateYOffset(int skills, int x, int y)
        {
            int offsetY = ModEntry.Config.Y;
            //See below for use of offsetX here
            int offsetX = ModEntry.Config.X;

            if (offsetY < 0)
            {
                int lowestYPixel = Game1.viewport.Height - Math.Abs(offsetY);
                return lowestYPixel - skills * EXP_BARS_INTERVAL;
            }
            else
            {
                //Experience Bars' UI only moves in The Mines if the Experience Bar UI itself is within the (25, 75) game region.
                //If the UI is anywhere else, it does not move the extra 75 pixels.
                //Added this code to check location of the Update Text (which is ideally 285px from where the EXP Bars are set)
                if (Game1.player.currentLocation != null && Game1.player.currentLocation is MineShaft &&
                offsetX <= (25 + x) && offsetY <= 75 + y)
                {
                    offsetY += MINESHAFT_LEVEL_PIXELS;
                }
                return offsetY;
            }
        }
    }
}
