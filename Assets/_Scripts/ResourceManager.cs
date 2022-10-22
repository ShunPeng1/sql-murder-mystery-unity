using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;


public class ResourceManager : PersistentSingleton<ResourceManager>
{
    [HideInInspector] public string[] SQLKeywords = new[]
    {
        "ADD",
        "ADD CONSTRAINT",
        "ALL",
        "ALTER",
        "ALTER COLUMN",
        "ALTER TABLE",
        "AND",
        "ANY",
        "AS",
        "ASC",
        "BACKUP DATABASE",
        "BETWEEN",
        "CASE",
        "CHECK",
        "COLUMN",
        "CONSTRAINT",
        "CREATE",
        "CREATE DATABASE",
        "CREATE INDEX",
        "CREATE OR REPLACE VIEW",
        "CREATE TABLE",
        "CREATE PROCEDURE",
        "CREATE UNIQUE INDEX",
        "CREATE VIEW",
        "DATABASE",
        "DEFAULT",
        "DELETE",
        "DESC",
        "DISTINCT",
        "DROP",
        "DROP COLUMN",
        "DROP CONSTRAINT",
        "DROP DATABASE",
        "DROP DEFAULT",
        "DROP INDEX",
        "DROP TABLE",
        "DROP VIEW",
        "EXEC",
        "EXISTS",
        "FOREIGN KEY",
        "FROM",
        "FULL OUTER JOIN",
        "GROUP BY",
        "HAVING",
        "IN",
        "INDEX",
        "INNER JOIN",
        "INSERT INTO",
        "INSERT INTO SELECT",
        "IS NULL",
        "IS NOT NULL",
        "JOIN",
        "LEFT JOIN",
        "LIKE",
        "LIMIT",
        "NOT",
        "NOT NULL",
        "OR",
        "ORDER BY",
        "OUTER JOIN",
        "PRIMARY KEY",
        "PROCEDURE",
        "RIGHT JOIN",
        "ROWNUM",
        "SELECT",
        "SELECT DISTINCT",
        "SELECT INTO",
        "SELECT TOP",
        "SET",
        "TABLE",
        "TOP",
        "TRUNCATE TABLE",
        "UNION",
        "UNION ALL",
        "UNIQUE",
        "UPDATE",
        "VALUES",
        "VIEW",
        "WHERE",
    };

    

    public Canvas Canvas;

    public GameObject Cell;
    public GameObject HistoryItem;
    public GameObject QueryResult;
    public GameObject BezierHistoryIndicator;
    
    public GameObject QueryResultRect;

    public List<(int, int)> GetKeywordIndicesAndLengths(string query)
    {
        List<(int, int)> indices = new List<(int, int)>();
        query = query.ToUpper();

        foreach (var keyword in SQLKeywords)
        {
            var index = query.IndexOf(keyword);
            if (index == -1) continue;

            bool start = index == 0 || !IsAlphaNumeric(query[index - 1]);
            bool end = index + keyword.Length == query.Length || query[index + keyword.Length] == ' ';

            if (start && end) indices.Add((index, keyword.Length));
        }

        return indices;

        bool IsAlphaNumeric(char c)
        {
            return Regex.IsMatch(c.ToString(), @"^[a-zA-Z0-9_]+$");
        }
    }
}