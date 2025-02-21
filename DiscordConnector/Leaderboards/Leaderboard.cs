﻿using System;
using System.Collections.Generic;
using System.Timers;

namespace DiscordConnector
{
    internal class LeaderbBoard
    {
        private Leaderboards.Base leaderBoard1;
        private Leaderboards.Base leaderBoard2;
        private Leaderboards.Base leaderBoard3;
        private Leaderboards.Base leaderBoard4;
        private Leaderboards.Base leaderBoard5;
        public static readonly int MAX_LEADER_BOARD_SIZE = 16;

        public LeaderbBoard()
        {
            leaderBoard1 = new Leaderboards.Composer(0);
            leaderBoard2 = new Leaderboards.Composer(1);
            leaderBoard3 = new Leaderboards.Composer(2);
            leaderBoard4 = new Leaderboards.Composer(3);
            leaderBoard5 = new Leaderboards.Composer(4);
        }

        public Leaderboards.Base LeaderBoard1 => leaderBoard1;
        public Leaderboards.Base LeaderBoard2 => leaderBoard2;
        public Leaderboards.Base LeaderBoard3 => leaderBoard3;
        public Leaderboards.Base LeaderBoard4 => leaderBoard4;
        public Leaderboards.Base LeaderBoard5 => leaderBoard5;

        /// <summary>
        /// Takes a sorted list <paramref name="rankings"/> and returns a string listing each member on a line prepended with 1, 2, 3, etc.
        /// </summary>
        /// <param name="rankings">A pre-sorted list of CountResults.</param>
        /// <returns>String ready to send to discord listing each player and their value.</returns>
        public static string RankedCountResultToString(List<Records.CountResult> rankings)
        {
            string res = "";
            for (int i = 0; i < rankings.Count; i++)
            {
                res += $"{i + 1}: {rankings[i].Name}: {rankings[i].Count}{Environment.NewLine}";
            }
            return res;
        }

        // https://stackoverflow.com/a/4423615/624900
        private static string ToReadableString(TimeSpan span)
        {
            string formatted = string.Format("{0}{1}{2}{3}",
                span.Duration().Days > 0 ? string.Format("{0:0} day{1}, ", span.Days, span.Days == 1 ? string.Empty : "s") : string.Empty,
                span.Duration().Hours > 0 ? string.Format("{0:0} hour{1}, ", span.Hours, span.Hours == 1 ? string.Empty : "s") : string.Empty,
                span.Duration().Minutes > 0 ? string.Format("{0:0} minute{1}, ", span.Minutes, span.Minutes == 1 ? string.Empty : "s") : string.Empty,
                span.Duration().Seconds > 0 ? string.Format("{0:0} second{1}", span.Seconds, span.Seconds == 1 ? string.Empty : "s") : string.Empty);

            if (formatted.EndsWith(", ")) formatted = formatted.Substring(0, formatted.Length - 2);

            if (string.IsNullOrEmpty(formatted)) formatted = "0 seconds";

            return formatted;
        }

        /// <summary>
        /// Same as RankedCountResultToString but formats as a time duration, assuming integer seconds.
        /// </summary>
        /// <param name="rankings">A pre-sorted list of CountResults.</param>
        /// <returns>String ready to send to discord listing each player and their duration.</returns>
        public static string RankedSecondsToString(List<Records.CountResult> rankings)
        {
            string res = "";
            for (int i = 0; i < rankings.Count; i++)
            {
                string formattedDuration = ToReadableString(TimeSpan.FromSeconds(rankings[i].Count));
                res += $"{i + 1}: {rankings[i].Name}: {formattedDuration}{Environment.NewLine}";
            }
            return res;
        }
    }
}

namespace DiscordConnector.Leaderboards
{
    /// <summary>
    /// A base class for leaderboards to inherit from. It includes a method that lets the leader board be sent on a timer
    /// and an abstract method which sends the leader board.
    /// </summary>
    internal abstract class Base
    {
        /// <summary>
        /// An interface for sending the leader board as a timer event.
        /// </summary>
        public void SendLeaderBoardOnTimer(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            System.Threading.Tasks.Task.Run(() =>
            {
                this.SendLeaderBoard();
            });
        }

