using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private int enemyKilledScore;
    public static GameManager Instance { get; private set; }

    public int Score
    {
        get
        {
            return Mathf.FloorToInt(GetSurvivalTime) + enemyKilledScore;
        }

        private set
        {
            
        }
    }

    public bool IsGameActive { get; private set; }

    public float GetSurvivalTime { get; private set; }
    
    private void Start()
    {
        StartGame(); 
    }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (IsGameActive)
            GetSurvivalTime +=  Time.deltaTime;
    }

    public void AddScore(int value)
    {
        enemyKilledScore += value;
    }

    public void StartGame()
    {
        IsGameActive = true;
    }

    public void GameOver()
    {
        IsGameActive = false;
        Time.timeScale = 0f; 
    }
}