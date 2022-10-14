using System;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private IDbConnection dbConnection;

    private void Awake()
    {
        Instance = this;

        string connection = "URI=file:" + "Assets/sql-murder-mystery.db";
        dbConnection = new SqliteConnection(connection);
        dbConnection.Open();
    }


    public bool TryExecuteQuery(string query, out List<List<string>> results)
    {
        results = null;

        IDbCommand cmnd_read = dbConnection.CreateCommand();
        IDataReader reader;
        cmnd_read.CommandText = query;
        try
        {
            reader = cmnd_read.ExecuteReader();
        }
        catch
        {
            return false;
        }

        results = new List<List<string>>();
        results.Add(new List<string>());
        for (int i = 0; i < reader.FieldCount; i++)
        {
            results[0].Add(reader.GetName(i));
        }

        while (reader.Read())
        {
            results.Add(new List<string>());
            for (int i = 0; i < reader.FieldCount; i++)
            {
                results[^1].Add(reader[i].ToString());
            }
        }

        return true;
    }
}