﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {


    
    public float startDelay = 3f;
    public float endDelay = 3f;

    public int scoreToWin;
    public int scoreToLose;

    public int pointsScored;
    public int pointsLost;

    public int scenesPreLevel = 2;

    public Text messageText;
    public GameObject[] balls;
    private GameObject _ball;
    public Transform spawnPointBall;

    public Text scorePlayer1;
    public Text scorePlayer2;

    private bool isPaused;
    private GameObject pauseGO;
    
    //public int roundsNumber;
    private WaitForSeconds startWait;
    private WaitForSeconds endWait;

    public PlayerManager[] players;

    //private PlayerManager roundWinner;
    private PlayerManager gameWinner;

    public bool playing = false;

    public bool ballTouchFloorLeft1;
    public bool ballTouchFloorRight2;

    private MusicManager _musicManager;
    
    // Use this for initialization
    public static GameManager instance = null;

    public GameObject PauseGO
    {
        get
        {
            return pauseGO;
        }

        set
        {
            pauseGO = value;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }
        DontDestroyOnLoad(this.gameObject);

        _musicManager = GameObject.Find("MusicManager").GetComponent<MusicManager>();
        pauseGO = GameObject.Find("PauseMenu");
    }

    void Start () {

        startWait = new WaitForSeconds(startDelay);
        endWait = new WaitForSeconds(endDelay);

        //start spawn players

        StartCoroutine(GameLoop());

        MusicManager.imageChanged = false;
        pauseGO.SetActive(false);
        isPaused = false;
    }

    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(PointStarting());
        yield return StartCoroutine(PointPlaying());
        yield return StartCoroutine(PointEnding());

        scorePlayer1.text = players[0].score.ToString();
        scorePlayer2.text = players[1].score.ToString();

        if (gameWinner != null)
        {
            Debug.Log("El ganador es PLAYER" + gameWinner.playerNumber);
            MusicManager.winner = gameWinner.playerNumber;
            SceneManager.LoadScene(5);

            _musicManager._playerWinner = gameWinner.playerNumber;
            Destroy(this.gameObject);
        }
        else
        {
            StartCoroutine(GameLoop());
        }
    }

    private IEnumerator PointStarting()
    {
        Debug.Log("round starting");
        SpawnPlayers();
        scorePlayer1.text = players[0].score.ToString();
        scorePlayer2.text = players[1].score.ToString();
        if (_ball != null)
        {
            Destroy(_ball);
        }
        SpawnBall();
        DisablePlayerControls();
        DisableBall();

        //choose one random map

        yield return startWait;
    }

    private IEnumerator PointPlaying()
    {
        playing = true;
        Debug.Log("round playing");
        EnablePlayerControls();
        EnableBall();
        

        while (!ballTouchFloorLeft1 && !ballTouchFloorRight2 && gameWinner==null)
        {
            CheckWin();
            scorePlayer1.text = players[0].score.ToString();
            scorePlayer2.text = players[1].score.ToString();
            if (gameWinner!=null)
            {
                DisablePlayerControls();
                DisableBall();
            }
            yield return null;
        }
    }

    private IEnumerator PointEnding()
    {
        playing = false;
        DisablePlayerControls();

        if (ballTouchFloorLeft1)
        {
            players[1].setScore(players[1].getScore() + pointsScored);
            players[0].setScore(players[0].getScore() - pointsLost);
            ballTouchFloorLeft1 = false;
        }

        else if (ballTouchFloorRight2)
        {
            players[0].setScore(players[0].getScore() + pointsScored);
            players[1].setScore(players[1].getScore() - pointsLost);
            ballTouchFloorRight2 = false;
        }

        CheckWin();
        

        yield return endWait;
            
    }
    private void DisablePlayerControls()
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].GetComponent<PlayerManager>().DisableControl();
        }
    }
    private void EnablePlayerControls()
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].GetComponent<PlayerManager>().EnableControl();
        }
    }
    private void SpawnPlayers()
    {
        for (int i = 0; i<players.Length; i++)
        {
            players[i].gameObject.transform.position = players[i].spawnPoint.transform.position;
        }
    }

    private void SpawnBall()
    {
        _ball = Instantiate(balls[(int)SceneManager.GetActiveScene().buildIndex-scenesPreLevel], spawnPointBall.position, Quaternion.identity);
        
    }
    private void DisableBall()
    {
        Debug.Log("disable the ball");
        _ball.GetComponent<Rigidbody>().useGravity = false;
        _ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
      
    }
    private void EnableBall()
    {
        Debug.Log("enable the ball");
        _ball.GetComponent<Rigidbody>().useGravity = true;

    }

    private void CheckWin()
    {
        //condition to win player1:

        if (players[0].getScore() == scoreToWin || players[1].getScore() == scoreToLose)
        {
            //win player 1
            gameWinner = players[0];
           
        }

        //condition to win player2
        else if (players[1].getScore() == scoreToWin || players[0].getScore() == scoreToLose)
        {
            //win player 2
            gameWinner = players[1];
    
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                pauseGO.SetActive(false);
                PauseGame(false);
                isPaused = false;
            }
            else
            {
                pauseGO.SetActive(true);
                PauseGame(true);
                isPaused = true;
            }
        }
    }

    public void PauseGame(bool pause)
    {
        if (pause)
            Time.timeScale = 0.1f;
        else
            Time.timeScale = 1.0f;
    }

    public void RestartGameLoop()
    {
        pauseGO.SetActive(false);
        PauseGame(false);
        StopAllCoroutines();
        StartCoroutine(GameLoop());
    }
}
