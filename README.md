# Long Night Time Limit
A small tweak mod for Touhou: Lost Branch of Legend.

This is an experiment that reworks the Nighttime Fatigue request to add Time Limit 1 when you shuffle your draw pile multiple times mid-turn, rather than adding a Long Night card to the hand at the start of combat.

By default, Time Limit is applied every 2 reshuffles. You can change this and more in the config file (see below).

## Reasoning

The intent behind this mod is as follows:

- Reducing the meta of thinning your deck and playing the same 1 or 2 cards over and over in a single turn, which trivializes the game. Time Limit hits you where it hurts
- Reducing the need to rely on Exile cards to get rid of Long Night in your hand, which would otherwise encourage the above playstyle even more
- Making large pile decks more fun and viable to play, since a larger deck means playing more cards before you run into Time Limit
- Making certain draw cards less of a brainless auto-pick, since they are now no longer always strictly positive in every case
- Making certain weaker Extra Turn cards a bit more powerful ("why would I play an extra turn with time limit when I can just draw 5 and generate bunches of mana?")
- Keeping it as an optional difficulty tweak by making it a Request, rather than an always-on thing

This is an early test, there may be edge cases I haven't thought of. This mod was made to prototype how this tweak would affect the game. Gameplay feedback is very much welcome and requested.

## Installation 

I recommend using the Steam Workshop or r2modman to install this mod (links below). For manual installation, grab the latest zip from the Releases section. Requires BepInEx since this is a Harmony mod.

## Configuration

The mod generates a Bepinex config file when run. Check `rokk.lbol.gameplay.LongNightTimeLimit.cfg` in the `config` folder for more options.

## Building

Clone this project, make sure the References are set up correctly (manually edit the `.csproj` to change the game dir), and then open the solution with Visual Studio. I use VS2022.

## Links

[Steam Workshop page for Long Night Time Limit](https://steamcommunity.com/sharedfiles/filedetails/?id=3637669349)

[Thunderstore page for Long Night Time Limit](https://thunderstore.io/c/touhou-lost-branch-of-legend/p/Rokk/LongNightTimeLimit/)

[Thumbnail/icon source](https://x.com/ttzlwrm/status/1981431833750622277)
