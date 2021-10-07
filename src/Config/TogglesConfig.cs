// toggles

// toggles.messages
// - launch 
// - loaded
// - stop
// - join
// - leave
// - death
// - shout
// - ping

// toggles.position
// - join
// - leave
// - pings
// - shouts
// - deaths

// toggles.stats
// - allow record join
// - allow record ..
// - allow record ..
// - allow record ..
// - allow record ..

// toggles.leaderboard
// - send pings leaderboard
// - send join leaderboard
// - send .. leaderboard
// - send .. leaderboard
// - send .. leaderboard

using System;
using BepInEx.Configuration;

namespace DiscordConnector
{
    internal class TogglesConfig
    {
        private static ConfigFile config;
        public static string ConfigExention = "toggles";

        // Config Header Strings
        private const string MESSAGES_TOGGLES = "Toggles.Messages";
        private const string POSITION_TOGGLES = "Toggles.Position";
        private const string STATS_TOGGLES = "Toggles.Stats";
        private const string LEADERBOARD_TOGGLES = "Toggles.Leaderboard";

        // Logged Information Toggles
        private ConfigEntry<bool> serverLaunchToggle;
        private ConfigEntry<bool> serverLoadedToggle;
        private ConfigEntry<bool> serverStopToggle;
        private ConfigEntry<bool> serverShutdownToggle;
        private ConfigEntry<bool> chatShoutToggle;
        private ConfigEntry<bool> chatShoutPosToggle;
        private ConfigEntry<bool> chatPingToggle;
        private ConfigEntry<bool> chatPingPosToggle;
        private ConfigEntry<bool> playerJoinToggle;
        private ConfigEntry<bool> playerJoinPosToggle;
        private ConfigEntry<bool> playerDeathToggle;
        private ConfigEntry<bool> playerDeathPosToggle;
        private ConfigEntry<bool> playerLeaveToggle;
        private ConfigEntry<bool> playerLeavePosToggle;

        // Statistic collection settings
        private ConfigEntry<bool> collectStatsJoins;
        private ConfigEntry<bool> collectStatsLeaves;
        private ConfigEntry<bool> collectStatsDeaths;
        private ConfigEntry<bool> collectStatsShouts;
        private ConfigEntry<bool> collectStatsPings;

        // Send Leaderboard Strings
        private ConfigEntry<bool> sendSessionLeaderboard;
        private ConfigEntry<bool> sendPingsLeaderboard;
        private ConfigEntry<bool> sendDeathsLeaderboard;
        private ConfigEntry<bool> sendShoutsLeaderboard;

        public TogglesConfig(ConfigFile configFile)
        {
            config = configFile;

            LoadConfig();
        }

