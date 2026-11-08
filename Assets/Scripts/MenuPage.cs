using System;
using System.Collections.Generic;
using UnityEngine;

public class MenuPage : MonoBehaviour
{
    // all playable tunes, in the order they appear on the menu
    public List<TuneData> tunes = new();

    // button prefab with a TuneButton component
    public PickTuneButton buttonPrefab;

    public Transform buttonContainer;

    public event Action<TuneData> TuneSelected;

    readonly List<PickTuneButton> buttons = new();

    void Awake()
    {
        foreach (var tune in tunes)
        {
            if (tune == null) continue;

            PickTuneButton button = Instantiate(buttonPrefab, buttonContainer);
            button.Setup(tune, selectedTune => TuneSelected?.Invoke(selectedTune));
            buttons.Add(button);
        }
    }

    public void SetInteractable(bool value)
    {
        foreach (var button in buttons) button.SetInteractable(value);
    }
}