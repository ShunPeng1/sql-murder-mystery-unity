using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QueryResult : MonoBehaviour
{
    private int index;
    [SerializeField] private ScrollRect scrollRect;

    public void Init(string query, List<List<string>> result)
    {
        index = GameManager.Instance.QueryCount;
        Vector2 scrollRectSize = new Vector2(1920, 400);
        scrollRect.GetComponent<RectTransform>().sizeDelta = scrollRectSize;
        
        float cellWidth = 100f;
        float cellHeight = 40f;

        float contentWidth = cellWidth * result[0].Count;
        float contentHeight = cellHeight * result.Count;
        scrollRect.content.sizeDelta = new Vector2(contentWidth, contentHeight);

        StopAllCoroutines();
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
}