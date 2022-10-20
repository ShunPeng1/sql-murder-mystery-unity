using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QueryResult : MonoBehaviour
{
    private static Vector2 AnchoredPosition = new Vector2(0, 190);
    private static Vector2 ScrollRectSize = new Vector2(1920, 700);

    [SerializeField] private ScrollRect scrollRect;
    public int Index { get; private set; }
    public string Query { get; private set; }

    private Vector3 mainPos;

    public void Init(string query, List<List<string>> result, int index)
    {
        if (result.Count > 100) result = result.GetRange(0, 101);

        Query = query;
        Index = index;

        GetComponent<RectTransform>().anchoredPosition = AnchoredPosition;
        GetComponent<RectTransform>().sizeDelta = ScrollRectSize;
        
        List<List<Cell>> cells = new List<List<Cell>>();
        List<float> widths = Enumerable.Repeat(0f, result[0].Count).ToList();
        List<float> heights = Enumerable.Repeat(0f, result.Count).ToList();

        mainPos = transform.position;

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

        scrollRect.content.sizeDelta = new Vector2(widthSoFar, heightSoFar);
    }

    public void Show()
    {
        transform.position = mainPos;
        SQLQueryBox.Instance.SetText(Query);
    }

    public void Hide()
    {
        transform.position = Vector3.left * 10000;
    }
}