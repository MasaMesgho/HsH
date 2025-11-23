using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class main_Controller : MonoBehaviour
{
    int score = 0;
    int speed = 0;
    int count = 0;
    public GameObject Player;
    public GameObject ScoreBoard;
    public GameObject SpeedBoard;
    public GameObject Boss;
    public int objAmount;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        objAmount = GameObject.FindGameObjectsWithTag("Objective").Length;
    }

    // Update is called once per frame
    void Update()
    {
        if (Player == null)
        {
            SceneManager.LoadScene("GameOver");
        }

        count++;
        if (count%15 == 0)
        {
            speed = (int)Player.GetComponent<Player_Controller>().Velocity;
            displaySpeed();

        }
        if (count >= 30)
        {
            count = 0;
            if (objAmount == 0) Boss.GetComponent<GrinchAI>().SetBoss(true);
        }
    }

    public void enemyKill()
    {
        score += 1;
        string temp = score.ToString();
        string final = "";
        for (int i = 0; i < 4-temp.Length; i++)
        {
            final += "0";
        }
        final = "Score: "+final+temp;
        ScoreBoard.GetComponent<TextMeshProUGUI>().text = final;
    }

    public void displaySpeed()
    {
        string temp = speed.ToString();
        string final = "";

        for (int i = 0; i < 3 - temp.Length; i++)
        {
            final += "0";
        }
        final = "Speed: " + final + temp;
        SpeedBoard.GetComponent<TextMeshProUGUI>().text = final;
    }

}
