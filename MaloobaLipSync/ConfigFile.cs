using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MaloobaLipSync
{
    /// <summary>
    /// Save and restore application configuration in a local file
    /// The file contains a JSON object with string valued properties 
    /// </summary>
    internal class ConfigFile
    {
        private readonly string configPath;
        private JObject config;

        /// <summary>
        /// Access configuration properties by name
        /// null is returned for non-existent properties
        /// Setting a property to null removes it
        /// </summary>
        /// <param name="name">Config property name</param>
        /// <returns>Config property value reference</returns>
        public string this[string name]
        {
            get { return (string)config[name]; }
            set
            {
                if(name.All(char.IsLetter))
                {
                    if(value == null)
                        config.Remove(name);
                    else
                        config[name] = value;
                }
                else
                {
                    throw new ApplicationException("Invalid setting name");
                }
            }
        }

        /// <summary>
        /// Open and restore configuration
        /// </summary>
        public ConfigFile(string applicationName, string fileName)
        {
            var configDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), applicationName);
            Directory.CreateDirectory(configDir);
            configPath = Path.Combine(configDir, fileName);
            Restore();
        }

        /// <summary>
        /// Restore configuration from file
        /// </summary>
        public void Restore()
        {
            try
            {
                var json = File.ReadAllText(configPath);
                config = JObject.Parse(json);
                if(config.Properties().Any(p => p.Value.Type != JTokenType.String))
                    throw new Exception();
            }
            catch(FileNotFoundException)
            {
                Clear();
            }
            catch
            {
                // MessageBox.Show("Settings are corrupted - Ignoring", "Warning", MessageBoxButton.OK);
                Clear();
            }
        }

        /// <summary>
        /// Save configuration to file
        /// </summary>
        public void Save()
        {
            File.WriteAllText(configPath, config.ToString(Formatting.Indented));
        }

        /// <summary>
        /// Clear all in-memory configuration
        /// </summary>
        public void Clear()
        {
            config = new JObject();
        }
    }
}
