using System;
using TMPro;
using UnityEngine;

public class main_Controller : MonoBehaviour
{
    int score = 0;
    int speed = 0;
    int count = 0;
    public GameObject Player;
    public GameObject ScoreBoard;
    public GameObject SpeedBoard;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        count++;
        if (count >= 15)
        {
            count = 0;
            speed = (int)Player.GetComponent<Player_Controller>().Velocity;
            displaySpeed();
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
        ScoreBoard.GetComponent<TextMeshPro>().text = final;
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
