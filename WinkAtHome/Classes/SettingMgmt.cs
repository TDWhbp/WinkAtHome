﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Web;

namespace WinkAtHome
{
    public class SettingMgmt
    {
        private static string dbPath = Common.dbPath;
        
        public class Setting
        {
            public string key;
            public string value;
            public bool isEncrypted = false;
            public bool isRequired = false;
        }
        public static List<Setting> Settings
        {
            get
            {
                return loadSettings();
            }
            set
            {
                _settings = value;
            }
        }
        private static List<Setting> _settings;

        public static string getSetting(string KeyName)
        {
            try
            {
                Setting setting = Settings.SingleOrDefault(s => s.key.ToLower().Equals(KeyName.ToLower()));
                if (setting != null)
                    return setting.value;
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw ex; //EventLog.WriteEntry("WinkAtHome.SettingsMgmt.getSetting", ex.Message, EventLogEntryType.Error);
                return null;
            }
        }

        public static List<Setting> loadSettings(bool forceReset = false)
        {
            try
            {
                if (_settings == null || forceReset)
                {
                    List<Setting> settings = new List<Setting>();

                    using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + dbPath + ";Version=3;"))
                    {
                        string sql = "select * from Settings";

                        connection.Open();
                        SQLiteCommand command = new SQLiteCommand(sql, connection);
                        SQLiteDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            Setting setting = new Setting();
                            setting.key = reader["name"].ToString();
                            string isEncrypted = reader["IsEncrypted"].ToString();
                            string isRequired = reader["isRequired"].ToString();

                            if (reader.IsDBNull(reader.GetOrdinal("value")) || string.IsNullOrWhiteSpace(reader["value"].ToString()))
                                setting.value = reader["defaultvalue"].ToString();
                            else
                            {
                                if (Convert.ToBoolean(isEncrypted))
                                    setting.value = Common.Decrypt(reader["value"].ToString());
                                else
                                    setting.value = reader["value"].ToString();
                            }

                            if (Convert.ToBoolean(isEncrypted))
                                setting.isEncrypted = true;
                            
                            if (Convert.ToBoolean(isRequired))
                                setting.isRequired = true;

                            settings.Add(setting);
                        }
                    }

                    _settings = settings;
                }
                return _settings;
            }
            catch (Exception ex)
            {
                throw ex; //EventLog.WriteEntry("WinkAtHome.SettingsMgmt.loadSettings", ex.Message, EventLogEntryType.Error);
                throw ex;
            }
        }

        public static void saveSetting(string key, string value)
        {
            try
            {
                Setting setting = Settings.SingleOrDefault(s => s.key == key);
                string newValue = (setting != null && setting.isEncrypted) ? Common.Encrypt(value) : value;
                using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + dbPath + ";Version=3;"))
                {

                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = "UPDATE Settings SET Value=@value WHERE name = @name;";
                        command.Parameters.Add(new SQLiteParameter("@name", key));
                        command.Parameters.Add(new SQLiteParameter("@value", newValue));
                        command.ExecuteNonQuery();

                        command.CommandText = "INSERT OR IGNORE INTO Settings(Name,Value) VALUES (@name,@value)";
                        command.ExecuteNonQuery();

                        command.CommandText = "DELETE FROM Settings WHERE IFNULL(Value, '') = '' and IFNULL(DefaultValue, '') = '' and IsEncrypted = 'false'";
                        command.ExecuteNonQuery();
                    }
                }

                loadSettings(true);
            }
            catch (Exception ex)
            {
                throw ex; //EventLog.WriteEntry("WinkAtHome.SettingsMgmt.saveSetting", ex.Message, EventLogEntryType.Error);
                throw ex;
            }
        }
        
        public static void saveManualEdit(string json)
        {
            try
            {
                JObject jsonTest = JObject.Parse(json);
                foreach (var jo in jsonTest)
                {
                    saveSetting(jo.Key, jo.Value.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex; //EventLog.WriteEntry("WinkAtHome.SettingsMgmt.saveSetting", ex.Message, EventLogEntryType.Error);
                throw ex;
            }
        }

        public static void wipeSettings()
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + dbPath + ";Version=3;"))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = "delete from Settings where defaultvalue is null";
                        command.ExecuteNonQuery();

                        command.CommandText = "update Settings set value = null";
                        command.ExecuteNonQuery();

                    }
                }

                loadSettings(true);
            }
            catch (Exception ex)
            {
                throw ex; //EventLog.WriteEntry("WinkAtHome.SettingsMgmt.wipeSettings", ex.Message, EventLogEntryType.Error);
            }

        }
    }
}