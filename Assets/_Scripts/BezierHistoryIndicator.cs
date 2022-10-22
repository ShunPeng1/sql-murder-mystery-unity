using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Shapes;
using UnityEngine;


public class BezierHistoryIndicator : MonoBehaviour
{
    [SerializeField] private Polygon polygon;

    [SerializeField] private Vector3 topStartControl;
    [SerializeField] private Vector3 topEndControl;
    [SerializeField] private Vector3 bottomStartControl;
    [SerializeField] private Vector3 bottomEndControl;

    private Vector3 tablePointTop;
    private Vector3 tablePointBottom;
    private Vector3 itemPointTop;
    private Vector3 itemPointBottom;
    private bool init = false;

    public void SetPoint(Vector3 tablePointTop, Vector3 tablePointBottom, Vector3 itemPointTop,
        Vector3 itemPointBottom)
    {
        this.tablePointTop = tablePointTop;
        this.tablePointBottom = tablePointBottom;
        this.itemPointTop = itemPointTop;
        this.itemPointBottom = itemPointBottom;
        init = true;
    }

    private void Update()
    {
        if (!init) return;

        List<Vector3> topList = new List<Vector3>();
        var tsc = Lerp(tablePointTop, itemPointTop, topStartControl);
        var tec = Lerp(tablePointTop, itemPointTop, topEndControl);
        DOCurve.CubicBezier.GetSegmentPointCloud(topList, tablePointTop, tsc, itemPointTop, tec, 20);

        List<Vector3> bottomList = new List<Vector3>();
        var bsc = Lerp(tablePointBottom, itemPointBottom, bottomStartControl);
        var bec = Lerp(tablePointBottom, itemPointBottom, bottomEndControl);
        DOCurve.CubicBezier.GetSegmentPointCloud(bottomList, tablePointBottom, bsc, itemPointBottom, bec,
            20);

        bottomList.Reverse();

        List<Vector2> list = new List<Vector2>();

        foreach (var point in topList)
        {
            list.Add(point);
        }

        foreach (var point in bottomList)
        {
            list.Add(point);
        }

        polygon.SetPoints(list);
    }

    public Vector2 Lerp(Vector2 a, Vector2 b, Vector2 t)
    {
        return new Vector2(Mathf.LerpUnclamped(a.x, b.x, t.x), Mathf.LerpUnclamped(a.y, b.y, t.y));
    }
}