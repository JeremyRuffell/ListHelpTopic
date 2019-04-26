using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ListHelpTopic
{
    class Program
    {
        // List that holds Class's HelpTopic Values for the class's that have a bug.
        public static List<Item> myList = new List<Item>();

        // Public string so that Config file data can be accessed anywhere within the program.
        public static Config.DataModel config;

        public static Version LatestVersion;


        static void Main(string[] args)
        {
            // Get Latest Version.
            HttpClient httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("User-Agent", "C# App");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "8d9d3a336bb6097748d0256ed1e331779e721d65");
            Task<string> APIRequest = httpClient.GetStringAsync("https://api.github.com/repos/JeremyRuffell/ListHelpTopic/releases/latest?access_token=ebfc43b144e93e4dfa5249ac20e3a5cc198c681e ");
            string APIResponse = APIRequest.Result;

            GithubAPIResponse deserializedObject = JsonConvert.DeserializeObject<GithubAPIResponse>(APIResponse);

            Version.TryParse(deserializedObject.tag_name, out LatestVersion);

            // Get Applications Version.
            Version ApplicationVersion = null;
            Version.TryParse("1.0.0.0", out ApplicationVersion);
            //Version.TryParse(Assembly.GetExecutingAssembly().GetName().Version.ToString(), out ApplicationVersion);

            if (ApplicationVersion < LatestVersion)
            {
                Console.WriteLine("Update");

                // Rename all files.
                DirectoryInfo directory = new DirectoryInfo("./");
                FileInfo[] FileNames = directory.GetFiles();

                foreach (FileInfo name in FileNames)
                {
                    File.Move(name.ToString(), $"old_{name.ToString()}");
                }

                Common.DownloadLatestRelease();
                Common.InstallLatestRelease();

                StartListHelpTopic();
            }
            else if (ApplicationVersion == LatestVersion)
            {
                Console.WriteLine("Up to Date");
            }

            // Load Config file.
            try
            {
                // Try load the config file.
                config = Config.Load();
            }
            catch (Exception)
            {
                // Create a default config file.
                Config.Create();
                // Load the config file.
                config = Config.Load();
            }

            // Add all Ignored Class's to a list.
            foreach (Config.DataModel.Toignore x in config.ToIgnore)
            {
                myList.Add(new Item { ClassName = x.ClassName, HelpTopicReturnValue = x.HelpTopicReturnValue });
            }

            // If AquiraPath from Config dosent exsist, get the valid Aquira Path and replace on Config.
            if (!Directory.Exists(config.AquiraPath))
            {
                // Setting the AquiraPath value on the config to the valid Aquira Path.
                config.AquiraPath = Common.GetAquiraDirectory();
                // Writing Data back to Config.
                Config.Write(config);
            }

            Console.WriteLine("sleeping for 5 seconds");
            int milliseconds = 5000;
            Thread.Sleep(milliseconds);
            // Finish Upadate.
            DirectoryInfo dir = new DirectoryInfo("./");
            var taskFiles = dir.GetFiles().Where(p => p.Name.StartsWith("old_"));
            foreach (var item in taskFiles)
            {
                File.Delete(item.ToString());
            }

            // Load DLL. 
            // Note: Aquira_WinControls is the only DLL that contains the wanted Interfaces.
            Assembly assembly = Assembly.LoadFrom(Path.Combine(config.AquiraPath, "Aquira_WinControls.dll"));

            //Get Types from DLL.
            Type[] types = assembly.GetTypes();

            foreach (Type type in types)
            {
                Type Interface = type.GetInterfaces().Where(x => x.Name == "IGetHelpTopic").FirstOrDefault();
                if (Interface != null)
                {
                    var check = Common.CheckIfValid(type.Name);
                    if (check != "Ok")
                    {
                        Common.SpaceGenerator(type.Name, check, "Warning");
                    }
                    else
                    {
                        ConstructorInfo ci = type.GetConstructor(new Type[] { });

                        object v = ci.Invoke(null);

                        MethodInfo m = type.GetProperty("HelpTopic").GetMethod;

                        object o = m.Invoke(v, null);

                        Common.SpaceGenerator(type.Name, o.ToString(), "Success");
                    }
                }
            }

            // Debugging Purposes.
            Console.Write("Press any key to continue . . . ");
            Console.ReadKey();
        }
        public static void StartListHelpTopic()
        {
            ProcessStartInfo processInfo = new ProcessStartInfo
            {
                // Setting this to false makes ListHelpTopics run within the same Console Window.
                UseShellExecute = true,
                FileName = "ListHelpTopic.exe"
            };

            // Launch ListHelpTopic.
            Console.WriteLine("Starting ListHelpTopic");
            using (Process process = Process.Start(processInfo))
            {
                process.WaitForExit();
            }
        }
    }
}
