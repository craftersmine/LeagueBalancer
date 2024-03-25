using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using craftersmine.LeagueBalancer.Localization;
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
                    RegionName = Locale.Region_Russia;
                    break;
                case RiotPlatform.EuropeWest:
                    RegionName = Locale.Region_EuropeWest;
                    break;
                case RiotPlatform.EuropeNordicEast:
                    RegionName = Locale.Region_EuropeNordicEast;
                    break;
                case RiotPlatform.NorthAmerica:
                    RegionName = Locale.Region_NorthAmerica;
                    break;
                case RiotPlatform.Brazil:
                    RegionName = Locale.Region_Brazil;
                    break;
                case RiotPlatform.Japan:
                    RegionName = Locale.Region_Japan;
                    break;
                case RiotPlatform.Korea:
                    RegionName = Locale.Region_Korea;
                    break;
                case RiotPlatform.LatinAmericaNorth:
                    RegionName = Locale.Region_LatinAmericaNorth;
                    break;
                case RiotPlatform.LatinAmericaSouth:
                    RegionName = Locale.Region_LatinAmericaSouth;
                    break;
                case RiotPlatform.Oceania:
                    RegionName = Locale.Region_Oceania;
                    break;
                case RiotPlatform.Turkey:
                    RegionName = Locale.Region_Turkey;
                    break;
                case RiotPlatform.Philippines:
                    RegionName = Locale.Region_Philippines;
                    break;
                case RiotPlatform.Singapore:
                    RegionName = Locale.Region_Singapore;
                    break;
                case RiotPlatform.Taiwan:
                    RegionName = Locale.Region_Taiwan;
                    break;
                case RiotPlatform.Thailand:
                    RegionName = Locale.Region_Thailand;
                    break;
                case RiotPlatform.Vietnam:
                    RegionName = Locale.Region_Vietnam;
                    break;
            }
        }

        public override string ToString()
        {
            return RegionName;
        }
    }
}
