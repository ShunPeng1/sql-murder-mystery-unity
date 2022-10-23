using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class HistoryList : PersistentSingleton<HistoryList>
{
    [SerializeField] private ScrollRect scrollRect;
    private List<HistoryItem> historyItems = new List<HistoryItem>();
    private float actualWidth;
    public bool JustCreated;

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
            rect.DOMoveY(itemRect.rect.height / 100f, 0.5f).SetRelative(true); // Magic number
        }

        historyItems.Add(historyItem);

        if (historyItems.Count > 10)
        {
            historyItems[0].Destroy();
            historyItems.RemoveAt(0);
        }

        StartCoroutine(JustCreatedExpire_CO());
    }
    
    private IEnumerator JustCreatedExpire_CO()
    {
        JustCreated = true;
        yield return new WaitForSeconds(1f);
        JustCreated = false;
    }
}