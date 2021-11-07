namespace RCVConverter.Models
{
    public class NationalBallot : Ballot
    {
        public Districts District { get; set; }

        public NationalBallot(Dictionary<string, int> voting, Districts district) : base(voting)
        {
            District = district;
        }
    }

    public class Ballot
    {
        public Dictionary<string, int> Voting { get; set; }

        public Ballot(Dictionary<string, int> voting)
        {
            Voting = voting;
        }
    }
}
