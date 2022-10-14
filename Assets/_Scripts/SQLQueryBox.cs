using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SQLQueryBox : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text text;
    [SerializeField] private Button button;

    private void Start()
    {
        inputField.onValueChanged.AddListener(OnValueChanged);
        button.onClick.AddListener(OnSubmit);
    }

    private void OnSubmit()
    {
        var query = inputField.text;
        if (GameManager.Instance.TryExecuteQuery(query, out List<List<string>> result))
        {
            var resultGO = ResourceManager.Instance.QueryResult;
            var canvasTransform = ResourceManager.Instance.Canvas.transform;
            var queryResult = Instantiate(resultGO).GetComponent<QueryResult>();
            queryResult.transform.SetParent(canvasTransform);
            queryResult.transform.localScale = Vector3.one;
            queryResult.Init(result);
        }
    }

    private void OnValueChanged(string query)
    {
        StringBuilder stringBuilder = new StringBuilder(query);

        var indicesAndLengths = ResourceManager.Instance.GetKeywordIndicesAndLengths(query);
        indicesAndLengths.Sort((a, b) => a.Item1 < b.Item1 ? 1 : -1);

        string color = ColorUtility.ToHtmlStringRGB(VisualManager.Instance.SQLKeywordColor);

        foreach (var (index, length) in indicesAndLengths)
        {
            stringBuilder.Insert(index + length, "</color>");
            stringBuilder.Insert(index, "<color=#" + color + ">");
        }

        text.text = stringBuilder.ToString();
    }
}