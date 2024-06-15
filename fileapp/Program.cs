using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;

namespace fileapp
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: DatabaseAnalysis <inputFilePath>");
                return;
            }

            string inputFilePath = args[0];
            List<DataBaseObject> connections = new List<DataBaseObject>();

            try
            {
                string[] lines = File.ReadAllLines(inputFilePath);
                List<string> currentConnection = new List<string>();

                foreach (string line in lines)
                {
                    if (line.StartsWith("[") && line.EndsWith("]"))
                    {
                        if (currentConnection.Count > 0)
                        {
                            ProcessConnection(currentConnection, connections);
                            currentConnection.Clear();
                        }
                        currentConnection.Add(line);
                    }
                    else
                    {
                        currentConnection.Add(line);
                    }
                }

                if (currentConnection.Count > 0)
                {
                    ProcessConnection(currentConnection, connections);
                }

                var fileValidations = new FileDatabaseValidation();
                var serverValidations = new ServerDatabaseValidation();

                List<DataBaseObject> validConnections = connections.Where(c =>
                {
                    if (c.Connect.StartsWith("File="))
                    {
                        return fileValidations.Validate(c.Connect);
                    }
                    else if (c.Connect.StartsWith("Srvr="))
                    {
                        return serverValidations.Validate(c.Connect);
                    }
                    return false;
                }).ToList();

                List<DataBaseObject> invalidConnections = connections.Except(validConnections).ToList();

                File.WriteAllLines("bad_data.txt", invalidConnections.Select(c => $"[{c.Header}]\\nConnect={c.Connect}\\n"));

                int connectionsPerFile = validConnections.Count / 5;
                for (int i = 0; i < 5; i++)
                {
                    var connectionsToSave = validConnections.Skip(i * connectionsPerFile).Take(connectionsPerFile);
                    File.WriteAllLines($"base_{i + 1}.txt", connectionsToSave.Select(c => $"[{c.Header}]\\nConnect={c.Connect}\\n"));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Возникла ошибка: " + ex.Message);
            }
        }

        static void ProcessConnection(List<string> connectionInfo, List<DataBaseObject> connections)
        {
            Dictionary < string, string> connectionData = new Dictionary<string, string>();
            string title = connectionInfo.First().Trim('[', ']');

            foreach (string line in connectionInfo.Skip(1))
            {
                string[] parts = line.Split('=');
                if (parts.Length == 2)
                {
                    connectionData[parts[0]] = parts[1];
                }
            }

            if (connectionData.ContainsKey("Connection"))
            {
                connections.Add(new DataBaseObject { Header = title, Connect = connectionData["Connection"] });
                Console.WriteLine("Данные успешно обработаны");
            }
            Console.ReadLine();
        }
    }
}