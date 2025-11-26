using UnityEngine;
using static GeneralDataManager;

public class GameManager : SingletonComponent<GameManager>
{
    #region Variables

    public GameObject noClickPanel;

    [Space(20)] [SerializeField] private GameObject launchScreen;
    [SerializeField] private GameObject homeScreen, gameplayScreen;

    #endregion

    #region Unity

    protected override void Awake() => ShowScreen(Screens.Launch);

    #endregion

    #region Screens

    internal void ShowScreen(Screens screen)
    {
        if (ActiveScreen == screen) return;
        if (ActiveScreenObj) ActiveScreenObj.SetActive(false);
        ActiveScreen = screen;

        switch (ActiveScreen)
        {
            case Screens.Home:
                ActiveScreenObj = homeScreen;
                break;
            case Screens.Launch:
                ActiveScreenObj = launchScreen;
                break;
            case Screens.GamePlay:
                ActiveScreenObj = gameplayScreen;
                break;
            default: return;
        }

        ActiveScreenObj.SetActive(true);
    }

    #endregion
}