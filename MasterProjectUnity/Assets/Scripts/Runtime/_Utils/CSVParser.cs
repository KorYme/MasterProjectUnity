using Codice.Client.Common.GameUI;
using Codice.ThemeImages;
using MasterProject.Debugging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace MasterProject.Utilities
{
    public static class CSVParser
    {
        private readonly static string TAG = nameof(CSVParser);

        public static List<List<string>> ParseCSV(string path)
        {
            try
            {
                List<List<string>> output = new List<List<string>>();
                using (StreamReader streamReader = new StreamReader(path))
                {
                    if (streamReader.EndOfStream)
                    {
                        DebugLogger.Warning(TAG, "The CSV you're trying to read is empty");
                        return output;
                    }
                    else
                    {
                        string[] line = streamReader.GetValuesFromLine();
                        for (int index = 0; index < line.Length; index++)
                        {
                            output.Add(new List<string>() { line[index] });
                        }
                    }
                    int lineCount = output.Count;
                    while (!streamReader.EndOfStream)
                    {
                        string[] line = streamReader.GetValuesFromLine();
                        for (int index = 0; index < lineCount; index++)
                        {
                            output[index].Add(line[index]);
                        }
                    }
                    return output;
                }
            }
            catch (Exception exc)
            {
                DebugLogger.Error(TAG, "An error occured while trying to parse a CSV.");
                throw exc;
            }
        }

        private static string[] GetValuesFromLine(this StreamReader streamReader, char separator = ';')
        {
            return streamReader.ReadLine().Split(separator);
        }
    }
}