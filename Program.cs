using RCVConverter.Models;
using RCVConverter;
using static RCVConverter.Settings;
using static RCVConverter.Parser;
using static RCVConverter.Files;
using System.Linq;

//Get the data
await SetUpSettings();
List<string> candidates = File.ReadAllLines("candidates.txt").ToList();

if (outputType == OutputType.Raw)
{
    Console.WriteLine("Starting Conversion!");

    IEnumerable<Dictionary<string, int>> totalVotes = null;
    if (electionType == ElectionType.National)
    {
        Dictionary<Districts, List<string>> districts = SettingToDistricts(DistrictNames);
        List<NationalBallot> ballots = ImportNationalBallots(candidates, districts);

        totalVotes = ballots.Select(x => x.Voting);

        foreach (Districts district in Enum.GetValues(typeof(Districts)))
        {
            var votes = ballots.Where(x => x.District == district).Select(x => x.Voting);
            if (!votes.Any()) continue;

            CreateRCVFile(votes, candidates.Count, district.ToString() + ".txt");
        }
    }
    else if (electionType == ElectionType.Standard) totalVotes = ImportBallots(candidates);

    CreateRCVFile(totalVotes, candidates.Count, "output.txt");
}
else
{
    Console.WriteLine("Starting Calculations!");

    if (electionType == ElectionType.National)
    {
        Dictionary<Districts, List<string>> districts = SettingToDistricts(DistrictNames);
        List<NationalBallot> ballots = ImportNationalBallots(candidates, districts);

        if (singleFile != true)
        {
            foreach (Districts district in Enum.GetValues(typeof(Districts)))
            {
                var votes = ballots.Where(x => x.District == district).Select(x => x.Voting).ToList();
                if (votes.Count == 0) continue;

                string name = district.ToString();
                FileCreate(name + ".txt", $"{name.Replace('_', ' ')}'s Election:\n\n{CalculateRCV(votes)}");
            }

            FileCreate("output.txt", CalculateRCV(ballots.Select(x => x.Voting).ToList()));

        }
        else
        {
            string results = "National Results:\n\n" + CalculateRCV(ballots.Select(x => x.Voting).ToList()) + "\n\n\n";
            foreach (Districts district in Enum.GetValues(typeof(Districts)))
            {
                var votes = ballots.Where(x => x.District == district).Select(x => x.Voting).ToList();
                if (votes.Count == 0) continue;


                string name = district.ToString();
                results += $"{name.Replace('_', ' ')}'s Election:\n\n{CalculateRCV(votes)}\n\n\n\n";
            }

            FileCreate("output.txt", results[0..^4]);
        }

    }
    else if (electionType == ElectionType.Standard)
    {
        FileCreate("output.txt", CalculateRCV(ImportBallots(candidates)));
    }
}

