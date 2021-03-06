﻿using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using CountersPlus.Custom;
using Newtonsoft.Json.Converters;
using BS_Utils.Utilities;
using System.Threading.Tasks;
using System.Threading;

namespace CountersPlus.Config
{
    public class ConfigLoader
    {
        public static MainConfigModel loadSettings()
        {
            MainConfigModel model = new MainConfigModel();
            try
            {
                Plugin.Log(model.missedConfig.Enabled.ToString());
            }
            catch
            {
                ResetSettings(model, out model);
            }
            if (File.Exists(Environment.CurrentDirectory.Replace('\\', '/') + "/UserData/CountersPlus.json"))
            {
                try
                {
                    string json = File.ReadAllText(Environment.CurrentDirectory.Replace('\\', '/') + "/UserData/CountersPlus.json");
                    model = JsonConvert.DeserializeObject<MainConfigModel>(json);
                    File.Delete(Environment.CurrentDirectory.Replace('\\', '/') + "/UserData/CountersPlus.json");
                    Plugin.Log("Loaded JSON for the last time. Goodbye, JSON!");
                }
                catch (Exception e)
                {
                    Plugin.Log("Error loading JSON!", Plugin.LogInfo.Error);
                }
            }
            Plugin.Log("Config loaded.");
            return model;
        }

        internal static void ResetSettings(MainConfigModel inSettings, out MainConfigModel settings)
        {
            settings = inSettings;
            settings.Enabled = true;
            settings.DisableMenus = false;
            settings.ComboOffset = 0.2f;
            settings.MultiplierOffset = 0.4f;
            try
            {
                ResetSetting(settings.missedConfig, true, ICounterPositions.BelowCombo, 0);
            }
            catch
            {
                settings.missedConfig = new MissedConfigModel();
                settings.noteConfig = new NoteConfigModel();
                settings.progressConfig = new ProgressConfigModel();
                settings.scoreConfig = new ScoreConfigModel();
                settings.speedConfig = new SpeedConfigModel();
                settings.cutConfig = new CutConfigModel();
                ResetSetting(settings.missedConfig, true, ICounterPositions.BelowCombo, 0);
            }
            ResetSetting(settings.noteConfig, true, ICounterPositions.BelowCombo, 1);
            ResetSetting(settings.progressConfig, true, ICounterPositions.BelowEnergy, 0);
            ResetSetting(settings.scoreConfig, true, ICounterPositions.BelowMultiplier, 0);
            ResetSetting(settings.speedConfig, false, ICounterPositions.AboveMultiplier, 0);
            ResetSetting(settings.cutConfig, false, ICounterPositions.AboveHighway, 0);
        }

        private static void ResetSetting<T>(T model, bool en, ICounterPositions pos, int index) where T : IConfigModel
        {
            model.Enabled = en;
            model.Position = pos;
            model.Index = index;
        }
    }
    
    public class MainConfigModel {
        public bool Enabled { get {
                return Plugin.config.GetBool("Main", "Enabled", true, true);
            } set {
                Plugin.config.SetBool("Main", "Enabled", value);
            } }
        public bool DisableMenus
        {
            get
            {
                return Plugin.config.GetBool("Main", "DisableMenus", true, true);
            }
            set
            {
                Plugin.config.SetBool("Main", "DisableMenus", value);
            }
        }
        public float ComboOffset {
            get
            {
                return Plugin.config.GetFloat("Main", "ComboOffset", 0.2f, true);
            }
            set
            {
                Plugin.config.SetFloat("Main", "ComboOffset", value);
            }
        }
        public float MultiplierOffset {
            get
            {
                return Plugin.config.GetFloat("Main", "MultiplierOffset", 0.2f, true);
            }
            set
            {
                Plugin.config.SetFloat("Main", "MultiplierOffset", value);
            }
        }
        public MissedConfigModel missedConfig;
        public NoteConfigModel noteConfig;
        public ProgressConfigModel progressConfig;
        public ScoreConfigModel scoreConfig;
        //public PBConfigModel pBConfig;
        public SpeedConfigModel speedConfig;
        public CutConfigModel cutConfig;
    }

