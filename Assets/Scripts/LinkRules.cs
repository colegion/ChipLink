using System.Collections.Generic;
using GridSystem;
using Interfaces;
using UnityEngine;

public static class LinkRules
{
    public static bool CanLink(List<ITappable> currentLink, ITappable newTappable)
    {
        if (currentLink.Count == 0)
            return true;

        if (currentLink.Contains(newTappable))
            return false;
        
        if (currentLink[^1] is not BaseTile lastTile || newTappable is not BaseTile newTile)
            return false;
        
        if (!HasSameChipType(lastTile, newTile))
            return false;
        
        Vector2Int delta = newTile.GetPosition() - lastTile.GetPosition();
        return IsAdjacent(delta);
    }

    private static bool HasSameChipType(BaseTile a, BaseTile b)
    {
        return a.ChipType == b.ChipType;
    }
    
    private static bool IsAdjacent(Vector2Int delta)
    {
        return Mathf.Abs(delta.x) <= 1 && Mathf.Abs(delta.y) <= 1 && (delta != Vector2Int.zero);
    }
}