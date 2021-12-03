using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BbcWebscrape
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> dates = GetDates(DateTime.Now.AddDays(-2), DateTime.Now);
            List<Results> results = GetResults(dates);

            Console.WriteLine("Hello World!");
        }

        static string GetFormattedDate(DateTime date)
        {
            string month = date.ToString("MMMM");
            string suffix = GetDaySuffix(date.Day);
            return date.DayOfWeek + "-" + date.Day + suffix + "-" + month;
        }
        static string GetDaySuffix(int day)
        {
            switch (day)
            {
                case 1:
                case 21:
                case 31:
                    return "st";
                case 2:
                case 22:
                    return "nd";
                case 3:
                case 23:
                    return "rd";
                default:
                    return "th";
            }
        }

        private static List<Results> GetResults(List<string> dates)
        {
            List<Results> results = new List<Results>();
            foreach (var date in dates)
            {
                string url = "https://push.api.bbci.co.uk/batch?t=%2Fdata%2Fbbc-morph-football-scores-match-list-data%2F" + date +
                                     "%2FstartDate%2F" + date +
                                     "%2FstartDate%2F2021-12-01%2FtodayDate%2F2021-12-03%2Ftournament%2Ffull-priority-order%2Fversion%2F2.4.6?timeout=5";
                string json = new System.Net.WebClient().DownloadString(url); // Gets JSON
                // 2021-11-30
                string replace = date.Substring(0, 4) + date.Substring(5, 2) + date.Substring(8, 2);
                json = json.Replace(GetFormattedDate(new DateTime(Convert.ToInt32(date.Substring(0, 4)), Convert.ToInt32(date.Substring(5, 2)),
                    Convert.ToInt32(date.Substring(8, 2)))), "Tuesday-30th-November");
                var objs = JsonConvert.DeserializeObject<Rootobject>(json); // Deserialises JSON
                foreach (var obj in objs.payload)
                {
                    if(obj.body == null) { continue; }
                    foreach (var match in obj.body.matchData)
                    {
                        foreach (var footballEvent in match.tournamentDatesWithEvents.Tuesday30thNovember)
                        {
                            foreach (var evnt in footballEvent.events) 
                            {
                                if (evnt.eventStatus != "post-event") { continue; }
                                Results res = new Results();
                                res.tournamentName = match.tournamentMeta.tournamentName.full;
                                res.matchDate = evnt.startTime.ToString().Substring(0, 10);
                                res.normalTimeMins = evnt.minutesElapsed.ToString();
                                res.minsAdded = evnt.minutesIntoAddedTime.ToString();
                                res.homeTeamName = evnt.homeTeam.name.full;
                                res.homeGoals = evnt.homeTeam.scores.score.ToString();
                                res.awayTeamName = evnt.awayTeam.name.full;
                                res.awayGoals = evnt.awayTeam.scores.score.ToString();
                                results.Add(res);
                            }
                        }
                    }
                }
            }
            return results;
        }


        private static List<string> GetDates(DateTime start, DateTime now)
        {
            List<string> dates = new List<string>();
            while (start <= now)
            {
                var mth = start.Month.ToString().Length == 2 ? start.Month.ToString() : '0' + start.Month.ToString();
                var dy = start.Day.ToString().Length == 2 ? start.Day.ToString() : '0' + start.Day.ToString();
                var year = start.Year;
                dates.Add(year + "-" + mth + "-" + dy);
                start = start.AddDays(1);
            }
            return dates;
        }
    }
    class Results
    {
        public string tournamentName;
        public string matchDate;
        public string homeTeamName;
        public string homeGoals;
        public string normalTimeMins;
        public string minsAdded;
        internal string awayTeamName;
        internal string awayGoals;
    }

    public class Rootobject
    {
        public Meta meta { get; set; }
        public Payload[] payload { get; set; }
    }

    public class Meta
    {
        public int pollFrequencyInMilliseconds { get; set; }
    }

    public class Payload
    {
        public Meta1 meta { get; set; }
        public Body body { get; set; }
    }

    public class Meta1
    {
        public int responseCode { get; set; }
        public string hash { get; set; }
        public string template { get; set; }
    }

    public class Body
    {
        public Fixturelistmeta fixtureListMeta { get; set; }
        public Matchdata[] matchData { get; set; }
    }

    public class Fixturelistmeta
    {
        public bool scorersButtonShouldBeEnabled { get; set; }
    }

    public class Matchdata
    {
        public Tournamentmeta tournamentMeta { get; set; }
        public Tournamentdateswithevents tournamentDatesWithEvents { get; set; }
    }

    public class Tournamentmeta
    {
        public string tournamentSlug { get; set; }
        public Tournamentname tournamentName { get; set; }
    }

    public class Tournamentname
    {
        public string first { get; set; }
        public string full { get; set; }
        public string abbreviation { get; set; }
    }

    public class Tournamentdateswithevents
    {
        public Tuesday30ThNovember[] Tuesday30thNovember { get; set; }
    }

    public class Tuesday30ThNovember
    {
        public Round round { get; set; }
        public Event[] events { get; set; }
    }

    public class Round
    {
        public string key { get; set; }
        public object name { get; set; }
    }

    public class Event
    {
        public string eventKey { get; set; }
        public DateTime startTime { get; set; }
        public bool isTBC { get; set; }
        public int? minutesElapsed { get; set; }
        public int? minutesIntoAddedTime { get; set; }
        public string eventStatus { get; set; }
        public string eventStatusNote { get; set; }
        public string eventStatusReason { get; set; }
        public string eventOutcomeType { get; set; }
        public string eventType { get; set; }
        public object seriesWinner { get; set; }
        public string cpsId { get; set; }
        public string cpsLive { get; set; }
        public Hometeam homeTeam { get; set; }
        public Awayteam awayTeam { get; set; }
        public Eventprogress eventProgress { get; set; }
        public Venue venue { get; set; }
        public object[] officials { get; set; }
        public object tournamentInfo { get; set; }
        public object eventActions { get; set; }
        public string startTimeInUKHHMM { get; set; }
        public string comment { get; set; }
        public string href { get; set; }
        public Tournamentname1 tournamentName { get; set; }
        public string tournamentSlug { get; set; }
    }

    public class Hometeam
    {
        public string key { get; set; }
        public Scores scores { get; set; }
        public object formation { get; set; }
        public string eventOutcome { get; set; }
        public Name name { get; set; }
    }

    public class Scores
    {
        public int? score { get; set; }
        public int? halfTime { get; set; }
        public int? fullTime { get; set; }
        public int? extraTime { get; set; }
        public int? shootout { get; set; }
        public object aggregate { get; set; }
        public object aggregateGoalsAway { get; set; }
    }

    public class Name
    {
        public string first { get; set; }
        public string full { get; set; }
        public string abbreviation { get; set; }
        public object last { get; set; }
    }

    public class Awayteam
    {
        public string key { get; set; }
        public Scores1 scores { get; set; }
        public object formation { get; set; }
        public string eventOutcome { get; set; }
        public Name1 name { get; set; }
    }

    public class Scores1
    {
        public int? score { get; set; }
        public int? halfTime { get; set; }
        public int? fullTime { get; set; }
        public int? extraTime { get; set; }
        public int? shootout { get; set; }
        public object aggregate { get; set; }
        public object aggregateGoalsAway { get; set; }
    }

    public class Name1
    {
        public string first { get; set; }
        public string full { get; set; }
        public string abbreviation { get; set; }
        public object last { get; set; }
    }

    public class Eventprogress
    {
        public string period { get; set; }
        public string status { get; set; }
    }

    public class Venue
    {
        public Name2 name { get; set; }
        public string homeCountry { get; set; }
    }

    public class Name2
    {
        public string abbreviation { get; set; }
        public string videCode { get; set; }
        public string first { get; set; }
        public string full { get; set; }
    }

    public class Tournamentname1
    {
        public string first { get; set; }
        public string full { get; set; }
        public string abbreviation { get; set; }
    }

}