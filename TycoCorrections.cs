using RCVConverter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCVConverter
{
    public static class TycoCorrections
    {
        public static List<NationalBallot> CorrectNonConsecutiveBallotNumbers(List<NationalBallot> ballots)
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

        public static List<Ballot> CorrectNonConsecutiveBallotNumbers(List<Ballot> ballots)
        {
            List<Ballot> newBallots = new();

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
                newBallots.Add(new(votes));
            }

            return newBallots;
        }
    }
}
