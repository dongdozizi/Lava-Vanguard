
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Async;
using System.IO;
using System.Xml.Linq;


public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }
    /// <summary>
    /// Default Data
    /// </summary>
    public static Dictionary<string, CardSpriteData> CardData;
    public static Dictionary<string, EnemyData> EnemyData;
    public static SequenceData SequenceData;
    public static InventoryData InventoryData;
    public static LevelData LevelData;

    /// <summary>
    /// Saved Data
    /// </summary>
    public static LevelData SavedLevelData;
    public static SequenceData SavedSequenceData;
    public static InventoryData SavedInventoryData;

    public static Dictionary<string, Sprite> BackgroundSprite;
    public static Dictionary<string, Sprite> OutlineSprite;
    public static Dictionary<string, Sprite> ContentSprite;
    public static CardConfig CardConfig;
    public static InventoryConfig InventoryConfig;

    private void Awake()
    {
        Instance = this;
        LoadData();
        LoadSprites();
        LoadConfigs();
    }
    private void Start()
    {

    }
    public static T LoadJson<T>(string fileNameWithoutExtension)
    {
        string fileName = fileNameWithoutExtension.EndsWith(".json") ? fileNameWithoutExtension : fileNameWithoutExtension + ".json";
        string persistentPath = Path.Combine(Application.persistentDataPath, fileName);
        // 1. Try loading from persistent path.
        if (File.Exists(persistentPath))
        {
            try
            {
                string json = File.ReadAllText(persistentPath);
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (JsonException ex)
            {
                Debug.LogError("Error parsing JSON from persistent path: " + ex.Message);
                return default;
            }
        }
        return LoadResourcesJson<T>(fileNameWithoutExtension);
    }
    public static Dictionary<string, T> LoadDictionaryJson<T>(string fileNameWithoutExtension)
    {
        string fileName = fileNameWithoutExtension.EndsWith(".json") ? fileNameWithoutExtension : fileNameWithoutExtension + ".json";
        string persistentPath = Path.Combine(Application.persistentDataPath, fileName);

        // 1. Try loading from persistent path.
        if (File.Exists(persistentPath))
        {
            try
            {
                string json = File.ReadAllText(persistentPath);
                return JsonConvert.DeserializeObject<Dictionary<string, T>>(json);
            }
            catch (JsonException ex)
            {
                Debug.LogError("Error parsing JSON from persistent path: " + ex.Message);
                return default;
            }
        }
        return LoadResourcesDictionaryJson<T>(fileNameWithoutExtension);
    }
    public static T LoadResourcesJson<T>(string fileNameWithoutExtension)
    {
        // 2. If not, go back to resources folder.
        TextAsset jsonText = Resources.Load<TextAsset>(fileNameWithoutExtension);
        if (jsonText == null)
        {
            Debug.LogError("No JSON file found at path: " + fileNameWithoutExtension);
            return default;
        }

        try
        {
            T data = JsonConvert.DeserializeObject<T>(jsonText.text);

            //File.WriteAllText(persistentPath, jsonText.text);
            //Debug.Log("Copied default JSON from Resources to: " + persistentPath);

            return data;
        }
        catch (JsonException ex)
        {
            Debug.LogError("Error parsing JSON from Resources: " + ex.Message);
            return default;
        }
    }
    public static Dictionary<string, T> LoadResourcesDictionaryJson<T>(string fileNameWithoutExtension)
    {
        // 2. If not, go back to resources folder.
        TextAsset jsonText = Resources.Load<TextAsset>(fileNameWithoutExtension);
        if (jsonText == null)
        {
            Debug.LogError("No JSON file found at path: " + fileNameWithoutExtension);
            return default;
        }

        try
        {
            var data = JsonConvert.DeserializeObject<Dictionary<string, T>>(jsonText.text);
            return data;
        }
        catch (JsonException ex)
        {
            Debug.LogError("Error parsing JSON from Resources: " + ex.Message);
            return default;
        }
    }
    public static void SaveJson<T>(string fullPath, ref T data)
    {
        try
        {
            string directory = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(fullPath, json);
        }
        catch (IOException ex)
        {
            Debug.LogError("Error saving JSON: " + ex.Message);
        }
    }
    public static void SaveData()
    {
        Debug.Log("SaveData");
        SavedSequenceData = new SequenceData();
        SavedSequenceData.CardDatas = new List<CardRankData>();
        foreach (var slot in SlotManager.Instance.slotViews)
        {
            if (slot == null) break;
            if (slot.content == null)
                SavedSequenceData.CardDatas.Add(new CardRankData() { ID = "0", CardID = "Card_Empty", Level = 0, LinkedSequenceID = "0" });
            else
                SavedSequenceData.CardDatas.Add(slot.content.cardRankData);
        }

        SavedInventoryData = new InventoryData();
        SavedInventoryData.CardDatas = new List<CardRankData>();
        foreach (var card in InventoryManager.Instance.inventoryView.cardViews)
        {
            SavedInventoryData.CardDatas.Add(card.cardRankData);
        }

        SavedLevelData = new LevelData() { Coin = PlayerManager.Instance.playerView.GetCoin(), Health = PlayerManager.Instance.playerView.GetHP(), Wave = LevelManager.Instance.wave };
        string sequenceDataPath = Path.Combine(Application.persistentDataPath, "Json/SequenceData.json");
        string inventoryDataPath = Path.Combine(Application.persistentDataPath, "Json/InventoryData.json");
        string levelDataPath = Path.Combine(Application.persistentDataPath, "Json/LevelData.json");
        SaveJson(sequenceDataPath, ref SavedSequenceData);
        SaveJson(inventoryDataPath, ref SavedInventoryData);
        SaveJson(levelDataPath, ref SavedLevelData);
    }
    private static void LoadData()
    {
        CardData = LoadResourcesDictionaryJson<CardSpriteData>("Json/CardData");
        EnemyData = LoadResourcesDictionaryJson<EnemyData>("Json/EnemyData");
        SequenceData = LoadResourcesJson<SequenceData>("Json/SequenceData");
        InventoryData = LoadResourcesJson<InventoryData>("Json/InventoryData");
        LevelData = LoadResourcesJson<LevelData>("Json/LevelData");

        SavedInventoryData = LoadJson<InventoryData>("Json/InventoryData");
        SavedSequenceData = LoadJson<SequenceData>("Json/SequenceData");
        SavedLevelData = LoadJson<LevelData>("Json/LevelData");
    }
    private static void LoadSprites()
    {
        BackgroundSprite = new Dictionary<string, Sprite>();
        OutlineSprite = new Dictionary<string, Sprite>();
        ContentSprite = new Dictionary<string, Sprite>();
        foreach (var c in CardData)
        {
            if (!BackgroundSprite.ContainsKey(c.Value.Background))
                BackgroundSprite.Add(c.Value.Background, Resources.Load<Sprite>("Sprite/Background/" + c.Value.Background));
            if (!OutlineSprite.ContainsKey(c.Value.Outline))
                OutlineSprite.Add(c.Value.Outline, Resources.Load<Sprite>("Sprite/Outline/" + c.Value.Outline));
            if (!ContentSprite.ContainsKey(c.Value.Content))
                ContentSprite.Add(c.Value.Content, Resources.Load<Sprite>("Sprite/Content/" + c.Value.Content));
        }
    }
    private static void LoadConfigs()
    {
        CardConfig = Resources.Load<CardConfig>("Config/CardConfig");
        InventoryConfig = Resources.Load<InventoryConfig>("Config/InventoryConfig");
    }

}
