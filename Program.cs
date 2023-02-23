using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json;
using JsonException = System.Text.Json.JsonException;

namespace HogwartsLocalisationConverter
{
    internal class Program
    {
        //private const string MAGIC = "A\0V\0A\0F\0D\0I\0C\0T\0 \02\0.\00\0\0\0";
        private const string MAGIC = "AVAFDICT 2.0   \0";
        private const long HEADER_SIZE = 72;
        
        public static void Main(string[] args)
        {
            var bin2JsonCommand = new Command("bin2json", "Convert .bin to .json")
            {
                new Argument<FileInfo>("file", "File to convert")
            };
            
            var json2BinCommand = new Command("json2bin", "Convert .json to .bin")
            {
                new Argument<FileInfo>("file", "File to convert")
            };

            var csv2BinCommand = new Command("csv2bin", "Convert .csv to .bin")
            {
                new Argument<FileInfo>("file", "File to convert")
            };
            
            
            var rootCommand = new RootCommand("Converts to and from Hogwarts Legacy language .bin files.")
            {
                new Argument<FileInfo>("file", "File to convert (will detect extension and attempt to use the necessary converter).")
            };
            
            rootCommand.Handler = CommandHandler.Create<FileInfo>(GuessTheFile);

            bin2JsonCommand.Handler = CommandHandler.Create<FileInfo>(BinToJson);
            json2BinCommand.Handler = CommandHandler.Create<FileInfo>(JsonToBin);
            csv2BinCommand.Handler = CommandHandler.Create<FileInfo>(CsvToBin);

            rootCommand.AddCommand(bin2JsonCommand);
            rootCommand.AddCommand(json2BinCommand);
            rootCommand.AddCommand(csv2BinCommand);
            
            rootCommand.Invoke(args);
        }

        
        private static void GuessTheFile(FileInfo file)
        {
            switch (file.Extension)
            {
                case ".bin":
                    Console.WriteLine($"Detected a BIN file. Converting to JSON...");
                    BinToJson(file);
                    break;
                case ".json":
                    Console.WriteLine($"Detected a JSON file. Converting to BIN...");
                    JsonToBin(file);
                    break;
                case ".csv":
                    Console.WriteLine($"Detected a CSV file. Converting to BIN...");
                    CsvToBin(file);
                    break;
                default:
                    Console.WriteLine($"Unknown file extension");
                    break;
            }
        }
        
        public class Foo
        {
            public string key { get; set; }
            public string value { get; set; }
        }

        private static void CsvToBin(FileInfo file)
        {
            var entries = new Dictionary<string, string>();
            
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
            };

            using (var reader = new StreamReader(file.FullName))
            {
                using (var csv = new CsvReader(reader, config))
                {
                    try
                    {
                        var records = csv.GetRecords<Foo>();
                        entries = records.ToDictionary(r => r.key, r => r.value.Replace(@"\\n", @"\n"));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        Console.WriteLine("Press Enter key to close...");
                        Console.ReadLine();
                        return;
                    }
                }
            }
            
            WriteBinFromDictionary(file.FullName, entries);
        }

        private static void JsonToBin(FileInfo file)
        {
            var entries = new Dictionary<string, string>();
            
            // load json into dictionary

            try
            {
                entries = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(file.FullName));
            }
            catch (JsonReaderException jre) 
            {
                Console.WriteLine($"Couldn't parse json. error={jre.Message}");
                return;
            }

