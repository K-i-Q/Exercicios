using System;
using System.Net.Http;
using Newtonsoft.Json.Linq;

public class Program
{
    private static readonly HttpClient client = new HttpClient();

    public static void Main()
    {
        string teamName = "Paris Saint-Germain";
        int year = 2013;
        int goalsByTeam1 = getTotalScoredGoals("Paris Saint-Germain", year, true);
        int goalsByTeam2 = getTotalScoredGoals("Paris Saint-Germain", year, false);

        int totalGoals = goalsByTeam1 + goalsByTeam2;
        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        teamName = "Chelsea";
        year = 2014;
        goalsByTeam1 = getTotalScoredGoals("Chelsea", year, true);
        goalsByTeam2 = getTotalScoredGoals("Chelsea", year, false);
        totalGoals = goalsByTeam1 + goalsByTeam2;
        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        // Output expected:
        // Team Paris Saint - Germain scored 109 goals in 2013
        // Team Chelsea scored 92 goals in 2014
    }

    public static int getTotalScoredGoals(string team, int year, bool isTeam1)
    {
        string apiUrl = $"https://jsonmock.hackerrank.com/api/football_matches?year={year}&{(isTeam1 ? "team1" : "team2")}={team}";
        HttpResponseMessage response = client.GetAsync(apiUrl).Result;
        string responseBody = response.Content.ReadAsStringAsync().Result;

        JObject json = JObject.Parse(responseBody);
        int totalPages = (int)json["total_pages"];
        int totalGoals = 0;

        for (int page = 1; page <= totalPages; page++)
        {
            string pageUrl = $"{apiUrl}&page={page}";
            HttpResponseMessage pageResponse = client.GetAsync(pageUrl).Result;
            string pageBody = pageResponse.Content.ReadAsStringAsync().Result;

            JObject pageJson = JObject.Parse(pageBody);
            JArray data = (JArray)pageJson["data"];

            foreach (JObject match in data)
            {
                if (team.Equals(match[isTeam1 ? "team1" : "team2"].ToString()))
                {
                    totalGoals += int.Parse(match[isTeam1 ? "team1goals" : "team2goals"].ToString());
                }
            }
        }

        return totalGoals;
    }


}
