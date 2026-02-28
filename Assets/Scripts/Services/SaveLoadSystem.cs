using System;
using System.IO;
using UnityEngine;
using OrbitLink.Data;

namespace OrbitLink.Services
{
    /// <summary>
    /// Handles disk serialization using atomic operations to prevent save corruption.
    /// </summary>
    public class SaveLoadSystem
    {
        private readonly string SaveFilePath;
        private readonly string TempSaveFilePath;

        public SaveLoadSystem()
        {
            // Unity's persistentDataPath is safe across Android and Editor
            SaveFilePath = Path.Combine(Application.persistentDataPath, "save.json");
            TempSaveFilePath = Path.Combine(Application.persistentDataPath, "save_temp.json");
        }

        public void Save(PersistentState state)
        {
            if (state == null) return;

            try
            {
                string json = JsonUtility.ToJson(state, true);

                // 1. Write to temp file first (Atomic operation setup)
                File.WriteAllText(TempSaveFilePath, json);

                // 2. If successful, replace the master file
                if (File.Exists(SaveFilePath))
                {
                    File.Delete(SaveFilePath);
                }
                
                File.Move(TempSaveFilePath, SaveFilePath);
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveLoadSystem] Failed to save state safely: {e.Message}");
            }
        }

        public PersistentState Load()
        {
            // Check master file
            if (File.Exists(SaveFilePath))
            {
                try
                {
                    string json = File.ReadAllText(SaveFilePath);
                    var state = JsonUtility.FromJson<PersistentState>(json);
                    if (state != null)
                        return state;
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[SaveLoadSystem] Master save corrupted: {e.Message}. Attempting recovery...");
                }
            }

            // Check temp file in case of crash during previous file swap
            if (File.Exists(TempSaveFilePath))
            {
                try
                {
                    string json = File.ReadAllText(TempSaveFilePath);
                    var state = JsonUtility.FromJson<PersistentState>(json);
                    
                    // Recover the file immediately
                    File.Move(TempSaveFilePath, SaveFilePath);
                    
                    if (state != null)
                        return state;
                }
                catch (Exception e)
                {
                    Debug.LogError($"[SaveLoadSystem] Temp save also corrupted: {e.Message}. Creating blank state.");
                }
            }

            // Returns fresh state if no save exists or all failed
            return new PersistentState();
        }
    }
}
