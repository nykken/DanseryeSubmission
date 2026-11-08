using UnityEngine;
using TMPro;


public class Streak : MonoBehaviour
{
    PointsManager _pointsManager;
    public TMP_Text currentStreak;

    public void Begin(PointsManager pointsManager)
    {
        _pointsManager = pointsManager;

    }


    void Update()
    {
        currentStreak.text = _pointsManager.CurrentStreak.ToString();
    }
}
