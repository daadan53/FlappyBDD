using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using System;

public class LooseTest
{
    private GameManager gameManager;

    [SetUp]
    public void SetUp()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        SceneManager.LoadScene("Scenes/SampleScene", LoadSceneMode.Single);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "SampleScene")
        {
            // Trouver GameManager dans la scène
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            Assert.IsNotNull(gameManager, "GameManager introuvable dans la scène.");
        }
    }

    [UnityTest]
    public IEnumerator Game_Over_And_Restart_Test()
    {
        // Attendre 5 secondes sans rien faire
        yield return new WaitForSeconds(6f);

        // Vérifier si le panneau de Game Over est actif
        Assert.IsTrue(gameManager.GameOverCanvas.activeSelf, "Le panneau Game Over ne s'est pas affiché.");

        // Vérifier que le leaderboard est bien rempli
        TextMeshProUGUI leaderBoardTxt = GameObject.Find("TextClassementGO")?.GetComponent<TextMeshProUGUI>();
        Assert.IsNotNull(leaderBoardTxt, "Le leaderboard est introuvable.");
        Assert.IsNotEmpty(leaderBoardTxt.text, "Le leaderboard est vide.");

        // Vérifier que les données du leaderboard correspondent à la BDD
        List<PlayerData> retrievedPlayers = null;
        yield return gameManager.StartCoroutine(gameManager.MongoManager.GetScoresCoroutine(true, (result) => retrievedPlayers = result));

        Assert.IsNotNull(retrievedPlayers, "Erreur : Impossible de récupérer les joueurs depuis la BDD.");
        Assert.IsNotEmpty(retrievedPlayers, "La BDD ne contient aucun joueur.");

        foreach (var player in retrievedPlayers)
        {
            Assert.IsTrue(leaderBoardTxt.text.Contains(player.name), $"Le joueur {player.name} n'est pas affiché dans le leaderboard.");
            Assert.IsTrue(leaderBoardTxt.text.Contains(player.highscore.ToString()), $"Le score du joueur {player.name} n'est pas affiché.");
            Assert.IsTrue(leaderBoardTxt.text.Contains(DateTime.Parse(player.date).ToString("dd/MM/yyyy")), $"La date du joueur {player.name} n'est pas affichée.");
        }

        // On replay
        gameManager.RestartGame();
        yield return new WaitForSeconds(1f); // Attendre un peu pour la transition

        // Vérifier que la scène a bien été rechargée
        Assert.AreEqual(SceneManager.GetActiveScene().buildIndex, 0, "La scène n'a pas été rechargée.");
    }
}