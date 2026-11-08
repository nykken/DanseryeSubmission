using UnityEngine;

public class MenuState : GameState
{
    public MenuPage menuPage;
    public LoadingState loadingState;

    void OnEnable()
    {
        menuPage.gameObject.SetActive(true);
        menuPage.SetInteractable(true);
        menuPage.TuneSelected += OnTuneSelected;
    }

    void OnDisable()
    {
        if (menuPage == null) return;
        menuPage.TuneSelected -= OnTuneSelected;
        menuPage.gameObject.SetActive(false);
    }

    void OnTuneSelected(TuneData tune)
    {
        loadingState.Tune = tune;
        SwitchState(loadingState);
    }
}
