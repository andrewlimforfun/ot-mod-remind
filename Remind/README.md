# Remind

A BepInEx mod for [On Together](https://store.steampowered.com/app/2688490/On_Together/) that adds scheduled reminder commands to in-game chat.

## In-Game Commands

Type any command into the in-game chat. Commands start with `/` and are **not sent** to other players.

| Command | Short | Description |
|---|---|---|
| `/remindhelp` | `/rh` | List all available commands |
| `/remindtoggle` | `/rt` | Toggle Remind on/off |
| `/remindshowcommand` | `/esc` | Toggle showing commands in chat |
| `/remindme` | `/rm` | Show a private notification after a delay or at a time |
| `/remindlocal` | `/rl` | Send a local chat message after a delay or at a time |
| `/remindglobal` | `/rg` | Send a global chat message after a delay or at a time |

### Reminder Syntax

All three reminder commands accept either `in` (relative delay) or `at` (absolute local time):

```
/remindme in 0:05:00 Take a break
/rm at 14:45 Stand-up meeting

/remindlocal in 1:00:00 One hour left!
/rl at 20:00 Game night starts

/remindglobal in 0:30:00 Checkpoint in 30 minutes
/rg at 15:00 Boss fight time
```

## Configuration

Located in `BepInEx/config/com.andrewlin.ontogether.remind.cfg`

- `EnableFeature` (default: `true`) — Enable or disable all mod features
- `ShowCommand` (default: `false`) — Show commands in chat when used

## Installation

Use `r2modman` or the Thunderstore app for the simplest install.

**Manual:**
1. Install [BepInEx](https://github.com/BepInEx/BepInEx/releases) into your On Together game folder
2. Copy `AndrewLin.Remind.dll` into `BepInEx/plugins/`
3. Launch the game — a config file will be generated automatically
