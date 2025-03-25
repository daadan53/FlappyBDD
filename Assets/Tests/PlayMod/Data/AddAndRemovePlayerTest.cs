using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.TestTools;
using System.Collections.Generic;

public class AddAndRemovePlayerTest
{
    private MongoManager mongoManager;
    private string testPlayerName = "TestPlayer_UnitTest";

    [SetUp]
    public void SetUp()
    {
        GameObject mongoManagerObject = new GameObject();
        mongoManager = mongoManagerObject.AddComponent<MongoManager>();
    }

    [UnityTest]
    public IEnumerator AddPlayer_AndVerify_ThenRemove()
    {
        bool playerAdded = false;

        // Ajouter un joueur
        yield return mongoManager.StartCoroutine(mongoManager.AddPlayerCoroutine(testPlayerName, (result) => playerAdded = result));

        Assert.IsTrue(playerAdded, "L'ajout du joueur a échoué.");

        // Vérifier que le joueur existe bien
        List<PlayerData> retrievedPlayers = null;
        yield return mongoManager.StartCoroutine(mongoManager.GetScoresCoroutine(true, (result) => retrievedPlayers = result));

        PlayerData foundPlayer = retrievedPlayers?.Find(p => p.name == testPlayerName);
        Assert.IsNotNull(foundPlayer, "Le joueur n'a pas été trouvé après l'ajout.");
        Assert.AreEqual(testPlayerName, foundPlayer.name, "Le pseudo du joueur ne correspond pas.");

        // Supprimer le joueur après le test
        yield return DeleteTestPlayerFromDatabase(testPlayerName);
    }

    private IEnumerator DeleteTestPlayerFromDatabase(string playerName)
    {
        WWWForm form = new WWWForm();
        form.AddField("name", playerName);

        using (UnityWebRequest request = UnityWebRequest.Post("http://localhost/FlappyAPI/delete_player.php", form))
        {
            yield return request.SendWebRequest();

            Assert.AreEqual(UnityWebRequest.Result.Success, request.result, "La suppression du joueur a échoué : " + request.error);
        }
    }
}
