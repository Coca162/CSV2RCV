using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RCVConverter.Models;

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
    }

    public enum ElectionType
    {
        National,
        Standard
    }
}