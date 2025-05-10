using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GridSystem;
using Helpers;
using Interfaces;
using ScriptableObjects.Level;
using UnityEngine;

namespace Controllers
{
    public class TileLinkController : MonoBehaviour
    {
        private readonly List<ITappable> _currentLink = new List<ITappable>();
        public void TryAppendToCurrentLink(ITappable tappable)
        {
            if (LinkRules.CanLink(_currentLink, tappable, out bool isBacktracking))
            {
                if (isBacktracking)
                {
                    _currentLink.RemoveAt(_currentLink.Count - 1);
                }
                else
                {
                    _currentLink.Add(tappable);
                }
            }

            if(_currentLink.Count > 0)
                GameController.Instance.HighlightAdjacentTiles(_currentLink[^1] as BaseTile);
        }

        public bool IsLinkProcessable()
        {
            if (_currentLink.Count == 0) return false;
            if (_currentLink.Count < Utilities.LinkThreshold)
            {
                _currentLink[^1].OnRelease();
                _currentLink.Clear();
                return false;
            }
            
            return true;
        }

        public IEnumerator TriggerLinkProcess(System.Action<ChipType, int> onComplete)
        {
            ChipType chipType = ChipType.Circle;
            int count = 0;

            foreach (BaseTile tile in _currentLink.OfType<BaseTile>())
            {
                tile.OnLinked();
                chipType = tile.ChipType;
                count++;
                yield return new WaitForSeconds(0.07f);
            }

            _currentLink.Clear();
            yield return new WaitForSeconds(0.2f);

            onComplete?.Invoke(chipType, count);
        }

        public List<ITappable> GetCurrentLink()
        {
            return _currentLink;
        }
    }
}
