using UnityEngine;

public class HomeScreen : MonoBehaviour
{
    [SerializeField] private GameObject loader;

    private void OnEnable() => loader.SetActive(false);

    private void OnDisable() => loader.SetActive(false);

    public void Play_Btn_Click()
    {
        PhotonConnector.Inst.ConnectToPhoton();
        loader.SetActive(true);
    }
}