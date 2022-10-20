using System;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using TMPro;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public RectTransform RectTransform;
    [SerializeField] private TMP_Text text;
    [SerializeField] private RectTransform textRect;
    [SerializeField] private Rectangle rectangle;
    [SerializeField] private RectTransform rectangleRect;

    public Vector2 InitText(string data)
    {
        data = string.Join(" ", data.Split(new [] {' ', '\n'}, StringSplitOptions.RemoveEmptyEntries));
        text.text = data;
        text.ForceMeshUpdate();
        return text.textBounds.size;
    }

    public void InitSize(float width, float height)
    {
        width += VisualManager.Instance.Padding * 2;
        height += VisualManager.Instance.Padding * 2;

        rectangle.Width = width;
        rectangle.Height = height;
        rectangleRect.anchoredPosition = new Vector2(-width / 2, height / 2);

        textRect.anchoredPosition += Vector2.right * VisualManager.Instance.Padding;
        textRect.sizeDelta = new Vector2(width, height);

        RectTransform.sizeDelta = new Vector2(width, height);
    }
}