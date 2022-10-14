using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SQLQueryBox : MonoBehaviour
{
    private TMP_InputField inputField;

    private void Start()
    {
        inputField = GetComponent<TMP_InputField>();
        inputField.onSubmit.AddListener(OnSubmit);
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
}