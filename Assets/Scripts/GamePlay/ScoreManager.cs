using Photon.Pun;
using ExitGames.Client.Photon;
using Newtonsoft.Json.Linq;
using Photon.Realtime;

public class ScoreManager : SingletonComponent<ScoreManager>, IOnEventCallback
{
    private int _myScore, _enemyScore;

    private void OnEnable() => PhotonNetwork.AddCallbackTarget(this);

    private void OnDisable() => PhotonNetwork.RemoveCallbackTarget(this);

    internal int GetMyScore() => _myScore;

    internal int GetEnemyScore() => _enemyScore;

    internal void ApplyFinalTurnScore(int myPower, int enemyPower)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        _myScore += myPower;
        _enemyScore += enemyPower;

        PhotonJSON.Send(new
        {
            action = "scoreUpdate",
            master_my = _myScore,
            master_enemy = _enemyScore
        });
    }

    public void OnEvent(EventData eventData)
    {
        if (eventData.Code != PhotonJSON.EventCode) return;

        var json = PhotonJSON.Decode(eventData);
        if (json == null) return;
        if (json["action"].ToString() != "scoreUpdate") return;

        var mMy = json["master_my"].Value<int>();
        var mEnemy = json["master_enemy"].Value<int>();

        if (PhotonNetwork.IsMasterClient)
        {
            _myScore = mMy;
            _enemyScore = mEnemy;
        }
        else
        {
            _myScore = mEnemy;
            _enemyScore = mMy;
        }

        GamePlayScreen.Inst.SetScores(_myScore, _enemyScore);
    }
}