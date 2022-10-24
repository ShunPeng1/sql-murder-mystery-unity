using System;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using TMPro;
using UnityEngine;

public class Test : MonoBehaviour
{
    private TMP_Text text;
    [SerializeField] private Rectangle rectangle;
    private void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        Debug.Log(text.textBounds.size);
        rectangle.Width = text.textBounds.size.x;
        rectangle.Height = text.textBounds.size.y;
    }
}
