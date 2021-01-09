using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;

namespace ExperienceUpdates
{
    public class ModEntry : Mod
    {
        private ExperienceCalculator calculator;
        private TextRenderer renderer;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.Display.RenderedHud += this.OnRenderedHud;
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            Calculator().Reset();
        }

        private void OnRenderedHud(object sender, RenderedHudEventArgs e)
        {
            var textsToRender = Calculator().GetUpdatableTexts();
            Renderer().Render(textsToRender);
        }

        private ExperienceCalculator Calculator()
        {
            if (calculator == null)
            {
                calculator = new ExperienceCalculator(Monitor);
            }
            return calculator;
        }

        private TextRenderer Renderer()
        {
            if (renderer == null)
            {
                renderer = new TextRenderer();
            }
            return renderer;
        }
    }
}