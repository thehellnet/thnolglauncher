using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;
using ThnOlgLauncher.model;

namespace ThnOlgLauncher.controller {
    class JsonStorage {
        private static String DEFAULT_FILENAME_SERVERS = "thnolg_servers.json";
        private static String DEFAULT_FILENAME_GAMES = "thnolg_games.json";

        private JavaScriptSerializer serializer = new JavaScriptSerializer();
        public String fileNameGames { get; set; }
        public String fileNameServers { get; set; }

        public JsonStorage() {
            fileNameGames = DEFAULT_FILENAME_SERVERS;
            fileNameServers = DEFAULT_FILENAME_GAMES;
        }

        private String readJsonGames() {
            if(!File.Exists(fileNameGames)) {
                return null;
            }
            StreamReader streamReader = new StreamReader(fileNameGames);
            String content = streamReader.ReadToEnd();
            streamReader.Close();
            return content;
        }

        private String readJsonServers() {
            if(!File.Exists(fileNameServers)) {
                return null;
            }
            StreamReader streamReader = new StreamReader(fileNameServers);
            String content = streamReader.ReadToEnd();
            streamReader.Close();
            return content;
        }

        private void loadJsonGames(DataStore data) {
            String fileContent = readJsonGames();
            if(fileContent == null) {
                return;
            }

            try {
                List<Game> dataFromJson = serializer.Deserialize<List<Game>>(fileContent);
                dataFromJson.ForEach(s => data.games.Add(s));
            } catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        private void loadJsonServers(DataStore data) {
            String fileContent = readJsonServers();
            if(fileContent == null) {
                return;
            }

            try {
                List<Server> dataFromJson = serializer.Deserialize<List<Server>>(fileContent);
                dataFromJson.ForEach(s => data.servers.Add(s));
            } catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        private void saveJsonGames(DataStore data) {
            String jsonString = serializer.Serialize(data.games);
            if(jsonString.Equals(readJsonGames())) {
                Console.WriteLine("Equals... Not writing...");
                return;
            }
            File.WriteAllText(fileNameGames, jsonString);
            Console.WriteLine("Updating Games JSON...");
        }

        private void saveJsonServers(DataStore data) {
            String jsonString = serializer.Serialize(data.servers);
            if(jsonString.Equals(readJsonServers())) {
                Console.WriteLine("Equals... Not writing...");
                return;
            }
            File.WriteAllText(fileNameServers, jsonString);
            Console.WriteLine("Updating Server JSON...");
        }

        public void loadJson(DataStore data) {
            loadJsonGames(data);
            loadJsonServers(data);
        }

        public void saveJson(DataStore data) {
            saveJsonGames(data);
            saveJsonServers(data);
        }

        public bool isDirty(DataStore data) {
            return (readJsonGames() != serializer.Serialize(data.games))
                || (readJsonServers() != serializer.Serialize(data.servers));
        }
    }
}
