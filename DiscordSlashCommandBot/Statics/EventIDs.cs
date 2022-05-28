using Microsoft.Extensions.Logging;

namespace DiscordSlashCommandBot.Statics
{
    /// <summary>
    /// Contains multiple static <see cref="EventId"/> used for error events.
    /// </summary>
    public class EventIDs
    {
        /// <summary>
        /// EventId 10001: Slash Command failed to register.
        /// </summary>
        public static EventId FailedSlashCommandRegistration = new EventId(10001, "Slash Command failed to register.");

        /// <summary>
        /// EventId 10002: No slash command was found to handle slash command event.
        /// </summary>
        public static EventId SlashCommandNotFound = new EventId(10002, "No slash command was found to handle slash command event.");
    }
}