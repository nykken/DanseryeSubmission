using Melanchall.DryWetMidi.Interaction;
using System.Collections.Generic;
using UnityEngine;


public struct LaneNote
{
    public double targetTimeInSeconds; 
    public float velocity; // 0 = softest, 1 = loudest
}

public class Lane : MonoBehaviour
{
    // midi note number for this lane
    // d timpanum: 38
    // g timpanum: 43
    // tambourine: 52
    public int pitchNumber;

    // color for this lane's notes
    public Color color;

    public Color paperColor;

    // how much the note color should be faded for a very soft note
    [Range(0f, 1f)] public float dynamicsMaxColorFade = 0.9f;

    public NoteScript notePrefab;

    readonly List<LaneNote> notes = new();

    readonly List<NoteScript> spawnedNotes = new();

    SheetMusic sheetMusic;

    // indices (in the notes list) of the next notes that need to be spawned/hit
    int nextNoteToSpawnIndex;
    int nextNoteToHitIndex;

    bool isRunning;

    // pp p mp mf f ff
    static readonly int[] dynamicVelocities = { 33, 49, 64, 80, 96, 112 };

    // index of the closest dynamic level
    static int DynamicLevel(int velocity)
    {
        int closest = 0;
        for (int i = 1; i < dynamicVelocities.Length; i++)
        {
            if (Mathf.Abs(velocity - dynamicVelocities[i]) >= Mathf.Abs(velocity - dynamicVelocities[closest]))
                break;
            closest = i;
        }
        return closest;
    }

    public void SetNotes(SheetMusic sheet, ICollection<Note> midiNotes, TempoMap tempoMap)
    {
        Clear();

        sheetMusic = sheet;

        foreach (var midiNote in midiNotes)
        {
            if ((byte)midiNote.NoteNumber != pitchNumber) continue;

            notes.Add(new LaneNote
            {
                targetTimeInSeconds = SheetMusic.ToSeconds(midiNote.Time, tempoMap),
                velocity = DynamicLevel((byte)midiNote.Velocity) / (dynamicVelocities.Length - 1f)
            });
        }
        Debug.Log($"{name}: {notes.Count} notes for pitch {pitchNumber}", this);

        isRunning = true;
    }

    public void Clear()
    {
        isRunning = false;
        notes.Clear();
        spawnedNotes.Clear();


        nextNoteToSpawnIndex = 0;
        nextNoteToHitIndex = 0;
    }


    void Update()
    {
        if (!isRunning || !sheetMusic.IsRunning) return;
        double now = sheetMusic.TuneTime;

        if (nextNoteToSpawnIndex < notes.Count &&
               notes[nextNoteToSpawnIndex].targetTimeInSeconds - now <= sheetMusic.approachTimeInSeconds)
        {
            SpawnNote(nextNoteToSpawnIndex);
            nextNoteToSpawnIndex++;
        }

        if (nextNoteToHitIndex < notes.Count 
            && PointsManager.CheckMiss(now, notes[nextNoteToHitIndex].targetTimeInSeconds))
        {
            nextNoteToHitIndex++;
        }
    }

    void SpawnNote(int index)
    {
        LaneNote laneNote = notes[index];

        NoteScript note = Instantiate(notePrefab, sheetMusic.transform);
        note.GetComponent<FallingItem>().Setup(sheetMusic, transform, laneNote.targetTimeInSeconds);

        float strength = Mathf.Lerp(1f - dynamicsMaxColorFade, 1f, laneNote.velocity);
        Color noteColor = Color.Lerp(paperColor, color, strength);
        noteColor.a = 1f;
        note.SetColor(noteColor);
        

        spawnedNotes.Add(note);

        Debug.Log($"Lane {name}: spawned note at {note.transform.localPosition}", note);
    }

    // The vr drum calls this
    public void RegisterHit()
    {
        Debug.Log($"Lane {name}: RegisterHit, running={isRunning}, sheet running={(sheetMusic != null && sheetMusic.IsRunning)}", this);

        if (!isRunning || !sheetMusic.IsRunning) return;

        double? nextNoteTime = nextNoteToHitIndex < notes.Count
            ? notes[nextNoteToHitIndex].targetTimeInSeconds
            : null;

        bool hitSucceeded = PointsManager.TryHit(sheetMusic.TuneTime, nextNoteTime);

        if (hitSucceeded)
        {
            NoteScript hitNote = spawnedNotes[nextNoteToHitIndex];
            if (hitNote != null) Destroy(hitNote.gameObject);

            nextNoteToHitIndex++;

        }
    }

}
