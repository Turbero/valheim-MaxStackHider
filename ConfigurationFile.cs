using BepInEx.Configuration;
using BepInEx;
using System.IO;

namespace MaxStackHider
{
    internal class ConfigurationFile
    {
        public static ConfigEntry<bool> debug;
        public static ConfigEntry<string> maxQuantityText;
        public static ConfigEntry<float> delayRefreshNumbers;

        private static ConfigFile configFile;
        private static string ConfigFileName = MaxStackHider.GUID + ".cfg";
        private static string ConfigFileFullPath = Paths.ConfigPath + Path.DirectorySeparatorChar + ConfigFileName;

        internal static void LoadConfig(BaseUnityPlugin plugin)
        {
            {
                configFile = plugin.Config;

                debug = configFile.Bind("1 - General", "DebugMode", false, "Enabling/Disabling the debugging in the console (default = false)");
                delayRefreshNumbers = configFile.Bind("1 - General", "DelayRefreshNumbers", 0.05f, "Necessary forced delay to refresh stack numbers. Only change under your own risk (default = 0.1f)");
                maxQuantityText = configFile.Bind("2 - Language", "MaxQuantityText", "Max Stack Size", "Translated text to display in tooltip for Max Stack Size");
                SetupWatcher();
            }
        }
        
        private static void SetupWatcher()
        {
            FileSystemWatcher watcher = new FileSystemWatcher(Paths.ConfigPath, ConfigFileName);
            watcher.Changed += ReadConfigValues;
            watcher.Created += ReadConfigValues;
            watcher.Renamed += ReadConfigValues;
            watcher.IncludeSubdirectories = true;
            watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
            watcher.EnableRaisingEvents = true;
        }
        
        private static void ReadConfigValues(object sender, FileSystemEventArgs e)
        {
            if (!File.Exists(ConfigFileFullPath)) return;
            try
            {
                Logger.Log("Attempting to reload configuration...");
                configFile.Reload();
            }
            catch
            {
                Logger.LogError($"There was an issue loading {ConfigFileName}");
            }
        }
    }
}