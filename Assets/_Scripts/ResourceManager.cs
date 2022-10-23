using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
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

    private string[] SQLKeywords = new[]
    {
        "ADD",
        "ALL",
        "ALTER",
        "AND",
        "ANY",
        "AS",
        "ASC",
        "BETWEEN",
        "BY",
        "CASE",
        "CHECK",
        "COLUMN",
        "CONSTRAINT",
        "CREATE",
        "DATABASE",
        "DEFAULT",
        "DELETE",
        "DESC",
        "DISTINCT",
        "DROP",
        "EXEC",
        "EXISTS",
        "FOREIGN",
        "FROM",
        "FULL",
        "GROUP",
        "HAVING",
        "IN",
        "INDEX",
        "INNER",
        "IS",
        "INTO",
        "NULL",
        "JOIN",
        "LEFT",
        "LIKE",
        "LIMIT",
        "KEY",
        "NOT",
        "OR",
        "ON",
        "ORDER",
        "OUTER",
        "PRIMARY",
        "PROCEDURE",
        "REPLACE",
        "RIGHT",
        "ROWNUM",
        "SELECT",
        "SET",
        "TABLE",
        "TOP",
        "TRUNCATE",
        "UNION",
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

    public List<SpecialWord> GetKeywordIndicesAndLengths(string _query)
    {
        List<SpecialWord> specialWords = new List<SpecialWord>();

        var query = _query.ToCharArray();
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

                    for (int j = stringIndex; j < i + 1; j++)
                    {
                        query[j] = '~';
                    }

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

                    for (int j = stringIndex; j < i + 1; j++)
                    {
                        query[j] = '~';
                    }

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
        {
            specialWords.Add(
                new SpecialWord(stringIndex, query.Length - stringIndex, SpecialType.String));
            for (int j = stringIndex; j < query.Length; j++)
            {
                query[j] = '~';
            }
        }

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
                    if (((numberIndex > 0 && !IsAlpha(query[numberIndex - 1])) || numberIndex == 0) &&
                        !IsAlpha(query[i]))
                        specialWords.Add(new SpecialWord(numberIndex, i - numberIndex,
                            SpecialType.Number));
                }

                lastNumNumeric = false;
            }
        }

        if (lastNumNumeric &&
            ((numberIndex > 0 && !IsAlpha(query[numberIndex - 1])) || numberIndex == 0))
            specialWords.Add(
                new SpecialWord(numberIndex, query.Length - numberIndex, SpecialType.Number));

        foreach (var special in SQLTableNames)
        {
            var indices = AllIndexesOf(query.ArrayToString(), special);

            foreach (var index in indices)
            {
                if (index == -1) continue;

                bool start = index == 0 || !IsAlphaNumeric(query[index - 1]);
                bool end = index + special.Length == query.Length ||
                           query[index + special.Length] == ' ' ||
                           query[index + special.Length] == '\n' ||
                           !IsAlphaNumeric(query[index + special.Length]);

                if (start && end)
                {
                    specialWords.Add(new SpecialWord(index, special.Length, SpecialType.Name));
                }
            }
        }

        query = query.ArrayToString().ToUpper().ToCharArray();

        foreach (var special in SQLKeywords)
        {
            var indices = AllIndexesOf(query.ArrayToString(), special);

            foreach (var index in indices)
            {
                if (index == -1) continue;

                bool start = index == 0 || !IsAlphaNumeric(query[index - 1]);
                bool end = index + special.Length == query.Length ||
                           query[index + special.Length] == ' ' ||
                           query[index + special.Length] == '\n' ||
                           !IsAlphaNumeric(query[index + special.Length]);

                if (start && end)
                    specialWords.Add(new SpecialWord(index, special.Length, SpecialType.Keyword));
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

    public static List<int> AllIndexesOf(string str, string value)
    {
        List<int> indexes = new List<int>();
        for (int index = 0;; index += value.Length)
        {
            index = str.IndexOf(value, index);
            if (index == -1) return indexes;
            indexes.Add(index);
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