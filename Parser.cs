using RCVConverter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static RCVConverter.Settings;

namespace RCVConverter
{
    public static class Parser
    {
        public static List<NationalBallot> ImportNationalBallots(List<string> candidates, Dictionary<Districts, List<string>> districtNames)
        {
            List<NationalBallot> ballots = new();

            foreach (var row in File.ReadAllLines("input.csv"))
            {
                Dictionary<string, int> votes = new();
                Districts? district = null;
                bool first = true;
                int i = 0;
                foreach (string field in row.Split(','))
                {
                    if (first)
                    {
                        district = districtNames.Where(x => x.Value.Contains(field.ToLowerInvariant())).Single().Key;
                        first = false;
                        continue;
                    }
                    string vote = Regex.Replace(field, "[^0-9]", "");

                    int rank;
                    if (string.IsNullOrEmpty(vote)) rank = 0;
                    else rank = int.Parse(vote);

                    votes.Add(candidates[i], rank);
                    i++;
                }

                if (correctRankNumbers) votes = CorrectSingleBallotNumbers(votes);
                ballots.Add(new NationalBallot(votes, (Districts)district));
            }

            return ballots;
        }

        public static List<Dictionary<string, int>> ImportBallots(List<string> candidates)
        {
            List<Dictionary<string, int>> ballots = new();

            foreach (var row in File.ReadAllLines("input.csv"))
            {
                Dictionary<string, int> votes = new();
                int i = 0;
                foreach (string field in row.Split(','))
                {
                    string vote = Regex.Replace(field, "[^0-9]", "");

                    int rank;
                    if (string.IsNullOrEmpty(vote)) rank = 0;
                    else rank = int.Parse(vote);

                    votes.Add(candidates[i], rank);
                    i++;
                }

                if (correctRankNumbers) votes = CorrectSingleBallotNumbers(votes);
                ballots.Add(votes);
            }

            return ballots;
        }

        //Tyco Corrections
        public static List<NationalBallot> CorrectNonConsecutiveBallotNumbers(IEnumerable<NationalBallot> ballots)
        {
            List<NationalBallot> newBallots = new();

            foreach (var ballot in ballots)
            {
                var voting = ballot.Voting.Where(y => y.Value != 0).OrderBy(y => y.Value).ToList();
                Dictionary<string, int> votes = new();
                foreach (var vote in ballot.Voting)
                {
                    if (vote.Value != 0)
                    {
                        votes.Add(vote.Key, voting.IndexOf(vote) + 1);
                    }
                    else
                    {
                        votes.Add(vote.Key, 0);
                    }
                }
                newBallots.Add(new(votes, ballot.District));
            }

            return newBallots;
        }

        public static List<Dictionary<string, int>> CorrectNonConsecutiveBallotNumbers(IEnumerable<Dictionary<string, int>> ballots)
        {
            List<Dictionary<string, int>> newBallots = new();

            foreach (var ballot in ballots)
            {
                Dictionary<string, int> votes = CorrectSingleBallotNumbers(ballot);
                newBallots.Add(votes);
            }

            return newBallots;
        }

        private static Dictionary<string, int> CorrectSingleBallotNumbers(Dictionary<string, int> ballot)
        {
            var voting = ballot.Where(y => y.Value != 0).OrderBy(y => y.Value).ToList();
            Dictionary<string, int> votes = new();
            foreach (var vote in ballot)
            {
                if (vote.Value != 0)
                {
                    votes.Add(vote.Key, voting.IndexOf(vote) + 1);
                }
                else
                {
                    votes.Add(vote.Key, 0);
                }
            }

            return votes;
        }
    }
}
