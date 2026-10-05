using System;
using System.IO;

namespace MyNamespace
{
    class MyClassCS
    {
        static void Main()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            using var watcher = new FileSystemWatcher($@"{currentDirectory}\alert-simulator\alert-simulator");
            watcher.InternalBufferSize = 65536;

            watcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;

            watcher.Created += OnCreated;

            watcher.Filter = "*.ready";
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;

            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
        }

        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            //string value = $"Created: {e.FullPath.Replace("alert.ready", "")}";
            var directoryPath = e.FullPath.Replace("alert.ready", "");
            File.Delete(e.FullPath);

            string[] message = Directory.GetFiles(directoryPath);
            var alert = File.ReadAllText(message[0]);
            Console.WriteLine(alert.GetType());
            //_logger.LogInformation($"Read alert: {message}, sending to kafka.");
            //TO Do - produce

            File.Delete(message[0]);
            Directory.Delete(directoryPath);
            //Console.WriteLine(value);
        }


    }
}