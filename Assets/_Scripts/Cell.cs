using System.Collections;
using System.Collections.Generic;
using Shapes;
using TMPro;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private TMP_Text text;
    [SerializeField] private Rectangle rectangle;

    public void Init(string data, float width, float height)
    {
        text.text = data;
        
        rectangle.Width = width;
        rectangle.Height = height;
        
        rectTransform.sizeDelta = new Vector2(width, height);
    }
}
