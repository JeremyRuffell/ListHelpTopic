using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListHelpTopic
{
    class Config
    {
        // Create's a Config File.
        public static void Create()
        {
            DataModel config = new DataModel();
            Write(config);
        }

        // Writes Data to config.
        public static void Write(object DataToWrite)
        {
            File.WriteAllText(@"config.json", JsonConvert.SerializeObject(DataToWrite, Formatting.Indented));
        }

        public static DataModel Load()
        {
            return JsonConvert.DeserializeObject<Config.DataModel>(File.ReadAllText("config.json"));
        }

        // Config file Data Model. 
        public class DataModel
        {
            public string AquiraPath { get; set; } = "C:\\Program Files\\RCS\\Aquira\\Aquira";
            public List<Toignore> ToIgnore { get; set; } = new List<Toignore> { new Toignore() };

            public class Toignore
            {
                public string ClassName { get; set; } = "WaveImportForm";
                public string HelpTopicReturnValue { get; set; } = "Setup_Wave";
            }
        }
    }

}
