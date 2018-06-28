using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;

namespace ThnOlgLauncher.model
{
    internal class JsonStorage
    {
        private const string DefaultFilenameServers = "thnolg_servers.json";
        private const string DefaultFilenameGames = "thnolg_games.json";

        private readonly JavaScriptSerializer _serializer = new JavaScriptSerializer();

        public string FileNameGames { get; set; }
        public string FileNameServers { get; set; }

        public JsonStorage()
        {
            FileNameGames = DefaultFilenameGames;
            FileNameServers = DefaultFilenameServers;
        }

        private string ReadJsonGames()
        {
            if (!File.Exists(FileNameGames))
            {
                return null;
            }

            var streamReader = new StreamReader(FileNameGames);
            var content = streamReader.ReadToEnd();
            streamReader.Close();
            return content;
        }

        private string ReadJsonServers()
        {
            if (!File.Exists(FileNameServers))
            {
                return null;
            }

            var streamReader = new StreamReader(FileNameServers);
            var content = streamReader.ReadToEnd();
            streamReader.Close();
            return content;
        }

        private void LoadJsonGames(DataStore data)
        {
            var fileContent = ReadJsonGames();
            if (fileContent == null)
            {
                return;
            }

            try
            {
                var dataFromJson = _serializer.Deserialize<List<Game>>(fileContent);
                dataFromJson.ForEach(s => data.Games.Add(s));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            data.Servers.ForEach(s =>
            {
                s.Ping = 0;
                s.Players = "";
                s.Map = "";
            });
        }

        private void LoadJsonServers(DataStore data)
        {
            var fileContent = ReadJsonServers();
            if (fileContent == null)
            {
                return;
            }

            try
            {
                var dataFromJson = _serializer.Deserialize<List<Server>>(fileContent);
                dataFromJson.ForEach(s => data.Servers.Add(s));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void SaveJsonGames(DataStore data)
        {
            var jsonString = _serializer.Serialize(data.Games);
            if (jsonString.Equals(ReadJsonGames()))
            {
                Console.WriteLine(@"Equals... Not writing...");
                return;
            }

            File.WriteAllText(FileNameGames, jsonString);
            Console.WriteLine(@"Updating Games JSON...");
        }

        public void SaveJsonServers(DataStore data)
        {
            var jsonString = _serializer.Serialize(data.Servers);
            if (jsonString.Equals(ReadJsonServers()))
            {
                Console.WriteLine(@"Equals... Not writing...");
                return;
            }

            File.WriteAllText(FileNameServers, jsonString);
            Console.WriteLine(@"Updating Server JSON...");
        }

        public void LoadJson(DataStore data)
        {
            LoadJsonGames(data);
            LoadJsonServers(data);
        }

        public void SaveJson(DataStore data)
        {
            SaveJsonGames(data);
            SaveJsonServers(data);
        }

        public bool IsDirtyGame(DataStore data)
        {
            return ReadJsonGames() != _serializer.Serialize(data.Games);
        }

        public bool IsDirtyServer(DataStore data)
        {
            return ReadJsonServers() != _serializer.Serialize(data.Servers);
        }
    }
}