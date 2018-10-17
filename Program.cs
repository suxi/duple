using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Duple
{
    class Program
    {
        static void Main(string[] args)
        {
            var files = new List<string>();
            foreach (var item in args)
            {
                var root = new DirectoryInfo(item);
                if (root.Exists)
                {
                    var fileNames = from dir in Directory.EnumerateFiles(root.FullName,"*.*", SearchOption.AllDirectories)
                                    select dir;
                    files.AddRange(fileNames);
                }
            }
            if (files.Count == 0)
            {
                Console.WriteLine("Duple [path1] [path2] ...");
                return;
            }
            
            var videos = from file in files.AsParallel()
                            let ext = Path.GetExtension(file)
                            where ext.ToLower() == ".mp4" || ext.ToLower() == ".mkv" || ext.ToLower() == ".avi"
                            let date = File.GetLastWriteTime(file)
                            let name = Path.GetFileNameWithoutExtension(file)
                            let path = Path.GetFullPath(file)
                            let size = new FileInfo(file).Length
                            select new {name,date,path,size};
            var reg = new Regex(@"[a-zA-Z]{2,5}-?\d{0,3}[ABab]?");
            var last = "";
            foreach (var item in videos.OrderBy(item => reg.Match(item.name).Value))
            {
                if (last == reg.Match(item.name).Value.Replace("-","").ToUpper() && reg.Match(item.name).Value.Replace("-","").Length > 0)
                {
                    Console.WriteLine($"*{item.name} \t{item.date:yyyy/MM/dd HH:mm:ss}\t{item.size,13:N0}\t{item.path}");
                }
                else
                {
                    Console.WriteLine($" {item.name} \t{item.date:yyyy/MM/dd HH:mm:ss}\t{item.size,13:N0}\t{item.path}");
                }
                last = reg.Match(item.name).Value.Replace("-","").ToUpper();
            }

            Console.WriteLine($"Done by {videos.Count()}");
        }
    }
}
