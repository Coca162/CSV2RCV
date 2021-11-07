using RCVConverter.Models;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System;
using RCVConverter;
using static RCVConverter.TycoCorrections;

Console.WriteLine("Starting Conversion!");

while (true)
{
    if (File.Exists("settings.txt"))
    {
        foreach (var setting in File.ReadAllLines("settings.txt"))
        {
            switch (setting.ToLower())
            {
                case "election type (use national when district data is given): national":
                    Settings.electionType = ElectionType.National;
                    break;
                case "correct rank numbers (use false if you are not tyco): false":
                    Settings.correctRankNumbers = false;
                    break;
            }
        }
        break;
    }
    else
    {
        FileCreate("settings.txt",
    @"Election Type (use national when district data is given): Standard
Correct Rank Numbers (use false if you are not Tyco): True");

        Console.WriteLine("Settings file has been generated. Press any key to carry on with the settings in settings.txt.");
        Console.ReadKey();
    }
}


List<string> candidates = File.ReadAllLines("candidates.txt").ToList();
if (Settings.electionType == ElectionType.Standard)
{
    List<Ballot> ballots = ImportBallots(candidates);

    CreateRCVFile(ballots.Select(x => x.Voting), candidates.Count, "output.txt");
}
else if (Settings.electionType == ElectionType.National)
{
    List<NationalBallot> ballots = ImportNationalBallots(candidates);

    CreateRCVFile(ballots.Select(x => x.Voting), candidates.Count, "output.txt");

    foreach (Districts district in Enum.GetValues(typeof(Districts)))
    {
        var votes = ballots.Where(x => x.District == district).Select(x => x.Voting);
        if (!votes.Any()) continue;

        CreateRCVFile(votes, candidates.Count, district.ToString() + ".txt");
    }
}

static List<NationalBallot> ImportNationalBallots(List<string> candidates)
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
                district = StringToDistrict(field);
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

    return Settings.correctRankNumbers ? CorrectNonConsecutiveBallotNumbers(ballots) : ballots;
}

static Districts StringToDistrict(string input)
{
    return input.ToLowerInvariant() switch
    {
        "avalon" or "old yam" => Districts.Avalon,
        "voopmont" or "icelesia" => Districts.Voopmont,
        "kogi" or "corgi" => Districts.Kogi,
        "lanatia" or "medievala" => Districts.Lanatia,
        "katonia" or "elysian katonia" or "katonian elysium" or "server past" or "servers past" => Districts.Katonia,
        "landing cove" => Districts.Landing_Cove,
        "los vooperis" => Districts.Los_Vooperis,
        "netherlands" or "ardenti terra" => Districts.Netherlands,
        "new yam" or "new avalon" => Districts.New_Avalon,
        "new spudland" or "spudland" => Districts.New_Spudland,
        "new vooperis" => Districts.New_Vooperis,
        "vooperia city" or "novastella" => Districts.Novastella,
        "old king" or "old king peninsula" => Districts.Old_King,
        "san vooperisco" => Districts.San_Vooperisco,
        "thesonica" or "queensland" => Districts.Thesonica,
        "offworld" => Districts.Offworld,
        _ => throw new Exception("District is not a district!"),
    };
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

    return Settings.correctRankNumbers ? CorrectNonConsecutiveBallotNumbers(ballots) : ballots;
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

