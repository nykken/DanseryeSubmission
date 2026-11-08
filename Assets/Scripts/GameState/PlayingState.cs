using Melanchall.DryWetMidi.Core;
using UnityEngine;

public class PlayingState : GameState
{
    public TuneClock conductor;
    public SheetMusic sheetMusic;
    public PointsManager pointsManager;
    public Streak streak;
    public GameObject sheetDisplay;
    public ResultsState resultsState;
   
    public TuneData Tune { get; set; }
    public MidiFile Midi { get; set; }

    void OnEnable()
    {
        sheetDisplay.SetActive(true);
        streak.gameObject.SetActive(true);
        conductor.Finished += OnTuneFinished;

        conductor.Play(Tune, sheetMusic.LeadInTimeInSeconds);
        sheetMusic.Begin(Midi, conductor);
        pointsManager.Begin(sheetMusic.TotalNoteCount);
        streak.Begin(pointsManager);
    }

    void OnDisable()
    {
        if (conductor != null)
        {
            conductor.Finished -= OnTuneFinished;
            conductor.Stop(); 
        }

        if (streak != null) streak.gameObject.SetActive(false);
        if (sheetMusic != null)
        {
            if (sheetMusic != null) sheetMusic.End();
            if (sheetDisplay != null) sheetDisplay.SetActive(false);
        }
    }

    void OnTuneFinished()
    {
        resultsState.Tune = Tune;
        resultsState.Results = pointsManager.Results;
        SwitchState(resultsState);
    }
}