            //File.WriteAllText(Path.ChangeExtension(file.FullName, ".json"), jsonString);
            WriteBinFromDictionary(file.FullName, entries);
        }
        
        static string AddSuffix(string filename, string suffix)
        {
            string fDir = Path.GetDirectoryName(filename);
            string fName = Path.GetFileNameWithoutExtension(filename);
            string fExt = Path.GetExtension(filename);
            return Path.Combine(fDir, String.Concat(fName, suffix, fExt));
        }
        
        private static void WriteBinFromDictionary(string file, Dictionary<string, string> entries)
        {
            if (entries.Count == 0)
            {
                Console.WriteLine($"Empty dictionary");
                return;
            }

            string modifiedPath = AddSuffix(file,"-modified");

            using (var fs = File.Open(modifiedPath, FileMode.Create))
            {
                using (var bw = new BinaryWriter(fs, Encoding.UTF8, false))
                {
                    //string magic = MAGIC; // always "AVAFDICT 2.0   \0"
                    long entryCount = entries.Count;
                    long headerSize = HEADER_SIZE; // always 72
                    long entriesSize = entryCount * 24; // each entry header is 24 bytes
                    long dataStart = entriesSize + headerSize; // start of data section is after the header and the entries
                    long dataSize = 0; // size of all the strings put together
                    
                    bw.Write(Encoding.Unicode.GetBytes(MAGIC));
                    bw.Write(entryCount);
                    bw.Write(headerSize);
                    bw.Write(entriesSize);
                    bw.Write(dataStart);
                    bw.Write(dataSize);

                    long offset = 0;
                    
                    // write headers section
                    for(int i = 0; i< entries.Count; i++)
                    {
                        var entry = entries.ElementAt(i);

                        int keySize = Encoding.UTF8.GetBytes(entry.Key).Length;
                        int valueSize = Encoding.UTF8.GetBytes(entry.Value).Length;
                        
                        bw.Write(offset); // 8 - key offset
                        offset += keySize;
                        
                        bw.Write(keySize); // 4 - key size,
                        
                        bw.Write(offset); // 8 - value offset, every 24 bytes, we write this? running total?
                        offset += valueSize;
                        
                        bw.Write(valueSize); // 4 - value size
                    }

                    long position = bw.BaseStream.Position;

                    // write data section
                    for(int i = 0; i< entries.Count; i++)
                    {
                        var entry = entries.ElementAt(i);
                        
                        bw.Write(Encoding.UTF8.GetBytes(entry.Key));
                        bw.Write(Encoding.UTF8.GetBytes(entry.Value));
                    }

                    dataSize = bw.BaseStream.Position - position;

                    // go back to beginning and write datasize
                    bw.Seek(64, SeekOrigin.Begin);
                    bw.Write(dataSize);
                }
            }
            
            
        }

        

        private static void BinToJson(FileInfo file)
        {
            using (var br = new BinaryReader(file.OpenRead()))
            {
                string magic;
                long entryCount;
                long headerSize;
                long entriesSize;
                long dataStart;
                long dataSize;
                
                magic = Encoding.Unicode.GetString(br.ReadBytes(32));
                //magic = br.ReadBytes(32);

                if (magic != MAGIC)
                {
                    Console.WriteLine("this is not a ava dict bin file");
                    return;
                }
                
                entryCount = br.ReadInt64();
                headerSize = br.ReadInt64();
                entriesSize = br.ReadInt64();
                dataStart = br.ReadInt64();
                dataSize = br.ReadInt64();

                Dictionary<string, string> entries = new Dictionary<string, string>();
                
                for(int i = 0; i< entryCount; i++)
                {
                    long keyOffset = br.ReadInt64();
                    int keySize = br.ReadInt32();
                    string key = ReadStringAtOffset(br, keyOffset + dataStart, keySize);
                    //IDs.Add(RemoveNewLine(ID));

                    long valueOffset = br.ReadInt64();
                    int valueSize = br.ReadInt32();
                    string value = ReadStringAtOffset(br, valueOffset + dataStart, valueSize);
                    
                    entries.Add(key, value);
                }
                
                var jsonString = JsonConvert.SerializeObject(entries, Formatting.Indented);
                
                string modifiedPath = AddSuffix(file.FullName,"-modified");
                File.WriteAllText(modifiedPath, jsonString);
            }
        }
        
        static string ReadStringAtOffset(BinaryReader br, long offset, int length)
        {
            long position = br.BaseStream.Position;
            br.BaseStream.Seek(offset,SeekOrigin.Begin);
            string s = Encoding.UTF8.GetString(br.ReadBytes(length));
            br.BaseStream.Seek(position, SeekOrigin.Begin);
            return s;
        }
    }

}