        /// <summary>
        /// Send the leader board to the DiscordAPI
        /// </summary>
        public abstract void SendLeaderBoard();
    }
    /// <summary>
    /// Time ranges that are supported for querying from the database using a "where" clause on the date.
    /// </summary>
    public enum TimeRange
    {
        [System.ComponentModel.Description("All Time")]
        AllTime,
        [System.ComponentModel.Description("Today")]
        Today,
        [System.ComponentModel.Description("Yesterday")]
        Yesterday,
        [System.ComponentModel.Description("Past 7 Days")]
        PastWeek,
        [System.ComponentModel.Description("Current Week, Sunday to Saturday")]
        WeekSundayToSaturday,
        [System.ComponentModel.Description("Current Week, Monday to Sunday")]
        WeekMondayToSunday,
    }
    /// <summary>
    /// Available options for sorting the results gathered from the database. This is used when defining the custom leader boards.
    /// </summary>
    public enum Ordering
    {
        [System.ComponentModel.Description("Most to Least (Descending)")]
        Descending,
        [System.ComponentModel.Description("Least to Most (Ascending)")]
        Ascending,
    }
    /// <summary>
    /// Tracked statistics which can be stored in the records database. The <see cref="TimeOnline"/> value is calculated dynamically.
    /// </summary>
    public enum Statistic
    {
        Death,
        Session,
        Shout,
        Ping,
        TimeOnline,
    }
    public static class DateHelper
    {
        /// <summary>
        /// A "dummy" date time, set to 20 years ago. This is used internally as both the start and end date to indicate all records.
        /// </summary>
        public static readonly System.DateTime DummyDateTime = System.DateTime.Now.AddYears(-20);

        /// <summary>
        /// Get a tuple with the start and end date for the specified <paramref name="timeRange"/>
        /// </summary>
        /// <param name="timeRange">TimeRange that you want the actual start and end date for</param>
        /// <returns>A tuple with two dates for the time range, where the earlier date is <code>Item1</code></returns>
        public static Tuple<System.DateTime, System.DateTime> StartEndDatesForTimeRange(TimeRange timeRange)
        {
            switch (timeRange)
            {
                case TimeRange.Today:
                    System.DateTime today = System.DateTime.Today;
                    return new Tuple<DateTime, DateTime>(today, today);

                case TimeRange.Yesterday:
                    System.DateTime yesterday = System.DateTime.Today.AddDays(-1.0);
                    return new Tuple<DateTime, DateTime>(yesterday, yesterday);

                case TimeRange.PastWeek:
                    System.DateTime weekAgo = System.DateTime.Today.AddDays(-7.0);
                    System.DateTime today1 = System.DateTime.Today;
                    return new Tuple<DateTime, DateTime>(weekAgo, today1);

                case TimeRange.WeekSundayToSaturday:
                    System.DateTime today2 = System.DateTime.Today;
                    int dow = (int)today2.DayOfWeek;

                    System.DateTime sunday = today2.AddDays(-dow);
                    System.DateTime saturday = today2.AddDays(6 - dow);
                    // If we are on sunday, show for the current week
                    if (today2.DayOfWeek == System.DayOfWeek.Sunday)
                    {
                        sunday = today2;
                        saturday = today2.AddDays(6);
                    }

                    return new Tuple<DateTime, DateTime>(sunday, saturday);

                case TimeRange.WeekMondayToSunday:
                    System.DateTime today3 = System.DateTime.Today;
                    int dow1 = (int)today3.DayOfWeek;

                    System.DateTime monday = today3.AddDays(1 - dow1); // Monday - day of week = goes backward to previous monday until we are in Sunday
                    System.DateTime sunday1 = today3.AddDays(7 - dow1); // (Next monday) - day of week = goes to next monday until we are in Sunday then shows next Sunday

                    // If we are on sunday, fix to show "current" week still
                    if (today3.DayOfWeek == System.DayOfWeek.Sunday)
                    {
                        monday = today3.AddDays(-6); // Sunday - 6 = previous monday
                        sunday = today3; // Sunday is today
                    }

                    return new Tuple<DateTime, DateTime>(monday, sunday1);

                case TimeRange.AllTime:
                    return new Tuple<DateTime, DateTime>(DummyDateTime, DummyDateTime);

                default:
                    DiscordConnectorPlugin.StaticLogger.LogWarning("DateHelper fell through, probably not wanted!");
                    return new Tuple<DateTime, DateTime>(DummyDateTime, DummyDateTime);
            }
        }
    }
}
