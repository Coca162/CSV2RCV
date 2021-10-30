using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using RCVConverter.Models;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

Console.WriteLine("Starting Conversion!");

List<Ballot> ballots = new();
Candidates[] candidates = (Candidates[])Enum.GetValues(typeof(Candidates));
var max = candidates.Max();

var config = new CsvConfiguration(CultureInfo.InvariantCulture)
{
    NewLine = Environment.NewLine,
    HasHeaderRecord = false,
};

foreach (var row in File.ReadAllLines("input.csv"))
{
    Dictionary<Candidates, int> votes = new();
    int i = 1;
    foreach (string field in row.Split(','))
    {
        string vote = Regex.Replace(field, "[^0-9]", "");
        int rank = int.Parse(vote);

        votes.Add((Candidates)i, rank);
        i++;
    }

    ballots.Add(new Ballot(votes));
}

string stringbuilder = "";
ballots.ForEach(x =>
{
    foreach(var vote in x.Voting)
    {
        stringbuilder += vote.Value;
        if (vote.Key != max)
        {
            stringbuilder += ",";
        }

        Console.WriteLine($"{vote.Key}: {vote.Value}");
    }
    stringbuilder += "\n";
});

using (FileStream fs = File.Create("output.txt"))
{
    byte[] info = new UTF8Encoding(true).GetBytes(string.Join(Environment.NewLine, stringbuilder));
    fs.Write(info, 0, info.Length);
}

string candidateString = "";
foreach(var candidate in candidates)
{
    candidateString += candidate.ToString();
    candidateString += "\n";
}

using (FileStream fs = File.Create("candidates.txt"))
{
    byte[] info = new UTF8Encoding(true).GetBytes(string.Join(Environment.NewLine, candidateString));
    fs.Write(info, 0, info.Length);
}

public enum Candidates
{
    Wizard = 1,
    Sh00b = 2,
    Falight = 3,
    Allegate = 4,
    Scarlett = 5
}