﻿using System;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using DG.Tweening;
using Mono.Data.Sqlite;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private IDbConnection dbConnection;

    public event Action<int> HistoryChosen;
    [HideInInspector] public int ResultCount = 0;

    private GameObject errorMessage;

    private bool isFullscreen = false;
    private void Awake()
    {
        Instance = this;

        string dir = Application.dataPath + "/StreamingAssets/";
        string fileName = "sql-murder-mystery.db";
        System.IO.File.Copy(dir + fileName, dir + "this-session.db", true);
        string connection = "URI=file:" + dir + "this-session.db";
        dbConnection = new SqliteConnection(connection);
        dbConnection.Open();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F11))
        {
            if (!isFullscreen)
            {
                //Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
                isFullscreen = true;
            }
            else
            {
                //Screen.fullScreenMode = FullScreenMode.Windowed;
                Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
                isFullscreen = false;
            }
        }
    }

    public void ExecuteQuery(string query)
    {
        if (query == string.Empty) return;

        IDbCommand cmnd_read = dbConnection.CreateCommand();
        IDataReader reader;
        cmnd_read.CommandText = query;
        List<List<string>> result = new List<List<string>> {new List<string>()};

        try
        {
            reader = cmnd_read.ExecuteReader();
        }
        catch (SqliteException e)
        {
            result[0]
                .Add(string.Join(" ",
                    e.Message.Split(new[] {'\r'}, StringSplitOptions.RemoveEmptyEntries)));
            CreateResultTable(query, result, false);
            return;
        }


        for (int i = 0; i < reader.FieldCount; i++)
        {
            result[0].Add(reader.GetName(i));
        }

        while (reader.Read())
        {
            result.Add(new List<string>());
            for (int i = 0; i < reader.FieldCount; i++)
            {
                result[^1].Add(reader[i].ToString());
            }
        }

        CreateResultTable(query, result, true);
    }

    private void CreateResultTable(string query, List<List<string>> result, bool record)
    {
        var resultGO = ResourceManager.Instance.QueryResult;
        var canvasTransform = ResourceManager.Instance.Canvas.transform;

        var queryResult = Instantiate(resultGO).GetComponent<QueryResult>();
        queryResult.transform.SetParent(canvasTransform);
        queryResult.transform.SetSiblingIndex(0);
        queryResult.transform.localScale = Vector3.one;

        if (record)
        {
            queryResult.Init(query, result, ResultCount);
            HistoryList.Instance.CreateHistoryItem(queryResult);
            OnHistoryChosen(ResultCount);
            ResultCount++;
        }
        else
        {
            QueryResult.ShowingResult = null;
            queryResult.Init(query, result, -1);
            OnHistoryChosen(-1);
            errorMessage = queryResult.gameObject;
        }
    }

    public void OnHistoryChosen(int index)
    {
        HistoryChosen?.Invoke(index);

        if (errorMessage != null)
        {
            errorMessage.transform.DOMoveX(-10, 1f)
                .SetEase(Ease.OutCubic)
                .SetRelative(true)
                .OnComplete(() => Destroy(errorMessage));
            errorMessage = null;
        }
    }
}