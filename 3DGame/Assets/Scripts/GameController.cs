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
    
    public GameObject finishObj;

    public AudioSource sound;
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
        Congratulations();
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
    
    public void Congratulations()
    {
        if (score == 6)
        {
            StartCoroutine("Finish");
        }
    }
    
    IEnumerator Finish()
    {
        yield return new WaitForSeconds(1.5f);
        sound.mute = true;
        finishObj.SetActive(true);
        Time.timeScale = 0f;
    }
}
