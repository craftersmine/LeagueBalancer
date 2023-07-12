using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using craftersmine.Riot.Api.Common;

namespace craftersmine.LeagueBalancer
{
    public class LeagueRegion
    {
        public RiotPlatform Region { get; set; }
        public string RegionName { get; set; }

        public LeagueRegion(RiotPlatform region)
        {
            Region = region;
            switch (Region)
            {
                case RiotPlatform.Russia:
                    RegionName = "Russia";
                    break;
                case RiotPlatform.EuropeWest:
                    RegionName = "Europe West";
                    break;
                case RiotPlatform.EuropeNordicEast:
                    RegionName = "Europe Nordic East";
                    break;
                case RiotPlatform.NorthAmerica:
                    RegionName = "North America";
                    break;
                case RiotPlatform.Brazil:
                    RegionName = "Brazil";
                    break;
                case RiotPlatform.Japan:
                    RegionName = "Japan";
                    break;
                case RiotPlatform.Korea:
                    RegionName = "Korea";
                    break;
                case RiotPlatform.LatinAmericaNorth:
                    RegionName = "Latin America North";
                    break;
                case RiotPlatform.LatinAmericaSouth:
                    RegionName = "Latin America South";
                    break;
                case RiotPlatform.Oceania:
                    RegionName = "Oceania";
                    break;
                case RiotPlatform.Turkey:
                    RegionName = "Turkey";
                    break;
            }
        }

        public override string ToString()
        {
            return RegionName;
        }
    }
}
