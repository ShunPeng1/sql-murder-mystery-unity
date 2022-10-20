using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SQLQueryBox : PersistentSingleton<SQLQueryBox>
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text frontText;
    [SerializeField] private TMP_Text actualText;
    [SerializeField] private Button submitButton;
    [SerializeField] private Button resetButton;

    private void Start()
    {
        inputField.onValueChanged.AddListener(OnValueChanged);
        submitButton.onClick.AddListener(OnSubmit);
        resetButton.onClick.AddListener(OnReset);
    }

    private void Update()
    {
        frontText.rectTransform.anchoredPosition = actualText.rectTransform.anchoredPosition;
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
        GameManager.Instance.ExecuteQuery(inputField.text);
    }

    private string SQLBeautifier(string query)
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

        return stringBuilder.ToString();
    }

    private void OnValueChanged(string query)
    {
        frontText.text = SQLBeautifier(query);
    }
}