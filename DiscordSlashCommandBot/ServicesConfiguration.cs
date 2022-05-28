// DiscordSlashCommandBot
// Copyright (C) 2022 Mark E. Kraus
// 
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see <https://www.gnu.org/licenses/>.

using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordSlashCommandBot.Commands;
using DiscordSlashCommandBot.Interfaces;
using DiscordSlashCommandBot.Options;
using DiscordSlashCommandBot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DiscordSlashCommandBot
{
    /// <summary>
    /// Houses the <see cref="AddBotServices(IServiceCollection)"/> extension method.
    /// </summary>
    public static class ServicesConfiguration
    {
        /// <summary>
        /// The name of the settings file to use for app configuration.
        /// Typically, this is set to appSettings.json
        /// </summary>
        private const string settingsFile = "appSettings.json";

        /// <summary>
        /// The environment variable name prefix to use for app configuration.
        /// If the Prefix is 'DISCORD_BOT_', then the value of 'DISCORD_BOT_LogLevel' can be used for the `LogLevel` configuration setting.
        /// </summary>
        private const string settingsEnvVarPrefix = "DISCORD_BOT_";

        /// <summary>
        /// <see cref="IServiceCollection"/> extension method for configuring the bot services.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to modify.</param>
        public static void AddBotServices(this IServiceCollection services)
        {
            // Builds the application configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddUserSecrets(Assembly.GetExecutingAssembly())
                .AddJsonFile(settingsFile, true)
                .AddEnvironmentVariables(settingsEnvVarPrefix)
                .Build();

            services.AddOptions();
            services.Configure<AppSettings>(configuration);
            var appSettings = services.BuildServiceProvider().GetRequiredService<IOptions<AppSettings>>().Value;

            // Configure logging
            services.AddLogging(logging =>
            {
                logging
                    .ClearProviders()
                    .SetMinimumLevel(appSettings.LogLevel)
                    .AddSystemdConsole(c =>
                    {
                        c.UseUtcTimestamp = true;
                        c.TimestampFormat = "[yyyy-MM-dd-HH:mm:ss] ";
                    });
            });

            // Add background services
            services.AddHostedService<BotService>();

            // Add DI objects to service container.
            // discord client
            services.AddSingleton<DiscordSocketClient>();
            services.Configure<InteractionServiceConfig>(c =>
            {
                c.DefaultRunMode = RunMode.Async;
                c.LogLevel = LogSeverity.Debug;
            });
            services.AddSingleton<InteractionService>();

            // Add Discord Bot Slash Commands
            services.AddSingleton<IBotSlashCommand, EchoCommand>();
        }
    }
}