using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HistoryItem : MonoBehaviour
{
    public QueryResult QueryResult;
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
    }
}