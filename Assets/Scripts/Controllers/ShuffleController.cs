using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GridSystem;
using Helpers;
using Interfaces;
using UnityEngine;
using Grid = GridSystem.Grid;

namespace Controllers
{
    public class ShuffleController : MonoBehaviour, IInjectable
    {
        private Grid _grid;
        private LinkSearcher _linkSearcher;
        private readonly int linkThreshold = Utilities.LinkThreshold;
    
        public void InjectDependencies()
        {
            _grid = ServiceLocator.Get<Grid>();
        }

        public void TriggerShuffle()
        {
            StartCoroutine(ShuffleBoard());
        }

        private IEnumerator ShuffleBoard()
        {
            if(_linkSearcher == null)
                _linkSearcher = ServiceLocator.Get<LinkSearcher>();
        
            var tiles = new List<BaseTile>();
            var coords = new List<Vector2Int>();

            for (int x = 0; x < _grid.Width; x++)
            {
                for (int y = 0; y < _grid.Height; y++)
                {
                    var cell = _grid.GetCell(x, y);
                    if (cell != null)
                    {
                        var tile = cell.GetTile(Utilities.DefaultChipLayer);
                        if (tile != null)
                        {
                            tiles.Add(tile);
                            coords.Add(new Vector2Int(x, y));
                        }
                    }
                }
            }

            if (tiles.Count != coords.Count)
            {
                Debug.LogError("Mismatch between tile count and valid cells!");
                yield break;
            }
        
            Vector3 center = GetBoardCenterWorldPos();
            foreach (var tile in tiles)
            {
                var offset = new Vector3(Random.Range(-0.3f, 0.3f), 0, Random.Range(-0.3f, 0.3f));
                tile.transform.DOMove(center + offset, 0.4f).SetEase(Ease.InOutQuad);
            }
            yield return new WaitForSeconds(0.45f);
        
            do
            {
                coords.Shuffle();
            }
            while (!_linkSearcher.HasValidLinkAfterShuffle(coords, tiles)); 
        
            for (int i = 0; i < tiles.Count; i++)
            {
                var tile = tiles[i];
                var newPos = coords[i];
            
                tile.UpdatePosition(newPos);

                var cell = _grid.GetCell(newPos.x, newPos.y);
                cell.SetTile(tile);

                var worldTarget = cell.GetWorldPosition();
                tile.transform.DOMove(worldTarget, 0.5f).SetEase(Ease.OutBack);
            }

            yield return new WaitForSeconds(0.55f);
        }
    
        private Vector3 GetBoardCenterWorldPos()
        {
            float cx = (_grid.Width - 1) / 2f;
            float cy = (_grid.Height - 1) / 2f;
            return _grid.GetCell(Mathf.FloorToInt(cx), Mathf.FloorToInt(cy)).GetWorldPosition();
        }

        private bool WouldHaveLinkAfter(List<Vector2Int> coords, List<BaseTile> tiles)
        {
            int w = _grid.Width, h = _grid.Height;
            ChipType?[,] map = new ChipType?[w, h];
            for (int i = 0; i < tiles.Count; i++)
                map[coords[i].x, coords[i].y] = tiles[i].ChipType;

            bool[,] seen = new bool[w, h];
            for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
            {
                if (seen[x, y] || !map[x, y].HasValue) continue;
                if (FloodCountTemp(x, y, map, seen) >= linkThreshold) return true;
            }
            return false;
        }

        private int FloodCountTemp(int sx, int sy, ChipType?[,] map, bool[,] seen)
        {
            var stack = new Stack<Vector2Int>();
            stack.Push(new Vector2Int(sx, sy));
            int cnt = 0;
            ChipType type = map[sx, sy].Value;

            while (stack.Count > 0)
            {
                var p = stack.Pop();
                if (p.x < 0 || p.x >= map.GetLength(0) || p.y < 0 || p.y >= map.GetLength(1)) continue;
                if (seen[p.x, p.y] || map[p.x, p.y] != type) continue;

                seen[p.x, p.y] = true;
                cnt++;
                foreach (var d in DirectionUtils.Directions.Values)
                    stack.Push(p + d);
            }
            return cnt;
        }
    }
}
