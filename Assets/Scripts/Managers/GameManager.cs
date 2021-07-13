using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum EGameState
    {
        Playing,
        GameOver
    }

    [SerializeField] ScoreHUD ScoreUI;
    [SerializeField] GameObject BallPrefab;
    [SerializeField] Transform BallSpawnPoint;

    [SerializeField] float InitialBallSpawnDelay = 2f;
    [SerializeField] float RespawnBallDelay = 2f;

    [SerializeField] int PointsForVictory = 5;

    [SerializeField] GameOverHUD GameOverUI;
    [SerializeField] UnityEvent OnGameOver = new UnityEvent();

    [SerializeField] PaddleController LeftPaddle;
    [SerializeField] PaddleController RightPaddle;

    EGameState State = EGameState.Playing;

    BallController SpawnedBall;

    int Player1Score = 0;
    int Player2Score = 0;

    public static GameManager Instance { get; private set; }

    public BallController ActiveBall => SpawnedBall;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found multiple GameManager");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        ScoreUI.OnResetScores();

        if (PlayerPrefs.GetInt(PlayerPrefKeys.IsHumanVsHuman, 1) == 1)
        {
            LeftPaddle.SetIsAIControlled(false);
            RightPaddle.SetIsAIControlled(false);
        }
        else
        {
            LeftPaddle.SetIsAIControlled(false);
            RightPaddle.SetIsAIControlled(true);            
        }

        StartCoroutine(SpawnNewBall(InitialBallSpawnDelay));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnRestartGame()
    {
        // reset scores
        ScoreUI.OnResetScores();
        Player1Score = Player2Score = 0;

        // resume playing
        State = EGameState.Playing;

        // prepare to spawn the ball
        StartCoroutine(SpawnNewBall(InitialBallSpawnDelay));
    }

    public void OnMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    IEnumerator SpawnNewBall(float spawnDelay)
    {
        yield return new WaitForSeconds(spawnDelay);

        // destroy previous ball if present
        if (SpawnedBall != null)
            Destroy(SpawnedBall.gameObject);

        // spawn and launch new ball
        var newBall = GameObject.Instantiate(BallPrefab, BallSpawnPoint.position, Quaternion.identity, transform);
        SpawnedBall = newBall.GetComponent<BallController>();
        SpawnedBall.Launch();
    }

    public void OnPlayer1GoalHit()
    {
        Player2Score += 1;
        ScoreUI.OnChangePlayer2Score(Player2Score);

        OnGoalScored();
    }

    public void OnPlayer2GoalHit()
    {
        Player1Score += 1;
        ScoreUI.OnChangePlayer1Score(Player1Score);

        OnGoalScored();
    }

    public void OnGoalScored()
    {
        // has either player won?
        if (Player1Score == PointsForVictory || Player2Score == PointsForVictory)
        {
            AudioManager.Instance.OnGameOver();
            State = EGameState.GameOver;

            // set the win text
            var winnerNumber = Player1Score > Player2Score ? 1 : 2;
            GameOverUI.OnSetWinText(winnerNumber, Mathf.Abs(Player1Score - Player2Score));

            OnGameOver.Invoke();
        }
        else
        {
            AudioManager.Instance.OnGoalScored();
            StartCoroutine(SpawnNewBall(RespawnBallDelay));
        }
    }
}
