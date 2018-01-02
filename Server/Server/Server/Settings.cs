using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Server {
    // An object that is used as an interface to the settings files
    public class Settings {
        protected static Settings instance; // The instance of the singleton
        protected string settingsLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\settings.cfg"; // A string of the location of the settings file
        protected string defaultSettingsLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\defaultsettings.cfg"; // A string of the location of the default settings file
        protected Dictionary<string, string> settingsDict = new Dictionary<string, string>(); // A dictionary that holds the settings from the settings file
        protected Dictionary<string, string> tempSettingsDict = new Dictionary<string, string>(); // A dictionary that holds the temporary settings that will not be saved
        protected Semaphore readLock = new Semaphore(10, 10); // A semaphore used to make sure that there are no reading threads running when a writing thread is running
        protected Mutex writeLock = new Mutex(); // A mutex used to make sure that no writing threads are running at the same time

        protected Settings() {
            if (File.Exists(settingsLocation))
                LoadSettings();
            else if (File.Exists(defaultSettingsLocation)) {
                LoadSettings(defaultSettingsLocation);
                UpdateSettingsFile();
            } else
                Printer.Print("No settings files found. The program will likely crush soon");
        }

        // Returns the instance of the singleton, creates a new one if there isn't one
        public static Settings Instance {
            get {
                if (instance == null) {
                    instance = new Settings();
                }
                return instance;
            }
        }

        // Returns the requested setting
        public string GetSetting(string key) {
            string setting;
            if (settingsDict.TryGetValue(key, out setting))
                return setting;
            return null;
        }

        // Sets the requested setting to the given value
        public bool SetSetting(string key, string value) {
            if (settingsDict.ContainsKey(key)) {
                settingsDict.Add(key, value);
                UpdateSettingsFile();
                return true;
            }
            return false;
        }

        // Returns whether the setting key exists
        public bool ContainsSettingKey(string key) {
            return settingsDict.ContainsKey(key);
        }

        // Loads the settings from the settings file to the settings dictionary
        protected void LoadSettings(string settingsLocation) {
            char[] settingSeperators = new char[] { '=' };
            string[] newLineSeperators = new string[] { "\r\n" };
            writeLock.WaitOne();
            readLock.WaitOne();
            writeLock.ReleaseMutex();
            StreamReader settingsFile = File.OpenText(settingsLocation);
            string[] settings_list = settingsFile.ReadToEnd().Split(newLineSeperators, StringSplitOptions.RemoveEmptyEntries);
            settingsFile.Close();
            readLock.Release();
            foreach (string setting in settings_list) {
                if (setting != "")
                    if (!setting.StartsWith("#")) {
                        string[] splitSetting = setting.Split(settingSeperators, 2);
                        if (splitSetting.Length == 2) {
                            splitSetting[1] = splitSetting[1].Replace("<CRLF>", "\r\n");
                            settingsDict.Add(splitSetting[0], splitSetting[1]);
                        }
                    }
            }
            LoadTempSettings();
        }

        // Calls LoadSettings() with the settings file location
        protected void LoadSettings() {
            LoadSettings(settingsLocation);
        }

        // Writes the current settings to the settings file
        protected void UpdateSettingsFile() {
            string newSettingsText = "";
            StreamReader settingsTemplate = File.OpenText(defaultSettingsLocation);
            string[] settingsList = settingsTemplate.ReadToEnd().Split('\n');
            settingsTemplate.Close();
            foreach (string setting in settingsList) {
                if (setting != "") {
                    if (setting[0] == '#')
                        newSettingsText += setting;
                    else {
                        string[] splitSetting = setting.Split('=');
                        string value;
                        settingsDict.TryGetValue(splitSetting[0], out value);
                        newSettingsText += string.Format("{0}={1}", splitSetting[0], value);
                    }
                }
                newSettingsText += "\r\n";
            }
            newSettingsText = newSettingsText.Substring(0, newSettingsText.Length - 2);
            writeLock.WaitOne();
            for (int i = 0; i < 10; i++)
                readLock.WaitOne();
            StreamWriter settingsFile = new StreamWriter(settingsLocation);
            settingsFile.Write(newSettingsText);
            settingsFile.Close();
            for (int i = 0; i < 10; i++)
                readLock.Release();
            writeLock.ReleaseMutex();
        }

        // Returns the requested temporary setting
        public string GetTempSetting(string key) {
            string setting;
            if (tempSettingsDict.TryGetValue(key, out setting))
                return setting;
            return null;
        }

        // Sets the requested temporary setting to the given value
        public bool SetTempSetting(string key, string value) {
            if (tempSettingsDict.ContainsKey(key)) {
                tempSettingsDict.Add(key, value);
                return true;
            }
            return false;
        }

        // Returns whether the temporary setting key exists
        public bool ContainsTempSettingKey(string key) {
            return tempSettingsDict.ContainsKey(key);
        }

        // Loads the settings from the settings dictionary to the temporary settings dictionary
        protected void LoadTempSettings() {
            tempSettingsDict = new Dictionary<string, string>(settingsDict);
        }
    }
}
