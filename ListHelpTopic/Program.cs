using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

        static void Main(string[] args)
        {

            LoadConfig();

            // Load DLL.
            Assembly assembly = Assembly.Load("Aquira_WinControls");

            //Get Types from DLL.
            Type[] types = assembly.GetTypes();

            foreach (Type type in types)
            {
                Type Interface = type.GetInterfaces().Where(x => x.Name == "IGetHelpTopic").FirstOrDefault();
                if (Interface != null)
                {
                    var check = CheckIfValid(type.Name);
                    if (check != "Ok")
                    {
                        Console.WriteLine($"{SpaceGenerator(type.Name)}{check}");
                    }
                    else
                    {
                        ConstructorInfo ci = type.GetConstructor(new Type[] { });

                        Object v = ci.Invoke(null);

                        MethodInfo m = type.GetProperty("HelpTopic").GetMethod;

                        object o = m.Invoke(v, null);

                        Console.WriteLine($"{SpaceGenerator(type.Name)}'{o.ToString()}'");
                    }
                }
            }

            // Debugging Purposes.
            Console.Write("Press any key to continue . . . ");
            Console.ReadKey();
        }

        // Item DataModel
        public class Item
        {
            public string ClassName { get; set; }
            public string HelpTopicReturnValue { get; set; }
        }

        // To check if Class is bugged and needs to get return value from 'myList'.
        public static string CheckIfValid(string ClassName)
        {
            Item item = myList.FirstOrDefault(x => x.ClassName == ClassName);
            if (item == null)
            {
                return "Ok";
            }
            else
            {
                return $"'{item.HelpTopicReturnValue}'  *";
                //return item != null ? item.HelpTopicReturnValue : string.Empty;
            }
        }

        // Generates and inserts them after string to provide a nicely formatted console display.
        public static string SpaceGenerator(string s)
        {
            string str = "                                     ";
            string newstr = str.Remove(str.Length - s.Length);
            return $"{s}:{newstr}";
        }

        public class ConfigDataModel
        {

            public class Rootobject
            {
                public List<Toignore> ToIgnore { get; set; }
            }

            public class Toignore
            {
                public string ClassName { get; set; }
                public string HelpTopicReturnValue { get; set; }
            }

        }

        // Load config file.
        public static void LoadConfig()
        {
            ConfigDataModel.Rootobject config = JsonConvert.DeserializeObject<ConfigDataModel.Rootobject>(File.ReadAllText("config.json"));
            foreach (ConfigDataModel.Toignore x in config.ToIgnore)
            {
                myList.Add(new Item { ClassName = x.ClassName, HelpTopicReturnValue = x.HelpTopicReturnValue });
            }
        }
    }
}
