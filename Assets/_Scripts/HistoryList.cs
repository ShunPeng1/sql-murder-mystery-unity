using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


public class HistoryList : PersistentSingleton<HistoryList>
{
    [SerializeField] private ScrollRect scrollRect;
    private List<HistoryItem> historyItems = new List<HistoryItem>();
    private float actualWidth;
    private float initialY;

    private void Start()
    {
        initialY = GetComponent<RectTransform>().anchoredPosition.y;
    }

    public void CreateHistoryItem(QueryResult queryResult)
    {
        var historyItem = Instantiate(ResourceManager.Instance.HistoryItem).GetComponent<HistoryItem>();
        historyItem.Init(queryResult);

        historyItem.transform.SetParent(scrollRect.content);

        var itemRect = historyItem.GetComponent<RectTransform>();
        itemRect.anchoredPosition = Vector2.zero;
        itemRect.localScale = Vector3.one;

        foreach (var item in historyItems)
        {
            var rect = item.GetComponent<RectTransform>();
            rect.DOMoveX(-itemRect.rect.width / 100f, 0.5f).SetRelative(true); // Magic number
        }

        historyItems.Add(historyItem);
        // actualWidth += itemRect.sizeDelta.x;
        // if (actualWidth > 1920)
        // {
        //     var size = new Vector2(actualWidth, scrollRect.content.sizeDelta.y);
        //     scrollRect.content.anchoredPosition = new Vector2(size.x, initialY);
        //     scrollRect.content.sizeDelta = size;
        // }
    }
}