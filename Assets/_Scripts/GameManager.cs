using System;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private IDbConnection dbConnection;

    public event Action<int> HistoryChosen;
    public int QueryCount = 0;

    public List<GameObject> queryTables;
    public List<GameObject> historyItems;

    private void Awake()
    {
        Instance = this;

        string connection = "URI=file:" + "Assets/sql-murder-mystery.db";
        dbConnection = new SqliteConnection(connection);
        dbConnection.Open();
    }

    public void ExecuteQuery(string query)
    {
        IDbCommand cmnd_read = dbConnection.CreateCommand();
        IDataReader reader;
        cmnd_read.CommandText = query;
        try
        {
            reader = cmnd_read.ExecuteReader();
        }
        catch
        {
            return;
        }

        List<List<string>> result = new List<List<string>> {new List<string>()};

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

        CreateResultTable(query, result);
    }

    private void CreateResultTable(string query, List<List<string>> result)
    {
        var resultGO = ResourceManager.Instance.QueryResult;
        var canvasTransform = ResourceManager.Instance.Canvas.transform;
        var queryResult = Instantiate(resultGO).GetComponent<QueryResult>();
        queryResult.transform.SetParent(canvasTransform);
        queryResult.transform.localScale = Vector3.one;
        queryResult.Init(query, result);
    }
}