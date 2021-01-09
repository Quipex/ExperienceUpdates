﻿using Microsoft.Xna.Framework;

namespace ExperienceUpdates
{
    class SkillColorHelper
    {
        private static Color[] skillColors;

        public static Color GetSkillColor(int skill)
        {
            if (skillColors != null)
                return skillColors[skill];

            // https://github.com/spacechase0/ExperienceBars
            skillColors = new Color[]
            {
                new Color( 115, 255, 56 ),
                new Color( 117, 225, 255 ),
                new Color( 0xCD, 0x7F, 0x32 ),
                new Color( 247, 31, 0 ),
                new Color( 178, 255, 211 ),
                new Color( 255, 255, 84 ),
            };

            return skillColors[skill];
        }
    }
}
