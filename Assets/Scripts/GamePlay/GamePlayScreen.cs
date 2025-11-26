using UnityEngine;
using TMPro;

public class GamePlayScreen : SingletonComponent<GamePlayScreen>
{
    #region Variables

    [SerializeField] private TextMeshProUGUI turnTxt, costTxt, myScoreTxt, oppScoreTxt, gameOverText;
    [SerializeField] private RectTransform myPlayedParent, oppPlayedParent;
    [SerializeField] private GameObject cardPrefab, gameOverPanel;

    #endregion

    #region Internal Methods

    internal void SetTurn(int t) => turnTxt.text = "TURN " + t;

    internal void SetCost(int cost) => costTxt.text = "COST: " + cost;

    internal void SetScores(int my, int opp)
    {
        myScoreTxt.text = my.ToString();
        oppScoreTxt.text = opp.ToString();
    }

    internal void ShowPlays(int[] myIds, int[] oppIds)
    {
        ClearPlayedArea(myPlayedParent);
        ClearPlayedArea(oppPlayedParent);

        foreach (var id in myIds)
        {
            var card = Instantiate(cardPrefab, myPlayedParent).GetComponent<CardController>();
            card.SetThis(CardDatabase.Inst.GetCardById(id));
            card.Disable();
        }

        foreach (var id in oppIds)
        {
            var card = Instantiate(cardPrefab, oppPlayedParent).GetComponent<CardController>();
            card.SetThis(CardDatabase.Inst.GetCardById(id));
            card.Disable();
        }
    }

    internal void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        var my = ScoreManager.Inst.GetMyScore();
        var opp = ScoreManager.Inst.GetEnemyScore();

        if (my > opp) gameOverText.text = "YOU WIN!";
        else if (my < opp) gameOverText.text = "YOU LOSE!";
        else gameOverText.text = "DRAW!";
    }

    #endregion

    #region Buttons

    public void PlayCard_Btn_Click()
    {
        ClearPlayedArea(myPlayedParent);

        foreach (var id in HandManager.Inst.GetLocalSelectedIds())
        {
            var card = Instantiate(cardPrefab, myPlayedParent).GetComponent<CardController>();
            card.SetThis(CardDatabase.Inst.GetCardById(id));
            card.Disable();
        }
    }

    public void EndTurn_Btn_Click() => HandManager.Inst.EndTurn();

    #endregion

    #region Private Methods

    private static void ClearPlayedArea(RectTransform parent)
    {
        foreach (Transform t in parent)
            Destroy(t.gameObject);
    }

    #endregion
}