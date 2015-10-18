using System;
using System.IO;
using System.Web.Script.Serialization;
using ThnOlgLauncher.model;

namespace ThnOlgLauncher.controller {
    class JsonStorage {
        private static JavaScriptSerializer serializer = new JavaScriptSerializer();
        private static String fileContent;
        public static String fileName { get; set; }

        public static void loadJson(DataStore data) {
            if(!File.Exists(fileName)) {
                return;
            }

            StreamReader streamReader = new StreamReader(fileName);
            fileContent = streamReader.ReadToEnd();

            try {
                DataStore dataFromJson = serializer.Deserialize<DataStore>(fileContent);
                dataFromJson.games.ForEach(g => data.games.Add(g));
                dataFromJson.servers.ForEach(s => data.servers.Add(s));
            } catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        public static void saveJson(DataStore data) {
            String jsonString = serializer.Serialize(data);
            if(jsonString.Equals(fileContent)) {
                Console.WriteLine("Equals... Not writing...");
                return;
            }
            fileContent = jsonString;
            File.WriteAllText(fileName, fileContent);
            Console.WriteLine("Updating JSON...");
        }

        public static bool isDirty(DataStore data) {
            String jsonString = serializer.Serialize(data);
            return !jsonString.Equals(fileContent);
        }
    }
}
