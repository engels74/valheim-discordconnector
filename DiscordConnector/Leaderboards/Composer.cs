using System;
using System.Collections.Generic;
using DiscordConnector.Config;
using DiscordConnector.Records;
// Required for embedding support
using DiscordConnector;

namespace DiscordConnector.Leaderboards;

internal class Composer : Base
{
    private readonly int leaderBoardIdx;

    public Composer(int leaderBoard)
    {
        leaderBoardIdx = leaderBoard;
    }

    public override void SendLeaderBoard()
    {
        if (DiscordConnectorPlugin.StaticConfig.DebugLeaderboardOperations)
        {
            DiscordConnectorPlugin.StaticLogger.LogInfo($"Starting to build leaderboard {leaderBoardIdx}");
        }

        if (leaderBoardIdx > DiscordConnectorPlugin.StaticConfig.LeaderBoards.Length || leaderBoardIdx < 0)
        {
            DiscordConnectorPlugin.StaticLogger.LogWarning(
                $"Tried to get leader board out of index bounds (index:{leaderBoardIdx}, length:{DiscordConnectorPlugin.StaticConfig.LeaderBoards.Length})");
            return;
        }

        LeaderBoardConfigReference settings = DiscordConnectorPlugin.StaticConfig.LeaderBoards[leaderBoardIdx];
        Webhook.Event ev = DiscordConnectorPlugin.StaticConfig.LeaderBoards[leaderBoardIdx].WebhookEvent;

        if (DiscordConnectorPlugin.StaticConfig.DebugLeaderboardOperations)
        {
            DiscordConnectorPlugin.StaticLogger.LogInfo($"Leaderboard {leaderBoardIdx} settings: Enabled={settings.Enabled}, Webhook Event={ev}");
        }

        if (!settings.Enabled)
        {
            if (DiscordConnectorPlugin.StaticConfig.DebugLeaderboardOperations)
            {
                DiscordConnectorPlugin.StaticLogger.LogInfo($"Leaderboard {leaderBoardIdx} is disabled, exiting");
            }
            return;
        }

        // Build standings
        if (DiscordConnectorPlugin.StaticConfig.DebugLeaderboardOperations)
        {
            DiscordConnectorPlugin.StaticLogger.LogInfo($"Building rankings for leaderboard {leaderBoardIdx}");
        }
        Dictionary<Statistic, List<CountResult>> rankings = makeRankings(settings);

        if (DiscordConnectorPlugin.StaticConfig.DebugLeaderboardOperations)
        {
            DiscordConnectorPlugin.StaticLogger.LogInfo($"Rankings retrieved for leaderboard {leaderBoardIdx}: {string.Join(", ", rankings.Keys)}");
            foreach (var key in rankings.Keys)
            {
                DiscordConnectorPlugin.StaticLogger.LogInfo($"  - {key}: {rankings[key]?.Count ?? 0} results");
            }
        }

        // Build leader board for discord
        List<Tuple<string, string>> leaderFields = new();

        // Check if there are rankings for each statistic. If there are, we want to build and add it.
        if (rankings.ContainsKey(Statistic.Death))
        {
            List<CountResult> deathRankings;
            if (rankings.TryGetValue(Statistic.Death, out deathRankings))
            {
                if (DiscordConnectorPlugin.StaticConfig.DebugLeaderboardOperations)
                {
                    DiscordConnectorPlugin.StaticLogger.LogInfo($"Found {deathRankings?.Count ?? 0} death rankings");
                }
                if (deathRankings?.Count > 0)
                {
                    leaderFields.Add(Tuple.Create("Deaths", LeaderbBoard.RankedCountResultToString(deathRankings)));
                }
            }
        }

        if (rankings.ContainsKey(Statistic.Session))
        {
            List<CountResult> sessionRankings;
            if (rankings.TryGetValue(Statistic.Session, out sessionRankings))
            {
                if (sessionRankings?.Count > 0)
                {
                    leaderFields.Add(Tuple.Create("Sessions", LeaderbBoard.RankedCountResultToString(sessionRankings)));
                }
            }
        }

        if (rankings.ContainsKey(Statistic.Shout))
        {
            List<CountResult> shoutRankings;
            if (rankings.TryGetValue(Statistic.Shout, out shoutRankings))
            {
                if (shoutRankings?.Count > 0)
                {
                    leaderFields.Add(Tuple.Create("Shouts", LeaderbBoard.RankedCountResultToString(shoutRankings)));
                }
            }
        }

        if (rankings.ContainsKey(Statistic.Ping))
        {
            List<CountResult> pingRankings;
            if (rankings.TryGetValue(Statistic.Ping, out pingRankings))
            {
                if (pingRankings?.Count > 0)
                {
                    leaderFields.Add(Tuple.Create("Pings", LeaderbBoard.RankedCountResultToString(pingRankings)));
                }
            }
        }

        if (rankings.ContainsKey(Statistic.TimeOnline))
        {
            List<CountResult> timeOnlineRankings;
            if (rankings.TryGetValue(Statistic.TimeOnline, out timeOnlineRankings))
            {
                if (timeOnlineRankings?.Count > 0)
                {
                    leaderFields.Add(
                        Tuple.Create("Time Online", LeaderbBoard.RankedSecondsToString(timeOnlineRankings)));
                }
            }
        }

        // Debug the leaderFields
        if (DiscordConnectorPlugin.StaticConfig.DebugLeaderboardOperations)
        {
            DiscordConnectorPlugin.StaticLogger.LogInfo($"Built {leaderFields.Count} leader fields for leaderboard {leaderBoardIdx}");
            foreach (var field in leaderFields)
            {
                DiscordConnectorPlugin.StaticLogger.LogInfo($"  - Field: {field.Item1}, Content length: {field.Item2?.Length ?? 0}");
            }
        }

        string discordContent = MessageTransformer.FormatLeaderBoardHeader(
            settings.DisplayedHeading, settings.NumberListings
        );

        // Get the world name if available
        string worldName = "unknown";
        if (ZNet.instance != null)
        {
            worldName = ZNet.instance.GetWorldName();
        }
        
        if (DiscordConnectorPlugin.StaticConfig.DebugLeaderboardOperations)
        {
            DiscordConnectorPlugin.StaticLogger.LogInfo($"About to send leaderboard {leaderBoardIdx} to Discord. Embed enabled: {DiscordConnectorPlugin.StaticConfig.DiscordEmbedsEnabled}, World: {worldName}, Field count: {leaderFields.Count}");
        }
        
        // Check if we have any fields to send
        if (leaderFields.Count == 0)
        {
            if (DiscordConnectorPlugin.StaticConfig.DebugLeaderboardOperations)
            {
                DiscordConnectorPlugin.StaticLogger.LogWarning($"No leader fields to send for leaderboard {leaderBoardIdx} - skipping webhook message");
            }
            return; // No point in sending an empty leaderboard
        }
        
        // Check if embeds are enabled in the configuration
        if (DiscordConnectorPlugin.StaticConfig.DiscordEmbedsEnabled)
        {
            // Create and send the embed using the new LeaderboardEmbed template
            var embedBuilder = EmbedTemplates.LeaderboardEmbed(
                settings.DisplayedHeading,
                leaderFields,
                worldName
            );
            
            if (DiscordConnectorPlugin.StaticConfig.DebugLeaderboardOperations)
            {
                DiscordConnectorPlugin.StaticLogger.LogInfo($"Sending embed for leaderboard {leaderBoardIdx}");
            }
            
            try {
                DiscordApi.SendEmbed(ev, embedBuilder);
                if (DiscordConnectorPlugin.StaticConfig.DebugLeaderboardOperations)
                {
                    DiscordConnectorPlugin.StaticLogger.LogInfo($"Successfully sent embed for leaderboard {leaderBoardIdx}");
                }
            } catch (Exception ex) {
                DiscordConnectorPlugin.StaticLogger.LogError($"Error sending leaderboard {leaderBoardIdx} embed: {ex.Message}");
            }
        }
        else
        {
            // Fallback to plain text version if embeds are not enabled
            if (DiscordConnectorPlugin.StaticConfig.DebugLeaderboardOperations)
            {
                DiscordConnectorPlugin.StaticLogger.LogInfo($"Sending plain text message for leaderboard {leaderBoardIdx}");
            }
            
            try {
                DiscordApi.SendMessageWithFields(ev, discordContent, leaderFields);
                if (DiscordConnectorPlugin.StaticConfig.DebugLeaderboardOperations)
                {
                    DiscordConnectorPlugin.StaticLogger.LogInfo($"Successfully sent plain text message for leaderboard {leaderBoardIdx}");
                }
            } catch (Exception ex) {
                DiscordConnectorPlugin.StaticLogger.LogError($"Error sending leaderboard {leaderBoardIdx} plain text: {ex.Message}");
            }
        }
    }

