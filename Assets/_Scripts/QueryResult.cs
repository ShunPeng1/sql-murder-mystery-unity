using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QueryResult : MonoBehaviour
{
    private static Vector2 AnchoredPosition = new Vector2(0, 190);
    private static Vector2 ScrollRectSize = new Vector2(1920, 700);

    [SerializeField] private ScrollRect scrollRect;
    public int Index { get; private set; }
    public string Query { get; private set; }

    public void Init(string query, List<List<string>> result, int index)
    {
        Query = query;
        Index = index;

        GetComponent<RectTransform>().anchoredPosition = AnchoredPosition;
        GetComponent<RectTransform>().sizeDelta = ScrollRectSize;

        float cellWidth = 100f;
        float cellHeight = 40f;

        float contentWidth = cellWidth * result[0].Count;
        float contentHeight = cellHeight * result.Count;
        scrollRect.content.sizeDelta = new Vector2(contentWidth, contentHeight);

        HistoryList.Instance.CreateHistoryItem(this);

        StartCoroutine(Init_CO());
        
        IEnumerator Init_CO()
        {
            for (var row = 0; row < result.Count; row++)
            {
                for (var col = 0; col < result[row].Count; col++)
                {
                    var cell = Instantiate(ResourceManager.Instance.Cell).GetComponent<Cell>();
                    cell.transform.SetParent(scrollRect.content);
                    var rectTransform = cell.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = new Vector3(col * cellWidth, row * -cellHeight, 0);
                    rectTransform.localScale = Vector3.one;
                    
                    cell.Init(result[row][col], cellWidth, cellHeight);
                }

                yield return null;
            }
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
        SQLQueryBox.Instance.SetText(Query);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}