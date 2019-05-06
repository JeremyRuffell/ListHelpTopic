using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AquiraHelpTopics
{
    public static class Common
    {
        // Generates and inserts them after string to provide a nicely formatted console display.
        public static void SpaceGenerator(string s, string s2, string type)
        {
            string str = "                                             ";
            string newstr = str.Remove(str.Length - s.Length);
            Console.Write($"{s}:{newstr}");

            if (type == "Success")
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else if (type == "Warning")
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
            }

            Console.WriteLine(s2);

            Console.ForegroundColor = ConsoleColor.White;
        }

        // To check if Class is bugged and needs to get return value from 'myList'.
        public static string CheckIfValid(string ClassName)
        {
            Item item = Program.myList.FirstOrDefault(x => x.ClassName == ClassName);
            if (item == null)
            {
                return "Ok";
            }
            else
            {
                return $"{item.HelpTopicReturnValue}";
                //return item != null ? item.HelpTopicReturnValue : string.Empty;
            }
        }

        public static string GetAquiraDirectory()
        {
            // Aquira Registry Key.
            const string AQUIRA_BASE_REG_KEY = "SOFTWARE\\WOW6432Node\\RCS\\Aquira";

            // String Array of SubKeys of Registry Key.
            string[] SubKeys = new string[] { };
            try
            {
                // Open the Aquira Registry Key.
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(AQUIRA_BASE_REG_KEY))
                {
                    if (key != null)
                    {
                        SubKeys = key.GetSubKeyNames();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("GetRegistrySubKeys::ERROR - " + ex);
            }

            // Creating a new version. (0.0.0.0)
            Version aquiraVer = new Version(0, 0, 0, 0);

            // Setting a blank string to hold a value for testin against.
            string aquiraRegKey = "";

            // Foreach SubKey Path in the Aquira Main Key.
            foreach (string SubKey in SubKeys)
            {
                // If  the SubKey's version exists and can be Parsed.
                if (Version.TryParse(SubKey, out Version version))
                {
                    // If the version on the current SubKey is greater then the previous SubKey.
                    if (version > aquiraVer)
                    {
                        aquiraVer = version;
                        aquiraRegKey = SubKey;
                    }
                }
            }

            // Get's Path Value off  the latest version of Aquira's Key.
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(Path.Combine(AQUIRA_BASE_REG_KEY, aquiraRegKey)))
            {
                return key.GetValue("AquiraDirectory").ToString();
            }
        }
        public static void DownloadLatestRelease()
        {
            // Gets the latest release and downloads it.
            using (var client = new WebClient())
            {
                client.DownloadFile("https://github.com/JeremyRuffell/AquiraHelpTopics/releases/latest/download/AquiraHelpTopics.zip", "AquiraHelpTopics.zip");
            }
        }

        public static void InstallLatestRelease()
        {
            // Extracts the downloaded release to {path}.
            ZipFile.ExtractToDirectory("AquiraHelpTopics.zip", AppDomain.CurrentDomain.BaseDirectory);

            // Deletes AquiraHelpTopics.zip as its just a junk file.
            File.Delete("AquiraHelpTopics.zip");
        }


    }
}
