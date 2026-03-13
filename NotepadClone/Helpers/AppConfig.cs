using System;
using System.IO;

namespace NotepadClone.Helpers
{
    public static class AppConfig
    {
        
        private static readonly string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.txt");

        
        public static void SaveExplorerState(bool isVisible)
        {
            try
            {
                File.WriteAllText(ConfigPath, isVisible.ToString());
            }
            catch
            {
                
            }
        }

        
        public static bool LoadExplorerState()
        {
            if (File.Exists(ConfigPath))
            {
                try
                {
                    string content = File.ReadAllText(ConfigPath);
                    if (bool.TryParse(content, out bool result))
                    {
                        return result; 
                    }
                }
                catch
                {
                    
                }
            }

            return true; 
        }
    }
}