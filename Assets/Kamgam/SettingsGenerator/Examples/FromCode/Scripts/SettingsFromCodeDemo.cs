using System.Collections.Generic;
using UnityEngine;

namespace Kamgam.SettingsGenerator.Examples
{
    // This class contains a LOT of comments to explain the code API.
    //
    // Many things shown here could be written in shorter code. It is
    // written in a lengthy style on purpose to keep things as simple
    // to understand as possible.
    //
    // NOTICE: While the ScriptAPI can do anything the ScriptableObject
    // based workflow can I encourage you to use the ScriptableObjects
    // instead. The reasons are:
    // 1) Iterating settings in code is rather slow (re-compile).
    // 2) The Settings Asset has a neat Editor UI which visualizes
    //    data nicely (colors are hard to interpret if seen in code).
    // 3) Assets can be changed by none programmers too (be kind
    //    to your artists).

    public class SettingsFromCodeDemo : MonoBehaviour
    {
        /// <summary>
        /// The SettingsProvider ScriptableObject is always needed. It serves as the
        /// access point for anything related to the settings. Simply drag in your
        /// SettingsProvider asset.
        /// </summary>
        public SettingsProvider Provider;

        public void Awake()
        {
            // A call to the .Settings property of the provider will automatically
            // create and (synchronously) LOAD the a Settings object. In this case
            // we have no Settings set in the provider. Open the "SettingsProvider (FromCode).asset"
            // and you'll notice the "Settings Asset" is left empty.

            var settings = Provider.Settings;

            // At this point the settings are already loaded. A setting will be created
            // automatically for each SettingData entry found in the loaded data. Yet these
            // settings still only are pure data. They need to be hooked up with the code
            // which reacts to setting changes.

            // Look into these methods. They contain detailed descriptions.
            addEnableBossFightsBoolean(settings);
            addHealthRegenerationPercentage(settings);

            // Difficulty
            // An options setting (with a little shorter code this time).
            var difficulties = new List<string>() { "Easy", "Normal", "Hard" };
            int defaultSelectedIndex = 1; // Normal is the default setting.
            var difficultySetting = settings.GetOrCreateOption("difficulty", defaultSelectedIndex, options: difficulties);
            // Instead of connections let's use a listener this time.
            difficultySetting.AddChangeListener((selectedIndex) => Debug.Log($"Selected difficulty is {difficulties[selectedIndex]}."));

            // Team color picker
            var teamColorSetting = settings.GetOrCreateColorOption("teamColor", 0);
            // You may wonder why there are not options defined.
            // Take a look at the UI in the "SettingsFromCodeDemo UI (loaded additive)" scene.
            // There you will find that there are some "ColorPickerButtonUGUI" buttons within
            // the "ColorPickerButtonUGUI" button.
            // If no options are defined in the setting data then the SettingResolver will try to
            // derive options from the UI (ColorPickerButtonUGUI buttons in this case).
            teamColorSetting.AddChangeListener((selectedColor) => Debug.Log($"Selected team color is {teamColorSetting.GetOptionLabels()[selectedColor]}."));

            // Opponent color
            var opponentsColors = new List<Color> { Color.red, Color.green, Color.blue };
            var opponentColor = settings.GetOrCreateColorOption("opponentColor", 0, options: opponentsColors);
            opponentColor.AddChangeListener((selectedColor) => Debug.Log($"Opponent color is {opponentColor.GetOptionLabels()[selectedColor]}."));

            // Audio enable/disable (start as disabled)
            // This is an example of using a predefined connection (there are many, they are listed in the manual).
            // If you want to implement your own connection just take a look into the existing ones.
            var audioEnabledSetting = settings.GetOrCreateBool("audioEnabled", connection: new AudioPausedConnection());

            // Thus far we have loaded and defined some new settings. As a final step we need them
            // to actually do something.
            // Right now the loaded settings data is stored in the setting objects but they have not
            // yet been applied (nothing in your game has changed yet).
            // To do this we need to call Apply() on each setting after load. There is a handy method
            // to do this for all settings. Apply() will call PushToConnection() and PullFromConnection()
            // on every single setting.
            //
            // Intially we want all connections to apply, not matter if their value has changed.
            // You could call MarkAsChanged() and then Apply() on each setting by hand too but this
            // is more convenient.
            settings.Apply(changedOnly: false);

            // If you add a new setting at runtime after this then don't forget to call MarkAsChanged()
            // and Apply() on that new setting.
        }

