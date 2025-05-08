using Helpers;
using Interfaces;
using ScriptableObjects.Chip;
using UnityEngine;

namespace GridSystem
{
    public class BaseTile : MonoBehaviour, ITappable, IPoolable
    {
        [SerializeField] private Collider tileCollider;
        [SerializeField] protected TileView tileView;
    
        protected int _x;
        protected int _y;
        protected int _layer;
        protected ChipType _chipType;
    
        public int X => _x;
        public int Y => _y;
        public int Layer => _layer;
        public ChipType ChipType => _chipType;

        protected Grid Grid;
        
        protected Vector2Int _position;
    
        public virtual void ConfigureSelf(ChipConfig config, int x, int y)
        {
            _x = x;
            _y = y;
            _position = new Vector2Int(x, y);
            _chipType = config.chipType;
            tileView.SetSprite(config.chipSprite);
            SetTransform();

            if(Grid == null) Grid = ServiceLocator.Get<Grid>();
            Grid.PlaceTileToParentCell(this);
        }
    
        public void OnTap()
        {
            tileView.AnimateOnHighlight(true);
        }

        public void OnRelease()
        {
            tileView.AnimateOnHighlight(false);
        }

        public void OnLinked()
        {
            tileView.Disappear(() =>
            {
                GameController.Instance.ReturnPooledObject(this);
            });
        }

        public void UpdatePosition(Vector2Int position)
        {
            SetPosition(position);
            tileView.MoveTowardsTarget(Grid.GetCell(_x, _y).GetTarget(), SetTransform);
        }

        public void SetLayer(int layer)
        {
            _layer = layer;
        }

        public void SetTransform()
        {
            if (Grid == null) Grid = ServiceLocator.Get<Grid>();

            BaseCell cell = Grid.GetCell(_x, _y);
            if (cell != null)
            {
                cell.SetTile(this);
                transform.position = cell.GetWorldPosition();
            }
            else
            {
                Debug.LogWarning($"Cell at {_x}, {_y} not found! Using fallback position.");
                transform.position = new Vector3(_x, .25f, _y);
            }
        }

        private void SetPosition(Vector2Int position)
        {
            _position = position;
            _x = _position.x;
            _y = _position.y;
        }

        protected virtual void ResetSelf()
        {
            Grid.ClearTileOfParentCell(this);
            tileView.ResetSelf();
            tileView.ToggleVisuals(false);
            _position = Vector2Int.zero;
        }

        public Vector2Int GetPosition()
        {
            return _position;
        }

        public void OnPooled()
        {
            tileView.ToggleVisuals(false);
        }

        public void OnFetchFromPool()
        {
            tileView.ToggleVisuals(true);
        }

        public void OnReturnPool()
        {
            ResetSelf();
        }

        public PoolableTypes GetPoolableType()
        {
            return PoolableTypes.BaseTile;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }
    }
}
