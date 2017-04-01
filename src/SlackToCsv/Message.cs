using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackToCsv
{
    public class Message
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("user")]
        public string UserId { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("ts")]
        public string TimestampString { get; set; }

        public TimeSpan TimeStamp
        {
            get
            {
                return new TimeSpan(long.Parse(TimestampString.Split('.').First()));
            }
        }

        public DateTime Time
        {
            get
            {
                System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                dtDateTime = dtDateTime.AddSeconds(this.TimeStamp.Ticks).ToLocalTime();
                return dtDateTime;
            }
        }
    }
}