static string CalculateRCV(List<Dictionary<string, int>> ballots)
{
    string result = "";

    List<List<(string candidate, int rank)>> allBallots = ballots.Select(x => x.Select(x => (x.Key, x.Value)).ToList()).ToList();
    List<string> currentCandidates = allBallots.SelectMany(x => x.Select(x => x.candidate)).Distinct().ToList();
    int count = currentCandidates.Count;

    string winner = null;

    for (int i = 1; i < count; i++)
    {
        result += $"Round #{i}\n\n";
        result += $"{currentCandidates.Count} candidates" + (acceptIncomplete ? $" and {ballots.Count} ballots.\n\n" : "\n\n");
        result += "Number of first votes per candidate:\n";

        allBallots.RemoveAll(x => x.Sum(x => x.rank) == 0);

        IEnumerable<string> firsts = allBallots.Select(item => item.MinBy(x => x.rank).candidate);
        IEnumerable<(string candidate, int rank)> firstCounts = firsts.Distinct().Select(x => (x, firsts.Where(y => y == x).Count()));

        foreach (var (candidate, rank) in firstCounts) result += $"{candidate}: {rank}\n";
        result += "\n\n";

        (string candidate, int rank) max = firstCounts.MaxBy(x => x.rank);
        (string candidate, int rank) min = firstCounts.MinBy(x => x.rank);

        double maxPercent = (double)max.rank / (double)ballots.Count * 100;
        double minPercent = (double)min.rank / (double)ballots.Count * 100;

        result += $"{max.candidate} has the highest number of votes with votes {max.rank} ({Math.Round(maxPercent, 2)}%)\n";
        result += $"{min.candidate} has the lowest number of votes with votes {min.rank} ({Math.Round(minPercent, 2)}%)\n";

        allBallots.ForEach(ballot => ballot.RemoveAll(vote => vote.candidate == min.candidate));
        currentCandidates.Remove(min.candidate);

        double percent = (double)min.rank / (double)firstCounts.Sum(x => x.rank);
        Console.WriteLine(percent);

        if (percent > 0.5) { winner = min.candidate; break; }

        //Console.WriteLine("Poop");
        //foreach (var idk in allBallots) idk.ForEach(vote => Console.WriteLine(vote.candidate + vote.rank));
    }

    if (currentCandidates.Count == 1) winner = currentCandidates.Single();
    if (winner is null) result += $"The folowing candidates were tied for the vote: {string.Join(',', currentCandidates)}";
    else result += $"{winner} won!";

    Console.WriteLine(result);
    return result;
    Console.WriteLine(currentCandidates.First());
}

//static string CalculateRCV(List<Dictionary<string, int>> fromBallots, List<string> fromCandidates)
//{
//    string result = "";

//    List<Dictionary<string, int>> ballots = new();
//    List<string> candidates = new();
//    ballots.AddRange(fromBallots);
//    candidates.AddRange(fromCandidates);
//    int candidatesCount = fromCandidates.Count;

//    for (int i = 1; i < candidatesCount;)
//    {
//        result += $"Round #{i}\n\n";

//        result += $"{candidates.Count} candidates and {ballots.Count} ballots.\n\n";
//        result += "Number of first votes per candidate:\n";

//        List<string> firstVotes = new();

//        foreach (Dictionary<string, int> vote in ballots)
//        {
//            firstVotes.Add(vote.OrderBy(x => x.Value).Select(x => x.Key).First());
//        }

//        Dictionary<string, int> totalFirstVotes = new();

//        foreach (string candidate in firstVotes.Distinct())
//        {
//            int count = firstVotes.Where(x => x == candidate).Count();
//            totalFirstVotes.Add(candidate, count);

//            result += $"{candidate}: {count}\n";
//        }
//        result += '\n';

//        var highest = totalFirstVotes.OrderByDescending(x => x.Value).First();
//        var ordered = totalFirstVotes.OrderBy(x => x.Value).ToList();

//        var lowests = ordered.Where(x => ordered.Select(x => x.Value).Min() == x.Value).ToList();

//        if (lowests.Contains(highest))
//        {
//            double highestPercent = (double)highest.Value / (double)ballots.Count * 100;
//            result += $"{highest.Key} has the highest number of votes with votes {highest.Value} ({Math.Round(highestPercent, 2)}%)\n";

//            var lowest = lowests.First();
//            double lowestPercent = (double)lowest.Value / (double)ballots.Count * 100;
//            if (lowests.Count() == 1)
//            {
//                result += $"{lowest.Key} has the lowest number of votes with votes {lowest.Value} ({Math.Round(lowestPercent, 2)}%)\n";
//            }
//            else
//            {
//                result += $"{lowests.Count()} candidates have the lowest number of votes with votes {lowest.Value} ({Math.Round(lowestPercent, 2)}%)\n";
//            }
//        }

//        foreach (Dictionary<string, int> vote in ballots)
//        {
//            if (vote.Values.Sum() == 0) ballots.Remove(vote);
//            else lowests.ForEach(x => {
//                vote.Remove(x.Key);
//                candidates.Remove(x.Key);
//            });
//            i =+ lowests.Count();
//        }
//    }
//    result += $"{candidates.Single()} Won!";

//    return result;
//}