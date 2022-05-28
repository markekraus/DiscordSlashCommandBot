// DiscordSlashCommandBot
// Copyright (C) 2022 Mark E. Kraus
// 
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see <https://www.gnu.org/licenses/>.

using Discord;
using Discord.WebSocket;
using DiscordSlashCommandBot.Interfaces;

namespace DiscordSlashCommandBot.Commands
{
    /// <summary>
    /// Echo Slash Command returns echo's back a message to the user.
    /// </summary>
    public class EchoCommand : IBotSlashCommand
    {
        /// <summary>
        /// The name of the slash command. 'echo' will be `/echo` in Discord.
        /// </summary>
        private const string commandName = "echo";

        /// <summary>
        /// 'message' option name.
        /// </summary>
        private const string optionMessage = "message";

        /// <summary>
        /// Default constructor
        /// </summary>
        public EchoCommand() { }
        public SlashCommandBuilder GetSlashCommandBuilder()
        {
            return new SlashCommandBuilder()
                .WithName(commandName)
                .WithDescription("Echos back whatever message is provided.")
                .AddOption(
                    optionMessage,
                    ApplicationCommandOptionType.String,
                    "The message to be echoed",
                    isRequired: true);
        }

        public async Task SlashCommandHandler(SocketSlashCommand command)
        {
            await command.RespondAsync(
                text: command.Data.Options
                    .Where(o => o.Name == optionMessage)
                    .First()
                    .Value
                    .ToString());
        }
    }
}