    private Dictionary<Statistic, List<CountResult>> makeRankings(LeaderBoardConfigReference settings)
    {
        if (DiscordConnectorPlugin.StaticConfig.DebugLeaderboardOperations)
        {
            DiscordConnectorPlugin.StaticLogger.LogInfo($"Making rankings with time range: {settings.TimeRange}");
        }
        
        if (settings.TimeRange == TimeRange.AllTime)
        {
            if (DiscordConnectorPlugin.StaticConfig.DebugLeaderboardOperations)
            {
                DiscordConnectorPlugin.StaticLogger.LogInfo("Using AllRankings method");
            }
            return AllRankings(settings);
        }

        Tuple<DateTime, DateTime>? BeginEndDate = DateHelper.StartEndDatesForTimeRange(settings.TimeRange);
        if (DiscordConnectorPlugin.StaticConfig.DebugLeaderboardOperations)
        {
            DiscordConnectorPlugin.StaticLogger.LogInfo($"Using TimeBasedRankings with range: {BeginEndDate.Item1} to {BeginEndDate.Item2}");
        }
        return TimeBasedRankings(settings, BeginEndDate.Item1, BeginEndDate.Item2);
    }

    private Dictionary<Statistic, List<CountResult>> AllRankings(LeaderBoardConfigReference settings)
    {
        Dictionary<Statistic, List<CountResult>> Dict = new();
        if (settings.Type == Ordering.Descending)
        {
            if (settings.Deaths)
            {
                Dict.Add(Statistic.Death, Helper.TopNResultForCategory(Categories.Death, settings.NumberListings));
            }

            if (settings.Sessions)
            {
                Dict.Add(Statistic.Session, Helper.TopNResultForCategory(Categories.Join, settings.NumberListings));
            }

            if (settings.Shouts)
            {
                Dict.Add(Statistic.Shout, Helper.TopNResultForCategory(Categories.Shout, settings.NumberListings));
            }

            if (settings.Pings)
            {
                Dict.Add(Statistic.Ping, Helper.TopNResultForCategory(Categories.Ping, settings.NumberListings));
            }

            if (settings.TimeOnline)
            {
                Dict.Add(Statistic.TimeOnline,
                    Helper.TopNResultForCategory(Categories.TimeOnline, settings.NumberListings));
            }
        }

        if (settings.Type == Ordering.Ascending)
        {
            if (settings.Deaths)
            {
                Dict.Add(Statistic.Death, Helper.BottomNResultForCategory(Categories.Death, settings.NumberListings));
            }

            if (settings.Sessions)
            {
                Dict.Add(Statistic.Session, Helper.BottomNResultForCategory(Categories.Join, settings.NumberListings));
            }

            if (settings.Shouts)
            {
                Dict.Add(Statistic.Shout, Helper.BottomNResultForCategory(Categories.Shout, settings.NumberListings));
            }

            if (settings.Pings)
            {
                Dict.Add(Statistic.Ping, Helper.BottomNResultForCategory(Categories.Ping, settings.NumberListings));
            }

            if (settings.TimeOnline)
            {
                Dict.Add(Statistic.TimeOnline,
                    Helper.BottomNResultForCategory(Categories.TimeOnline, settings.NumberListings));
            }
        }

        DiscordConnectorPlugin.StaticLogger.LogDebug($"Prepared to send leaderboard for {Dict.Keys.Count} values");
        printDict(Dict);

        return Dict;
    }


