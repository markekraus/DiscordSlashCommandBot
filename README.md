# DiscordSlashCommandBot - The Discord Slash Command Bot Framework

This project demonstrates how to configure an extensible Discord Slash Command bot that uses Configuration and Dependency Injection.

Table of Contents

- [DiscordSlashCommandBot - The Discord Slash Command Bot Framework](#discordslashcommandbot---the-discord-slash-command-bot-framework)
  - [Structure](#structure)
  - [Configuration](#configuration)
  - [Dependency Injection](#dependency-injection)
  - [Creating New Slash Commands](#creating-new-slash-commands)

## Structure

- [.\DiscordSlashCommandBot.Commands](DiscordSlashCommandBot.Commands/) - Contains definitions and logic for Discord Slash Commands.
- [.\DiscordSlashCommandBot.Interfaces](DiscordSlashCommandBot.Interfaces/) - Contains interface definitions.
- [.\DiscordSlashCommandBot\Options](DiscordSlashCommandBot/Options/) - Contains IOptions models for configuration.
- [.\DiscordSlashCommandBot\Services\](DiscordSlashCommandBot/Services/) - Services to be consumed by the ServiceProvider container, including the primary bot service.
- [.\DiscordSlashCommandBot\Services\BotService.cs](DiscordSlashCommandBot/Services/BotService.cs) - The primary bot service responsible for the application logic.
- [.\DiscordSlashCommandBot\ServicesConfiguration.cs](DiscordSlashCommandBot/ServicesConfiguration.cs) - Where services are registered.
- [.\DiscordSlashCommandBot\Statics](DiscordSlashCommandBot/Statics/) - Stores static classes and values used throughout the application.
- [.\DiscordSlashCommandBot\Program.cs](DiscordSlashCommandBot/Program.cs) - Launches the program.

## Configuration

The `AppSettings` class defines the model for all application settings.
These can be set:

- With [User Secrets](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows) for development.
- In `appSettings.json`,
- Or as Environment variables prefixed with `DISCORD_BOT_` (e.g. `DISCORD_BOT_LogLevel`),

User secrets will be overwritten by `appSettings.json`, and those will be over written by environment variables.
You can change this precedence by changing the order in which they are add in `ServicesConfiguration.cs`.

## Dependency Injection

Dependency Injection is handled by [.\DiscordSlashCommandBot\ServicesConfiguration.cs](DiscordSlashCommandBot/ServicesConfiguration.cs).
The `AddBotServices()` extension method handles all configuration and registers all services and options.

## Creating New Slash Commands

1. Create class (preferably ending with `Command`, e.g. `EchoCommand`) and implement the `IBotSlashCommand` interface.
1. Create and return and un-built `SlashCommandBuilder` in `GetSlashCommandBuilder()`.
  The `BotService` will build the command and register it for you.
  **Do not implement your own command registration!**
1. Create your command handling logic in `SlashCommandHandler(SocketSlashCommand command)`.
  This will be called for you by `BotService` when a user uses your slash command.
1. Register the command as a singleton in `ServicesConfiguration`.
  Example: `services.AddSingleton<IBotSlashCommand, EchoCommand>();`
