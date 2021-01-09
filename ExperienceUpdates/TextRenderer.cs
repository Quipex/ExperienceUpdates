using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using System.Collections.Generic;

namespace ExperienceUpdates
{
    class TextRenderer
    {
        internal void Render(Dictionary<int, SparklingText> textsToSkill)
        {
            if (textsToSkill.Count != 0)
            {
                int offsetY = 20;
                if (Game1.player.currentLocation != null && Game1.player.currentLocation is MineShaft)
                {
                    offsetY += 75;
                }
                foreach (var textToSkill in textsToSkill)
                {
                    textToSkill.Value.draw(Game1.spriteBatch, new Vector2(295, offsetY + textToSkill.Key * 40));
                }
            }
        }
    }
}
