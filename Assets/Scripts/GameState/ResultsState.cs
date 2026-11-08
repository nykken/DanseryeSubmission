public class ResultsState : GameState
{
    public ResultsPage resultsPage;
    public PlayingState playingState;
    public MenuState menuState;
    public TuneData Tune { get; set; }
    public Results Results { get; set; }

    void OnEnable()
    {
        resultsPage.gameObject.SetActive(true);
        resultsPage.Show(Tune, Results);
        resultsPage.ReplayPressed += OnReplay;
        resultsPage.MenuPressed += OnMenu;
    }

    void OnDisable()
    {
        if (resultsPage == null) return;
        resultsPage.ReplayPressed -= OnReplay;
        resultsPage.MenuPressed -= OnMenu;
        resultsPage.gameObject.SetActive(false);
    }

    void OnReplay() => SwitchState(playingState);

    void OnMenu() => SwitchState(menuState);
}