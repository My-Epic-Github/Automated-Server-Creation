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
                string fileUrl = "https://piston-data.mojang.com/v1/objects/5b868151bd02b41319f54c8d4061b8cae84e665c/server.jar";

                string[] jvmArgs = { "java ", "-Xms512M ", "-Xmx1G ", "-jar server.jar ", "nogui " };

                if (!Directory.Exists("Server"))
                {
                    Directory.CreateDirectory("Server");
                }


                if (!File.Exists("Server\\server.jar"))
                {
                    using (WebClient webClient = new WebClient())
                    {
                        Console.WriteLine("Downloading Server.jar..");
                        webClient.DownloadFile(fileUrl, "Server\\server.jar");
                        Console.WriteLine($"Downloaded {fileUrl} successfully!");
                    }
                }

                using (StreamWriter sw = new StreamWriter("Server\\run.bat", false))
                {
                    foreach (string arg in jvmArgs)
                    {
                        sw.Write(arg);
                        Console.Write(arg);
                    }
                    Console.WriteLine();
                    sw.WriteLine("\nRem Alter the Xms value to adjust the minimum amount of memory dedicated");
                    sw.WriteLine("Rem Alter the Xmx value to adjust the maximum amount of memory dedicated");
                    sw.Close();
                }

                runBatchFile();
                
                String eulaContents = File.ReadAllText("Server\\eula.txt");
                if (eulaContents.Contains("false"))
                {
                    Console.WriteLine("Agreeing to eula..");
                    eulaContents = eulaContents.Replace("false", "true");
                    Console.WriteLine("Successfully agreed to eula");
                }
                File.WriteAllText("Server\\eula.txt", eulaContents);
                Console.WriteLine(eulaContents);





                Console.WriteLine("\nPress any key to exit..");
                Console.ReadLine();


            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }
        static void runBatchFile()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "Server\\run.bat",
                UseShellExecute = false,
                CreateNoWindow = false
            };

            using (Process process = Process.Start(startInfo))
            {
                Console.WriteLine("Executing run.bat..");
                process.WaitForExit();
            }
        }
    }

}

