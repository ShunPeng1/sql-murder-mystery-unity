using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HistoryItem : MonoBehaviour
{
    private QueryResult queryResult;
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text text;

    public void Init(QueryResult queryResult)
    {
        this.queryResult = queryResult;
        GameManager.Instance.HistoryChosen += HistoryChosenHandler;
        button.onClick.AddListener(() => GameManager.Instance.OnHistoryChosen(queryResult.Index));

        text.text = queryResult.Query.Substring(0, 10) + "...";
    }

    private void HistoryChosenHandler(int index)
    {
        if (index == queryResult.Index) queryResult.Show();
        else queryResult.Hide();
    }
}