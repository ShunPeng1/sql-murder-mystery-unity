using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Utility
{
    public static bool IsElementsSame(this List<int> list)
    {
        if (list.Count == 0) return true;
        int first = list[0];
        for (int i = 1; i < list.Count; i++)
        {
            if (list[i] != first) return false;
        }

        return true;
    }

    public static Vector2Int ToVector2Int(this Vector2 v)
    {
        return new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
    }

    public static Vector2Int ToVector2Int(this Vector3 v)
    {
        return new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
    }

    public static Vector3Int ToVector3Int(this Vector3 v)
    {
        return new Vector3Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z));
    }

    public static Vector3Int ToVector3Int(this Vector2 v)
    {
        return new Vector3Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), 0);
    }

    public static Vector3 TransformToPositiveCoordinate(this Vector3 v)
    {
        return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
    }

    public static List<Vector2Int> TransformToPositiveCoordinates(this List<Vector2Int> list)
    {
        Vector2Int minV = new Vector2Int(int.MaxValue, int.MaxValue);

        foreach (Vector2Int i in list)
        {
            if (i.x < minV.x) minV.x = i.x;
            if (i.y < minV.y) minV.y = i.y;
        }

        List<Vector2Int> results = new List<Vector2Int>();
        foreach (Vector2Int i in list)
        {
            results.Add(i - minV);
        }

        return results;
    }

    public static Color MultiplyHSV(this Color color, float hMul, float sMul, float vMul)
    {
        Color.RGBToHSV(color, out float h, out float s, out float v);

        return Color.HSVToRGB(Mathf.Clamp01(h * hMul), Mathf.Clamp01(s * sMul), Mathf.Clamp01(v * vMul));
    }

    public static Color CopySV(this Color color, Color copy)
    {
        Color.RGBToHSV(color, out float h, out float _, out float _);
        Color.RGBToHSV(copy, out float _, out float s, out float v);
        return Color.HSVToRGB(h, s, v);
    }

    /// <summary>
    /// Set the HSV components of a Color. Provide a negative float to leave a component intact.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="h"></param>
    /// <param name="s"></param>
    /// <param name="v"></param>
    /// <returns></returns>
    public static Color SetHSV(this Color color, float h, float s, float v)
    {
        Color.RGBToHSV(color, out float H, out float S, out float V);
        if (h >= 0) H = h;
        if (s >= 0) S = s;
        if (v >= 0) V = v;
        return Color.HSVToRGB(H, S, V);
    }

    public static Color SetAlpha(this Color color, float a)
    {
        Color newColor = new Color(color.r, color.g, color.b, a);
        return newColor;
    }

    public static void SetAll<T>(this IList<T> list, T value)
    {
        for (int i = 0; i < list.Count; i++)
        {
            list[i] = value;
        }
    }

    public static int Normalize(this int i)
    {
        return i != 0 ? 1 : 0;
    }

    public static int Normalize(this IComparable i)
    {
        return i.Equals(default(IComparable)) ? 1 : 0;
    }

    public static int Sign(this IComparable i)
    {
        return i.CompareTo(default(IComparable));
    }

    public static int IsEqual(this int i, int equalTo)
    {
        return i == equalTo ? 1 : 0;
    }

    public static int ToInt(this bool b)
    {
        return b ? 1 : 0;
    }

    public static bool IsAdjacent(this Vector2Int vector2Int, Vector2Int other)
    {
        return Mathf.Approximately(Vector2Int.Distance(vector2Int, other), 1f);
    }

    public static Vector3 ToVector3(this Vector2Int vector2Int)
    {
        return new Vector3(vector2Int.x, vector2Int.y);
    }

    public static Vector3 NewX(this Vector3 vector3, float x)
    {
        return new Vector3(x, vector3.y, vector3.z);
    }

    public static Vector3 NewY(this Vector3 vector3, float y)
    {
        return new Vector3(vector3.x, y, vector3.z);
    }

    public static Vector3 NewZ(this Vector3 vector3, float z)
    {
        return new Vector3(vector3.x, vector3.y, z);
    }

    public static Vector3 NewZ(this Vector2 vector2, float z)
    {
        return new Vector3(vector2.x, vector2.y, z);
    }

    public static void Shuffle<T>(this IList<T> list, System.Random random)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }

    public static Vector3 RandomVector3((float, float) x, (float, float) y, (float, float) z)
    {
        return new Vector3(Random.Range(x.Item1, x.Item2), Random.Range(y.Item1, y.Item2),
            Random.Range(z.Item1, z.Item2));
    }

    public static Vector3 RandomVector3((float, float) x, (float, float) y)
    {
        return new Vector3(Random.Range(x.Item1, x.Item2), Random.Range(y.Item1, y.Item2), 0);
    }

    /// <summary>
    /// Generate a Vector3Int with randomly generated components in ranges.
    /// </summary>
    /// <param name="x">Start of x (inclusive) and end of x (exclusive).</param>
    /// <param name="y">Start of y (inclusive) and end of y (exclusive).</param>
    /// <returns></returns>
    public static Vector3 RandomVector3Int((int, int) x, (int, int) y)
    {
        return new Vector3(Random.Range(x.Item1, x.Item2), Random.Range(y.Item1, y.Item2), 0);
    }

    public static async void DelayedExecution(Action action, float seconds)
    {
        await Task.Delay((int)(seconds * 1000));
        action?.Invoke();
    }

    public static bool IsInBetweenInclusive(this IComparable mid, IComparable firstLimit,
        IComparable secondLimit)
    {
        return mid.CompareTo(firstLimit) >= 0 && mid.CompareTo(secondLimit) <= 0 ||
               mid.CompareTo(secondLimit) >= 0 && mid.CompareTo(firstLimit) <= 0;
    }

    public static bool IsInBetweenExclusive(this IComparable mid, IComparable firstLimit,
        IComparable secondLimit)
    {
        return mid.CompareTo(firstLimit) > 0 && mid.CompareTo(secondLimit) < 0 ||
               mid.CompareTo(secondLimit) > 0 && mid.CompareTo(firstLimit) < 0;
    }

    public static int RandomSign()
    {
        return Random.Range(0, 2) == 0 ? 1 : -1;
    }

    public static Vector3 OnlyY(this Vector3 v)
    {
        return Vector3.up * v.y;
    }

    public static bool Contains<T1>(this (T1, T1) tuple, T1 value)
    {
        return tuple.Item1.Equals(value) || tuple.Item2.Equals(value);
    }

    public static float RoundPortion(this float f, float portion)
    {
        return Mathf.Round(f / portion) * portion;
    }

    public static Vector3 MultiplyComponents(this Vector3 v, float xMul, float yMul, float zMul)
    {
        return new Vector3(v.x * xMul, v.y * yMul, v.z * zMul);
    }


    public static Vector2 GetMainDirection(this Vector2 v)
    {
        if (Mathfs.Approximately(v, Vector2.zero)) return Vector2.zero;
        v = v.normalized;
        float absX = Mathf.Abs(v.x);
        float absY = Mathf.Abs(v.y);

        if (absX > absY)
        {
            if (v.x > 0) return Vector2.right;
            else return Vector2.left;
        }
        else
        {
            if (v.y > 0) return Vector2.up;

            else return Vector2.down;
        }
    }

    public static Vector2 GetMainDirection(this Vector3 v)
    {
        if (Mathfs.Approximately((Vector2)v, Vector2.zero)) return Vector2.zero;
        v = v.normalized;
        float absX = Mathf.Abs(v.x);
        float absY = Mathf.Abs(v.y);

        if (absX > absY)
        {
            if (v.x > 0) return Vector2.right;
            else return Vector2.left;
        }
        else
        {
            if (v.y > 0) return Vector2.up;
            else return Vector2.down;
        }
    }

    public static float GetH(this Color color)
    {
        Color.RGBToHSV(color, out float h, out _, out _);
        return h;
    }

    public static void SetX(ref this Vector3 vector3, float x)
    {
        vector3.x = x;
    }

    public static Vector3 GetRandomPointInsideCollider(this BoxCollider boxCollider)
    {
        Vector3 extents = boxCollider.size / 2f;
        Vector3 point = new Vector3(Random.Range(-extents.x, extents.x),
            Random.Range(-extents.y, extents.y), Random.Range(-extents.z, extents.z));

        return boxCollider.transform.TransformPoint(point);
    }

    public static Vector2 GetRandomPointInsideCollider(this BoxCollider2D boxCollider)
    {
        Vector2 extents = boxCollider.size / 2f;
        Vector2 point = new Vector2(Random.Range(-extents.x, extents.x),
            Random.Range(-extents.y, extents.y));

        return boxCollider.transform.TransformPoint(point);
    }

    public static Vector2 GetLastPoint(this LineRenderer lineRenderer)
    {
        return lineRenderer.GetPosition(lineRenderer.positionCount - 1);
    }

    public static void AddNewPoint(this LineRenderer lineRenderer, Vector3 pos)
    {
        lineRenderer.positionCount++;
        lineRenderer.UpdateLastPoint(pos);
    }

    public static void RemoveLastPoint(this LineRenderer lineRenderer)
    {
        if (lineRenderer.positionCount == 0) return;
        lineRenderer.positionCount--;
    }
    
    public static void UpdateLastPoint(this LineRenderer lineRenderer, Vector3 pos)
    {
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, pos);
    }

    public static List<Vector2> GenerateRegularShape(int sideCount, float radius, float rotation = 0)
    {
        List<Vector2> newPath = new List<Vector2>(sideCount);
        float rad = Mathf.PI / sideCount;
        if (sideCount % 2 == 1) rad = Mathf.PI / 2;
        if (sideCount % 4 == 2) rad = 0;

        for (int i = 0; i < sideCount; i++)
        {
            rad += 2 * Mathf.PI / sideCount;
            newPath.Add(new Vector2(Mathf.Cos(rad + rotation), Mathf.Sin(rad + rotation)) * radius);
        }

        return newPath;
    }

    public static bool IsIntersectOnLines(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2,
        out Vector2 intersection, bool checkHead = true)
    {
        // No case for when 2 vectors are on the same line. Possible bug.
        intersection = Vector2.zero;

        if (Mathfs.Approximately((a1 - a2).normalized, (b1 - b2).normalized))
        {
            return false;
        }

        try
        {
            intersection.x =
                (b1.y - a1.y - b1.x * ((b2.y - b1.y) / (b2.x - b1.x)) +
                 a1.x * ((a2.y - a1.y) / (a2.x - a1.x))) /
                ((a2.y - a1.y) / (a2.x - a1.x) - (b2.y - b1.y) / (b2.x - b1.x));
            intersection.y = (a2.y - a1.y) / (a2.x - a1.x) * (intersection.x - a1.x) + a1.y;
        }
        catch
        {
            return false;
        }

        if (checkHead)
            return intersection.x.IsInBetweenInclusive(a1.x, a2.x) &&
                   intersection.x.IsInBetweenInclusive(b1.x, b2.x) &&
                   intersection.y.IsInBetweenInclusive(a1.y, a2.y) &&
                   intersection.y.IsInBetweenInclusive(b1.y, b2.y);
        else
            return intersection.x.IsInBetweenExclusive(a1.x, a2.x) &&
                   intersection.x.IsInBetweenExclusive(b1.x, b2.x) &&
                   intersection.y.IsInBetweenExclusive(a1.y, a2.y) &&
                   intersection.y.IsInBetweenExclusive(b1.y, b2.y);
    }

    public static Vector2? GetIntersection(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
    {
        var intersection = Vector2.zero;

        if (Mathfs.Approximately((a1 - a2).normalized, (b1 - b2).normalized))
        {
            Debug.Log("approx");
            return null;
        }

        if (Mathf.Approximately(a1.x, a2.x))
        {
            intersection.x = a1.x;
            intersection.y = (b2.y - b1.y) / (b2.x - b1.x) * (intersection.x - b1.x) + b1.y;
        }
        else if (Mathf.Approximately(b1.x, b2.x))
        {
            intersection.x = b1.x;
            intersection.y = (a2.y - a1.y) / (a2.x - a1.x) * (intersection.x - a1.x) + a1.y;
        }
        else
        {
            intersection.x =
                (b1.y - a1.y - b1.x * ((b2.y - b1.y) / (b2.x - b1.x)) +
                 a1.x * ((a2.y - a1.y) / (a2.x - a1.x))) /
                ((a2.y - a1.y) / (a2.x - a1.x) - (b2.y - b1.y) / (b2.x - b1.x));
            intersection.y = (a2.y - a1.y) / (a2.x - a1.x) * (intersection.x - a1.x) + a1.y;
        }

        return intersection;
    }

    public static bool TryGetIntersectionOneLineExtended(Vector2 extendA, Vector2 extendB, Vector2 lineA,
        Vector2 lineB, out Vector2 intersection)
    {
        Vector2? test = GetIntersection(extendA, extendB, lineA, lineB);
        intersection = test.GetValueOrDefault();
        Debug.Log("Intersection " + "extendA: " + extendA + "extendB: " + extendB + "lineA: " + lineA +
                  "lineB: " + lineB + " is " + intersection);
        return test.HasValue && IsBetweenPointsInclusive(intersection, lineA, lineB);
    }

    public static bool IsBetweenPointsInclusive(this Vector2 point, Vector2 a, Vector2 b)
    {
        Debug.Log("Between " + point + ", " + a + ", " + b + ": " + Math.Abs(Vector2.Distance(point, a) +
            Vector2.Distance(point, b) - Vector2.Distance(a, b)));
        return Math.Abs(Vector2.Distance(point, a) + Vector2.Distance(point, b) -
                        Vector2.Distance(a, b)) < 0.001f;
    }

    public static bool IsInsideTriangle(this Vector2 point, Vector2 a, Vector2 b, Vector2 c,
        float errorTolerance = -1f)
    {
        float s1 = Sign(point, a, b);
        float s2 = Sign(point, b, c);
        float s3 = Sign(point, c, a);

        // if (errorTolerance < 0) errorTolerance = Mathf.Epsilon;
        //
        // if (Mathf.Abs(s1) <= errorTolerance || Mathf.Abs(s2) <= errorTolerance ||
        //     Mathf.Abs(s3) <= errorTolerance) return true;

        return (s1 < 0 && s2 < 0 && s3 < 0) || (s1 > 0 && s2 > 0 && s3 > 0);

        float Sign(Vector2 a, Vector2 b, Vector2 c)
        {
            return (a.x - c.x) * (b.y - c.y) - (b.x - c.x) * (a.y - c.y);
        }
    }

    public static bool IsLineBetweenPoints(Vector2 center, Vector2 lineEnd, Vector2 pointA,
        Vector2 pointB)
    {
        var ALine = (pointA - center).normalized;
        var BLine = (pointB - center).normalized;
        var testLine = (lineEnd - center).normalized;
        return Mathf.Approximately(Vector2.Angle(testLine, ALine) + Vector2.Angle(testLine, BLine),
            Vector2.Angle(ALine, BLine));
    }

    public static float SignedAngle(this Vector2 center, Vector2 from, Vector2 to)
    {
        return Vector2.SignedAngle(from - center, to - center);
    }

    public static float Angle(this Vector2 center, Vector2 from, Vector2 to)
    {
        return Vector2.Angle(from - center, to - center);
    }

    public static List<T> RemoveRange<T>(this List<T> list, List<T> removerList)
    {
        foreach (var remover in removerList) list.Remove(remover);
        return list;
    }

    public static Vector2 Reflection(this Vector2 v, Vector2 center)
    {
        return 2 * center - v;
    }

    public static int Orientation(Vector2 a, Vector2 b, Vector2 c)
    {
        float val = (b.y - a.y) * (c.x - b.x) - (c.y - b.y) * (b.x - a.x);
        if (Mathf.Approximately(val, 0f)) return 0;
        if (val > 0f) return 1;
        return -1;
    }

    public static Vector2 ClampMagnitude(this Vector2 v, float minMagnitude, float maxMagnitude)
    {
        var mag = v.magnitude;
        if (mag < minMagnitude) return v.normalized * minMagnitude;
        if (mag > maxMagnitude) return v.normalized * maxMagnitude;
        return v;
    }

    public static Vector2 AddLength(this Vector2 v, float length)
    {
        if (v.magnitude == 0) return v;
        return v * (v.magnitude + length) / v.magnitude;
    }

    public static bool IsPointInPolygon(this Vector2 v, Vector2[] polygon)
    {
        bool orientation = Orientation(v, polygon[^1], polygon[0]) == 1;
        for (int i = 0; i < polygon.Length - 1; i++)
        {
            if (orientation != (Orientation(v, polygon[i], polygon[i + i]) == 1)) return false;
        }

        return true;
    }

    public static bool IsPointInPolygon(this Vector3 v, Vector2[] polygon)
    {
        bool orientation = Orientation(v, polygon[^1], polygon[0]) == 1;
        for (int i = 0; i < polygon.Length - 1; i++)
        {
            if (orientation != (Orientation(v, polygon[i], polygon[i + 1]) == 1)) return false;
        }

        return true;
    }

    public static Vector2[] ScalePolygon(this Vector2[] polygon, float scale)
    {
        Vector2[] newPolygon = new Vector2[polygon.Length];
        for (int i = 0; i < polygon.Length; i++)
        {
            newPolygon[i] = polygon[i] * scale;
        }

        return newPolygon;
    }

    public static Vector2 UnclampedLerp(Vector2 a, Vector2 b, float t)
    {
        return a * (1 - t) + b * t;
    }
}