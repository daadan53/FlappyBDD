using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static event Action<float> OnSetVelocity;
    float velocity = 1.3f;

    public static event Action<bool> OnEnableToStart;

    [SerializeField] private GameObject gameOverCanvas;
    
    float timerStart = 3f;
    [SerializeField] TextMeshProUGUI countDownTxt;

    [SerializeField] TMP_InputField pseudoInputField;
    string pseudo;
    [SerializeField] private SaveDataJSON saveManager;
    [SerializeField] private Canvas pseudoCanvas;

    [SerializeField] private TextMeshProUGUI popUpErrorTxt;
    [SerializeField] private TextMeshProUGUI showPseudo;

    private int actualScore;
    [SerializeField] private TextMeshProUGUI currentScoreTxt;
    [SerializeField] private TextMeshProUGUI highScoreTxt;

    [SerializeField] private TextMeshProUGUI leaderBoardTxt;
    [SerializeField] private TextMeshProUGUI leaderBoardGOTxt;

    public bool canSavePreviousPseudo = true;
    private bool isCroissant = true;
    private bool thisMonth = false;

    void Awake()
    {
        //Pseudo.OnPseudoSubmited += StartWithDelay;
        if(instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        if(!canSavePreviousPseudo)
        {
            PlayerPrefs.DeleteAll();
        }
        
        //Vérifie si un pseudo à déjà jouer sur cet appareil et le "reconnecte" si oui
        if (PlayerPrefs.HasKey("SavedPseudo"))
        {
            pseudo = PlayerPrefs.GetString("SavedPseudo");
            pseudoCanvas.enabled = false; // Cache l'écran de connexion
            highScoreTxt.text = saveManager.LoadData(pseudo).ToString();
            StartCoroutine(StartCountdown()); // Lancer directement le jeu
        }
        else
        {
            ShowLeaderBoard();
        }
    }

    public void CreatePseudo()
    {
        pseudo = pseudoInputField.text.Trim(); // Récupère le pseudo et enlève les espaces inutiles
        if (string.IsNullOrEmpty(pseudo))
        {
            Debug.LogWarning("Le pseudo ne peut pas être vide !");
            return;
        }

        if(saveManager.PseudoExistance(pseudo))
        {
            popUpErrorTxt.text = "Le pseudo renseigné existe déjà, veuillez vous connecter";
        }
        else
        {
            OnStartGame();
        }
    }

    public void Login()
    {
        pseudo = pseudoInputField.text.Trim(); // Récupère le pseudo et enlève les espaces inutiles
        if (string.IsNullOrEmpty(pseudo))
        {
            Debug.LogWarning("Le pseudo ne peut pas être vide !");
            return;
        }

        if(saveManager.PseudoExistance(pseudo))
        {
            OnStartGame();
        }
        else
        {
            popUpErrorTxt.text = "Le pseudo renseigné n'existe pas, veuillez le créer";
        }
    }

    private void OnStartGame()
    {
        popUpErrorTxt.text = "";

        PlayerPrefs.SetString("SavedPseudo", pseudo); // Sauvegarde du pseudo pour la prochaine partie
        //PlayerPrefs.Save();

        int savedHighScore = saveManager.LoadData(pseudo);
        highScoreTxt.text = savedHighScore.ToString();

        pseudoCanvas.enabled = false;
        StartCoroutine(StartCountdown());
    }

    public void GameOver()
    {
        gameOverCanvas.SetActive(true);

        Time.timeScale = 0f;
        OnEnableToStart?.Invoke(false);

        ShowLeaderBoard();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //Démarre un compte à rebour de 3s avant que le jeu ne commence
    private IEnumerator StartCountdown()
    {
        float timeRemaining = timerStart;
        countDownTxt.enabled = true;

        while (timeRemaining > 0)
        {
            countDownTxt.text = timeRemaining.ToString("F0"); // Affiche sans décimales
            yield return new WaitForSeconds(1f);
            timeRemaining--;
        }

        countDownTxt.text = "GO!";
        yield return new WaitForSeconds(0.5f);

        countDownTxt.enabled = false;

        Time.timeScale = 1f;

        OnSetVelocity?.Invoke(velocity);
        OnEnableToStart?.Invoke(true);

        showPseudo.text = pseudo;
    }

    public void UpdateScore()
    {
        actualScore++;
        currentScoreTxt.text = actualScore.ToString();
        UpdateHighScore();
    }

    private void UpdateHighScore()
    {
        if(actualScore > saveManager.LoadData(pseudo))
        {
            saveManager.SaveData(pseudo, actualScore);
            Debug.Log($"Highscore actualisé pour {pseudo}");
            highScoreTxt.text = actualScore.ToString();
        }
    }

    public void ChangePseudo()
    {
        PlayerPrefs.DeleteKey("SavedPseudo");
        RestartGame();
    }

    public void OnToogleChanged(bool _isOn)
    {
        isCroissant = _isOn;
        ShowLeaderBoard();
    }

    public void OnToogleDateChanged(bool _isOn)
    {
        thisMonth = _isOn;
        ShowLeaderBoard();
    }

    //Gestion du leaderBoard. Par défaut croissant
    public void ShowLeaderBoard() // Mon premier parametre ne peut etre rien
    {   
        List<PlayerData> sortedPlayers = saveManager.GetSortedLeaderboard(isCroissant); // True par défaut

        // Filtrer les scores du mois
        if(thisMonth)
        {
            DateTime currentMonth = DateTime.Now;
            sortedPlayers = sortedPlayers.Where(player => 
                DateTime.Parse(player.date).Month == currentMonth.Month &&
                DateTime.Parse(player.date).Year == currentMonth.Year).ToList();
        }

        leaderBoardTxt.text = "";
        
        int rank = 1;
        foreach(var player in sortedPlayers)
        {
            string formattedDate = DateTime.Parse(player.date).ToString("dd/MM/yyyy"); // Affiche jour/mois/année

            leaderBoardTxt.text += $"{rank}. {player.pseudo} - {player.highscore} : {formattedDate}\n";
            rank++;
        }

        leaderBoardGOTxt.text = leaderBoardTxt.text;
    }

    void OnDestroy()
    {
        instance = null;
    }
}
