using UnityEngine;

[CreateAssetMenu(fileName = "TuneData", menuName = "Danserye/Tune")]
public class TuneData : ScriptableObject
{
    public string title;
    public string author;

    public AudioClip clip;

    public float audioOffset;

    public string midiFile;

    // needed because the midi files exported by musescore have wrong bpm values
    public float quarterNotePerMinutes;

}
