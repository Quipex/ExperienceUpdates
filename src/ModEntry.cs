using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace ExperienceUpdates
{
    public class ModEntry : Mod
    {
        private ExperienceCalculator calculator;
        private TextRenderer renderer;

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.ReturnedToTitle += this.OnReturnedToTitle;
            helper.Events.Display.RenderedHud += this.OnRenderedHud;
        }

        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            Monitor.Log("Went to title, stopping");
            Calculator().Stop();
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            Monitor.Log("Save loaded, resetting counters");
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
