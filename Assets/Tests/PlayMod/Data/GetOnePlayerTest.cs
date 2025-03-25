using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GetOnePlayerTest
{
    private MongoManager mongoManager;
    private string testPlayerName = "Test";
    private int retrievedHighScore = -1;

    [SetUp]
    public void SetUp()
    {
        GameObject mongoManagerObject = new GameObject();
        mongoManager = mongoManagerObject.AddComponent<MongoManager>();
    }

    [UnityTest]
    public IEnumerator GetSinglePlayer_ReturnsValidData()
    {
        yield return mongoManager.StartCoroutine(mongoManager.GetScoreOfThisPlayerCoroutine(testPlayerName, (score) => retrievedHighScore = score));

        Assert.AreNotEqual(-1, retrievedHighScore, "Le score du joueur n'a pas été récupéré.");
        Assert.GreaterOrEqual(retrievedHighScore, 0, "Le score du joueur est négatif.");

        List<PlayerData> retrievedPlayers = null;
        yield return mongoManager.StartCoroutine(mongoManager.GetScoresCoroutine(true, (result) => retrievedPlayers = result));

        Assert.IsNotNull(retrievedPlayers, "La liste des joueurs est null.");
        Assert.IsNotEmpty(retrievedPlayers, "La liste des joueurs est vide.");

        PlayerData foundPlayer = retrievedPlayers.Find(p => p.name == testPlayerName);
        Assert.IsNotNull(foundPlayer, "Le joueur n'a pas été trouvé dans la liste.");
        Assert.AreEqual(testPlayerName, foundPlayer.name, "Le pseudo du joueur ne correspond pas.");
        Assert.GreaterOrEqual(foundPlayer.highscore, 0, "Le score du joueur est invalide.");
        Assert.IsNotNull(foundPlayer.date, "Le joueur n'a pas de date associée.");
    }
}
