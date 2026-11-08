using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System;
using System.IO;
using UnityEngine;


public class LoadingState : GameState
{
    public MenuPage menuPage;
    public PlayingState playingState;
    public MenuState menuState;

    public TuneData Tune { get; set; }

    void OnEnable()
    {
        menuPage.gameObject.SetActive(true);
        menuPage.SetInteractable(false);
        LoadTune();
    }

    void OnDisable()
    {
        if (menuPage != null) menuPage.gameObject.SetActive(false);
    }

    async void LoadTune()
    {
        MidiFile midi;

        try
        {
            byte[] bytes = await ReadMidi.Load(Tune);
            midi = MidiFile.Read(new MemoryStream(bytes));
            SetTempo(midi, Tune.quarterNotePerMinutes);
        }
        catch (Exception e)
        {
            Debug.LogError($"Could not load '{Tune.title}': {e.Message}");
            if (this != null && enabled) SwitchState(menuState);
            return;
        }

        if (this == null || !enabled) return;

        playingState.Tune = Tune;
        playingState.Midi = midi;
        SwitchState(playingState);
    }

    static void SetTempo(MidiFile midi, float quarterNotesPerMinute)
    {
        using var tempoMapManager = midi.ManageTempoMap();
        tempoMapManager.ClearTempo(0); 
        tempoMapManager.SetTempo(0, Tempo.FromBeatsPerMinute(quarterNotesPerMinute));
    }
}