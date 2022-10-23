using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SQLQueryBox : PersistentSingleton<SQLQueryBox>
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text frontText;
    [SerializeField] private TMP_Text actualText;
    [SerializeField] private TMP_Text lineCountText;
    [SerializeField] private Button submitButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button helpButton;

    private void Start()
    {
        inputField.onValueChanged.AddListener(OnValueChanged);
        submitButton.onClick.AddListener(OnSubmit);
        resetButton.onClick.AddListener(OnReset);
        helpButton.onClick.AddListener(() => InfoPanel.Instance.Show());
    }

    private void Update()
    {
        var pos = actualText.rectTransform.anchoredPosition;
        lineCountText.rectTransform.anchoredPosition =
            new Vector2(lineCountText.rectTransform.anchoredPosition.x, pos.y);
        frontText.rectTransform.anchoredPosition = pos;

        StringBuilder str = new StringBuilder();
        for (int i = 0; i < actualText.textInfo.lineCount; i++)
        {
            str.Append((i + 1).ToString() + '\n');
        }

        lineCountText.text = str.ToString();
    }

    public void SetText(string query)
    {
        inputField.text = query;
        frontText.text = SQLBeautifier(query);
    }

    private void OnReset()
    {
        SetText("");
    }

    private void OnSubmit()
    {
        if (QueryResult.IsCurrentResultAnimating) return;
        GameManager.Instance.ExecuteQuery(inputField.text);
    }

    private string SQLBeautifier(string query)
    {
        StringBuilder stringBuilder = new StringBuilder(query);

        var list = ResourceManager.Instance.GetKeywordIndicesAndLengths(query);
        list.Sort((a, b) => a.Index < b.Index ? 1 : -1);

        foreach (var word in list)
        {
            stringBuilder.Insert(word.Index + word.Length, "</color>");
            stringBuilder.Insert(word.Index, "<color=#" + word.GetColorHex() + ">");
        }

        return stringBuilder.ToString();
    }

    private void OnValueChanged(string query)
    {
        frontText.text = SQLBeautifier(query);
    }
}