using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Core;
using UnityEngine;
using System.Linq; 

public class SheetMusic : MonoBehaviour
{
    public Lane[] lanes;

    public BarLines barLines;

    TuneClock _clock;

    public float spawnY;
    public float hitY;
    public float despawnY;

    // duration of the note fall
    public float approachTimeInSeconds = 2f;

    public float extraLeadInTimeInSeconds = 1f;
    
    // playing needs to start at least approachTimeInSeconds before the fist beat
    // so that the first notes can have the full falling time and not spawn at the hit target
    public double LeadInTimeInSeconds => approachTimeInSeconds + extraLeadInTimeInSeconds;

    public bool IsRunning => _clock != null && _clock.IsPlaying;
    public double TuneTime => _clock.TuneTime;


    public int TotalNoteCount { get; private set; }


    public void Begin(MidiFile midi, TuneClock conductor)
    {
        _clock = conductor;

        var tempoMap = midi.GetTempoMap();

        var notes = midi.GetNotes();
        TotalNoteCount = notes.Count;

        foreach (var lane in lanes) lane.SetNotes(this, notes, tempoMap);
        barLines.SetBars(this, midi);
    }

    public void End()
    {
        foreach (var item in GetComponentsInChildren<FallingItem>())
            Destroy(item.gameObject);

        foreach (var lane in lanes) lane.Clear();
        barLines.Clear();
        _clock = null;
    }

    public static double ToSeconds(long ticks, TempoMap tempoMap) =>
        TimeConverter.ConvertTo<MetricTimeSpan>(ticks, tempoMap).TotalMicroseconds / 1_000_000.0;


    void OnDrawGizmosSelected()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        DrawHeight(spawnY, Color.green);
        DrawHeight(hitY, Color.yellow);
        DrawHeight(despawnY, Color.red);
    }

    static void DrawHeight(float y, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawLine(new Vector3(-10f, y, 0f), new Vector3(10f, y, 0f));
    }
}
