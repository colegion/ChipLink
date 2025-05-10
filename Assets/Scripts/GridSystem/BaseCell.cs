using System.Collections.Generic;
using Helpers;
using UnityEngine;

namespace GridSystem
{
    public class BaseCell : MonoBehaviour
    {
        [SerializeField] private Transform target;
        private int _x;
        private int _y;

        public int X => _x;
        public int Y => _y;

        private Dictionary<int, BaseTile> _tiles = new Dictionary<int, BaseTile>();
        private Grid _grid;

        public void ConfigureSelf(int x, int y)
        {
            _x = x;
            _y = y;
            name = $"{name}{x}_{y}";
            _grid = ServiceLocator.Get<Grid>();
            _grid.PlaceCell(this);
        }
    
        public void SetTile(BaseTile baseTile)
        {
            _tiles[baseTile.Layer] = baseTile;
        }
    
        public void SetTileNull(int layer)
        {
            if (!_tiles.ContainsKey(layer)) return;
            
            var tile = _tiles.GetValueOrDefault(layer);
            if (tile != null)
            {
                _tiles[layer] = null;
            }
        }

        public bool IsTileAvailableForLayer(int layer)
        {
            if (!_tiles.ContainsKey(layer)) return true;
            return _tiles[layer] == null;
        }

        public bool IsTilesAllFree()
        {
            foreach (var tile in _tiles.Values)
            {
                if (tile != null)
                {
                    return false;
                }
            }
            return true;
        }
    
        public BaseTile GetTile(int layer)
        {
            return _tiles.GetValueOrDefault(layer);
        }

        public Transform GetTarget()
        {
            return target;
        }
    
        public Vector3 GetWorldPosition()
        {
            return target != null ? target.position : transform.position + (Vector3.up * 0.25f);
        }
    }
}