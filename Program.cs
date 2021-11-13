using RCVConverter.Models;
using RCVConverter;
using static RCVConverter.Settings;
using static RCVConverter.Parser;
using static RCVConverter.Files;

Console.WriteLine("Starting Conversion!");

//Get the data
await SetUpSettings();
Dictionary<Districts, List<string>> districts = SettingToDistricts(DistrictNames);
List<string> candidates = File.ReadAllLines("candidates.txt").ToList();

IEnumerable<Dictionary<string, int>> totalVotes = null;

if (electionType == ElectionType.National)
{
    List<NationalBallot> ballots = ImportNationalBallots(candidates, districts);

    totalVotes = ballots.Select(x => x.Voting);

    foreach (Districts district in Enum.GetValues(typeof(Districts)))
    {
        var votes = ballots.Where(x => x.District == district).Select(x => x.Voting);
        if (!votes.Any()) continue;

        CreateRCVFile(votes, candidates.Count, district.ToString() + ".txt");
    }
}
else
{
    List<Ballot> ballots = ImportBallots(candidates);
    totalVotes = ballots.Select(x => x.Voting);
}

CreateRCVFile(totalVotes, candidates.Count, "output.txt");