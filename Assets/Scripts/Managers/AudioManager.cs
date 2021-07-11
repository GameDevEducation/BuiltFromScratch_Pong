using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] List<AudioClip> SFX_BallHitWall;
    [SerializeField] List<AudioClip> SFX_BallHitPaddle;
    [SerializeField] List<AudioClip> SFX_GoalScored;
    [SerializeField] List<AudioClip> SFX_GameOver;

    [SerializeField] AudioSource SFXEmitter;
    [SerializeField] AudioSource BGMEmitter;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found multiple AudioManagers");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    AudioClip PickRandomClip(List<AudioClip> clips)
    {
        if (clips == null || clips.Count == 0)
            Debug.LogError("No audio clips configured!");

        return clips[Random.Range(0, clips.Count)];
    }

    public void OnBallHitWall()
    {
        SFXEmitter.PlayOneShot(PickRandomClip(SFX_BallHitWall));
    }

    public void OnBallHitPaddle()
    {
        SFXEmitter.PlayOneShot(PickRandomClip(SFX_BallHitPaddle));
    }

    public void OnGoalScored()
    {
        SFXEmitter.PlayOneShot(PickRandomClip(SFX_GoalScored));
    }

    public void OnGameOver()
    {
        SFXEmitter.PlayOneShot(PickRandomClip(SFX_GameOver));
    }
}
