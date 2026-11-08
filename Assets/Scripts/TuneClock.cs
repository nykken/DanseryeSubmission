using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TuneClock : MonoBehaviour
{
    public event Action Finished;

    // how much to wait after audio ends to count song as "finished"
    public float endPadding = 1f;

    public bool IsPlaying { get; private set; }
    public double TuneTime => tuneTime; 

    AudioSource audioSource;

    // how long after the recording starts the midi/song actually start
    float audioOffset;
   
    // audio clock time at song time 0
    double dspStart;
    double endTime;

    // Starts as negative during the lead in time, 0 at the tune's actual start
    double tuneTime;

    bool isPaused;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void Play(TuneData tune, double leadInTimeInSeconds)
    {
        Stop();

        audioOffset = tune.audioOffset;
        audioSource.clip = tune.clip;

        dspStart = AudioSettings.dspTime + leadInTimeInSeconds;

        double audioTimeBeforeStart = Math.Min(leadInTimeInSeconds, audioOffset);

        audioSource.time = (float)(audioOffset - audioTimeBeforeStart);
        audioSource.PlayScheduled(dspStart - audioTimeBeforeStart);

        endTime = tune.clip.length - audioOffset + endPadding;
        tuneTime = -leadInTimeInSeconds;

        IsPlaying = true;
    }

    public void Stop()
    {
        IsPlaying = false;
        audioSource.Stop();
    }

    void Update()
    {
        if (!IsPlaying || isPaused) return;

        if (AudioSettings.dspTime < dspStart)
        {
            tuneTime = AudioSettings.dspTime - dspStart;
        }
        else if (audioSource.isPlaying)
        {
            tuneTime = ((double)audioSource.timeSamples / audioSource.clip.frequency) - audioOffset;
        }
        else
        {
            tuneTime += Time.unscaledDeltaTime;

            if (tuneTime >= endTime)
            {
                IsPlaying = false;
                Finished?.Invoke();
            }
        }    
    }

}
