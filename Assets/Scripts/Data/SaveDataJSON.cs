using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

//L'objet player
[Serializable]
public class SaveData
{
    public List<PlayerData> players = new List<PlayerData>();
}

public class SaveDataJSON : MonoBehaviour
{
    private SaveData saveData = new SaveData();
    private string savePath;

    void Awake()
    {
        savePath = Application.persistentDataPath + "/SaveData.json";
        LoadAllData();
    }

    public void SaveData(string _pseudo, int _score)
    {
        PlayerData existingPlayer = saveData.players.Find(p => p.pseudo == _pseudo); //Verification de l'existence du pseudo

        if (existingPlayer != null)
        {
            if (_score > existingPlayer.highscore)
            {
                existingPlayer.highscore = _score;
                existingPlayer.date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        else
        {
            saveData.players.Add(new PlayerData { pseudo = _pseudo, highscore = _score, date = DateTime.Now.ToString("yyyy-MM-dd") });
        }

        SaveToFile();
    }

    void SaveToFile()
    {
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(savePath, json);
    }

    public int LoadData(string _pseudo)
    {
        if (saveData.players.Exists(p => p.pseudo == _pseudo))
        {
            return saveData.players.Find(p => p.pseudo == _pseudo).highscore;
        }

        Debug.Log($"Le pseudo {_pseudo} n'existe pas");
        return 0; // Retourne 0 si le pseudo n'existe pas
    }

    public bool PseudoExistance(string _pseudo)
    {
        return saveData.players.Exists(p => p.pseudo == _pseudo);
    }

    //On renvoie la liste des joueurs en ordre croissant/décroissant
    public List<PlayerData> GetSortedLeaderboard(bool _isCroissant)
    {
        if(!_isCroissant) //Décroissant
        {
            return saveData.players.OrderByDescending(p => p.highscore).ToList();
        }
        else
        {
            return saveData.players.OrderBy(p => p.highscore).ToList();
        }
    }

    private void LoadAllData()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            saveData = JsonUtility.FromJson<SaveData>(json);
        }
    }
}
