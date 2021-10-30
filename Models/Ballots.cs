namespace RCVConverter.Models
{
    public class NationalBallot : Ballot
    {
        public Districts District { get; set; }

        public NationalBallot(Dictionary<Candidates, int> voting, Districts district) : base(voting)
        {
            District = district;
        }
    }

    public class Ballot
    {
        public Dictionary<Candidates, int> Voting { get; set; }

        public Ballot(Dictionary<Candidates, int> voting)
        {
            Voting = voting;
        }
    }
}
