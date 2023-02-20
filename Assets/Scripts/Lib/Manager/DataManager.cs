using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public enum DataFormat
{
    Text, Binary
}

public class ExtensionKeyward
{
    public string Text { get; } = ".txt";
    public string Binary { get; } = ".dat";
    public string Json { get; } = ".json";
}

public class DataManager : Singleton<DataManager>
{
    private ExtensionKeyward formatKey;
    private string splitEliment = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    private string splitLine = @"\r\n|\n\r|\n|\r";
    private char[] trimChar = { '\"' };

    public void SaveJson<T>(T inputData, string fileName) where T : SaveData
    {
        // Json 데이터 세이브
        if(formatKey == null)
        {
            formatKey = new ExtensionKeyward();
        }
        Save(inputData, GetPath(fileName, formatKey.Json), DataFormat.Text);
    }

    public T LoadJson<T>( string fileName) where T : SaveData
    {
        // Json 데이터 로드
        if (formatKey == null)
        {
            formatKey = new ExtensionKeyward();
        }
        return Load<T>(GetPath(fileName, formatKey.Json), DataFormat.Text);
    }

    public List<Dictionary<string, object>> ReadCSV(string fileName)
    {
        // CSV 데이터 읽기
        var dataList = new List<Dictionary<string, object>>();
        string[] lines;

        // Step.1 : 파일 불러오기
        string path = GetPath(fileName, ".csv");
        if (File.Exists(path))
        {
            string source;
            StreamReader reader = new StreamReader(path);
            source = reader.ReadToEnd();
            reader.Close();

            lines = Regex.Split(source, splitLine);
            Debug.Log($"파일을 불러옵니다.\nPath : {path}");
        }
        else
        {
            Debug.LogError($"파일이 존재하지 않습니다.\nPath : {path}");
            return null;
        }

        // Step.2 파일 정제
        if(lines.Length <= 1)
        {
            return dataList;
        }

        string[] header = Regex.Split(lines[0], splitEliment);
        for(int i = 1; i < lines.Length; ++i)
        {
            string[] values = Regex.Split(lines[i], splitEliment);
            if(values.Length == 0 || values[0] == "") 
            {
                continue;
            }
            var entry = new Dictionary<string, object>();
            for(int j = 0; j < header.Length && j < values.Length; ++j)
            {
                string value = values[j];
                value = value.TrimStart(trimChar).TrimEnd(trimChar).Replace("\\", "");
                value = value.Replace("<br>", "\n");
                value = value.Replace("<c>", ",");
                object final = value;
                if(int.TryParse(value, out int intValue))
                {
                    final = intValue;
                }
                else if(float.TryParse(value, out float floatValue))
                {
                    final = floatValue;
                }
                entry[header[j]] = final;
            }
            dataList.Add(entry);
        }

        // Step.3 정제된 데이터 반환
        return dataList;
    }

    public void WriteCSV(List<string[]> rowData, string fileName)
    {
        // CSV 데이터 쓰기
        string[][] output = new string[rowData.Count][];

        // Step.1 데이터 정제
        for(int i = 0; i < output.Length; ++i)
        {
            output[i] = rowData[i];
        }

        string delimiter = ",";
        StringBuilder stringBuilder = new StringBuilder();

        for(int i = 0; i < output.GetLength(0); ++i)
        {
            stringBuilder.AppendLine(string.Join(delimiter, output[i]));
        }

        // Step.2 데이터 쓰기
        Stream fs = new FileStream(GetPath(fileName, ".csv"), FileMode.CreateNew, FileAccess.Write);
        StreamWriter outStream = new StreamWriter(fs, Encoding.UTF8);
        outStream.WriteLine(stringBuilder);
        outStream.Close();
    }

    private void SaveText(string inputData, string filePath)
    {
        // 텍스트 파일 세이브
        File.WriteAllText(filePath, inputData);
    }

    private void SaveBinary(string inputData, string filePath)
    {
        // 바이너리 파일 세이브
        using (FileStream fs = File.Create(filePath))
        {
            BinaryFormatter format = new BinaryFormatter();
            format.Serialize(fs, inputData);
            fs.Close();
        }
    }

    private void Save<T>(T inputData, string filePath, DataFormat format = DataFormat.Binary) where T : SaveData
    {
        // JSON 파일 세이브
        string data = JsonUtility.ToJson(inputData, true);

        switch (format)
        {
            case DataFormat.Text:
                SaveText(data, filePath);
                break; 
            case DataFormat.Binary:
                SaveBinary(data, filePath);
                break;
            default:
                throw new Exception($"{format} : 데이터 포맷 설정이 잘못되었습니다.");
        }
    }

    private string LoadText(string filePath)
    {
        // 텍스트 파일 로드
        if (!File.Exists(filePath))
        {
            return string.Empty;
        }
        return File.ReadAllText(filePath);
    }

    private string LoadBinary(string filePath)
    {
        // 바이너리 파일 로드
        if (!File.Exists(filePath))
        {
            return string.Empty;
        }
        string result;
        using (FileStream fs = File.Open(filePath, FileMode.Open))
        {
            BinaryFormatter format = new BinaryFormatter();
            result = (string)format.Deserialize(fs);
            fs.Close();
        }
        return result;

    }

    private T Load<T>(string filePath, DataFormat format = DataFormat.Binary) where T : SaveData
    {
        // JSON 파일 로드

        string result = string.Empty;
        switch (format)
        {
            case DataFormat.Text:
                result = LoadText(filePath);
                break;
            case DataFormat.Binary:
                result = LoadBinary(filePath);
                break;
            default:
                throw new Exception($"{format} : 데이터 포맷 설정이 잘못되었습니다.");
        }
        if(result == string.Empty)
        {
            return null;
        }
        return JsonUtility.FromJson<T>(result);
    }

    private string GetPath(string fileName, string format, RuntimePlatform platform = RuntimePlatform.WindowsPlayer)
    {
        // 파일 경로 불러오기
        switch(platform)
        {
            case RuntimePlatform.WindowsPlayer:
                return $"{Application.dataPath}/{fileName}{format}";
            case RuntimePlatform.Android:
                return $"{Application.persistentDataPath}/{fileName}{format}";
            default:
                return string.Empty;
        }
    }
}
