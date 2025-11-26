using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText, costText, powerText;
    [SerializeField] private Image background;
    [SerializeField] private Outline outlineObj;
    [SerializeField] private Button button;

    public System.Action<CardController> OnSelected;

    private CardData _data;

    internal void SetThis(CardData card)
    {
        _data = card;
        nameText.text = card.name;
        costText.text = card.cost.ToString();
        powerText.text = card.power.ToString();
        if (ColorUtility.TryParseHtmlString(card.color, out var c)) background.color = c;
        outlineObj.enabled = false;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => OnSelected?.Invoke(this));
    }

    internal void SelectHighlight(bool on) => outlineObj.enabled = on;

    internal CardData GetData() => _data;

    internal void Disable() => button.interactable = false;

    internal void Enable() => button.interactable = true;
}