        private static void addEnableBossFightsBoolean(Settings settings)
        {
            // First let's add the simplest setting possible (a boolean that does nothing).
            settings.GetOrCreateBool(
                // Each setting needs an ID. This ID is used in variouos places to
                // find the setting. Most importantly it is used in the UI (SettingResolver).
                id: "enableBossFights",

                // This is the default value this setting will reset to if setting.ResetToDefault()
                // is called. This is also the initial value of the setting at first boot.
                defaultValue: true
                );

            // That's it. If you look in Update() you see we now can log the current value of this setting.

            // As a seasoned programmer you are most likely not thrilled by the idea of having to
            // pull the setting values. The next setting ("healthRegeneration") will use some
            // callback to react to changes instead.
        }



        #region Health Regeneration

        /// <summary>
        /// A simple int for a slider from 0 to 100 (the range is defined in the UI).
        /// It defines a percentage of how strong the health regeneration should be.
        /// </summary>
        protected int _healthRegeneration;

        protected void addHealthRegenerationPercentage(Settings settings)
        {
            // This setting will react to changes in the setting and propagate those
            // to a local field "_healthRegeneration".
            //
            // To connect a setting (data) with some logic we use Connection objects.
            // These are very simple. They have a Get() and a Set(value) method.
            //
            // Get() means getting a value from the connection and saving it within the setting (pull).
            // Set(value) means sending a new value to the connection (push).
            // There are many specialized predefined Connections (like the "FrameRateConnection").
            //
            // For this example we use a simple generic GetSetConnection<T> connection.
            // This connection does nothing except forward Get() and Set(value) calls to
            // some other methods (getter and setter).

            var connection = new GetSetConnection<int>(
                getter: getHealthRegeneration, // executed if connection.Get() is called.
                setter: setHealthRegeneration  // executed if connection.Set(value) is called.
            );

            // Now that we have our connection we need to hook it up with our setting.
            // In fact at first boot we also have to create our setting. Luckily there
            // is a handy GetOrCreateInt() method so we don't have to worry about that.
            var healthSetting = settings.GetOrCreateInt(

                // Each setting needs an ID.
                id: "healthRegeneration",

                // The default value is the fallback default value used if no connection is
                // set. In this case we are using a connection and the default value is pulled
                // initially from that connection. Therefore we actually don't need to specify it.
                // defaultValue: false

                // We want to use a connection to get/set the values.
                connection: connection
            );

            // If all you need is to listen for changes in a setting then you may not even need a
            // Connection object. There is a healthSetting.AddChangeListener() method which you
            // can use for that.
        }

        // This simply returns the current state of "_healthRegeneration".
        // This getter is called at the very first use of the setting
        // and the return value will be stored as the default value (used
        // if you call setting.ResetToDefault()).
        //
        // It may also be called at any time by the settings system and
        // should return the current state of the value in your game.
        //
        // If this value is changed from outside the settings system, then
        // you need to call setting.PullFromConnection() to update the interal
        // value of the setting.
        //
        // "Pull" in this context is meant as from the view point of the setting.
        // I.e. "pull the value from the connection into the setting and update the UI".
        // During this pull process connection.Get() is called which calls
        // this getter.
        //
        // There is also a setting.PushToConnection() method which does to opposite.
        // It pushes the value from the setting into the connection (connection.Set())
        protected int getHealthRegeneration()
        {
            return _healthRegeneration;
        }

        // This simply sets the local field and logs the new value.
        protected void setHealthRegeneration(int value)
        {
            _healthRegeneration = value;
            Debug.Log("Health regeneration has been set to: " + value);
        }
        #endregion


        #region logging
        float _logTimer;

        public void Update()
        {
            // Execute once every two seconds
            _logTimer += Time.deltaTime;
            if(_logTimer > 2f)
            {
                _logTimer = 0f;

                // Log enableBossFights
                SettingBool enableBossFightsSetting = Provider.Settings.GetBool("enableBossFights");
                bool enableBossFights = enableBossFightsSetting.GetValue();
                Debug.Log("Enable Boss Fights is: " + enableBossFights + " (time: " + Time.realtimeSinceStartup + ")");
            }
        }
        #endregion
    }
}
