using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Shapes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class QueryResult : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Rectangle innerRectangle;
    [SerializeField] private Rectangle outerRectangle;

    private Vector2 maxSize;

    [SerializeField] private float margin;
    public RectTransform TopPoint;
    public RectTransform BottomPoint;

    public int Index { get; private set; }
    public string Query { get; private set; }

    private Vector3 mainPos;

    public void Init(string query, List<List<string>> result, int index)
    {
        if (result.Count > 100)
        {
            result = result.GetRange(0, 101);
            result.Add(Enumerable.Repeat("...", result[0].Count).ToList());
        }

        Query = query;
        Index = index;

        var rectTransform = GetComponent<RectTransform>();
        rectTransform.SetParent(ResourceManager.Instance.QueryResultRect.transform);
        maxSize = ResourceManager.Instance.QueryResultRect.GetComponent<RectTransform>().sizeDelta;

        List<List<Cell>> cells = new List<List<Cell>>();
        List<float> widths = Enumerable.Repeat(0f, result[0].Count).ToList();
        List<float> heights = Enumerable.Repeat(0f, result.Count).ToList();

        for (var row = 0; row < result.Count; row++)
        {
            cells.Add(new List<Cell>());
            for (var col = 0; col < result[row].Count; col++)
            {
                var cell = Instantiate(ResourceManager.Instance.Cell).GetComponent<Cell>();
                cell.transform.SetParent(scrollRect.content);
                var size = cell.InitText(result[row][col]);
                cells[row].Add(cell);
                widths[col] = Mathf.Max(widths[col], size.x);
                heights[row] = Mathf.Max(heights[row], size.y);
            }
        }

        float widthSoFar = 0f;
        float heightSoFar = 0f;
        for (var row = 0; row < result.Count; row++)
        {
            widthSoFar = 0f;

            for (var col = 0; col < result[row].Count; col++)
            {
                cells[row][col].RectTransform.anchoredPosition = new Vector2(widthSoFar, -heightSoFar);
                cells[row][col].RectTransform.localScale = Vector3.one;
                cells[row][col].InitSize(widths[col], heights[row]);
                widthSoFar += widths[col] + VisualManager.Instance.Padding * 2;
            }

            heightSoFar += heights[row] + VisualManager.Instance.Padding * 2;
        }

        var outerRectangleSize = new Vector2(Mathf.Min(widthSoFar, maxSize.x) + margin,
            Mathf.Min(heightSoFar, maxSize.y));

        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = outerRectangleSize;
        mainPos = transform.position;

        scrollRect.content.sizeDelta = new Vector2(widthSoFar, heightSoFar);
        scrollRect.GetComponent<RectTransform>().sizeDelta = new Vector2(-margin, -margin);
        outerRectangle.Width = outerRectangleSize.x;
        outerRectangle.Height = outerRectangleSize.y;

        innerRectangle.Width = outerRectangleSize.x - margin;
        innerRectangle.Height = outerRectangleSize.y - margin;
    }

    public void Show()
    {
        transform.position = mainPos;
        //SQLQueryBox.Instance.SetText(Query);
    }

    public void Hide()
    {
        transform.position = Vector3.left * 10000;
    }
}