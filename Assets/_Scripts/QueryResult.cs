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

    private List<List<Cell>> cells = new List<List<Cell>>();

    [SerializeField] private float margin;
    public RectTransform TopPoint;
    public RectTransform BottomPoint;
    private Vector2 maxSize;

    public int Index { get; private set; }
    public string Query { get; private set; }

    public static bool IsCurrentResultDoneInit = true;

    public void Init(string query, List<List<string>> result, int index)
    {
        IsCurrentResultDoneInit = false;
        StartCoroutine(Init_CO(query, result, index));
    }

    private IEnumerator Init_CO(string query, List<List<string>> result, int index)
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

        List<float> widths = Enumerable.Repeat(0f, result[0].Count).ToList();
        List<float> heights = Enumerable.Repeat(0f, result.Count).ToList();

        float extendWidth = result[0].Count switch
        {
            1 => 2000f,
            2 => 1500f,
            3 => 100f,
            4 => 700f,
            5 => 500f,
            _ => -1
        };

        for (var row = 0; row < result.Count; row++)
        {
            cells.Add(new List<Cell>());
            for (var col = 0; col < result[row].Count; col++)
            {
                var cell = ResourceManager.Instance.GetCell();
                cell.transform.SetParent(scrollRect.content);
                if (extendWidth > 0) cell.ExtendWidth(extendWidth);
                var size = cell.InitText(result[row][col]);
                cells[row].Add(cell);
                widths[col] = Mathf.Max(widths[col], size.x);
                heights[row] = Mathf.Max(heights[row], size.y);
            }

            if (row % 10 == 9) yield return null;
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

        var outerRectangleSize = new Vector2(Mathf.Min(widthSoFar + margin, maxSize.x),
            Mathf.Min(heightSoFar + margin, maxSize.y));

        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = outerRectangleSize;

        scrollRect.content.sizeDelta = new Vector2(widthSoFar, heightSoFar);
        scrollRect.GetComponent<RectTransform>().sizeDelta = new Vector2(-margin, -margin);
        outerRectangle.Width = outerRectangleSize.x;
        outerRectangle.Height = outerRectangleSize.y;

        innerRectangle.Width = outerRectangleSize.x - margin;
        innerRectangle.Height = outerRectangleSize.y - margin;

        IsCurrentResultDoneInit = true;
    }

    public void Show(Vector2 topPoint, Vector2 bottomPoint)
    {
        gameObject.SetActive(true);
        StartCoroutine(ShowIndicator_CO(topPoint, bottomPoint));
    }

    private IEnumerator ShowIndicator_CO(Vector2 topPoint, Vector2 bottomPoint)
    {
        while (!IsCurrentResultDoneInit)
        {
            yield return null;
        }

        var historyIndicator = Instantiate(ResourceManager.Instance.BezierHistoryIndicator)
            .GetComponent<BezierHistoryIndicator>();

        historyIndicator.SetPoint(TopPoint.position, BottomPoint.position, topPoint, bottomPoint);

        GameManager.Instance.SetNewIndicator(historyIndicator);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Destroy()
    {
        foreach (var row in cells)
        {
            foreach (var cell in row)
            {
                cell.ReturnToPool();
            }
        }

        Destroy(gameObject, 3f);
    }
}