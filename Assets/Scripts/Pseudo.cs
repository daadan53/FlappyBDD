using System;
using TMPro;
using UnityEngine;

public class Pseudo : MonoBehaviour
{
    public static event Action OnPseudoSubmited;
    [SerializeField] private TMP_InputField inputField;
    int pseudoCount;
    int j = 0;

    void Start()
    {
        this.gameObject.GetComponent<Canvas>().enabled = true;
        Time.timeScale = 0;

        //Premier démarrage
        pseudoCount = PlayerPrefs.GetInt("NbPlayer", 0);
        Debug.Log(pseudoCount);
    }

    public void SubmitPseudo()
    {
        j=0;
        do
        {
            if(!PlayerPrefs.HasKey($"Pseudo{j}"))
            {
                PlayerPrefs.SetString($"Pseudo{j}", inputField.text);
                Debug.Log($"Le pseudo {j} : {inputField.text}, a été ajouté" );


                pseudoCount++;
                PlayerPrefs.SetInt("NbPlayer", pseudoCount);

                this.gameObject.GetComponent<Canvas>().enabled = false;
                OnPseudoSubmited?.Invoke();

                break;
            }
            else
            j++;
        }while(j < pseudoCount);

        Debug.Log($"Nb total de joueurs : {PlayerPrefs.GetInt("NbPlayer")}");
    }
}
