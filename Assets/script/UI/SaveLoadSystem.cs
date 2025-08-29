using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// 游戏存档数据结构
/// </summary>
[Serializable]
public class GameSaveData
{
    public string saveName;               // 存档名称
    public string playerName;             // 玩家名称
    public int playerLevel;               // 玩家等级
    public string currentLocation;        // 当前位置
    public float playTime;                // 游戏时长（秒）
    public string saveDateTime;           // 存档日期时间
    public Dictionary<string, object> playerStats;  // 玩家属性
    public Dictionary<string, object> gameProgress; // 游戏进度数据

    /// <summary>
    /// 创建新的存档数据实例
    /// </summary>
    public GameSaveData()
    {
        saveName = "Save Game";
        playerName = "Hero";
        playerLevel = 1;
        currentLocation = "Starting Area";
        playTime = 0f;
        saveDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        playerStats = new Dictionary<string, object>();
        gameProgress = new Dictionary<string, object>();
    }

    /// <summary>
    /// 更新存档时间
    /// </summary>
    public void UpdateSaveTime()
    {
        saveDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    /// <summary>
    /// 格式化存档信息用于显示
    /// </summary>
    /// <returns>格式化的存档信息字符串</returns>
    public string GetFormattedInfo()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(playTime);
        string formattedTime = $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
        
        return $"{saveName}\n" +
               $"Player: {playerName} (Lvl {playerLevel})\n" +
               $"Location: {currentLocation}\n" +
               $"Time Played: {formattedTime}\n" +
               $"Saved: {saveDateTime}";
    }
}

/// <summary>
/// 存档槽信息
/// </summary>
public class SaveSlotInfo
{
    public int slotIndex;                 // 存档槽索引
    public bool hasSave;                  // 是否有存档
    public string displayInfo;            // 显示信息
    public string saveDateTime;           // 存档日期时间
    public float playTime;                // 游戏时长

    public SaveSlotInfo(int index)
    {
        slotIndex = index;
        hasSave = false;
        displayInfo = "Empty Slot";
        saveDateTime = string.Empty;
        playTime = 0f;
    }
}

/// <summary>
/// 存档管理系统
/// </summary>
public class SaveLoadSystem : MonoBehaviour
{
    public static SaveLoadSystem Instance { get; private set; }

    private const string SAVE_FOLDER = "/Saves/";
    private const string SAVE_FILE_PREFIX = "save_";
    private const string SAVE_FILE_EXTENSION = ".json";
    private const int MAX_SAVE_SLOTS = 4;  // 最大存档槽数量

    private List<SaveSlotInfo> saveSlotInfos; // 存档槽信息列表

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 初始化存档系统
    /// </summary>
    private void Initialize()
    {
        // 确保存档文件夹存在
        string saveFolderPath = Application.persistentDataPath + SAVE_FOLDER;
        if (!Directory.Exists(saveFolderPath))
        {
            Directory.CreateDirectory(saveFolderPath);
        }

        // 初始化存档槽信息
        saveSlotInfos = new List<SaveSlotInfo>();
        for (int i = 0; i < MAX_SAVE_SLOTS; i++)
        {
            saveSlotInfos.Add(new SaveSlotInfo(i));
        }

        // 加载所有存档槽信息
        RefreshAllSaveSlotInfos();
    }

    /// <summary>
    /// 获取存档文件路径
    /// </summary>
    /// <param name="slotIndex">存档槽索引</param>
    /// <returns>存档文件路径</returns>
    private string GetSaveFilePath(int slotIndex)
    {
        return Application.persistentDataPath + SAVE_FOLDER + 
               SAVE_FILE_PREFIX + slotIndex + SAVE_FILE_EXTENSION;
    }

    /// <summary>
    /// 检查指定存档槽是否有存档
    /// </summary>
    /// <param name="slotIndex">存档槽索引</param>
    /// <returns>是否有存档</returns>
    public bool SaveExists(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= MAX_SAVE_SLOTS)
        {
            Debug.LogError("Invalid save slot index: " + slotIndex);
            return false;
        }

