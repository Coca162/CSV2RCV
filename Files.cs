using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCVConverter
{
    public static class Files
    {
        public static void CreateRCVFile(IEnumerable<Dictionary<string, int>> ballots, int candidateCount, string filePath)
        {
            string stringbuilder = "";
            foreach (var x in ballots)
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

        public static void FileCreate(string path, string content)
        {
            using FileStream fs = File.Create(path);
            byte[] info = new UTF8Encoding(true).GetBytes(string.Join(Environment.NewLine, content));
            fs.Write(info, 0, info.Length);
        }
    }
}
