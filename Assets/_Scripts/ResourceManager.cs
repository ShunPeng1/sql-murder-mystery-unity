using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;


public class ResourceManager : PersistentSingleton<ResourceManager>
{
    [HideInInspector] public string[] SQLTableNames = new[]
    {
        "crime_scene_report",
        "drivers_license",
        "facebook_event_checkin",
        "interview",
        "get_fit_now_member",
        "get_fit_now_check_in",
        "solution",
        "income",
        "person"
    };

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

    [SerializeField] private GameObject cell;
    public GameObject HistoryItem;
    public GameObject QueryResult;
    public GameObject BezierHistoryIndicator;

    public GameObject QueryResultRect;

    private List<Cell> cellPool = new List<Cell>();
    private List<Cell> retiredPool = new List<Cell>();


    private void Start()
    {
        StartCoroutine(InitPool_CO(500));
    }

    private IEnumerator InitPool_CO(int count)
    {
        for (int i = 0; i < count; i++)
        {
            InstantiateCell();
            if (i % 100 == 99) yield return null;
        }
    }

    public Cell GetCell()
    {
        if (cellPool.Count == 0) InstantiateCell();
        var c = cellPool[^1];
        cellPool.RemoveAt(cellPool.Count - 1);
        return c;
    }

    public void Retire(Cell cell)
    {
        retiredPool.Add(cell);
        cell.gameObject.SetActive(false);
    }

    private void Update()
    {
        InstantiateCell();
        DestroyCell();
    }

    private void InstantiateCell()
    {
        if (cellPool.Count > 2000) return;
        for (int i = 0; i < 3; i++)
        {
            cellPool.Add(Instantiate(cell).GetComponent<Cell>());
        }
    }

    private void DestroyCell()
    {
        for (int i = 0; i < 1; i++)
        {
            if (retiredPool.Count == 0) return;
            Destroy(retiredPool[^1].gameObject);
            retiredPool.RemoveAt(retiredPool.Count - 1);
        }
    }

    public List<SpecialWord> GetKeywordIndicesAndLengths(string query, SpecialType specialType)
    {
        List<SpecialWord> specialWords = new List<SpecialWord>();

        if (specialType == SpecialType.String)
        {
            int stringIndex = 0;
            char stringIndicator = '~';
            for (int i = 0; i < query.Length; i++)
            {
                if (query[i] == '\'')
                {
                    if (stringIndicator == '\'')
                    {
                        specialWords.Add(new SpecialWord(stringIndex, i - stringIndex + 1,
                            SpecialType.String));
                        stringIndicator = '~';
                    }
                    else
                    {
                        stringIndex = i;
                        stringIndicator = '\'';
                    }
                }
                else if (query[i] == '"')
                {
                    if (stringIndicator == '"')
                    {
                        specialWords.Add(new SpecialWord(stringIndex, i - stringIndex + 1,
                            SpecialType.String));
                        stringIndicator = '~';
                    }
                    else
                    {
                        stringIndex = i;
                        stringIndicator = '"';
                    }
                }
            }

            if (stringIndicator is ' ' or '\n' or '"' or '\'')
                specialWords.Add(new SpecialWord(stringIndex, query.Length - stringIndex,
                    SpecialType.String));
        }
        else if (specialType == SpecialType.Number)
        {
            int numberIndex = 0;
            bool lastNumNumeric = false;
            for (int i = 0; i < query.Length; i++)
            {
                if (IsNumeric(query[i]))
                {
                    if (!lastNumNumeric)
                    {
                        numberIndex = i;
                    }

                    lastNumNumeric = true;
                }
                else
                {
                    if (lastNumNumeric)
                    {
                        if (numberIndex > 0 && !IsAlpha(query[numberIndex]) && !IsAlpha(query[i]))
                            specialWords.Add(new SpecialWord(numberIndex, i - numberIndex,
                                SpecialType.Number));
                    }

                    lastNumNumeric = false;
                }
            }

            if (lastNumNumeric)
                specialWords.Add(new SpecialWord(numberIndex, query.Length - numberIndex,
                    SpecialType.Number));
        }
        else
        {
            if (specialType == SpecialType.Keyword) query = query.ToUpper();

            string[] list = specialType switch
            {
                SpecialType.Keyword => SQLKeywords,
                SpecialType.Name => SQLTableNames,
                _ => new string[] { }
            };

            foreach (var special in list)
            {
                var index = query.IndexOf(special);
                if (index == -1) continue;

                bool start = index == 0 || !IsAlphaNumeric(query[index - 1]);
                bool end = index + special.Length == query.Length ||
                           query[index + special.Length] == ' ' || query[index + special.Length] == '\n';

                if (start && end) specialWords.Add(new SpecialWord(index, special.Length, specialType));
            }
        }

        return specialWords;

        bool IsAlphaNumeric(char c)
        {
            return Regex.IsMatch(c.ToString(), @"^[a-zA-Z0-9_]+$");
        }

        bool IsNumeric(char c)
        {
            return Regex.IsMatch(c.ToString(), @"^[0-9]+$");
        }

        bool IsAlpha(char c)
        {
            return Regex.IsMatch(c.ToString(), @"^[a-zA-Z_]+$");
        }
    }
}

public enum SpecialType
{
    Keyword,
    Name,
    String,
    Number
}

public class SpecialWord
{
    public int Index;
    public int Length;
    public SpecialType SpecialType;

    public SpecialWord(int index, int length, SpecialType specialType)
    {
        Index = index;
        Length = length;
        SpecialType = specialType;
    }

    public string GetColorHex()
    {
        switch (SpecialType)
        {
            case SpecialType.Keyword:

                return ColorUtility.ToHtmlStringRGB(VisualManager.Instance.SQLKeywordColor);
            case SpecialType.Name:
                return ColorUtility.ToHtmlStringRGB(VisualManager.Instance.SQLNameColor);
            case SpecialType.String:
                return ColorUtility.ToHtmlStringRGB(VisualManager.Instance.SQLStringColor);
            case SpecialType.Number:
                return ColorUtility.ToHtmlStringRGB(VisualManager.Instance.SQLNumberColor);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}