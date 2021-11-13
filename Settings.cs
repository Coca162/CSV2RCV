using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using RCVConverter.Models;
using static RCVConverter.Files;

namespace RCVConverter
{
    public static class Settings
    {
        public static ElectionType electionType = ElectionType.Standard;
        public static bool correctRankNumbers = true;

        public static Dictionary<string, List<string>> DistrictNames = new()
        {
            { "avalon", new() { "avalon", "old yam", "avalon (previously: old yam)" } },
            { "ardenti_terra", new() { "netherlands", "ardenti terra", "ardenti-terra (previously: netherlands)" } },
            { "kōgi", new() { "kōgi", "kogi", "corgi", "kōgi (previously: corgi)" } },
            { "katonia", new() { "katonia", "elysian katonia", "katonian elysium", "server past", "servers past", "katonia (previously: server's past)" } },
            { "lanatia", new() { "lanatia", "medievala", "lanatia (previously: medivala)" } },
            { "landing_cove", new() { "landing cove" } },
            { "los_vooperis", new() { "los vooperis" } },
            { "new_avalon", new() { "new yam", "new avalon", "new avalon (previously: new yam)" } },
            { "new_spudland", new() { "new spudland", "spudland" } },
            { "new_vooperis", new() { "new vooperis" } },
            { "novastella", new() { "vooperia city", "novastella", "novestella (previously: vooperia city)" } },
            { "old_king", new() { "old king", "old king peninsula" } },
            { "san_vooperisco", new() { "san vooperisco" } },
            { "thesonica", new() { "thesonica", "queensland", "thesonica (previously: queensland)" } },
            { "voopmont", new() { "voopmont", "icelesia", "voopmont (previously: icelesia)" } },
            { "offworld", new() { "offworld", "off-world" } }
        };

        public static Dictionary<Districts, List<string>> SettingToDistricts(Dictionary<string, List<string>> settings)
        {
            Dictionary<Districts, List<string>> districts = new();

            foreach ((string name, List<string> list) in settings)
            {
                Districts district = name.ToLowerInvariant() switch
                {
                    "avalon" => Districts.Avalon,
                    "ardenti_terra" => Districts.Ardenti_Terra,
                    "kōgi" or "kōgi (previously: corgi)" => Districts.Kogi,
                    "lanatia" => Districts.Lanatia,
                    "katonia" => Districts.Katonia,
                    "landing_cove" => Districts.Landing_Cove,
                    "los_vooperis" => Districts.Los_Vooperis,
                    "new_avalon" => Districts.New_Avalon,
                    "new_spudland" => Districts.New_Spudland,
                    "new_vooperis" => Districts.New_Vooperis,
                    "novastella" => Districts.Novastella,
                    "old_king" => Districts.Old_King,
                    "san_vooperisco" => Districts.San_Vooperisco,
                    "thesonica" => Districts.Thesonica,
                    "voopmont" => Districts.Voopmont,
                    "offworld" => Districts.Offworld,
                    _ => throw new Exception("District is not a district!"),
                };

                districts.Add(district, list);
            }
            return districts;
        }

        public static async Task SetUpSettings()
        {
            while (!File.Exists("settings.txt"))
            {
                FileCreate("settings.txt",
@"Election Type (use National when district data is given): Standard
Correct Rank Numbers (use False if you are not Tyco): True");

                Console.WriteLine("Settings file has been generated. Press any key to carry on with the settings in settings.txt.");
                Console.ReadKey();
            }

            foreach (var setting in File.ReadAllLines("settings.txt"))
            {
                switch (setting.ToLowerInvariant().Split(":0")[2])
                {
                    case "national":
                        electionType = ElectionType.National;
                        break;
                    case "false":
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
        }
    }

    public enum ElectionType
    {
        National,
        Standard
    }

    public enum OutputType
    {
        Calculated,
        Raw
    }
}