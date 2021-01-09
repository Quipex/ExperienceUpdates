using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using System.Collections.Generic;

namespace ExperienceUpdates
{
    class TextRenderer
    {
        private const int MINESHAFT_LEVEL_PIXELS = 75;
        private const int TOP_SCREEN_PIXELS = 20;
        // intended for using with experience bars mod https://github.com/spacechase0/ExperienceBars
        private const int DEFAULT_LEFT_OFFSET_OF_EXP_BARS = 295;
        private const int EXP_BARS_INTERVAL = 40;

        internal void Render(Dictionary<int, SparklingText> textsToSkill)
        {
            if (textsToSkill.Count != 0)
            {
                int offsetY = TOP_SCREEN_PIXELS;
                if (Game1.player.currentLocation != null && Game1.player.currentLocation is MineShaft)
                {
                    offsetY += MINESHAFT_LEVEL_PIXELS;
                }
                foreach (var textToSkill in textsToSkill)
                {
                    textToSkill.Value.draw(Game1.spriteBatch, new Vector2(DEFAULT_LEFT_OFFSET_OF_EXP_BARS, offsetY + textToSkill.Key * EXP_BARS_INTERVAL));
                }
            }
        }
    }
}
