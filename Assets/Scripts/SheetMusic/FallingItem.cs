using UnityEngine;

public class FallingItem : MonoBehaviour
{   SheetMusic sheetMusic;
    double targetTime;
    float speed;

    public void Setup(SheetMusic sheet, Transform hitTarget, double time)
    {
        sheetMusic = sheet;
        targetTime = time;

        speed = (sheet.spawnY - sheet.hitY) / sheet.approachTimeInSeconds;

        float x = sheet.transform.InverseTransformPoint(hitTarget.position).x;
        transform.localPosition = new Vector3(x, CurrentHeight(), 0f);
    }

    void Update()
    {
        if (sheetMusic == null || !sheetMusic.IsRunning) return;

        float y = CurrentHeight();
        if (y < sheetMusic.despawnY)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 position = transform.localPosition;
        position.y = y;
        transform.localPosition = position;

    }

    float CurrentHeight()
    {
        double secondsToTarget = targetTime - sheetMusic.TuneTime;
        return sheetMusic.hitY + (float)secondsToTarget * speed;

    }

}
