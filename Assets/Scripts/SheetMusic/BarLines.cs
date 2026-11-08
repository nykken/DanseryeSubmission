using System.Collections.Generic;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using UnityEngine;

public class BarLines : MonoBehaviour
{
    public FallingItem barLinePrefab;

    readonly List<double> barTimestampsInSeconds = new();

    SheetMusic sheetMusic;
    int nextLineToSpawnIndex;
    bool isRunning;

    public void SetBars(SheetMusic sheet, MidiFile midi)
    {
        Clear();
        sheetMusic = sheet;

        var tempoMap = midi.GetTempoMap();

        long nrOfBars = midi.GetDuration<BarBeatTicksTimeSpan>().Bars;

        for (long bar = 0; bar <= nrOfBars; bar++)
        {
            var barStart = TimeConverter.ConvertTo<MetricTimeSpan>(new BarBeatTicksTimeSpan(bar, 0, 0), tempoMap);
            barTimestampsInSeconds.Add(barStart.TotalMicroseconds / 1_000_000.0);
        }

        isRunning = true;
    }

    public void Clear()
    {
        isRunning = false;
        barTimestampsInSeconds.Clear();
        nextLineToSpawnIndex = 0;
    }

    void Update()
    {
        if (!isRunning || !sheetMusic.IsRunning) return;
        double now = sheetMusic.TuneTime;

        while (
            nextLineToSpawnIndex < barTimestampsInSeconds.Count 
            && barTimestampsInSeconds[nextLineToSpawnIndex] - now <= sheetMusic.approachTimeInSeconds
            )
        {
            FallingItem barLine = Instantiate(barLinePrefab, sheetMusic.transform);
            barLine.Setup(sheetMusic, transform, barTimestampsInSeconds[nextLineToSpawnIndex]);
            nextLineToSpawnIndex++;
        }

    }

}
