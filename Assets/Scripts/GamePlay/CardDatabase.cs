using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;

public class CardDatabase : SingletonComponent<CardDatabase>
{
    internal List<CardData> AllCards = new();

    protected override void Awake()
    {
        AllCards = JsonConvert.DeserializeObject<List<CardData>>(Resources.Load<TextAsset>("cards").text);
    }

    internal CardData GetCardById(int id) => AllCards.Find(c => c.id == id);
}