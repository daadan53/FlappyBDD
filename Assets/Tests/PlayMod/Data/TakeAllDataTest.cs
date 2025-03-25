using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TakeAllDataTest
{
    private MongoManager mongoManager;

    [SetUp]
    public void SetUp()
    {
        GameObject mongoManagerObject = new GameObject();
        mongoManager = mongoManagerObject.AddComponent<MongoManager>();
    }

    [UnityTest]
    public IEnumerator GetScoresCoroutine_ReturnsPlayersList()
    {
        bool isCroissant = true; // Tester le tri croissant
        List<PlayerData> retrievedPlayers = null;

        yield return mongoManager.StartCoroutine(mongoManager.GetScoresCoroutine(isCroissant, (result) => retrievedPlayers = result));

        Assert.IsNotNull(retrievedPlayers, "La liste des joueurs récupérée est null.");
        Assert.IsNotEmpty(retrievedPlayers, "La liste des joueurs est vide.");
        
        foreach (var player in retrievedPlayers)
        {
            Assert.IsNotNull(player.name, "Un joueur n'a pas de nom.");
            Assert.GreaterOrEqual(player.highscore, 0, "Un score est négatif.");
            Assert.IsNotNull(player.date, "Un joueur n'a pas de date associée.");
        }
    }
}
