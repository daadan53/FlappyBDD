using System;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public string pseudo;
    public int highscore;
    public string date;

    /*private static PlayerData instance = null;

    public static PlayerData Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new PlayerData();
            }

            return instance;
        }
    }*/

    /*public PlayerData(string _pseudo, int _highscore, string _date)
    {
        this.pseudo = _pseudo;
        this.highscore = _highscore;
        this.date = _date;
    }*/
}
