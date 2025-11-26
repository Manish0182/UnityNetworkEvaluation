using Photon.Pun;
using ExitGames.Client.Photon;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using Photon.Realtime;

public static class PhotonJSON
{
    public const byte EventCode = 1;

    internal static void Send(object payload)
    {
        PhotonNetwork.RaiseEvent(EventCode, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload)),
            new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
    }

    internal static JObject Decode(EventData e)
    {
        if (e.Code != EventCode) return null;
        if (e.CustomData == null) return null;
        return e.CustomData is not byte[] bytes ? null : JObject.Parse(Encoding.UTF8.GetString(bytes));
    }
}