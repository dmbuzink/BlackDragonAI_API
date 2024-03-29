#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace BlackDragonAIAPI.Models
{
    public class StreamPlanning
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public string TimeSlot { get; set; }
        public string Game { get; set; }
        public string StreamType { get; set; }
        public string GameType { get; set; }
        public string TrailerUri { get; set; }
        
        public static IEnumerable<StreamPlanning> MultiParse(string text)
        {
            var rawStreamPlannings = text.Replace("\r", string.Empty).Split("\n\n");
            foreach (var rawStreamPlanning in rawStreamPlannings)
            {
                StreamPlanning parsedStreamPlanning;
                try
                {
                    parsedStreamPlanning = Parse(rawStreamPlanning);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to parse stream planning:\n{rawStreamPlanning}\nWith the exception:\n{e.Message}\n");
                    continue;
                }

                yield return parsedStreamPlanning;
            }
        }
        
        public static StreamPlanning Parse(string text)
        {
            const int trailerTextLength = 8;
            var groupRegex = new Regex("([|]([^|\n])+)|(\n([^|\n])+)");
            var dateRegex = new Regex("[0-9]{2}-[0-9]{2}"); 
            
            var streamPlanning = new StreamPlanning();
            var dateMatch = dateRegex.Match(text);
            streamPlanning.Date = new DateTime(DateTime.Now.Year,
                int.Parse(dateMatch.Value.Substring(3, 2)),
                int.Parse(dateMatch.Value.Substring(0, 2)));
            var groups = groupRegex.Matches(text);
            streamPlanning.TimeSlot = CleanTextGroup(groups.FirstOrDefault());
            streamPlanning.Game = CleanTextGroup(groups.Skip(1).FirstOrDefault());
            streamPlanning.StreamType = CleanTextGroup(groups.Skip(2).FirstOrDefault());
            streamPlanning.GameType = CleanTextGroup(groups.Skip(3).FirstOrDefault());
            string rawTrailerGroup = CleanTextGroup(groups.Skip(4).FirstOrDefault());
            var trailerIndex = rawTrailerGroup.IndexOf("Trailer:",
                StringComparison.InvariantCultureIgnoreCase);
            streamPlanning.TrailerUri = trailerIndex != -1 ?
                    rawTrailerGroup[(trailerIndex + trailerTextLength + 1)..].Trim() :
                    string.Empty;
            streamPlanning.TrailerUri = streamPlanning.TrailerUri
                .Replace("<", string.Empty)
                .Replace(">", string.Empty);
            return streamPlanning;
        }

        private static string CleanTextGroup(Match? groupMatch) =>
            groupMatch?.Value.Replace("|", string.Empty).Trim() ?? string.Empty;
        
        public static string SerializeMultiple(IEnumerable<StreamPlanning> streamPlannings)
        {
            var sb = new StringBuilder();
            foreach (var streamPlanning in streamPlannings)
            {
                sb.Append(streamPlanning.Serialize());
                sb.Append("\r\n\r\n");
            }
            return sb.ToString().TrimEnd();
        }

        public string Serialize() => ToString();
        
        /// <summary>
        /// Serializes the stream planning
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return
                $"{GetDayAbbreviation(Date.DayOfWeek)}. {Date.Day:D2}-{Date.Month:D2}  |  {TimeSlot}  |  {Game}  |  {StreamType}\r\n" +
                $"{GameType}  |  Trailer: <{TrailerUri}>";
        }

        private string GetDayAbbreviation(DayOfWeek dayOfWeek) => dayOfWeek switch
        {
            DayOfWeek.Monday => "Ma",
            DayOfWeek.Tuesday => "Di",
            DayOfWeek.Wednesday => "Wo",
            DayOfWeek.Thursday => "Do",
            DayOfWeek.Friday => "Vr",
            DayOfWeek.Saturday => "Za",
            DayOfWeek.Sunday => "Zo",
            _ => throw new ArgumentOutOfRangeException(nameof(dayOfWeek), dayOfWeek, null)
        };
    }
}