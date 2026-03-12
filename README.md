# ot-mod-remind

BepInEx mods for [On Together](https://store.steampowered.com/app/2688490/On_Together/) that gives chat reminder utility.

## Mods

### [Remind](Remind/)

Core mod (required by all add-ons). Provides configurable message limits, scheduled reminders (`/remindme`, `/remindlocal`, `/remindglobal`) with `in`/`at` syntax, in-game utility commands, and the shared `IChatSink` / `CommandManager` infrastructure that add-ons plug into.

- Install from [Thunderstore](https://thunderstore.io/c/on-together/p/AndrewLin/Remind/)
- See [Remind/README.md](Remind/README.md) for full documentation

## Building from Source

Requires .NET SDK.

```sh
# Build all projects
dotnet build Remind.slnx

# Build a specific project
dotnet build Remind/Remind.csproj

# Package for Thunderstore
dotnet build Remind/Remind.csproj -c Release

# Install locally for testing (copies to r2modman profile)
dotnet build Remind/Remind.csproj -c Install
```

## License

[MIT](LICENSE)
