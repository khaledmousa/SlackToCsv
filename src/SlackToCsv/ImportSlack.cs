using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SlackToCsv
{
    public class ImportSlack
    {
        private const string USERS_FILE = "users.json";
        private const string CHANNELS_FILE = "channels.json";
        
        private static Regex _messagesRegex = new Regex("[0-9]{4}-[0-9]{2}-[0-9]{2}.json", RegexOptions.Compiled);

        public string FolderPath { get; private set; }
        private Dictionary<string, User> _userMap;
        private Dictionary<string, Channel> _channelMap;

        public ImportSlack(string folderPath)
        {
            _userMap = GetUserMap(folderPath, USERS_FILE);
            _channelMap = GetChannelMap(folderPath, CHANNELS_FILE);
            FolderPath = folderPath;
        }

        public StringBuilder Import()
        {
            var csvString = new StringBuilder();
            foreach(var directory in Directory.GetDirectories(FolderPath))
            {
                var channel = _channelMap.Values.FirstOrDefault(c => c.Name == Path.GetFileName(directory));
                foreach(var file in Directory.GetFiles(directory, "*.json"))
                {
                    var messages = JsonConvert.DeserializeObject<List<Message>>(
                            ReadFileContent(directory, file)
                        );

                    foreach(var msg in messages)
                    {
                        var messageText = MessageToString(msg, channel);
                        if(!string.IsNullOrEmpty(messageText)) csvString.AppendLine(messageText);
                    }
                }
            }
            return csvString;
        }

        private string MessageToString(Message message, Channel channel)
        {
            if (message.UserId == null) return string.Empty;
            var user = _userMap[message.UserId];
            return string.Format("{0},{1},{2},{3},{4},{5}",
                Csv.Escape(user.Id),
                Csv.Escape(user.Name),
                Csv.Escape(channel.Id),
                Csv.Escape(channel.Name),
                Csv.Escape(message.Text),
                Csv.Escape(message.Time.ToString("dd.MM.yyyy HH:mm:ss"))
                );
        }

        private Dictionary<string, User> GetUserMap(string folderPath, string usersFile)
        {
            var usersJson = ReadFileContent(folderPath, usersFile);
            var json = new Newtonsoft.Json.JsonSerializer();
            return JsonConvert.DeserializeObject<List<User>>(usersJson)
                .ToDictionary(u => u.Id, u => u);
        }

        private Dictionary<string, Channel> GetChannelMap(string folderPath, string channelsFile)
        {
            var channelsJson = ReadFileContent(folderPath, channelsFile);
            var json = new Newtonsoft.Json.JsonSerializer();
            return JsonConvert.DeserializeObject<List<Channel>>(channelsJson)
                .ToDictionary(c => c.Id, c => c);
        }

        private string ReadFileContent(string folder, string file)
        {
            string fullPath = Path.Combine(folder, file);
            return File.ReadAllText(fullPath);
        }
    }
}
