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

    public void SetPoint(Vector3 tablePointTop, Vector3 tablePointBottom, Vector3 itemPointTop,
        Vector3 itemPointBottom)
    {
        List<Vector3> topList = new List<Vector3>();

        DOCurve.CubicBezier.GetSegmentPointCloud(topList, tablePointTop, tablePointTop + topStartControl,
            itemPointTop, itemPointTop + topEndControl, 20);

        List<Vector3> bottomList = new List<Vector3>();
        DOCurve.CubicBezier.GetSegmentPointCloud(bottomList, tablePointBottom,
            tablePointBottom + bottomStartControl, itemPointBottom, itemPointBottom + bottomEndControl,
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
}