namespace RCVConverter.Models
{
    public class NationalElection : Election
    {
        public string Name { get; set; }

        public new NationalBallot Ballot { get; set; }

        public NationalElection(NationalBallot ballot, List<string> candidates, string name) : base(ballot, candidates)
        {
            Name = name;
            Ballot = ballot;
        }
    }

    public class DistrictElection : Election
    {
        public Districts District { get; set; }

        public DistrictElection(Ballot ballot, List<string> candidates, Districts district) : base(ballot, candidates)
        {
            District = district;
        }
    }

    public class Election
    {
        public List<string> Candidates { get; set; }

        public Ballot Ballot { get; set; }

        public Election(Ballot ballot, List<string> candidates)
        {
            Ballot = ballot;
            Candidates = candidates;
        }
    }
}
