using System;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace MinecraftServerAutomated
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string path = string.Empty;

                string fileUrl = "https://piston-data.mojang.com/v1/objects/5b868151bd02b41319f54c8d4061b8cae84e665c/server.jar";

                string[] jvmArgs = { "java ", "-Xms512M ", "-Xmx1G ", "-jar server.jar ", "nogui " };

                Console.WriteLine("Input output path:\n");
                path = Console.ReadLine() + "\\Server";

                if (path == "")
                {
                    Console.WriteLine("Defaulting to default directory..\n");
                    path = "Server\\";
                }
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }


                if (!File.Exists($"{path}\\server.jar"))
                {
                    using (WebClient webClient = new WebClient())
                    {
                        Console.WriteLine("Downloading Server.jar..");
                        webClient.DownloadFile(fileUrl, $"{path}\\server.jar");
                        Console.WriteLine($"Downloaded {fileUrl} successfully!");
                    }
                }

                using (StreamWriter sw = new StreamWriter($"{path}\\run.bat", false))
                {
                    foreach (string arg in jvmArgs)
                    {
                        sw.Write(arg);
                        Console.Write(arg);
                    }
                    Console.WriteLine();
                    sw.Close();
                }

                runBatchFile(path);
                
                String eulaContents = File.ReadAllText($"{path}\\eula.txt");
                if (eulaContents.Contains("false"))
                {
                    Console.WriteLine("Agreeing to eula..");
                    eulaContents = eulaContents.Replace("false", "true");
                    Console.WriteLine("Successfully agreed to eula");
                    File.WriteAllText($"{path}\\eula.txt", eulaContents);
                    Console.WriteLine(eulaContents);
                    runBatchFile(path);
                }





                Console.WriteLine("\nPress any key to exit..");
                Console.ReadLine();


            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }
        static void runBatchFile(string path)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                WorkingDirectory = Path.GetFullPath(path),
                FileName = "cmd.exe",
                Arguments = "/C run.bat",
                UseShellExecute = false,
                CreateNoWindow = false,
            };

            using (Process process = Process.Start(startInfo))
            {
                Console.WriteLine("Executing run.bat..");
                process.WaitForExit();
            }
        }
    }

}

