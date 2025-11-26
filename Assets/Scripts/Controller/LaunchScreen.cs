using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LaunchScreen : MonoBehaviour
{
    [SerializeField] private Slider launchSlider;

    private void Awake()
    {
        launchSlider.DOValue(100, 6f).SetEase(Ease.Linear).OnComplete(() =>
        {
            GameManager.Inst.ShowScreen(GeneralDataManager.Screens.Home);
        });
    }
}