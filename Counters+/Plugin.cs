﻿using IllusionPlugin;
using System;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using BS_Utils.Utilities;

namespace CountersPlus
{
    public class Plugin : IPlugin
    {
        public string Name => "Counters+";
        public string Version => "1.3.0";
        public enum LogInfo { Info, Warning, Error, Fatal };
        internal static BS_Utils.Utilities.Config config = new BS_Utils.Utilities.Config("CountersPlus"); //Conflicts with CountersPlus.Config POG

        public void OnApplicationStart()
        {
            if (!File.Exists(Environment.CurrentDirectory.Replace('\\', '/') + "/UserData/CountersPlus.ini"))
                File.Create(Environment.CurrentDirectory.Replace('\\', '/') + "/UserData/CountersPlus.ini");
            SceneManager.activeSceneChanged += SceneManager_sceneLoaded;
            SceneManager.sceneLoaded += addUI;
            CountersController.OnLoad();
        }

        private void SceneManager_sceneLoaded(Scene arg0, Scene arg1)
        {
            //if (CountersController.settings.Enabled) CountersController.OnLoad();
            if (arg1.name == "GameCore" &&
                CountersController.settings.Enabled &&
                (!Resources.FindObjectsOfTypeAll<PlayerDataModelSO>()
                    .FirstOrDefault()?
                    .currentLocalPlayer.playerSpecificSettings.noTextsAndHuds ?? true)
                )
            {
                CountersController.LoadCounters();
            }
        }

        private void addUI(Scene arg, LoadSceneMode hiBrian)
        {
            try
            {
                if (arg.name == "Menu") CountersSettingsUI.CreateSettingsUI();
            }catch(Exception e)
            {
                Log(e.ToString(), LogInfo.Fatal);
            }
        }

        public void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= SceneManager_sceneLoaded;
            SceneManager.sceneLoaded -= addUI;
        }

        public void OnLevelWasLoaded(int level) { }
        public void OnLevelWasInitialized(int level) { }
        public void OnUpdate() { }
        public void OnFixedUpdate() { }

        public static void Log(string m)
        {
            Log(m, LogInfo.Info);
        }

        public static void Log(string m, LogInfo l)
        {
            Console.WriteLine("Counters+ [" + l.ToString() + "] | " + m);
            if (l == LogInfo.Fatal)
            {
                Console.WriteLine("Counters+ [IMPORTANT] | Contact Caeden117#0117 on Discord with this issue!");
            }
        }
    }
}
