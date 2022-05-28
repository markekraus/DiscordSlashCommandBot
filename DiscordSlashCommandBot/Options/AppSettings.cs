// DiscordSlashCommandBot
// Copyright (C) 2022 Mark E. Kraus
// 
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with this program. If not, see <https://www.gnu.org/licenses/>.

using Microsoft.Extensions.Logging;

namespace DiscordSlashCommandBot.Options
{
    /// <summary>
    /// DiscordSlashCommandBot application settings.
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// The log level to use.
        /// </summary>
        /// <value>Default is <see cref="LogLevel.Warning"/></value>
        public LogLevel LogLevel {get; set;} = LogLevel.Warning;
        
        /// <summary>
        /// The discord bot token.
        /// For more info see <seealso href="https://discordnet.dev/guides/getting_started/first-bot.html" />
        /// </summary>
        /// <value>Example: OTY4NjczMTgwksgkhJG56Tk2NTY0.YmiRMw.aPixSEwmTqRr1SzfviYbLWoUNqo</value>
        public string DiscordBotToken {get; set;} = null!;

        /// <summary>
        /// List of Guild IDs for the bot to join.
        /// For more information see <seealso href="https://support.discord.com/hc/en-us/articles/206346498-Where-can-I-find-my-User-Server-Message-ID-"/>
        /// </summary>
        /// <value>Example: 969755114183217213</value>
        public IList<ulong> GuildIds {get; set;} = null!;
    }
}
