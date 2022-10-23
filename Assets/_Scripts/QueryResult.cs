using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
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
    [SerializeField] private Transform TopPoint;
    [SerializeField] private Transform BottomPoint;
    private List<List<Cell>> cells = new List<List<Cell>>();

    [SerializeField] private float margin;
    public HistoryItem HistoryItem;
    private Vector2 maxSize;
    private RectTransform rectTransform;
    private BezierHistoryIndicator historyIndicator;
    private Vector3 initTransformPos;
    private Vector2 initSizeDelta;
    private Vector2 initAnchorPos;
    private float rectanglesHeightDelta;

    [SerializeField] private float expandShrinkTime;
    [SerializeField] private float moveTime;
    [SerializeField] private float pauseTime;

    public int Index { get; private set; }
    public string Query { get; private set; }

    public static QueryResult ShowingResult;
    public static bool IsCurrentResultAnimating = false;
    private bool doneInit = false;

    private IEnumerator Show_CO()
    {
        while (IsCurrentResultAnimating || !doneInit) yield return null;
        IsCurrentResultAnimating = true;

        rectTransform.anchoredPosition = Vector2.zero;
        historyIndicator.transform.position = Vector3.zero;

        historyIndicator.SetPoint(TopPoint, BottomPoint, HistoryItem.TopPoint, HistoryItem.BottomPoint);

        float timer = expandShrinkTime;

        var historyItemRect = HistoryItem.GetComponent<RectTransform>();

        var historyItemActualHeight = HistoryItem.Background.sizeDelta.y;

        var aSize = initSizeDelta;
        var bSize = historyItemRect.sizeDelta + Vector2.up * historyItemActualHeight;

        var aPos = initTransformPos;
        var bPos = historyItemRect.position + Vector3.up * historyItemRect.sizeDelta.y / 100;

        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, bSize.y);
        rectTransform.position = new Vector2(rectTransform.position.x + 20,
            bPos.y + historyItemActualHeight / 200);

        outerRectangle.Height = rectTransform.sizeDelta.y;
        innerRectangle.Height = rectTransform.sizeDelta.y - rectanglesHeightDelta;

        historyIndicator.transform.DOMoveX(0, moveTime).SetEase(Ease.OutCubic);
        var tween = transform.DOMoveX(initTransformPos.x, moveTime).SetEase(Ease.OutCubic);
        while (tween.IsPlaying())
        {
            historyIndicator.UpdateShape();
            yield return null;
        }

        float pauseTimer = pauseTime;
        while (pauseTimer > 0)
        {
            yield return null;
            pauseTimer -= Time.deltaTime;
        }

        while (timer > 0)
        {
            var t = DOVirtual.EasedValue(0, 1, 1 - timer / expandShrinkTime, Ease.InOutCubic);
            rectTransform.sizeDelta =
                new Vector2(rectTransform.sizeDelta.x, Mathf.Lerp(bSize.y, aSize.y, t));
            rectTransform.position = new Vector2(rectTransform.position.x,
                Mathf.Lerp(bPos.y, aPos.y, t));

            outerRectangle.Height = rectTransform.sizeDelta.y;
            innerRectangle.Height = rectTransform.sizeDelta.y - rectanglesHeightDelta;

            historyIndicator.UpdateShape();

            yield return null;
            timer -= Time.deltaTime;
        }

        IsCurrentResultAnimating = false;
    }

    private IEnumerator Hide_CO(bool justCreated)
    {
        while (IsCurrentResultAnimating) yield return null;
        IsCurrentResultAnimating = true;

        float timer = expandShrinkTime;

        var historyItemRect = HistoryItem.GetComponent<RectTransform>();

        var historyItemActualHeight = HistoryItem.Background.sizeDelta.y;

        var aSize = rectTransform.sizeDelta;
        var bSize = historyItemRect.sizeDelta + Vector2.up * historyItemActualHeight;

        var aPos = rectTransform.position;
        var bPos = historyItemRect.position +
                   Vector3.up * historyItemRect.sizeDelta.y / 100 * (justCreated ? 2 : 1);

        while (timer > 0)
        {
            var t = DOVirtual.EasedValue(0, 1, 1 - timer / expandShrinkTime, Ease.InOutCubic);
            rectTransform.sizeDelta =
                new Vector2(rectTransform.sizeDelta.x, Mathf.Lerp(aSize.y, bSize.y, t));
            rectTransform.position = new Vector2(rectTransform.position.x,
                Mathf.Lerp(aPos.y, bPos.y, t) + historyItemActualHeight / 200);

            outerRectangle.Height = rectTransform.sizeDelta.y;
            innerRectangle.Height = rectTransform.sizeDelta.y - rectanglesHeightDelta;

            historyIndicator.UpdateShape();

            yield return null;
            timer -= Time.deltaTime;
        }

        float pauseTimer = pauseTime;
        while (pauseTimer > 0)
        {
            historyIndicator.UpdateShape();
            yield return null;
            pauseTimer -= Time.deltaTime;
        }

        historyIndicator.UpdateShape();
        historyIndicator.transform.SetParent(transform);
        var tween = transform.DOMoveX(20, moveTime).SetEase(Ease.InCubic);
        while (tween.IsPlaying()) yield return null;
        historyIndicator.transform.SetParent(null);

        gameObject.SetActive(false);

        scrollRect.content.anchoredPosition = Vector2.zero;
        
        IsCurrentResultAnimating = false;
    }

    public void Init(string query, List<List<string>> result, int index)
    {
        StartCoroutine(Init_CO(query, result, index));
    }

    private IEnumerator Init_CO(string query, List<List<string>> result, int index)
    {
        historyIndicator = Instantiate(ResourceManager.Instance.BezierHistoryIndicator)
            .GetComponent<BezierHistoryIndicator>();
        historyIndicator.transform.position = Vector3.right * 100;

        if (result.Count > 100)
        {
            result = result.GetRange(0, 101);
            result.Add(Enumerable.Repeat("...", result[0].Count).ToList());
        }

        Query = query;
        Index = index;

        rectTransform = GetComponent<RectTransform>();
        rectTransform.SetParent(ResourceManager.Instance.QueryResultRect.transform);
        rectTransform.anchoredPosition = Vector2.right * 100000;
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

            if (row % 5 == 4) yield return null;
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

        initTransformPos = rectTransform.position;
        initSizeDelta = rectTransform.sizeDelta;
        initAnchorPos = rectTransform.anchoredPosition;
        rectanglesHeightDelta = outerRectangle.Height - innerRectangle.Height;

        rectTransform.anchoredPosition = Vector2.right * 100000;

        doneInit = true;
    }

    public void Show()
    {
        if (ShowingResult != this && ShowingResult != null)
            ShowingResult.Hide(HistoryList.Instance.JustCreated);

        if (ShowingResult != this)
        {
            gameObject.SetActive(true);
            StartCoroutine(Show_CO());
        }

        ShowingResult = this;
    }

    public void Hide(bool justCreated)
    {
        StartCoroutine(Hide_CO(justCreated));
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