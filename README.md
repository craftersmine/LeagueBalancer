# craftersmine.LeagueBalancer

League of Legends closed-game balancer and champion randomizer

[![Created under Riot Legal Jibber Jabber](https://img.shields.io/badge/created_under-Riot_Legal_Jibber_Jabber-red?logo=riot-games)](https://www.riotgames.com/en/legal)
[![GitHub License](https://img.shields.io/github/license/craftersmine/LeagueBalancer)](https://github.com/craftersmine/LeagueBalancer/tree/master/LICENSE)
[![GitHub Releases](https://img.shields.io/github/downloads/craftersmine/LeagueBalancer/total?label=github%20downloads&logo=github)](https://github.com/craftersmine/LeagueBalancer/releases)
![Discord](https://img.shields.io/badge/discord-@craftersmine-5865f2?logo=discord&logoColor=white)
![GitHub Repo stars](https://img.shields.io/github/stars/craftersmine/LeagueBalancer)
![Maintenance](https://img.shields.io/maintenance/yes/2023)

Application preview:
![Controls Preview](https://raw.githubusercontent.com/craftersmine/LeagueBalancer/master/.github/ApplicationPreview.png)

## Installation:
* **THE APP REQUIRES .NET 6!** You can download it [here](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-6.0.20-windows-x64-installer)
* Download latest application archive [here](https://github.com/craftersmine/LeagueBalancer/releases)
* Extract in whatever folder your soul wants
* Launch `craftersmine.LeagueBalancer.exe` executable

## Usage
* Add players in the participants list (up to 10 players allowed)
* Click `BALANCE` to balance teams according total LP
* Shuffle team participants in League client according balancing result
* If you want to get randomized champions, click `Get Randomized Summoner Champions`
* Then select desired player in participants list

## Champion randomizer weights
| Champion    | Weight formula                                                                              |
|-------------|---------------------------------------------------------------------------------------------|
| Main / OTP  | `0.6 * 0.95 = ~0.57`                                                                        |
| Has mastery | `(1 - (ChampionMastery / MainMastery)) * 0.95 = (1 - (2000000 / 10000000)) * 0.95  = ~0.76` | 
| Others      | `1 - 0.05 = ~0.95`                                                                          |

###### This project was created under Riot Games' ["Legal Jibber Jabber"](https://www.riotgames.com/en/legal) policy using assets owned by [Riot Games](https://www.riotgames.com). [Riot Games](https://www.riotgames.com) does not endorse or sponsor this project.
