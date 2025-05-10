using System.Collections.Generic;
using GridSystem;
using Helpers;
using UnityEngine;
using Grid = GridSystem.Grid;

public class LinkSearcher
{
    private readonly Grid _grid = ServiceLocator.Get<Grid>();
    private readonly int _threshold = Utilities.LinkThreshold;

    public bool HasPossibleLink()
    {
        int w = _grid.Width, h = _grid.Height;
        bool[,] seen = new bool[w, h];

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                if (seen[x, y]) continue;

                var cell = _grid.GetCell(x, y);
                var tile = cell?.GetTile(Utilities.DefaultChipLayer) as BaseTile;
                if (tile == null) continue;

                if (FloodCount(x, y, tile.ChipType, seen) >= _threshold)
                    return true;
            }
        }
        return false;
    }

    public bool HasValidLinkAfterShuffle(List<Vector2Int> coords, List<BaseTile> tiles)
    {
        int w = _grid.Width, h = _grid.Height;
        ChipType?[][] map = new ChipType?[w][];
        for (int index = 0; index < w; index++)
        {
            map[index] = new ChipType?[h];
        }
        
        for (int i = 0; i < tiles.Count; i++)
        {
            map[coords[i].x][coords[i].y] = tiles[i].ChipType;
        }

        bool[,] seen = new bool[w, h];
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                if (seen[x, y] || !map[x][y].HasValue) continue;

                if (FloodCount(x, y, map[x][y].Value, seen) >= _threshold)
                    return true;
            }
        }

        return false;
    }

    private int FloodCount(int sx, int sy, ChipType type, bool[,] seen)
    {
        return FloodFill(sx, sy, (x, y) => 
        {
            var cell = _grid.GetCell(x, y);
            var tile = cell?.GetTile(Utilities.DefaultChipLayer) as BaseTile;
            return tile != null && tile.ChipType == type;
        }, seen);
    }

    private int FloodFill(int sx, int sy, System.Func<int, int, bool> isValidTile, bool[,] seen)
    {
        var stack = new Stack<Vector2Int>();
        stack.Push(new Vector2Int(sx, sy));
        int count = 0;

        while (stack.Count > 0)
        {
            var p = stack.Pop();
            if (p.x < 0 || p.x >= _grid.Width || p.y < 0 || p.y >= _grid.Height || seen[p.x, p.y])
                continue;

            if (!isValidTile(p.x, p.y)) continue;

            seen[p.x, p.y] = true;
            count++;

            foreach (var d in DirectionUtils.Directions.Values)
                stack.Push(p + d);
        }

        return count;
    }
}
