using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;

public class AbilityResolver : SingletonComponent<AbilityResolver>
{
    internal static void ResolveAndApply(int[] localIds, int[] oppIds, System.Action onComplete)
    {
        var local = IdsToCards(localIds);
        var opp = IdsToCards(oppIds);

        DestroyCards(local, opp);
        DoublePower(local, opp);
        GainPoints(local, opp);
        StealPoints(local, opp);
        DrawExtra(local, opp);
        DiscardRandom(local, opp);

        if (PhotonNetwork.IsMasterClient)
        {
            var myPower = local.Sum(x => x.power);
            var enemyPower = opp.Sum(x => x.power);
            ScoreManager.Inst.ApplyFinalTurnScore(myPower, enemyPower);
        }

        onComplete?.Invoke();
    }

    private static List<CardData> IdsToCards(int[] ids) =>
        ids.Select(id => CardDatabase.Inst.GetCardById(id)).ToList();

    private static void DestroyCards(List<CardData> local, List<CardData> opp)
    {
        foreach (var c in local.Where(x => x.ability?.type == "DestroyOpponentCardInPlay"))
        {
            for (var i = 0; i < c.ability.value && opp.Count > 0; i++)
                opp.RemoveAt(0);
        }

        foreach (var c in opp.Where(x => x.ability?.type == "DestroyOpponentCardInPlay"))
        {
            for (var i = 0; i < c.ability.value && local.Count > 0; i++)
                local.RemoveAt(0);
        }
    }

    private static void DoublePower(List<CardData> local, List<CardData> opp)
    {
        foreach (var c in local.Where(x => x.ability?.type == "DoublePower"))
            c.power *= c.ability.value;

        foreach (var c in opp.Where(x => x.ability?.type == "DoublePower"))
            c.power *= c.ability.value;
    }

    private static void GainPoints(List<CardData> local, List<CardData> opp)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        foreach (var c in local.Where(x => x.ability?.type == "GainPoints"))
            ScoreManager.Inst.ApplyFinalTurnScore(c.ability.value, 0);

        foreach (var c in opp.Where(x => x.ability?.type == "GainPoints"))
            ScoreManager.Inst.ApplyFinalTurnScore(0, c.ability.value);
    }

    private static void StealPoints(List<CardData> local, List<CardData> opp)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        foreach (var c in local.Where(x => x.ability?.type == "StealPoints"))
            ScoreManager.Inst.ApplyFinalTurnScore(c.ability.value, -c.ability.value);

        foreach (var c in opp.Where(x => x.ability?.type == "StealPoints"))
            ScoreManager.Inst.ApplyFinalTurnScore(-c.ability.value, c.ability.value);
    }

    private static void DrawExtra(List<CardData> local, List<CardData> opp)
    {
        foreach (var c in local.Where(x => x.ability?.type == "DrawExtraCard"))
            HandManager.Inst.DrawExtraLocal(c.ability.value);

        foreach (var c in opp.Where(x => x.ability?.type == "DrawExtraCard"))
            HandManager.Inst.DrawExtraLocal(c.ability.value);
    }

    private static void DiscardRandom(List<CardData> local, List<CardData> opp)
    {
        var seed = local.Sum(x => x.id) + opp.Sum(x => x.id);
        Random.InitState(seed);
        foreach (var c in opp.Where(x => x.ability?.type == "DiscardOpponentRandomCard"))
            HandManager.Inst.DiscardRandomLocal(c.ability.value);
    }
}