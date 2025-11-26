using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using ExitGames.Client.Photon;
using Newtonsoft.Json.Linq;
using Photon.Realtime;

public class HandManager : SingletonComponent<HandManager>, IOnEventCallback
{
    #region Variables

    [SerializeField] private RectTransform handContainer;
    [SerializeField] private GameObject cardPrefab;

    private readonly List<CardData> _hand = new();
    private readonly List<CardController> _uiCards = new();
    private readonly List<int> _selectedIds = new();
    private bool _localEnded, _remoteEnded;
    private int _availableCost = 1;

    #endregion

    #region Unity

    private void OnEnable() => PhotonNetwork.AddCallbackTarget(this);

    private void OnDisable() => PhotonNetwork.RemoveCallbackTarget(this);

    #endregion

    #region Internal Methods

    internal void InitializeInitialHand(List<CardData> cards)
    {
        ClearHandUI();
        _hand.Clear();
        _selectedIds.Clear();
        foreach (var c in cards) AddCard(c);
    }

    internal void OnTurnStart(int cost)
    {
        _availableCost = cost;
        _localEnded = false;
        _remoteEnded = false;
        var drawn = DeckManager.Inst.DrawOne();
        if (drawn != null) AddCard(drawn);
        foreach (var ui in _uiCards) ui.Enable();
    }

    internal void EndTurn()
    {
        if (_localEnded) return;

        _localEnded = true;
        foreach (var ui in _uiCards) ui.Disable();

        PhotonJSON.Send(new
        {
            action = "endTurn",
            player = PhotonNetwork.LocalPlayer.ActorNumber,
            cards = _selectedIds.ToArray()
        });

        TryReveal();
    }

    internal void RemovePlayedCards(int[] played)
    {
        foreach (var id in played)
        {
            var ui = _uiCards.Find(x => x.GetData().id == id);
            if (ui != null)
            {
                _uiCards.Remove(ui);
                Destroy(ui.gameObject);
            }

            _hand.RemoveAll(x => x.id == id);
        }

        _selectedIds.Clear();
    }

    internal void DrawExtraLocal(int amount)
    {
        for (var i = 0; i < amount; i++)
        {
            var c = DeckManager.Inst.DrawOne();
            if (c != null) AddCard(c);
        }
    }

    internal void DiscardRandomLocal(int amount)
    {
        for (var i = 0; i < amount; i++)
        {
            if (_hand.Count == 0) return;

            var index = Random.Range(0, _hand.Count);
            var card = _hand[index];

            var ui = _uiCards.Find(x => x.GetData().id == card.id);
            if (ui != null)
            {
                _uiCards.Remove(ui);
                Destroy(ui.gameObject);
            }

            _hand.RemoveAt(index);
        }
    }

    internal int[] GetLocalSelectedIds() => _selectedIds.ToArray();

    #endregion

    #region Private Methods

    private void ClearHandUI()
    {
        foreach (Transform t in handContainer) Destroy(t.gameObject);
        _uiCards.Clear();
    }

    private void AddCard(CardData c)
    {
        _hand.Add(c);
        var card = Instantiate(cardPrefab, handContainer).GetComponent<CardController>();
        card.SetThis(c);
        card.OnSelected = OnCardClick;
        _uiCards.Add(card);
    }

    private void OnCardClick(CardController ui)
    {
        if (_localEnded) return;

        var data = ui.GetData();
        var id = data.id;
        var isSelected = _selectedIds.Contains(id);

        var selectedCost = _selectedIds.Sum(x => CardDatabase.Inst.GetCardById(x).cost);

        if (!isSelected)
        {
            if (_selectedIds.Count == 0)
            {
                if (data.cost > _availableCost) return;
                _selectedIds.Add(id);
                ui.SelectHighlight(true);
                return;
            }

            if (selectedCost + data.cost > _availableCost)
            {
                Debug.Log("COST LIMIT REACHED");
                return;
            }

            _selectedIds.Add(id);
            ui.SelectHighlight(true);
            return;
        }

        _selectedIds.Remove(id);
        ui.SelectHighlight(false);
    }

    private void TryReveal()
    {
        if (!_localEnded || !_remoteEnded) return;
        PhotonJSON.Send(new { action = "reveal" });
        RevealManager.Inst.StartReveal(_selectedIds.ToArray());
    }

    #endregion

    #region Event

    public void OnEvent(EventData e)
    {
        if (e.Code != PhotonJSON.EventCode) return;

        var json = PhotonJSON.Decode(e);
        if (json == null) return;

        var action = json["action"].ToString();

        switch (action)
        {
            case "turnStart":
                OnTurnStart(json["cost"].Value<int>());
                return;
            case "endTurn":
                var sender = json["player"].Value<int>();

                if (sender != PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    _remoteEnded = true;
                    int[] cards = json["cards"].ToObject<int[]>();
                    RevealManager.Inst.SetOpponentPlayedIds(cards);
                }

                TryReveal();
                return;
        }
    }

    #endregion
}