using System;
using System.Collections.Generic;
using System.Linq;
using DatdevUlts;
using UnityEngine;
using UnityEngine.UI;

namespace PoolingScroll
{
    public class ListViewComputer : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _listPrefab = new List<GameObject>();

        private ScrollRect _scrollRect;
        private RectTransform _contentRectTransform;
        private HorizontalOrVerticalLayoutGroup _layoutGroup;
        private RectTransform _firstPadding;
        private RectTransform _lastPadding;
        private bool _inited;
        private float _cachedContentSize;
        private int _firstViewIndex;
        private int _lastViewIndex;
        private ListViewItem _firstViewItem;
        private ListViewItem _lastViewItem;

        private List<PoolObjectManager<ListViewItem>> _listPoolManager = new List<PoolObjectManager<ListViewItem>>();
        private Func<ListViewComputer, int, string> _funcFindPrefab;

        /// <summary>
        /// Initializes the view with a specified number of items and a function to find the prefab for each item.
        /// </summary>
        /// <param name="countItems">The total number of items to display in the view.</param>
        /// <param name="funcFindPrefab">A function that takes the ListViewComputer instance and an index, returning the corresponding ListViewItem prefab.</param>
        public void InitView(int countItems, Func<ListViewComputer, int, string> funcFindPrefab)
        {
            _inited = true;

            _funcFindPrefab = funcFindPrefab;

            for (int i = 0; i < _listPrefab.Count; i++)
            {
                var item = _listPrefab[i];
                item.gameObject.SetActive(false);
                if (!item.GetComponent<ListViewItem>())
                {
                    item.AddComponent<ListViewItem>();
                }

                _listPoolManager.Add(new PoolObjectManager<ListViewItem>());
            }

            _scrollRect = GetComponent<ScrollRect>();
            _contentRectTransform = _scrollRect.content;
            _layoutGroup = _contentRectTransform.GetComponent<HorizontalOrVerticalLayoutGroup>();

            _firstPadding = new GameObject().AddComponent<RectTransform>();
            _firstPadding.SetParent(_contentRectTransform);
            _lastPadding = new GameObject().AddComponent<RectTransform>();
            _lastPadding.SetParent(_contentRectTransform);
            
            float size = _layoutGroup.spacing * (countItems - 1);
            for (int i = 0; i < countItems; i++)
            {
                var sizeDelta = ((RectTransform)GetPoolListViewItemNonCreate(funcFindPrefab.Invoke(this, i)).transform)
                    .sizeDelta;
                size += GetSize(sizeDelta);
            }

            _lastPadding.sizeDelta = SetSizeDelta(_lastPadding.sizeDelta,size);
        }

        public ListViewItem GetPoolListViewItem(string nameObject)
        {
            var index = GetIndexPrefab(nameObject);
            if (index < 0)
            {
                return null;
            }

            var item = _listPoolManager[index].GetObject(() =>
                Instantiate(_listPrefab[index], _contentRectTransform).GetComponent<ListViewItem>());

            return item;
        }

        private void Update()
        {
            if (_inited)
            {
            }
        }

        public int GetIndexPrefab(string nameObject)
        {
            var prefab = _listPrefab.FirstOrDefault(obj => obj.name == nameObject);
            return _listPrefab.IndexOf(prefab);
        }

        private ListViewItem GetPoolListViewItemNonCreate(string nameObject)
        {
            var index = GetIndexPrefab(nameObject);
            if (index < 0)
            {
                return null;
            }

            return _listPrefab[index].GetComponent<ListViewItem>();
        }

        public float GetSize(Vector2 sizeXY)
        {
            if (_layoutGroup is HorizontalLayoutGroup)
            {
                return sizeXY.x;
            }

            return sizeXY.y;
        }

        public Vector2 SetSizeDelta(Vector2 sizeXY, float size)
        {
            if (_layoutGroup is HorizontalLayoutGroup)
            {
                sizeXY.x = size;
            }
            else
            {
                sizeXY.y = size;
            }

            return sizeXY;
        }
    }
}