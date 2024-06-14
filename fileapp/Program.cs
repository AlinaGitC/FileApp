using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace fileapp
{    
    internal class Program
    {
        static List<DataBaseObject> ReadDataFromFile(string filePath)
        {
            var data = new List<DataBaseObject>();
            var lines = File.ReadAllLines(filePath);

            string header = null, connect = null;

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    if (header != null && connect != null)
                    {
                        data.Add(new DataBaseObject { Header = header, Connect = connect });
                    }
                    header = null;
                    connect = null;
                }
                else if (line.StartsWith("["))
                {
                    header = line.Trim('[', ']');
                }
                else
                {
                    var parts = line.Split('=');
                    if (parts.Length == 2)
                    {
                        var key = parts[0].Trim();
                        var value = parts[1].Trim();
                        if (key == "Connect")
                        {
                            connect = value;
                        }
                    }
                }
            }

            if (header != null && connect != null)
            {
                data.Add(new DataBaseObject { Header = header, Connect = connect });
            }

            return data;
        }


        static (List<DataBaseObject>, List<DataBaseObject>) CheckData(List<DataBaseObject> data)
        {
            var validData = new List<DataBaseObject>();
            var invalidData = new List<DataBaseObject>();

            foreach (var obj in data)
            {
                if (obj.Connect.StartsWith("File="))
                {
                    var path = obj.Connect.Substring(5);
                    if (!path.Any(c => Path.GetInvalidPathChars().Contains(c)))
                    {
                        validData.Add(obj);
                    }
                    else
                    {
                        invalidData.Add(obj);
                    }
                }
                else if (obj.Connect.StartsWith("Srvr=") && obj.Connect.Contains("Ref="))
                {
                    validData.Add(obj);
                }
                else
                {
                    invalidData.Add(obj);
                }
            }

            return (validData, invalidData);
        }

        static void SplitAndSaveData(List<DataBaseObject> validData)
        {
            int numParts = 5;
            int chunkSize = validData.Count / numParts;

            for (int i = 0; i < numParts; i++)
            {
                var chunk = validData.Skip(i * chunkSize).Take(chunkSize);
                File.WriteAllLines($"base_{i + 1}.txt", chunk.Select(obj => $"[{obj.Header}]\nConnect={obj.Connect}\n"));
            }
        }


        static void Main(string[] args)
        {
            /*Console.Write("Введите путь к файлу: ");
            string filePath = Console.ReadLine();*/

            //Добавлен запуск программы и ввод пути к файлу из командной строки
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: DatabaseAnalysis <inputFilePath>");
                return;
            }

            string filePath = args[0];
            List<DataBaseObject> connections = new List<DataBaseObject>();
            try
            {
                if (File.Exists(filePath))
                {
                    var data = ReadDataFromFile(filePath);
                    var (validData, invalidData) = CheckData(data);

                    if (invalidData.Any())
                    {
                        File.WriteAllLines("bad_data.txt", invalidData.Select(obj => $"[{obj.Header}]\nConnect={obj.Connect}\n"));
                    }

                    SplitAndSaveData(validData);

                    Console.WriteLine("Данные успешно обработаны.");
                }
                else
                {
                    Console.WriteLine("Файл не найден.");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
            Console.ReadLine();
        }
    }
}