        private void LoadConfig()
        {
            // Message Toggles

            serverLaunchToggle = config.Bind<bool>(MESSAGES_TOGGLES,
                "Server Launch Notifications",
                true,
                "If enabled, this will send a message to Discord when the server launches (and the plugin is loaded)." + Environment.NewLine +
                "EX: Server has started. | Server has stopped.");

            serverLoadedToggle = config.Bind<bool>(MESSAGES_TOGGLES,
                "Server Loaded Notifications",
                true,
                "If enabled, this will send a message to Discord when the server has loaded the map and is ready for connections." + Environment.NewLine +
                "EX: Server has started. | Server has stopped.");

            serverStopToggle = config.Bind<bool>(MESSAGES_TOGGLES,
                "Server Stopping Notifications",
                true,
                "If enabled, this will send a message to Discord when the server begins stopping." + Environment.NewLine +
                "EX: Server is stopping.");

            serverShutdownToggle = config.Bind<bool>(MESSAGES_TOGGLES,
                "Server Shutdown Notifications",
                true,
                "If enabled, this will send a message to Discord when the server has shut down." + Environment.NewLine +
                "EX: Server has stopped.");

            chatShoutToggle = config.Bind<bool>(MESSAGES_TOGGLES,
                "Chat Shout Messages Notifications",
                true,
                "If enabled, this will send a message to Discord when a player joins the server." + Environment.NewLine +
                "EX: Nick shouted \"Hey you!\"");

            chatShoutPosToggle = config.Bind<bool>(POSITION_TOGGLES,
                "Chat Shout Messages Position Notifications",
                true,
                "If enabled, include a position with the arrival message." + Environment.NewLine +
                "EX: Nick shouted \"Hey you!\" (at -124, 81.4, -198.9)");

            chatPingToggle = config.Bind<bool>(MESSAGES_TOGGLES,
                "Ping Notifications",
                true,
                "If enabled, include a position with the arrival message." + Environment.NewLine +
                "If the top-level chat notifications are disabled, that will disable these messages." + Environment.NewLine +
                "EX: Nick pinged the map!");

            chatPingPosToggle = config.Bind<bool>(POSITION_TOGGLES,
                "Ping Notificiations Include Position",
                true,
                "If enabled, includes the coordinates of the ping.");

            playerJoinToggle = config.Bind<bool>(MESSAGES_TOGGLES,
                "Player Join Notifications",
                true,
                "If enabled, this will send a message to Discord when a player joins the server." + Environment.NewLine +
                "EX: Player has joined");

            playerJoinPosToggle = config.Bind<bool>(POSITION_TOGGLES,
                "Include POS With Player Join",
                true,
                "If enabled, this will include the coordinates of the player when they join.");

            playerDeathToggle = config.Bind<bool>(MESSAGES_TOGGLES,
                "Player Death Notifications",
                true,
                "If enabled, this will send a message to Discord when a player dies on the server." + Environment.NewLine +
                "EX: Player has died");

            playerDeathPosToggle = config.Bind<bool>(POSITION_TOGGLES,
                "Include POS With Player Death",
                true,
                "If enabled, this will include the coordinates of the player when they die.");

            playerLeaveToggle = config.Bind<bool>(MESSAGES_TOGGLES,
                "Player Leave Notifications",
                true,
                "If enabled, this will send a message to Discord when a player leaves the server." + Environment.NewLine +
                "EX: Player has left.");

            playerLeavePosToggle = config.Bind<bool>(POSITION_TOGGLES,
                "Include POS With Player Leave",
                true,
                "If enabled, this will include the coordinates of the player when they leave.");



            // Statistic Settings
            collectStatsDeaths = config.Bind<bool>(STATS_TOGGLES,
                "Collect and Send Player Death Stats",
                true,
                "If enabled, will collect and enable sending player death statistics.");
            collectStatsJoins = config.Bind<bool>(STATS_TOGGLES,
                "Collect and Send Player Join Stats",
                true,
                "If enabled, will collect and enable sending stat announcements for how many times a player has joined the game.");
            collectStatsLeaves = config.Bind<bool>(STATS_TOGGLES,
                "Collect and Send Player Leave Stats",
                true,
                "If enabled, will collect and enable sending stat announcements for how many times a player has left the game.");
            collectStatsPings = config.Bind<bool>(STATS_TOGGLES,
                "Collect and Send Player Ping Stats",
                true,
                "If enabled, will collect and enable sending stat announcements for number of pings made by a player.");
            collectStatsShouts = config.Bind<bool>(STATS_TOGGLES,
                "Collect and Send Player Shout Stats",
                true,
                "If enabled, will collect and enable sending stat announcements for number of times a player has shouted.");


            sendDeathsLeaderboard = config.Bind<bool>(LEADERBOARD_TOGGLES,
                "Send Periodic Leaderboard for Player Deaths",
                true,
                "If enabled (and leaderboards are enabled), will send a leaderboard for player deaths at the interval.");
            sendPingsLeaderboard = config.Bind<bool>(LEADERBOARD_TOGGLES,
                "Send Periodic Leaderboard for Player Pings",
                false,
                "If enabled (and leaderboards are enabled), will send a leaderboard for player pings at the interval.");
            sendSessionLeaderboard = config.Bind<bool>(LEADERBOARD_TOGGLES,
                "Send Periodic Leaderboard for Player Sessions",
                false,
                "If enabled (and leaderboards are enabled), will send a leaderboard for player sessions at the interval.");
            sendShoutsLeaderboard = config.Bind<bool>(LEADERBOARD_TOGGLES,
                "Send Periodic Leaderboard for Player Shouts",
                false,
                "If enabled (and leaderboards are enabled), will send a leaderboard for player shouts at the interval.");


            config.Save();
        }

