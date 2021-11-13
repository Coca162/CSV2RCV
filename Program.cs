using RCVConverter.Models;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System;
using System.Text.Json;
using RCVConverter;
using static RCVConverter.Settings;
using static RCVConverter.TycoCorrections;
using System.Text.Unicode;
using System.Text.Encodings.Web;

Console.WriteLine("Starting Conversion!");

while (!File.Exists("settings.txt"))
{
    FileCreate("settings.txt",
@"Election Type (use national when district data is given): Standard
Correct Rank Numbers (use false if you are not Tyco): True");

    Console.WriteLine("Settings file has been generated. Press any key to carry on with the settings in settings.txt.");
    Console.ReadKey();
}

foreach (var setting in File.ReadAllLines("settings.txt"))
{
    switch (setting.ToLower())
    {
        case "election type (use national when district data is given): national":
            electionType = ElectionType.National;
            break;
        case "correct rank numbers (use false if you are not tyco): false":
            correctRankNumbers = false;
            break;
    }
}

while (!File.Exists("districtnames.json"))
{
    var file = JsonSerializer.Serialize(DistrictNames, options: new JsonSerializerOptions 
    { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
    FileCreate("districtnames.json", file);

    Console.WriteLine("District Name file has been generated. Press any key to carry on with the district names in districtnames.json.");
    Console.ReadKey();
}

var stream = File.OpenRead("districtnames.json");
DistrictNames = await JsonSerializer.DeserializeAsync<Dictionary<string, List<string>>>(stream);
Dictionary<Districts, List<string>> districts = SettingToDistricts(DistrictNames);

List<string> candidates = File.ReadAllLines("candidates.txt").ToList();
if (electionType == ElectionType.Standard)
{
    List<Ballot> ballots = ImportBallots(candidates);

    CreateRCVFile(ballots.Select(x => x.Voting), candidates.Count, "output.txt");
}
else if (electionType == ElectionType.National)
{
    List<NationalBallot> ballots = ImportNationalBallots(candidates, districts);

    CreateRCVFile(ballots.Select(x => x.Voting), candidates.Count, "output.txt");

    foreach (Districts district in Enum.GetValues(typeof(Districts)))
    {
        var votes = ballots.Where(x => x.District == district).Select(x => x.Voting);
        if (!votes.Any()) continue;

        CreateRCVFile(votes, candidates.Count, district.ToString() + ".txt");
    }
}

static List<NationalBallot> ImportNationalBallots(List<string> candidates, Dictionary<Districts, List<string>> districtNames)
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

        ballots.Add(new NationalBallot(votes, (Districts)district));
    }

    return correctRankNumbers ? CorrectNonConsecutiveBallotNumbers(ballots) : ballots;
}

static List<Ballot> ImportBallots(List<string> candidates)
{
    List<Ballot> ballots = new();

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

        ballots.Add(new Ballot(votes));
    }

    return correctRankNumbers ? CorrectNonConsecutiveBallotNumbers(ballots) : ballots;
}

static void CreateRCVFile(IEnumerable<Dictionary<string, int>> ballots, int candidateCount, string filePath)
{
    string stringbuilder = "";
    foreach(var x in ballots)
    {
        int i = 0;
        foreach (var vote in x)
        {
            stringbuilder += vote.Value;
            if (i < candidateCount - 1)
            {
                stringbuilder += ",";
            }

            Console.WriteLine($"{vote.Key}: {vote.Value}");
            i++;
        }
        stringbuilder += "\n";
    }

    FileCreate(filePath, stringbuilder[0..^1]);
}

static void FileCreate(string path, string content)
{
    using FileStream fs = File.Create(path);
    byte[] info = new UTF8Encoding(true).GetBytes(string.Join(Environment.NewLine, content));
    fs.Write(info, 0, info.Length);
}