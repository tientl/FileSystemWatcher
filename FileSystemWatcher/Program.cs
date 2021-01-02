using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FileSystemWatcherTest
{
    class Program
    {
        static DirectoryInfo fdA = new DirectoryInfo(@"C:\Users\Admin\Desktop\A");
        static DirectoryInfo fdB = new DirectoryInfo(@"C:\Users\Admin\Desktop\B");
        static void Main(string[] args)
        {
           
            FileSystemWatcher watcher = new FileSystemWatcher(@"C:\Users\Admin\Desktop\A");
            watcher.EnableRaisingEvents = true;
            watcher.IncludeSubdirectories = true;

            watcher.Changed += watcher_Created;
            watcher.Created += watcher_Created;
            watcher.Deleted += watcher_Deleted;
            watcher.Renamed += watcher_Renamed;
            Console.Read();
            
        }
        

        private static void watcher_Renamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine("File:{0} renamed to {1} at time: {2}", e.OldFullPath, e.FullPath, DateTime.Now.ToLocalTime());
            FileInfo fi = new FileInfo(e.OldFullPath);
            foreach (var f in fdB.GetFiles())
            {
                Console.WriteLine(f.DirectoryName);
                
            }
            foreach (var f in fdB.GetDirectories())
            {
                Console.WriteLine( f);
            }

            if (fi.Exists)
            {
                fi.MoveTo(@"C:\Users\Admin\Desktop\B");
            }          
        }

        private static void watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("File:{0} Deleted at time: {1}", e.Name, DateTime.Now.ToLocalTime());

        }

        private static void watcher_Created(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("File:{0} Created to {1} at time: {2}", e.Name, e.Name, DateTime.Now.ToLocalTime());
            foreach (var file in fdA.GetFiles())
            {
                if (file.Name == e.Name)
                {
                    Thread.Sleep(50);
                    file.CopyTo(Path.Combine(fdB.FullName, file.Name), true);
                }
            }

            foreach (var SubDir in fdA.GetDirectories())
            {
                Console.WriteLine(SubDir.Name);
                //subdir: thư mục con
                //coppysubdir: thư mục con coppy

                Thread.Sleep(50);
                var CopySubDir = fdB.CreateSubdirectory(SubDir.Name);
                CopyFile(SubDir, CopySubDir);
            }
        }

        private static void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("File:{0} Changed to {1} at time: {2}", e.Name, e.Name, DateTime.Now.ToLocalTime());

        }

        public static void CopyFile(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);
            foreach (var file in source.GetFiles())
            {
                Thread.Sleep(50);
                file.CopyTo(Path.Combine(target.FullName, file.Name), true);
            }
            foreach (var SubDir in source.GetDirectories())
            {
                Thread.Sleep(50);
                var CopySubDir = target.CreateSubdirectory(SubDir.Name);
                CopyFile(SubDir, CopySubDir);
            }
        }

    }
}
