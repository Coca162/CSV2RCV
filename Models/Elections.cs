namespace RCVConverter.Models
{
    public class NationalElection : Election
    {
        public string Name { get; set; }

        public new NationalBallot Ballot { get; set; }

        public NationalElection(NationalBallot ballot, string name) : base(ballot)
        {
            Name = name;
            Ballot = ballot;
        }
    }

    public class DistrictElection : Election
    {
        public Districts District { get; set; }

        public DistrictElection(Ballot ballot, Districts district) : base(ballot)
        {
            District = district;
        }
    }

    public class Election
    {
        public List<string> Candidates { get; set; }

        public Ballot Ballot { get; set; }

        public Election(Ballot ballot)
        {
            Ballot = ballot;
            Candidates = ((Candidates[])Enum.GetValues(typeof(Candidates))).Select(c => c.ToString()).ToList();
        }
    }
}
