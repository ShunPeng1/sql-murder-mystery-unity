using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

public class SQLQueryBox : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text text;

    private void Start()
    {
        inputField.onSubmit.AddListener(OnSubmit);
        inputField.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnSubmit(string query)
    {
        if (GameManager.Instance.TryExecuteQuery(query, out List<List<string>> results))
        {
            foreach (var result in results)
            {
                string l = "";
                foreach (var r in result)
                {
                    l += r + " | ";
                }

                Debug.Log(l);
            }
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