        return File.Exists(GetSaveFilePath(slotIndex));
    }

    /// <summary>
    /// 刷新所有存档槽信息
    /// </summary>
    public void RefreshAllSaveSlotInfos()
    {
        for (int i = 0; i < MAX_SAVE_SLOTS; i++)
        {
            RefreshSaveSlotInfo(i);
        }
    }

    /// <summary>
    /// 刷新指定存档槽信息
    /// </summary>
    /// <param name="slotIndex">存档槽索引</param>
    private void RefreshSaveSlotInfo(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= saveSlotInfos.Count)
        {
            return;
        }

        SaveSlotInfo slotInfo = saveSlotInfos[slotIndex];
        slotInfo.hasSave = SaveExists(slotIndex);

        if (slotInfo.hasSave)
        {
            try
            {
                GameSaveData saveData = LoadSaveData(slotIndex);
                if (saveData != null)
                {
                    slotInfo.displayInfo = saveData.GetFormattedInfo();
                    slotInfo.saveDateTime = saveData.saveDateTime;
                    slotInfo.playTime = saveData.playTime;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to load save slot info: " + e.Message);
                slotInfo.hasSave = false;
                slotInfo.displayInfo = "Corrupted Save";
            }
        }
        else
        {
            slotInfo.displayInfo = "Empty Slot";
            slotInfo.saveDateTime = string.Empty;
            slotInfo.playTime = 0f;
        }
    }

    /// <summary>
    /// 获取指定存档槽的显示信息
    /// </summary>
    /// <param name="slotIndex">存档槽索引</param>
    /// <returns>存档显示信息</returns>
    public string GetSaveSlotDisplayInfo(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= saveSlotInfos.Count)
        {
            return "Invalid Slot";
        }

        return saveSlotInfos[slotIndex].displayInfo;
    }

    /// <summary>
    /// 获取所有存档槽信息
    /// </summary>
    /// <returns>存档槽信息数组</returns>
    public SaveSlotInfo[] GetAllSaveSlotInfos()
    {
        return saveSlotInfos.ToArray();
    }

    /// <summary>
    /// 保存游戏数据到指定槽位
    /// </summary>
    /// <param name="slotIndex">存档槽索引</param>
    /// <param name="saveData">游戏存档数据</param>
    /// <returns>保存是否成功</returns>
    public bool SaveGame(int slotIndex, GameSaveData saveData)
    {
        if (slotIndex < 0 || slotIndex >= MAX_SAVE_SLOTS)
        {
            Debug.LogError("Invalid save slot index: " + slotIndex);
            return false;
        }

        try
        {
            // 更新存档时间
            saveData.UpdateSaveTime();

            // 序列化数据到JSON
            string jsonData = JsonUtility.ToJson(saveData, true);

            // 写入文件
            File.WriteAllText(GetSaveFilePath(slotIndex), jsonData);

            // 更新存档槽信息
            RefreshSaveSlotInfo(slotIndex);

            Debug.Log("Game saved successfully to slot " + slotIndex);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save game: " + e.Message);
            return false;
        }
    }

    /// <summary>
    /// 从指定槽位加载游戏数据
    /// </summary>
    /// <param name="slotIndex">存档槽索引</param>
    /// <returns>游戏存档数据</returns>
    public GameSaveData LoadSaveData(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= MAX_SAVE_SLOTS)
        {
            Debug.LogError("Invalid save slot index: " + slotIndex);
            return null;
        }

        try
        {
            string filePath = GetSaveFilePath(slotIndex);
            if (!File.Exists(filePath))
            {
                Debug.LogWarning("Save file not found: " + filePath);
                return null;
            }

            // 读取文件内容
            string jsonData = File.ReadAllText(filePath);

            // 反序列化为GameSaveData对象
            GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(jsonData);

            return saveData;
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to load game save: " + e.Message);
            return null;
        }
    }

    /// <summary>
    /// 删除指定槽位的存档
    /// </summary>
    /// <param name="slotIndex">存档槽索引</param>
    /// <returns>删除是否成功</returns>
    public bool DeleteSave(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= MAX_SAVE_SLOTS)
        {
            Debug.LogError("Invalid save slot index: " + slotIndex);
            return false;
        }

        try
        {
            string filePath = GetSaveFilePath(slotIndex);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                // 更新存档槽信息
                RefreshSaveSlotInfo(slotIndex);
                Debug.Log("Save deleted successfully from slot " + slotIndex);
                return true;
            }
            return false;
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to delete save: " + e.Message);
            return false;
        }
    }

    /// <summary>
    /// 创建新游戏的默认存档数据
    /// </summary>
    /// <returns>默认存档数据</returns>
    public GameSaveData CreateNewGameSaveData()
    {
        GameSaveData newSaveData = new GameSaveData();
        
        // 设置初始玩家属性
        newSaveData.playerStats.Add("health", 100);
        newSaveData.playerStats.Add("mana", 50);
        newSaveData.playerStats.Add("strength", 10);
        newSaveData.playerStats.Add("agility", 10);
        newSaveData.playerStats.Add("intelligence", 10);
        
        // 设置初始游戏进度
        newSaveData.gameProgress.Add("mainQuestCompleted", false);
        newSaveData.gameProgress.Add("tutorialCompleted", false);
        newSaveData.gameProgress.Add("areasDiscovered", new List<string> { "Starting Area" });
        
        return newSaveData;
    }
}