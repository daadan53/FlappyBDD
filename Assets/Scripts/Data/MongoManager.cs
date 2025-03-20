using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;

[Serializable]
public class SaveDataList
{
    public List<PlayerData> players;
}

public class MongoManager : MonoBehaviour
{
    private string baseUrl = "http://localhost/FlappyAPI/";

    public IEnumerator AddPlayerCoroutine(string _playerName, Action<bool> _callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("name", _playerName);

        using (UnityWebRequest www = UnityWebRequest.Post(baseUrl + "add_player.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string responseTxt = www.downloadHandler.text;
                var json = JSON.Parse(responseTxt); // On parse la réponse JSON

                if (json.HasKey("success")) //Si le joueur existe on récupère la réponse de php
                {
                    bool isSuccess = json["success"].AsBool;
                    _callback(isSuccess);
                }
                else //Le pseudo n'existe pas
                {
                    _callback(false);
                }
            }
            else
            {
                Debug.LogError("Erreur : " + www.error);
                _callback(false);
            }
        }
    }

    public IEnumerator UpdateScoreCoroutine(string _playerName, int _score)
    {
        WWWForm form = new WWWForm();
        form.AddField("name", _playerName);
        form.AddField("highscore", _score);

        using (UnityWebRequest www = UnityWebRequest.Post(baseUrl + "update_score.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Réponse : " + www.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Erreur : " + www.error);
            }
        }
    }

    public IEnumerator GetScoresCoroutine(bool _isCroissant, Action<List<PlayerData>> _callback)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(baseUrl + "get_scores.php"))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Réponse : " + www.downloadHandler.text);

                List<PlayerData> scores = JsonUtility.FromJson<SaveDataList>("{\"players\":" + www.downloadHandler.text + "}").players;

                if (_isCroissant)
                {
                    scores = scores.OrderBy(p => p.highscore).ToList(); // On trie par odre croissant 
                }
                else
                {
                    scores = scores.OrderByDescending(p => p.highscore).ToList(); // On trie par ordre décroissant
                }
        
                _callback(scores);
            }
            else
            {
                Debug.LogError("Erreur : " + www.error);
            }
        }
    }

    public IEnumerator CheckPseudoExistCoroutine(string _playerName, Action<bool> _callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("name", _playerName);

        using (UnityWebRequest www = UnityWebRequest.Post(baseUrl + "get_scores.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Réponse : " + www.downloadHandler.text);

                string json = www.downloadHandler.text;

                if (!string.IsNullOrEmpty(json) && json != "[]") 
                {
                    _callback(true);
                }
                else 
                {
                    _callback(false);
                }
            }
            else
            {
                Debug.LogError("Erreur : " + www.error);
            }
        }
    }

    public IEnumerator GetScoreOfThisPlayerCoroutine(string _playerName, Action<int> _callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("name", _playerName);

        using (UnityWebRequest www = UnityWebRequest.Post(baseUrl + "get_scores.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string responseTxt = www.downloadHandler.text;
                Debug.Log("Réponse : " + responseTxt);

                var json = JSON.Parse(responseTxt); // On parse la réponse du serveur pour en faire une liste
                
                if (json.Count > 0)
                {
                    int highScore = json["highscore"].AsInt; // On récupère seulement la première clé du tableau et seulement highscore 
                    _callback(highScore);
                }
                else
                {
                    Debug.LogWarning("Aucun meilleur score trouvé !");
                    _callback(0); // Si le score n'est pas trouvé, retourne 0
                }
            }
            else
            {
                Debug.LogError("Erreur de requête : " + www.error);
                _callback(0); // En cas d'erreur, retourne 0
            }
        }

    }

    public void CheckPseudoExist(string _playerName )
    {

    }
}