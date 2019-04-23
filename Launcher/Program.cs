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

namespace Launcher
{
    class Program
    {
        // Notes: When using HttpClient dont use a using statement.
        public static HttpClient httpClient = new HttpClient();

        static void Main(string[] args)
        {
            httpClient.DefaultRequestHeaders.Add("User-Agent", "C# App");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "ebfc43b144e93e4dfa5249ac20e3a5cc198c681e ");
            Task<string> APIRequest = httpClient.GetStringAsync("https://api.github.com/repos/JeremyRuffell/ListHelpTopic/releases/latest");
            string APIResponse = APIRequest.Result;

            // Deserialize APIResponse.
            DataModel deserializedObject = JsonConvert.DeserializeObject<DataModel>(APIResponse);

            // Get Application Version.
            Version ApplicationVersion = null;
            Version.TryParse(Assembly.GetExecutingAssembly().GetName().Version.ToString(), out ApplicationVersion);
            // Get Latest Version.
            Version LatestVersion = null;
            Version.TryParse(deserializedObject.tag_name, out LatestVersion);


            if (ApplicationVersion < LatestVersion)
            {
                Console.WriteLine("Updating...");
                using (var client = new WebClient())
                {
                    client.DownloadFile("http://example.com/file/song/a.mpeg", "a.mpeg");
                }

            }
            else
            {
                ProcessStartInfo processInfo = new ProcessStartInfo
                {
                    // Setting this to false makes ListHelpTopics run within the same Console Window.
                    UseShellExecute = false, 
                    FileName = Path.Combine(Directory.GetCurrentDirectory(), "ListHelpTopic/ListHelpTopic.exe")
                };

                // Launch ListHelpTopics.
                Console.WriteLine("Starting ListHelpTopics");
                using (Process process = Process.Start(processInfo))
                {
                    process.WaitForExit();
                }
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
