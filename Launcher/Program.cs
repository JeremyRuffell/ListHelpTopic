using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;

namespace Launcher
{
    class Program
    {
        // Notes: When using HttpClient dont use a using statement.
        public static HttpClient httpClient = new HttpClient();

        public static Version LatestVersion;
        public static string path = "ListHelpTopic";

        static void Main(string[] args)
        {
            // Get Latest Version number.
            httpClient.DefaultRequestHeaders.Add("User-Agent", "C# App");
            Task<string> APIRequest = httpClient.GetStringAsync("https://api.github.com/repos/JeremyRuffell/ListHelpTopic/releases/latest");
            string APIResponse = APIRequest.Result;
            
            // Deserialize APIResponse.
            DataModel deserializedObject = JsonConvert.DeserializeObject<DataModel>(APIResponse);

            Version.TryParse(deserializedObject.tag_name, out LatestVersion); 

            // Checking if all neccesary files are in Dir.
            if (!Directory.Exists(path))
            {
                DownloadLatestRelease();
                InstallLatestRelease();
            }

            // Get Application Version.
            Version ApplicationVersion = null;
            //Version.TryParse("1.0.0.0", out ApplicationVersion);
            Version.TryParse(Assembly.LoadFrom($"{path}/ListHelpTopic.exe").GetName().Version.ToString(), out ApplicationVersion);
           

            Console.WriteLine("Checking for updates\n ");

            // If the Applications Version is less than the latest version.
            if (ApplicationVersion < LatestVersion)
            {
                Console.WriteLine($"Update Needed!\nUpdating from {ApplicationVersion} > {LatestVersion}\n ");

                // Checking if the {path} Dir exists.
                Console.Write($"Does /{path} Exist?");
                if (Directory.Exists(path))
                {
                    Console.WriteLine(" Yes\n ");

                    // In order to delete a directory you need to delete all the files in that directory.
                    // This Foreach deletes all the current files in {path}.
                    foreach (var item in Directory.GetFiles(path))
                    {
                        Console.WriteLine($"Deleting file /{path}/{item}");
                        File.Delete(item);
                    }

                    // After all the files have been deleted out of {path}, delete the direcotry itslef.
                    Console.WriteLine($"\nDeleting Directory: {path}\n ");
                    Directory.Delete(path);
                }

                // Gets the latest release and downloads it.
                DownloadLatestRelease();

                // Install the latest release to {path}.
                InstallLatestRelease();

                Console.WriteLine("Update Complete.");

                // Starts the ListHelpTopic Program.
                StartListHelpTopic();
            }

            // If the Applications Version and latest version are the same.
            else if (ApplicationVersion == LatestVersion)
            {
                Console.WriteLine("ListHelpTopic is Up to Date\n");

                // Starts the ListHelpTopic Program.
                StartListHelpTopic();
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        public static void StartListHelpTopic()
        {
            ProcessStartInfo processInfo = new ProcessStartInfo
            {
                // Setting this to false makes ListHelpTopics run within the same Console Window.
                UseShellExecute = false,
                FileName = Path.Combine(Directory.GetCurrentDirectory(), $"{path}/ListHelpTopic.exe")
            };

            // Launch ListHelpTopic.
            Console.WriteLine("Starting ListHelpTopic");
            using (Process process = Process.Start(processInfo))
            {
                process.WaitForExit();
            }
        }

        public static void DownloadLatestRelease()
        {
            // Gets the latest release and downloads it.
            using (var client = new WebClient())
            {
                Console.WriteLine($"Downloading Release: {LatestVersion}\n ");
                client.DownloadFile("https://github.com/JeremyRuffell/ListHelpTopic/releases/latest/download/ListHelpTopic.zip", "ListHelpTopic.zip");
            }
        }

        public static void InstallLatestRelease()
        {
            // Extracts the downloaded release to {path}.
            Console.WriteLine($"Extracting ListHelpTopic.zip to /{path}\n ");
            ZipFile.ExtractToDirectory("ListHelpTopic.zip", path);

            // Deletes ListHelpTopic.zip as its just a junk file.
            Console.WriteLine("Deleting ListHelpTopic.zip\n ");
            File.Delete("ListHelpTopic.zip");
        }
    }
}
