using UnityEngine;
using System;

public class Results
{
    public int score;
    public int perfect;
    public int good;
    public int miss;
    public int extraHits;
    public int longestStreak;
    public int totalNotes;
}

public class PointsManager : MonoBehaviour
{
    public static PointsManager Instance;

    public float perfectWindow = 0.06f;
    public float goodWindow = 0.12f;
    public float latencyOffset = 0f;

    public int perfectPoints = 100;
    public int goodPoints = 90;
    public int extraHitPenalty = 20;

    public Results Results { get; private set; } = new();

    public int CurrentStreak { get; private set; }
    
    void Awake() => Instance = this;

    public void Begin(int totalNotes)
    {
        Results = new Results { totalNotes = totalNotes };
        CurrentStreak = 0;
    }

    public static bool TryHit(double hitTime, double? nextNoteTargetTime)
    {
        var results = Instance.Results;

        double error = nextNoteTargetTime == null
            ? double.MaxValue
            : Math.Abs(hitTime - Instance.latencyOffset - nextNoteTargetTime.Value);


        if (error > Instance.goodWindow)
        {
            results.extraHits++;
            results.score -= Instance.extraHitPenalty;
            Instance.CurrentStreak = 0;
            return false;
        }

        if (error <= Instance.perfectWindow)
        {
            results.perfect++;
            results.score += Instance.perfectPoints;
        }
        else
        {
            results.good++;
            results.score += Instance.goodPoints;
        }

        Instance.CurrentStreak++;
        results.longestStreak = Mathf.Max(results.longestStreak, Instance.CurrentStreak);
        return true;
    }

    public static bool CheckMiss(double now, double noteTargetTime)
    {
        if (now - noteTargetTime <= Instance.goodWindow) return false;

        Instance.Results.miss++;
        Instance.CurrentStreak = 0;
        return true;
    }
}
