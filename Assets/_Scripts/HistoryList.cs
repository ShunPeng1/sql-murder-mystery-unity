using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


public class HistoryList : PersistentSingleton<HistoryList>
{
    [SerializeField] private ScrollRect scrollRect;
    private List<HistoryItem> historyItems = new List<HistoryItem>();

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
    }
}