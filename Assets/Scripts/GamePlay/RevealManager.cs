public class RevealManager : SingletonComponent<RevealManager>
{
    private int[] _localIds, _oppIds;

    internal void SetOpponentPlayedIds(int[] ids) => _oppIds = ids;

    internal void StartReveal(int[] local)
    {
        _localIds = local;
        GamePlayScreen.Inst.ShowPlays(_localIds, _oppIds);
        AbilityResolver.ResolveAndApply(_localIds, _oppIds, OnResolved);
    }

    private void OnResolved()
    {
        HandManager.Inst.RemovePlayedCards(_localIds);
        TurnManager.Inst.NextTurn();
    }
}