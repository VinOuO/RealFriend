using System;
using System.IO;

namespace Aishizu.Native
{
    public static class aszLogger
    {
        private static readonly string s_LogPath;

        static aszLogger()
        {
            // Log file under Documents\AishizuLogs (auto-created)
            string folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "AishizuLogs"
            );
            Directory.CreateDirectory(folder);

            s_LogPath = Path.Combine(folder, "Aishizu.Native.log");

            // Optional: rotate old log
            if (File.Exists(s_LogPath))
            {
                string backup = s_LogPath.Replace(".log", $"_{DateTime.Now:yyyyMMdd_HHmmss}.bak");
                File.Move(s_LogPath, backup);
            }

            WriteLine("==== Aishizu.Native log started ====");
        }

        public static void WriteLine(string message)
        {
            try
            {
                File.AppendAllText(
                    s_LogPath,
                    $"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}"
                );
            }
            catch (Exception ex)
            {
                // Fallback: last resort to console
                Console.WriteLine("[aszLogger Error] " + ex.Message);
            }
        }
    }
}
