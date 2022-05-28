// DiscordSlashCommandBot
// Copyright (C) 2022 Mark E. Kraus
// 
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see <https://www.gnu.org/licenses/>.

using Discord;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using DiscordSlashCommandBot.Interfaces;
using DiscordSlashCommandBot.Options;
using DiscordSlashCommandBot.Statics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DiscordSlashCommandBot.Services
{
    /// <summary>
    /// Primary background service for operating the discord bot.
    /// </summary>
    public class BotService : IHostedService
    {
        private IOptions<AppSettings> _settings;
        private ILogger<BotService> _log;
        private DiscordSocketClient _client;
        private InteractionService _service;
        private IEnumerable<IBotSlashCommand> _botSlashCommands;
        private Dictionary<string, IBotSlashCommand> _commandMap = new Dictionary<string, IBotSlashCommand>();
        /// <summary>
        /// Creates a new instance of the <see cref="BotService"/> for operating the discord bot.
        /// </summary>
        /// <param name="Settings"><see cref="AppSettings" /> instance for configuring the service</param>
        /// <param name="Logger"><see cref="ILogger" /> used for logging.</param>
        /// <param name="Client"><see cref="DiscordSocketClient" /> discord client use to access Discord.</param>
        /// <param name="Service"><see cref="InteractionService" /> discord interactions service for handling slash commands and other interactions.</param>
        /// <param name="BotSlashCommands">Enumerable collection of <see cref="IBotSlashCommand" /> bot slash commands.</param>
        public BotService(
            IOptions<AppSettings> Settings,
            ILogger<BotService> Logger,
            DiscordSocketClient Client,
            InteractionService Service,
            IEnumerable<IBotSlashCommand> BotSlashCommands
        )
        {
            _settings = Settings;
            _log = Logger;
            _client = Client;
            _service = Service;
            _botSlashCommands = BotSlashCommands;

            _client.Log += OnDiscordLog;
            _service.Log += OnDiscordLog;
            _client.Ready += OnDiscordClientReadyAsync;
            _client.SlashCommandExecuted += OnDiscordSlashCommandExecutedAsync;
        }

        /// <summary>
        /// Callback method used for <see cref="InteractionService.SlashCommandExecuted"/>.
        /// Fires every time a Slash Command is issued by a discord user
        /// </summary>
        /// <param name="command"> The <see cref="SocketSlashCommand"/> that was issued by the user.</param>
        /// <returns></returns>
        private async Task OnDiscordSlashCommandExecutedAsync(SocketSlashCommand command)
        {
            if (!_commandMap.ContainsKey(command.CommandName))
            {
                _log.LogWarning(EventIDs.SlashCommandNotFound, $"Unable to find slash command handler for slash command '{command.CommandName}'");
                return;
            }
            var botSlashCommand = _commandMap[command.CommandName];
            _log.LogInformation($"Processing slash command '{command.CommandName}' from user '{command.User.Id}' in channel '{command.Channel.Id}'.");
            await botSlashCommand.SlashCommandHandler(command);
        }

        /// <summary>
        /// Callback method used for <see cref="DiscordSocketClient.Ready"/>.
        /// Fires when the discord client is connected and ready for use.
        /// Registers slash commands with discord.
        /// </summary>
        /// <returns></returns>
        private async Task OnDiscordClientReadyAsync()
        {
            foreach (var command in _botSlashCommands)
            {
                var builder = command.GetSlashCommandBuilder();
                _commandMap.Add(builder.Name, command);
                foreach (var guildId in _settings.Value.GuildIds)
                {
                    var guild = _client.GetGuild(guildId);
                    try
                    {
                        await guild.CreateApplicationCommandAsync(builder.Build());
                        _log.LogInformation($"Registered '{builder.Name}' to guildId '{guildId}' guildName '{guild.Name}'.");
                    }
                    catch (HttpException exception)
                    {
                        _log.LogError(EventIDs.FailedSlashCommandRegistration, exception, $"Failed to register '{builder.Name}' to guildId '{guildId}' guildName '{guild.Name}'.", builder);
                    }
                }
            }
        }

        /// <summary>
        /// Callback method used for <see cref="DiscordSocketClient.Log"/> and <see cref="InteractionService.Log"/>.
        /// Maps the <see cref="LogSeverity"/> in <see cref="Discord"/> to <see cref="ILogger"/> log levels and logs the message.
        /// </summary>
        /// <param name="msg">The <see cref="LogMessage"/> from the discord client</param>
        /// <returns></returns>
        private Task OnDiscordLog(LogMessage msg)
        {
            switch (msg.Severity)
            {
                case LogSeverity.Info:
                    _log.LogInformation(msg.ToString(), msg);
                    break;
                case LogSeverity.Critical:
                    _log.LogCritical(msg.ToString(), msg);
                    break;
                case LogSeverity.Error:
                    _log.LogError(msg.ToString(), msg);
                    break;
                case LogSeverity.Warning:
                    _log.LogWarning(msg.ToString(), msg);
                    break;
                case LogSeverity.Debug:
                    _log.LogDebug(msg.ToString(), msg);
                    break;
                case LogSeverity.Verbose:
                    _log.LogTrace(msg.ToString(), msg);
                    break;
                default: break;
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Primary background task to keep the server running.
        /// Performs the discord client login and start, then sleep until the cancellation token is cancelled. 
        /// </summary>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> passed in from <see cref="StartAsync(CancellationToken)"/>.</param>
        /// <returns></returns>
        private async Task RunAsync(CancellationToken cancellationToken)
        {
            _log.LogInformation("Starting bot service.");
            _log.LogInformation($"LogLevel: {_settings.Value.LogLevel}");
            _log.LogInformation($"DiscordBotToken Found: {!string.IsNullOrWhiteSpace(_settings.Value.DiscordBotToken)}");

            await _client.LoginAsync(TokenType.Bot, _settings.Value.DiscordBotToken);
            await _client.StartAsync();

            cancellationToken.WaitHandle.WaitOne();
        }

        /// <summary>
        /// Called by <see cref="HostingAbstractionsHostExtensions.RunAsync"/> when the host starts.
        /// Starts the bot service.
        /// </summary>
        /// <param name="cancellationToken"><see cref="CancellationToken"/>passed in by the host.</param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(() => RunAsync(cancellationToken));
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called by <see cref="HostingAbstractionsHostExtensions.StopAsync"/> when the host stops.
        /// Stops the bot service.
        /// </summary>
        /// <param name="cancellationToken"><see cref="CancellationToken"/>passed in by the host.</param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _log.LogInformation("Stopping bot service");
            return Task.CompletedTask;
        }
    }
}
