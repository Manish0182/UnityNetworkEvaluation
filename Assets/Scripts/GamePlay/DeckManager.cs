using UnityEngine;
using System.Collections.Generic;

public class DeckManager : SingletonComponent<DeckManager>
{
    private List<CardData> _deck = new();

    internal void InitDeck()
    {
        _deck.Clear();
        _deck.AddRange(CardDatabase.Inst.AllCards);

        for (var i = 0; i < _deck.Count; i++)
        {
            var r = Random.Range(0, _deck.Count);
            (_deck[i], _deck[r]) = (_deck[r], _deck[i]);
        }

        _deck = _deck.GetRange(0, 12);
    }

    internal CardData DrawOne()
    {
        if (_deck.Count == 0) return null;
        var c = _deck[0];
        _deck.RemoveAt(0);
        return c;
    }
}