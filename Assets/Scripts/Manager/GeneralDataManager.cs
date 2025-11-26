using UnityEngine;

public class GeneralDataManager : SingletonComponent<GeneralDataManager>
{
    #region Unity

    protected override void Awake()
    {
        Application.lowMemory += OnLowMemory;
    }

    private void OnDestroy()
    {
        Application.lowMemory -= OnLowMemory;
    }

    #endregion

    #region Private Methods

    private static void OnLowMemory()
    {
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }

    #endregion

    #region Enum

    public enum Screens
    {
        Launch,
        Home,
        GamePlay
    }

    internal static Screens ActiveScreen = Screens.GamePlay;
    internal static GameObject ActiveScreenObj;

    #endregion
}

#region Class

[System.Serializable]
public class CardData
{
    public int id;
    public string name;
    public int cost;
    public int power;
    public string color;
    public CardAbility ability;
}

[System.Serializable]
public class CardAbility
{
    public string type;
    public int value;
}

#endregion