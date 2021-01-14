using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;

namespace ExperienceUpdates
{
    public class ModEntry : Mod
    {
        private const int NUMBER_OF_SKILLS = 6;
        private ExperienceCalculator calculator;
        private TextRenderer renderer;
        public static Configuration Config;

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<Configuration>();

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
            helper.Events.Display.RenderedHud += OnRenderedHud;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var capi = Helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
            if (capi != null)
            {
                capi.RegisterModConfig(ModManifest, () => Config = new Configuration(), () => Helper.WriteConfig(Config));
                capi.RegisterSimpleOption(ModManifest, "X coordinate", "Pixels from the left side of the screen if positive. From the right side if negative", () => Config.X, (int val) => Config.X = val);
                capi.RegisterSimpleOption(ModManifest, "Y coordinate", "Pixels from the top of the screen if positive. From the bottom of the screen if negative", () => Config.Y, (int val) => Config.Y = val);
                capi.RegisterSimpleOption(ModManifest, "Animation duration", "Time in milliseconds for the numbers animation to perform. 4000 is 4 seconds", () => Config.TextDurationMS, (int val) => Config.TextDurationMS = val);
            }
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
            Renderer().Render(textsToRender, NUMBER_OF_SKILLS);
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
