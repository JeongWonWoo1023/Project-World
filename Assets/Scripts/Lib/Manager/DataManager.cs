using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public enum DataFormat
{
    Text, Binary, Json
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

    public void SaveData<T>(T inputData, string additionalPath, string fileName, DataFormat format = DataFormat.Json) where T : SaveData
    {
        // 데이터 세이브
        if(formatKey == null)
        {
            formatKey = new ExtensionKeyward();
        }
        switch(format)
        {
            case DataFormat.Json:
                SaveJson(inputData, GetPath(additionalPath, fileName, formatKey.Json),DataFormat.Text);
                break;
            default:
                throw new Exception($"{format} : 데이터 포맷 설정이 잘못되었습니다.");
        }
    }

    public T LoadData<T>(string additionalPath, string fileName, DataFormat format = DataFormat.Json) where T : SaveData
    {
        // 데이터 로드
        if (formatKey == null)
        {
            formatKey = new ExtensionKeyward();
        }
        T result = default;
        switch (format)
        {
            case DataFormat.Json:
                result = LoadJson<T>(GetPath(additionalPath, fileName, formatKey.Json), DataFormat.Text);
                break;
            default:
                throw new Exception($"{format} : 데이터 포맷 설정이 잘못되었습니다.");
        }
        return result;
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

    private void SaveJson<T>(T inputData, string filePath, DataFormat format = DataFormat.Binary) where T : SaveData
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

    private T LoadJson<T>(string filePath, DataFormat format = DataFormat.Binary) where T : SaveData
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

    private string GetPath(string additionalPath,string fileName, string format, RuntimePlatform platform = RuntimePlatform.WindowsPlayer)
    {
        // 파일 경로 불러오기
        switch(platform)
        {
            case RuntimePlatform.WindowsPlayer:
                return $"{Application.dataPath}/{additionalPath}/{fileName}{format}";
            case RuntimePlatform.Android:
                return $"{Application.persistentDataPath}/{additionalPath}/{fileName}{format}";
            default:
                return string.Empty;
        }
    }
}
