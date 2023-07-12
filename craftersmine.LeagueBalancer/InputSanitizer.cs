using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace craftersmine.LeagueBalancer
{
    public class InputSanitizer
    {
        public static readonly string[] PossibleLobbyMessages =
        {
            " присоединился к лобби",
            " покинул лобби",
            " joined the lobby",
            " left the lobby"
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
                foreach (string possibleMessage in PossibleLobbyMessages)
                {
                    if (line.Contains(possibleMessage))
                        possibleSummonerNames.Add(line.Replace(possibleMessage, ""));
                }
            }
            
            return possibleSummonerNames.Distinct().ToArray();
        }
    }
}
