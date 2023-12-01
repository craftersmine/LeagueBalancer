using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace craftersmine.LeagueBalancer
{
    public class InputSanitizer
    {
        public static readonly Regex[] PossibleLobbyMessages =
        {
            new Regex("(?<riotIdName>.*) #(?<riotIdTag>.*) присоединился к лобби"),
            new Regex("(?<riotIdName>.*) #(?<riotIdTag>.*) joined the lobby"),
        };

        public static readonly string[] LineEndings =
        {
            "\r\n",
            "\r",
            "\n"
        };

        public static string[] SanitizeInputFromChat(string input)
        {
            string[] lines = input.Split(LineEndings, StringSplitOptions.RemoveEmptyEntries);
            List<string> possibleSummonerNames = new List<string>();
            foreach (string line in lines)
            {
                foreach (Regex possibleMessage in PossibleLobbyMessages)
                {
                    if (possibleMessage.IsMatch(line))
                    {
                        Match match = possibleMessage.Match(line);
                        string riotId = match.Groups["riotIdName"].Value;
                        string riotTag = match.Groups["riotIdTag"].Value;

                        possibleSummonerNames.Add(riotId + "#" + riotTag);
                    }
                }
            }
            
            return possibleSummonerNames.Distinct().ToArray();
        }
    }
}
