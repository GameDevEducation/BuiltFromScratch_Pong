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

    public enum EGameMode
    {
        Classic,
        WithItems
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

    [SerializeField] EGameMode Mode = EGameMode.Classic;
    [SerializeField] BoxCollider ItemSpawnZone;
    [SerializeField] float ItemSpawnInterval = 15f;
    [SerializeField] int MaxItems = 3;
    [SerializeField] GameObject[] ItemPrefabs;
    float NextItemSpawnTime = 0f;
    List<GameObject> SpawnedItems = new List<GameObject>();

    List<BaseItemEffect> LeftPaddleEffects = new List<BaseItemEffect>();
    List<BaseItemEffect> RightPaddleEffects = new List<BaseItemEffect>();

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

        if (PlayerPrefs.GetInt(PlayerPrefKeys.IsWithItems, 0) == 1)
            Mode = EGameMode.WithItems;
        else
            Mode = EGameMode.Classic;

        StartCoroutine(SpawnNewBall(InitialBallSpawnDelay));
    }

    // Update is called once per frame
    void Update()
    {
        if (Mode == EGameMode.WithItems)
        {
            // update the effects
            TickEffects(LeftPaddleEffects);
            TickEffects(RightPaddleEffects);

            // do we have space to spawn a new item
            if (SpawnedItems.Count < MaxItems)
            {
                // update spawn time
                NextItemSpawnTime -= Time.deltaTime;
                if (NextItemSpawnTime <= 0)
                {
                    NextItemSpawnTime = ItemSpawnInterval;

                    SpawnItem();
                }
            }
        }
    }

    public void ConsumeItem(GameObject item)
    {
        // clean up the item
        SpawnedItems.Remove(item);
        Destroy(item);
    }

    void SpawnItem()
    {
        // pick an item to spawn
        var itemPrefab = ItemPrefabs[Random.Range(0, ItemPrefabs.Length)];

        // pick a random spawn point
        Vector3 spawnPoint = new Vector3(Random.Range(ItemSpawnZone.bounds.min.x, ItemSpawnZone.bounds.max.x),
                                         0f,
                                         Random.Range(ItemSpawnZone.bounds.min.z, ItemSpawnZone.bounds.max.z));

        // spawn the item
        SpawnedItems.Add(Instantiate(itemPrefab, spawnPoint, Quaternion.identity));
    }

    void TickEffects(List<BaseItemEffect> effects)
    {
        // update the effects
        foreach(var effect in effects)
        {
            effect.TickEffect();
        }

        // remove any completed effects
        for(int index = effects.Count - 1; index >= 0; --index)
        {
            if (!effects[index].IsActive)
                effects.RemoveAt(index);
        }
    }

    public void ApplyEffect(BaseItemEffect effect)
    {
        if (Mode == EGameMode.Classic || ActiveBall == null)
            return;

        // apply to last person to touch ball?
        if (effect.AppliesToLastTouchedBy)
        {
            if (ActiveBall.LastTouchedBy == BallController.ELastTouchedBy.Player1)
                LeftPaddleEffects.Add(ScriptableObject.Instantiate(effect));
            else if (ActiveBall.LastTouchedBy == BallController.ELastTouchedBy.Player2)
                RightPaddleEffects.Add(ScriptableObject.Instantiate(effect));
        }

        // apply to opponent of last person to touch ball?
        if (effect.AppliesToOpponent)
        {
            if (ActiveBall.LastTouchedBy == BallController.ELastTouchedBy.Player1)
                RightPaddleEffects.Add(ScriptableObject.Instantiate(effect));
            else if (ActiveBall.LastTouchedBy == BallController.ELastTouchedBy.Player2)
                LeftPaddleEffects.Add(ScriptableObject.Instantiate(effect));
        }        
    }

    public float Effects_ModifyInput(PaddleController paddle, float currentInput)
    {
        if (Mode == EGameMode.Classic)
            return currentInput;

        var effectStack = paddle == LeftPaddle ? LeftPaddleEffects : RightPaddleEffects;

        // apply the effects
        foreach(var effect in effectStack)
        {
            currentInput = effect.ModifyInput(currentInput);
        }

        return currentInput;
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
