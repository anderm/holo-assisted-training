using UnityEngine;
using UnityEngine.UI;
using HoloToolkit.Unity;

public class ScoreManager : Singleton<ScoreManager>
{

    [SerializeField]
    Scoring scoring;

    public Text scoreText;

    private uint _score;
    public uint score
    {
        get
        {
            return _score;
        }
        set
        {
            _score = value;
            UpdateScoreText();
        }
    }

    // Use this for initialization
    void Start()
    {
        Reset();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void UpdateScoreText()
    {
        scoreText.text = score.ToString();
    }

    void Reset()
    {
        score = 0;
    }

    void OnReset()
    {
        Debug.Log("Score: Reseting player");
        Reset();
    }

    public void OnCalculateFinalScore(float timeRemaining)
    {
        // only update score with bonus if time remaining
        if (timeRemaining > 0)
        {
            score = (uint)Mathf.Round(score * timeRemaining * scoring.timeBonusMultiplier);
            Debug.Log(string.Format("Score bonus time: {0} x {1}", timeRemaining, scoring.timeBonusMultiplier));
        }
        Debug.Log(string.Format("Score final: {0}", score));
    }

    public void OnScoreAttempts(uint attempts)
    {
        uint attemptScore = GetScore(attempts);
        score += attemptScore;
        Debug.Log(string.Format("Attempt {0} Score: {1} Total: {2}", attempts, attemptScore, score));
    }

    uint GetScore(uint attempts)
    {
        uint attemptScore = scoring.more;
        switch (attempts)
        {
            case 1:
                attemptScore = scoring.first;
                break;
            case 2:
                attemptScore = scoring.second;
                break;
            case 3:
                attemptScore = scoring.third;
                break;
        }
        return attemptScore;
    }
}

// Scoring model
[System.Serializable]
public class Scoring
{
    // scoring for each attempt
    public uint first = 100; // maximum score
    public uint second = 50;
    public uint third = 25;
    public uint more = 10; // minimun score

    public float timeBonusMultiplier = 1.0f;
}