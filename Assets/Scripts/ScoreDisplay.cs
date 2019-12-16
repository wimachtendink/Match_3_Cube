using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{

    public TextMeshProUGUI tm_ScoreText;
    //public Text scoreText;
    public int score;

    public void ResetScore()
    {
        score = 0;
        tm_ScoreText.text = score.ToString();
    }
    private void Start()
    {
        score = 0;
        //scoreText.text = score.ToString();
        tm_ScoreText.text = score.ToString();
    }

    public void IncrementScore()
    {
        score++;
        //scoreText.text = score.ToString();
        tm_ScoreText.text = score.ToString();
    }

}