    public abstract class IConfigModel {
        public string DisplayName { get; internal set; }
        public virtual bool Enabled { get {
                return Plugin.config.GetBool(DisplayName, "Enabled", true, true);
            }set{  Plugin.config.SetBool(DisplayName, "Enabled", value); } }
        [JsonConverter(typeof(StringEnumConverter))]
        public virtual ICounterPositions Position { get {
                return (ICounterPositions)Enum.Parse(typeof(ICounterPositions), Plugin.config.GetString(DisplayName, "Position", "BelowCombo", true));
            } set {
                Plugin.config.SetString(DisplayName, "Position", value.ToString());
            } }
        public virtual int Index { get{
                return Plugin.config.GetInt(DisplayName, "Index", 0, true);
            }
            set { Plugin.config.SetInt(DisplayName, "Index", value); }
        }
    }

    public sealed class MissedConfigModel : IConfigModel {
        public MissedConfigModel() { DisplayName = "Missed"; }
    }

    public sealed class NoteConfigModel : IConfigModel {
        public NoteConfigModel() { DisplayName = "Notes"; }
        public bool ShowPercentage
        {
            get
            {
                return Plugin.config.GetBool(DisplayName, "ShowPercentage", true, true);
            }
            set { Plugin.config.SetBool(DisplayName, "ShowPercentage", value); }
        }
        public int DecimalPrecision
        {
            get
            {
                return Plugin.config.GetInt(DisplayName, "DecimalPrecision", 2, true);
            }
            set { Plugin.config.SetInt(DisplayName, "DecimalPrecision", value); }
        }
    }

    public sealed class ProgressConfigModel : IConfigModel {
        public ProgressConfigModel() { DisplayName = "Progress"; }
        public ICounterMode Mode
        {
            get
            {
                return (ICounterMode)Enum.Parse(typeof(ICounterMode), Plugin.config.GetString(DisplayName, "Mode", "Original", true));
            }
            set
            {
                Plugin.config.SetString(DisplayName, "Mode", value.ToString());
            }
        }
        public bool ProgressTimeLeft
        {
            get
            {
                return Plugin.config.GetBool(DisplayName, "ProgressTimeLeft", false, true);
            }
            set { Plugin.config.SetBool(DisplayName, "ProgressTimeLeft", value); }
        }
    }

    public sealed class ScoreConfigModel : IConfigModel
    {
        public ScoreConfigModel() { DisplayName = "Score"; }
        public bool UseOld
        {
            get
            {
                return Plugin.config.GetBool(DisplayName, "UseOld", false, true);
            }
            set { Plugin.config.SetBool(DisplayName, "UseOld", value); }
        }
        public int DecimalPrecision
        {
            get
            {
                return Plugin.config.GetInt(DisplayName, "DecimalPrecision", 2, true);
            }
            set { Plugin.config.SetInt(DisplayName, "DecimalPrecision", value); }
        }
        public bool DisplayRank
        {
            get
            {
                return Plugin.config.GetBool(DisplayName, "DisplayRank", true, true);
            }
            set { Plugin.config.SetBool(DisplayName, "DisplayRank", value); }
        }
    }

    public sealed class PBConfigModel : IConfigModel{
        public PBConfigModel() { DisplayName = "PB"; }
        public int DecimalPrecision
        {
            get
            {
                return Plugin.config.GetInt(DisplayName, "DecimalPrecision", 0, true);
            }
            set { Plugin.config.SetInt(DisplayName, "DecimalPrecision", value); }
        }
    }

    public sealed class SpeedConfigModel : IConfigModel
    {
        public SpeedConfigModel() { DisplayName = "Speed"; }
        public int DecimalPrecision
        {
            get
            {
                return Plugin.config.GetInt(DisplayName, "DecimalPrecision", 2, true);
            }
            set { Plugin.config.SetInt(DisplayName, "DecimalPrecision", value); }
        }
        public ICounterMode Mode
        {
            get
            {
                return (ICounterMode)Enum.Parse(typeof(ICounterMode), Plugin.config.GetString(DisplayName, "Mode", "Average", true));
            }
            set
            {
                Plugin.config.SetString(DisplayName, "Mode", value.ToString());
            }
        }
    }

    public class CutConfigModel : IConfigModel {
        public CutConfigModel() { DisplayName = "Cut"; }
    }
    
    public enum ICounterPositions { BelowCombo, AboveCombo, BelowMultiplier, AboveMultiplier, BelowEnergy, AboveHighway }

    public enum ICounterMode { Average, Top5Sec, Both, SplitAverage, SplitBoth, //Speed
                              BaseGame, Original, Percent //Progress
    };
}
