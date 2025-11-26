using Photon.Pun;
using ExitGames.Client.Photon;
using Newtonsoft.Json.Linq;
using Photon.Realtime;

public class TurnManager : SingletonComponent<TurnManager>, IOnEventCallback
{
    #region Variables & Unity Methods

    private int _currentTurn;
    private const int MaxTurns = 6;

    private void OnEnable() => PhotonNetwork.AddCallbackTarget(this);

    private void OnDisable() => PhotonNetwork.RemoveCallbackTarget(this);

    #endregion

    #region Internal Methods

    internal void StartMatch()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        _currentTurn = 0;
        NextTurn();
    }

    internal void NextTurn()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        _currentTurn++;

        if (_currentTurn > MaxTurns)
        {
            PhotonJSON.Send(new { action = "gameOver" });
            return;
        }

        PhotonJSON.Send(new
        {
            action = "turnStart",
            turn = _currentTurn,
            cost = _currentTurn
        });
    }

    #endregion

    #region Events

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code != PhotonJSON.EventCode) return;

        var json = PhotonJSON.Decode(photonEvent);
        if (json == null) return;

        var action = json["action"].ToString();

        switch (action)
        {
            case "turnStart":
                var turn = json["turn"].Value<int>();
                var cost = json["cost"].Value<int>();
                _currentTurn = turn;
                GamePlayScreen.Inst.SetTurn(turn);
                GamePlayScreen.Inst.SetCost(cost);
                HandManager.Inst.OnTurnStart(cost);
                return;
            case "gameOver":
                GamePlayScreen.Inst.ShowGameOver();
                return;
        }
    }

    #endregion
}