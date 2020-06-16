using System;
using System.IO;
using Console;
using UnityEngine;
using Utilities;

namespace Management
{
    public class Settings
    {
        public static Settings instance = null;

        // Game session setting
        private bool _shareGameSession = true; 
        public bool shareGameSession
        {
            get => _shareGameSession;
            set { _shareGameSession = value; SaveSettings(); }
        }
        
        // Fullscreen setting
        private bool _fullscreen = false;
        public bool fullscreen
        {
            get => _fullscreen;
            set { _fullscreen = value; SaveSettings(); }
        }

        public Settings()
        {
            if (instance == null || instance.Equals(null)) instance = this;
            else return;
            
            try { LoadSettings(); GlobalConsole.Log("Setting file loaded"); }
            catch (Exception e)
            {
                if (e is FileNotFoundException) GlobalConsole.Error("Setting file not found. Generating new!");
                if (e is FileLoadException) GlobalConsole.Error("Setting file couldn't be loaded. Generating new!");
                SaveSettings();
            }
        }
        
        public static void LoadSettings() => instance = JsonUtility.FromJson<Settings>(File.ReadAllText(Globals.settingPath));
        public static void SaveSettings() => File.WriteAllText(Globals.settingPath,  JsonUtility.ToJson(instance));
        
        
        
    }
}