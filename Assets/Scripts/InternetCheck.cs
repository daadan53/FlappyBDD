using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class InternetCheck : MonoBehaviour
{
    [SerializeField] private GameObject errorPanel;
    [SerializeField] private TextMeshProUGUI errorMessage;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button quitButton;
    public bool internetConnection = true;

    private string testUrl = "https://www.google.com"; // URL pour tester la connexion

    void Start()
    {
        retryButton.onClick.AddListener(() => StartCoroutine(CheckInternetConnection()));
        quitButton.onClick.AddListener(() => OnApplicationQuit());
        StartCoroutine(CheckInternetConnection());
    }

    private void SetPause(bool isActive)
    {
        if(!isActive)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    IEnumerator CheckInternetConnection()
    {
        yield return null;
        
        // Désactive les contrôles au début
        SetPause(false);

        //1. Vérifier la connexion au réseau
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            ShowError("Pas de connexion réseau. Vérifiez votre Wi-Fi.");
            yield break;
        }

        // 2. Vérifier l'accès réel à Internet via une requête HTTP
        using (UnityWebRequest request = UnityWebRequest.Get(testUrl))
        {
            request.timeout = 5; // Temps d'attente max pour éviter un blocage
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                ShowError("Pas d'acces a Internet. Verifiez votre connexion.");
            }
            else
            {
                Debug.Log("Connexion Internet confirmée. Lancement du jeu...");
                HideError();
            }
        }

        SetPause(internetConnection);
        //Debug : 
        if(!internetConnection)
        {
            ShowError("Pas d'acces a Internet. Verifiez votre connexion.");
        }
    }

    void ShowError(string message)
    {
        if (errorPanel != null && errorMessage != null)
        {
            errorMessage.text = message;
            errorPanel.SetActive(true);
            SetPause(false);
        }
        else
        {
            Debug.LogError("Pannel d'erreur non assigné dans l'inspecteur.");
        }
    }

    void HideError()
    {
        if (errorPanel != null)
        {
            errorPanel.SetActive(false);
        }
    }

    public void OnApplicationQuit() 
    {
        Application.Quit();
    }
}
