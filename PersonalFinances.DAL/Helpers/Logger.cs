namespace PersonalFinances.DAL.Helpers
{
    using System;
    using System.IO;

    namespace FocusTrack.DAL.Helpers
    {
        public enum LogStatus
        {
            Success = 1,
            Error = 2,
            Info = 3
        }

        public static class Logger
        {
            private static readonly object _lock = new object();
            private static readonly string _logFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            private static readonly TimeSpan _maxLogAge = TimeSpan.FromDays(15);

            static Logger()
            {
                Directory.CreateDirectory(_logFolderPath);
            }

            public static void WriteLog(string message, LogStatus status = LogStatus.Info)
            {
                lock (_lock)
                {
                    string logFile = Path.Combine(_logFolderPath, $"{DateTime.Now:dd-MM-yyyy}.txt");
                    string statusString = status.ToString().ToUpper();

                    using (StreamWriter sw = new StreamWriter(logFile, true))
                    {
                        sw.WriteLine($"{DateTime.Now} [{statusString}] : {message}");
                    }

                    DeleteOldLogs();
                }
            }

            private static void DeleteOldLogs()
            {
                foreach (var logFile in Directory.GetFiles(_logFolderPath, "*.txt"))
                {
                    DateTime fileCreationTime = File.GetCreationTime(logFile);
                    if (DateTime.Now - fileCreationTime > _maxLogAge)
                    {
                        File.Delete(logFile);
                    }
                }
            }
        }
    }
}
