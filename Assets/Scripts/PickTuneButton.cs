using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PickTuneButton : MonoBehaviour
{
    public TMP_Text titleText;

    Button button;

    public void Setup(TuneData tune, Action<TuneData> onPressed)
    {
        button = GetComponent<Button>();

        titleText.text = $"{tune.title} ({tune.author})";

        button.onClick.AddListener(() => onPressed(tune));
    }

    public void SetInteractable(bool value) => button.interactable = value;
}