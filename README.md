# Discord Connector

[![CodeQL](https://github.com/nwesterhausen/valheim-discordconnector/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/nwesterhausen/valheim-discordconnector/actions/workflows/codeql-analysis.yml)
[![Build](https://github.com/nwesterhausen/valheim-discordconnector/actions/workflows/build-on-push-pr.yml/badge.svg)](https://github.com/nwesterhausen/valheim-discordconnector/actions/workflows/build-on-push-pr.yml)
![Dynamic JSON Badge](https://img.shields.io/badge/dynamic/json?url=https%3A%2F%2Fgithub.com%2Fnwesterhausen%2Fvalheim-discordconnector%2Fraw%2Frefs%2Fheads%2Fmain%2FDiscordConnector%2FMetadata%2Fmanifest.json&query=%24.version_number&prefix=v&style=flat&label=Server%20Version%20(Main)&color=darkviolet&labelColor=%2332393F)
![Dynamic JSON Badge - Client Version](https://img.shields.io/badge/dynamic/json?url=https%3A%2F%2Fgithub.com%2Fnwesterhausen%2Fvalheim-discordconnector%2Fraw%2Frefs%2Fheads%2Fmain%2FDiscordConnector.Client%2FMetadata%2Fmanifest.json&query=%24.version_number&prefix=v&style=flat&label=Client%20Version%20(Main)&color=darkviolet&labelColor=%2332393F)
![Built against Valheim version](https://img.shields.io/badge/Built_against_Valheim-0.220.4-purple?style=flat&labelColor=%2332393F)
![Thunderstore Version](https://img.shields.io/thunderstore/v/nwesterhausen/DiscordConnector?label=Thunderstore&labelColor=%2332393F)
![Thunderstore Version Client](https://img.shields.io/thunderstore/v/nwesterhausen/DiscordConnector_Client?label=Thunderstore%20(Client)&labelColor=%2332393F)
[![NexusMods](https://img.shields.io/badge/NexusMods-2.1.14-%23D98F40?style=flat&labelColor=%2332393F)](https://www.nexusmods.com/valheim/mods/1551/)

Connect your Valheim server to Discord. ([See website for installation or configuration instructions](https://discord-connector.valheim.games.nwest.one/)). This plugin is largely based on [valheim-discord-notifier](https://github.com/aequasi/valheim-discord-notifier), but this plugin supports randomized messages, muting players, and Discord message embeds.

## Plugin Details

See [the README](Metadata/README.md) for the plugin.

## Changelog

See [the changelog](docs/changelog.md).

## Building

To build, first get the path to your Valheim installation and also use the publicize tool to create a publicized version of the game. I'm not sure without that, if it will fail to build or not.

Then, run the following command to build the project:

```shell
dotnet build \
   -c Release \
   /p:GamePath="C:\Program Files (x86)\Steam\steamapps\common\Valheim" \
   DiscordConnector.sln
```

Post build, the compiled library and its dependencies get copied into `bin/DiscordConnector` which enables you to simply copy that folder into `$(GamePath)/BePinEx/plugins` for testing or use.

The compiled plugin will be in a zip ready for upload at `bin/DiscordConnector.zip`.

### Dependencies

For JSON serialization, using Newtonsoft.Json

For data storage/retrieval using [LiteDB](https://www.litedb.org/)
(If you want to read the database file generated, you can use [LiteDB Studio](https://github.com/mbdavid/LiteDB.Studio/releases/latest))

## Release Steps

Before release, to bump the version the following needs changed:

1. Update the version of the plugin in these files
   - `src/PluginInfo.cs`
   - `Metadata/DiscordConnector-Nexus.readme`
   - `Metadata/manifest.json`
   - `Metadata/thunderstore.toml`
2. Finalize the changelog entry in `docs/changelog.md`
3. Copy changelog notes into `Metadata/README.md`
