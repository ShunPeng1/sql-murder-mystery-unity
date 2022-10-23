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

    private Transform tablePointTop;
    private Transform tablePointBottom;
    private Transform itemPointTop;
    private Transform itemPointBottom;

    public void SetPoint(Transform tablePointTop, Transform tablePointBottom, Transform itemPointTop,
        Transform itemPointBottom)
    {
        this.tablePointTop = tablePointTop;
        this.tablePointBottom = tablePointBottom;
        this.itemPointTop = itemPointTop;
        this.itemPointBottom = itemPointBottom;

        UpdateShape();
    }

    public void UpdateShape()
    {
        List<Vector3> topList = new List<Vector3>();

        DOCurve.CubicBezier.GetSegmentPointCloud(topList, tablePointTop.position,
            tablePointTop.position + topStartControl, itemPointTop.position,
            itemPointTop.position + topEndControl, 20);

        List<Vector3> bottomList = new List<Vector3>();
        DOCurve.CubicBezier.GetSegmentPointCloud(bottomList, tablePointBottom.position,
            tablePointBottom.position + bottomStartControl, itemPointBottom.position,
            itemPointBottom.position + bottomEndControl, 20);

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