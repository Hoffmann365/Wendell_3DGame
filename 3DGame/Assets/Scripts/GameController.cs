using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public Text healthText;
    public int score;
    public Text scoreText;

    public int totalScore;

    public static GameController instance;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void UpdateScore(int value)
    {
        score += value;
        scoreText.text = "x " + score.ToString();
        
    }
    
    public void UpdateLives(int value)
    {
        healthText.text = "x " + value.ToString();
        
    }
    
    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
}