    private Dictionary<Statistic, List<CountResult>> TimeBasedRankings(LeaderBoardConfigReference settings,
        DateTime startDate, DateTime endDate)
    {
        Dictionary<Statistic, List<CountResult>> Dict = new();
        if (settings.Type == Ordering.Descending)
        {
            if (settings.Deaths)
            {
                Dict.Add(Statistic.Death,
                    Helper.TopNResultForCategory(Categories.Death, settings.NumberListings, startDate, endDate));
            }

            if (settings.Sessions)
            {
                Dict.Add(Statistic.Session,
                    Helper.TopNResultForCategory(Categories.Join, settings.NumberListings, startDate, endDate));
            }

            if (settings.Shouts)
            {
                Dict.Add(Statistic.Shout,
                    Helper.TopNResultForCategory(Categories.Shout, settings.NumberListings, startDate, endDate));
            }

            if (settings.Pings)
            {
                Dict.Add(Statistic.Ping,
                    Helper.TopNResultForCategory(Categories.Ping, settings.NumberListings, startDate, endDate));
            }

            if (settings.TimeOnline)
            {
                Dict.Add(Statistic.TimeOnline,
                    Helper.TopNResultForCategory(Categories.TimeOnline, settings.NumberListings, startDate, endDate));
            }
        }

        if (settings.Type == Ordering.Ascending)
        {
            if (settings.Deaths)
            {
                Dict.Add(Statistic.Death,
                    Helper.BottomNResultForCategory(Categories.Death, settings.NumberListings, startDate, endDate));
            }

            if (settings.Sessions)
            {
                Dict.Add(Statistic.Session,
                    Helper.BottomNResultForCategory(Categories.Join, settings.NumberListings, startDate, endDate));
            }

            if (settings.Shouts)
            {
                Dict.Add(Statistic.Shout,
                    Helper.BottomNResultForCategory(Categories.Shout, settings.NumberListings, startDate, endDate));
            }

            if (settings.Pings)
            {
                Dict.Add(Statistic.Ping,
                    Helper.BottomNResultForCategory(Categories.Ping, settings.NumberListings, startDate, endDate));
            }

            if (settings.TimeOnline)
            {
                Dict.Add(Statistic.TimeOnline,
                    Helper.BottomNResultForCategory(Categories.Ping, settings.NumberListings, startDate, endDate));
            }
        }

        DiscordConnectorPlugin.StaticLogger.LogDebug($"Prepared to send leaderboard for {Dict.Keys.Count} values");
        printDict(Dict);

        return Dict;
    }

    private static void printDict(Dictionary<Statistic, List<CountResult>> dict)
    {
        foreach (KeyValuePair<Statistic, List<CountResult>> pair in dict)
        {
            string outStr = "";
            foreach (CountResult? x in pair.Value)
            {
                outStr += x + ",";
            }

            DiscordConnectorPlugin.StaticLogger.LogDebug($"{pair.Key}: {outStr}");
        }
    }
}