        public string ConfigAsJson()
        {
            string jsonString = "{";
            jsonString += $"\"{MESSAGES_TOGGLES}\":{{";
            jsonString += $"\"launchMessageEnabled\":\"{LaunchMessageEnabled}\",";
            jsonString += $"\"loadedMessageEnabled\":\"{LoadedMessageEnabled}\",";
            jsonString += $"\"stopMessageEnabled\":\"{StopMessageEnabled}\",";
            jsonString += $"\"shutdownMessageEnabled\":\"{ShutdownMessageEnabled}\",";
            jsonString += $"\"chatShoutEnabled\":\"{ChatShoutEnabled}\",";
            jsonString += $"\"chatPingEnabled\":\"{ChatPingEnabled}\",";
            jsonString += $"\"playerJoinEnabled\":\"{PlayerJoinMessageEnabled}\",";
            jsonString += $"\"playerLeaveEnabled\":\"{PlayerLeaveMessageEnabled}\",";
            jsonString += $"\"playerDeathEnabled\":\"{PlayerDeathMessageEnabled}\",";
            jsonString += "},";

            jsonString += $"\"{POSITION_TOGGLES}\":{{";
            jsonString += $"\"chatShoutPosEnabled\":\"{ChatShoutPosEnabled}\",";
            jsonString += $"\"chatPingPosEnabled\":\"{ChatPingPosEnabled}\",";
            jsonString += $"\"playerJoinPosEnabled\":\"{PlayerJoinPosEnabled}\",";
            jsonString += $"\"playerLeavePosEnabled\":\"{PlayerLeavePosEnabled}\",";
            jsonString += $"\"playerDeathPosEnabled\":\"{PlayerDeathPosEnabled}\"";
            jsonString += "},";

            jsonString += $"\"{STATS_TOGGLES}\":{{";
            jsonString += $"\"statsDeathEnabled\":\"{StatsDeathEnabled}\",";
            jsonString += $"\"statsJoinEnabled\":\"{StatsJoinEnabled}\",";
            jsonString += $"\"statsLeaveEnabled\":\"{StatsLeaveEnabled}\",";
            jsonString += $"\"statsPingEnabled\":\"{StatsPingEnabled}\",";
            jsonString += $"\"statsShoutEnabled\":\"{StatsShoutEnabled}\"";
            jsonString += "},";

            jsonString += $"\"{LEADERBOARD_TOGGLES}\":{{";
            jsonString += $"\"leaderboardDeathEnabled\":\"{LeaderboardDeathEnabled}\",";
            jsonString += $"\"leaderboardPingEnabled\":\"{LeaderboardPingEnabled}\",";
            jsonString += $"\"leaderboardShoutEnabled\":\"{LeaderboardShoutEnabled}\",";
            jsonString += $"\"leaderboardSessionEnabled\":\"{LeaderboardSessionEnabled}\"";
            jsonString += "}";

            jsonString += "}";
            return jsonString;
        }

        public bool LaunchMessageEnabled => serverLaunchToggle.Value;
        public bool LoadedMessageEnabled => serverLaunchToggle.Value;
        public bool StopMessageEnabled => serverLaunchToggle.Value;
        public bool ShutdownMessageEnabled => serverShutdownToggle.Value;
        public bool ChatShoutEnabled => chatShoutToggle.Value;
        public bool ChatShoutPosEnabled => chatShoutPosToggle.Value;
        public bool ChatPingEnabled => chatPingToggle.Value;
        public bool ChatPingPosEnabled => chatPingPosToggle.Value;
        public bool PlayerJoinMessageEnabled => playerJoinToggle.Value;
        public bool PlayerJoinPosEnabled => playerJoinPosToggle.Value;
        public bool PlayerDeathMessageEnabled => playerJoinToggle.Value;
        public bool PlayerDeathPosEnabled => playerJoinPosToggle.Value;
        public bool PlayerLeaveMessageEnabled => playerLeaveToggle.Value;
        public bool PlayerLeavePosEnabled => playerLeavePosToggle.Value;
        public bool StatsDeathEnabled => collectStatsDeaths.Value;
        public bool StatsJoinEnabled => collectStatsJoins.Value;
        public bool StatsLeaveEnabled => collectStatsLeaves.Value;
        public bool StatsPingEnabled => collectStatsPings.Value;
        public bool StatsShoutEnabled => collectStatsShouts.Value;
        public bool LeaderboardDeathEnabled => sendDeathsLeaderboard.Value;
        public bool LeaderboardPingEnabled => sendPingsLeaderboard.Value;
        public bool LeaderboardSessionEnabled => sendSessionLeaderboard.Value;
        public bool LeaderboardShoutEnabled => sendShoutsLeaderboard.Value;
    }
}