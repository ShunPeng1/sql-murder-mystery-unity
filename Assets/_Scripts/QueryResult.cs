using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueryResult : MonoBehaviour
{
    public void Init(List<List<string>> result)
    {
        StopAllCoroutines();
        StartCoroutine(Init_CO());

        IEnumerator Init_CO()
        {
            for (var row = 0; row < result.Count; row++)
            {
                for (var col = 0; col < result[row].Count; col++)
                {
                    var cell = Instantiate(ResourceManager.Instance.Cell).GetComponent<Cell>();
                    cell.transform.SetParent(transform);
                    var rectTransform = cell.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = new Vector3(col * 100, row * -40f, 0);
                    rectTransform.localScale = Vector3.one;
                    cell.Init(result[row][col], 100f, 40f);
                }

                yield return null;
            }
        }
    }
}