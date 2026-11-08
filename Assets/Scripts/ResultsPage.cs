using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultsPage : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text scoreText;
    public TMP_Text detailsText;

    public Button replayButton;
    public Button menuButton;

    public event Action ReplayPressed;
    public event Action MenuPressed;

    void Awake()
    {
        replayButton.onClick.AddListener(() => ReplayPressed?.Invoke());
        menuButton.onClick.AddListener(() => MenuPressed?.Invoke());
    }

    public void Show(TuneData tune, Results results)
    {
        titleText.text = tune.title;
        scoreText.text = $"{results.score} Points";
        detailsText.text = 
            $"Perfect: {results.perfect}   Good: {results.good}   Miss: {results.miss}\n" +
            $"Extra hits: {results.extraHits}\n" +
            $"Longest streak: {results.longestStreak}";
    }
}
