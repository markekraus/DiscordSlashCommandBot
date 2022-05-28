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
    /// Source Code Slash Command returns a link to the bot's source code.
    /// </summary>
    public class SourceCodeCommand : IBotSlashCommand
    {
        /// <summary>
        /// The name of the slash command. 'source-code' will be `/echo` in Discord.
        /// </summary>
        private const string commandName = "source-code";

        /// <summary>
        /// The url to the bot's source code.
        /// </summary>
        private const string sourceCodeUrl = "https://github.com/markekraus/DiscordSlashCommandBot";

        /// <summary>
        /// Default constructor
        /// </summary>
        public SourceCodeCommand() { }
        public SlashCommandBuilder GetSlashCommandBuilder()
        {
            return new SlashCommandBuilder()
                .WithName(commandName)
                .WithDescription("Provides a link to the bot's source code.");
        }

        public async Task SlashCommandHandler(SocketSlashCommand command)
        {
            await command.RespondAsync(text: sourceCodeUrl);
        }
    }
}