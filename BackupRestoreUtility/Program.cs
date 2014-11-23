namespace BackupRestoreUtility
{
    using System;
    using System.Diagnostics;
    using System.IO;

    public class Program
    {
        //Arguments:
        //Backup:   /b <dbpath> <backuppath>
        //Restore:  /r <dbpath> <restorepath> <processname>

        private static void Main(string[] args)
        {
            if (args == null)
                return;

            if (args[0] == "/b")
                PerformBackup(TrimQuotes(args[1]), TrimQuotes(args[2]));
            else if (args[0] == "/r")
            {
                KillProcess(args[3]);
                PerformRestore(TrimQuotes(args[1]), TrimQuotes(args[2]));
                RunProcess(args[3]);
            }

            Console.WriteLine("Operation complete.");

            if (Debugger.IsAttached)
                Console.ReadLine();
        }

        private static void PerformBackup(string backupPath, string destination)
        {
            if (!Directory.Exists(Path.GetDirectoryName(destination)))
                Directory.CreateDirectory(destination);

            try
            {
                File.Copy(backupPath, destination, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private static void PerformRestore(string backupPath, string restorePath)
        {
            try
            {
                File.Copy(backupPath, restorePath, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private static void KillProcess(string executablePath)
        {
            string processName = Path.GetFileNameWithoutExtension(executablePath);

            foreach (Process proc in Process.GetProcessesByName(processName))
            {
                try
                {
                    proc.Kill();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }

        private static void RunProcess(string executablePath)
        {
            try
            {
                Process.Start(executablePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static string TrimQuotes(string input)
        {
            return input.Trim(char.Parse(@""""));
        }
    }
}