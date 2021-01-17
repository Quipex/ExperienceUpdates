using System;
using System.Reflection;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Microsoft.Xna.Framework.Input;
using SpaceShared.APIs;

namespace ExperienceUpdates
{
    public class ModEntry : Mod
    {
        private const int NUMBER_OF_SKILLS = 6;
        private ExperienceCalculator calculator;
        private TextRenderer renderer;
        public static Configuration Config;
        public static bool show;
        public static SButton ToggleText;

        public object instance;

        //285 and 10 are the "best" default distances for the Updates' text in relation to the upper-left most corner of the Experience Bars UI
        public static int EXPERIENCE_BARS_PIXELS_OFFSETX = 285;
        public static int EXPERIENCE_BARS_PIXELS_OFFSETY = 10;

        public override void Entry(IModHelper helper)
        {
            //Checks to see if Experience Bar is loaded/enabled, if not Updates mod doesn't load rest of mod
            if (Helper.ModRegistry.IsLoaded("spacechase0.ExperienceBars"))
            {
                Config = helper.ReadConfig<Configuration>();
                helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
                helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
                helper.Events.Display.RenderedHud += OnRenderedHud;

                helper.Events.GameLoop.GameLaunched += onGameLaunched;
                helper.Events.Input.ButtonPressed += onButtonPressed;

                //Getting Config information directly from Experience Bars
                try
                {
                    instance = Type.GetType("ExperienceBars.Mod, ExperienceBars").GetField("Config", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
                }
                catch (Exception ex)
                {
                    Monitor.Log("Exception during experience bar compat: " + ex, LogLevel.Error);
                }

            } else
            {
                Monitor.Log("WARNING: [Experience Bars] is not loaded. Please make sure it is downloaded and enabled in order to use this mod.", LogLevel.Warn);
                
            }

        }

        private void onGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            if (Helper.ModRegistry.IsLoaded("spacechase0.ExperienceBars"))
            {
                try
                {
                    //Setting Toggle Button for Updates to Experience Bars' toggle button and then sets visibility to EXP Bars' current setting
                    var toggleBtn = Helper.Reflection.GetProperty<SButton>(instance, "ToggleBars").GetValue();
                    ToggleText = toggleBtn;

                    ////This is so that when the mods are loaded, their visibility states are synced (ie both "Bars" and "Updates" being shown/hidden at the same time)
                    var shown = Helper.Reflection.GetField<bool>(Type.GetType("ExperienceBars.Mod, ExperienceBars"), "show").GetValue();
                    show = shown;

                    Monitor.Log($"Game Launched: {toggleBtn}/{ToggleText}, {shown}/{show}");
                }
                catch (Exception ex)
                {
                    Monitor.Log("Exception during experience bar compat: " + ex, LogLevel.Error);
                }

            }

            //This adds the Generic Mod Config Menu API to Experience Updates and allows updating mod settings without quitting game
            var capi = Helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
            if (capi != null)
            {
                capi.RegisterModConfig(ModManifest, () => Config = new Configuration(), () => Helper.WriteConfig(Config));
                capi.RegisterSimpleOption(ModManifest, "UI X Offset", "The offset of the X position of the text on-screen.", () => Config.offsetX, (int val) => Config.offsetX = val);
                capi.RegisterSimpleOption(ModManifest, "UI Y Offset", "The offset of the Y position of the text on-screen.", () => Config.offsetY, (int val) => Config.offsetY = val);
                capi.RegisterSimpleOption(ModManifest, "Text Duration", "The time in milliseconds for the numbers animation to perform (4000 = 4 seconds). ", () => Config.TextDurationMS, (int val) => Config.TextDurationMS = val);
            }
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        public void onButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            var toggleBtn = Helper.Reflection.GetProperty<SButton>(instance, "ToggleBars").GetValue();
            //When Toggle Button for Experience Bars is changed in Generic Mod Config Menu, this checks to make sure the new button is synced with Experience Updates
            if (Game1.player.currentLocation == null) {
                if (toggleBtn != ToggleText)
                {
                    Monitor.Log("Toggle Buttons not synced! Begin syncing...", LogLevel.Debug);
                    ToggleText = toggleBtn;
                }
            }

            if (e.Button == ToggleText)
            {
                if (show && (Game1.GetKeyboardState().IsKeyDown(Keys.LeftShift) || Game1.GetKeyboardState().IsKeyDown(Keys.RightShift)))
                {
                    Config.X = (int)e.Cursor.ScreenPixels.X + EXPERIENCE_BARS_PIXELS_OFFSETX + Config.offsetX;
                    Config.Y = (int)e.Cursor.ScreenPixels.Y + EXPERIENCE_BARS_PIXELS_OFFSETY + Config.offsetY;
                    Helper.WriteConfig(Config);
                }
                else
                {
                    show = !show;
                }
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

            var configX = Helper.Reflection.GetField<int>(instance, "X").GetValue();
            var configY = Helper.Reflection.GetField<int>(instance, "Y").GetValue();

            //Sets text positions according to EXP Bars position, ideal offsets, and player provided additional offsets
            Config.X = configX + EXPERIENCE_BARS_PIXELS_OFFSETX + Config.offsetX;
            Config.Y = configY + EXPERIENCE_BARS_PIXELS_OFFSETY + Config.offsetY;
            Helper.WriteConfig(Config);
            
            Monitor.Log($"Save Loaded...\nX: {configX}/{Config.X}, Y: {configY}/{Config.Y}");

            Calculator().Reset();
        }

        private void OnRenderedHud(object sender, RenderedHudEventArgs e)
        {
            if (!show || Game1.activeClickableMenu != null || Game1.eventUp || !Context.IsPlayerFree)
                return;

            var textsToRender = Calculator().GetUpdatableTexts();
            Renderer().Render(textsToRender, NUMBER_OF_SKILLS, EXPERIENCE_BARS_PIXELS_OFFSETX, EXPERIENCE_BARS_PIXELS_OFFSETY);
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
