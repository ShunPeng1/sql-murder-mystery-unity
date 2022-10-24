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

    private List<List<string>> queryResult;
    private int maxRowCount = 69;
    float widthSoFar = 0f;
    float heightSoFar = 0f;
    List<float> widths;
    List<float> heights;

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
        while (tween.IsPlaying())
        {
            historyIndicator.UpdateShape();
            yield return null;
        }

        historyIndicator.transform.SetParent(null);

        gameObject.SetActive(false);

        scrollRect.content.anchoredPosition = Vector2.zero;

        IsCurrentResultAnimating = false;
    }

    public void Init(string query, List<List<string>> result, int index)
    {
        if (result.Count > maxRowCount)
        {
            queryResult = result.GetRange(0, maxRowCount);
            queryResult.Add(Enumerable.Repeat("...", queryResult[0].Count).ToList());
        }
        else queryResult = result;

        if (queryResult.Count == 0 || queryResult[0].Count == 0)
            queryResult[0].Add("The query returned nothing.");
        StartCoroutine(Init_CO(query, result, index));
    }

    private IEnumerator Init_CO(string query, List<List<string>> result, int index)
    {
        historyIndicator = Instantiate(ResourceManager.Instance.BezierHistoryIndicator)
            .GetComponent<BezierHistoryIndicator>();
        historyIndicator.transform.position = Vector3.right * 100;

        Query = query;
        Index = index;

        rectTransform = GetComponent<RectTransform>();
        rectTransform.SetParent(ResourceManager.Instance.QueryResultRect.transform);
        rectTransform.anchoredPosition = Vector2.right * 100000;
        maxSize = ResourceManager.Instance.QueryResultRect.GetComponent<RectTransform>().sizeDelta;

        widths = Enumerable.Repeat(0f, queryResult[0].Count).ToList();
        heights = Enumerable.Repeat(0f, queryResult.Count).ToList();

        float extendWidth = queryResult[0].Count switch
        {
            1 => 1500f,
            2 => 1000f,
            3 => 700f,
            4 => 500f,
            _ => -1
        };

        for (var row = 0; row < queryResult.Count; row++)
        {
            cells.Add(new List<Cell>());
            for (var col = 0; col < queryResult[row].Count; col++)
            {
                var cell = ResourceManager.Instance.GetCell();
                cell.transform.SetParent(scrollRect.content);
                if (extendWidth > 0) cell.ExtendWidth(extendWidth);
                var size = cell.InitText(queryResult[row][col], row == 0);
                cells[row].Add(cell);
                widths[col] = Mathf.Max(widths[col], size.x);
                heights[row] = Mathf.Max(heights[row], size.y);
            }

            if (row % 4 == 3) yield return null;
        }

        for (var row = 0; row < queryResult.Count; row++)
        {
            widthSoFar = 0f;

            for (var col = 0; col < queryResult[row].Count; col++)
            {
                cells[row][col].RectTransform.anchoredPosition = new Vector2(widthSoFar, -heightSoFar);
                cells[row][col].RectTransform.localScale = Vector3.one;
                cells[row][col].InitSize(widths[col], heights[row]);
                widthSoFar += widths[col] + VisualManager.Instance.Padding * 2;
            }

            heightSoFar += heights[row] + VisualManager.Instance.Padding * 2;
        }

        scrollRect.content.sizeDelta = new Vector2(widthSoFar, heightSoFar);

        var outerRectangleSize = new Vector2(Mathf.Min(widthSoFar + margin, maxSize.x),
            Mathf.Min(heightSoFar + margin, maxSize.y));

        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = outerRectangleSize;

        scrollRect.GetComponent<RectTransform>().sizeDelta = new Vector2(-margin, -margin);
        outerRectangle.Width = outerRectangleSize.x;
        outerRectangle.Height = outerRectangleSize.y;

        innerRectangle.Width = outerRectangleSize.x - margin;
        innerRectangle.Height = outerRectangleSize.y - margin;

        initTransformPos = rectTransform.position;
        initSizeDelta = rectTransform.sizeDelta;
        initAnchorPos = rectTransform.anchoredPosition;
        rectanglesHeightDelta = outerRectangle.Height - innerRectangle.Height;

        if (index != -1) rectTransform.anchoredPosition = Vector2.right * 100000;
        else
        {
            outerRectangle.CornerRadii = Vector4.one * 10;
            rectTransform.position = new Vector3(-30, rectTransform.position.y);
            rectTransform.DOAnchorPosX(0, 1f)
                .SetEase(Ease.OutCubic)
                .OnComplete(() =>
                {
                    rectTransform.DOMoveX(-30, 1f).SetEase(Ease.InCubic).SetDelay(3f);
                });
            Destroy(gameObject, 5f);
        }

        doneInit = true;
    }

    public void Show()
    {
        if (ShowingResult != this && ShowingResult != null)
            ShowingResult.Hide();

        if (ShowingResult != this)
        {
            gameObject.SetActive(true);
            StartCoroutine(Show_CO());
        }

        ShowingResult = this;
    }

    public void Hide()
    {
        if (gameObject.activeInHierarchy) StartCoroutine(Hide_CO(HistoryList.Instance.JustCreated));
    }

    public void Destroy()
    {
        gameObject.SetActive(true);
        StartCoroutine(Destroy_CO());
    }

    private IEnumerator Destroy_CO()
    {
        if (ShowingResult == this) yield return Hide_CO(HistoryList.Instance.JustCreated);
        Destroy(historyIndicator.gameObject);
        yield return new WaitForSeconds(3f);
        for (var row = 0; row < cells.Count; row++)
        {
            foreach (var cell in cells[row])
            {
                cell.ReturnToPool();
            }

            yield return null;
        }

        Destroy(gameObject, 1f);
    }
}