using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ListHelpTopic
{
    class Program
    {
        // List that holds Class's HelpTopic Values for the class's that have a bug.
        public static List<Item> myList = new List<Item>();

        // Public string so that Config file data can be accessed anywhere within the program.
        public static Config.DataModel config;

        static void Main(string[] args)
        {
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
    }
}
