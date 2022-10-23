using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class InfoPanel : PersistentSingleton<InfoPanel>
{
    private bool isShowing = true;
    [SerializeField] private Image fader;
    [SerializeField] private float timer;
    [SerializeField] private List<Transform> lights;
    [SerializeField] private Transform detective;
    private List<Tween> lightTweens = new List<Tween>();

    private void Start()
    {
        Show();
        foreach (var light in lights)
        {
            lightTweens.Add(light.DORotate(new Vector3(0, 0, Random.Range(-40, 40)), Random.Range(5, 10))
                    .SetEase(Ease.InOutCubic)
                    .SetSpeedBased(true));
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && isShowing)
        {
            Hide();
        }

        //detective.transform.localScale = new Vector3(1, Mathf.Sin(Time.time) * 0.02f + 1, 1);

        for (int i = 0; i < lightTweens.Count; i++)
        {
            if (lightTweens[i].IsPlaying()) continue;
            lightTweens[i].Complete();
            lightTweens[i] = lights[i]
                .DORotate(new Vector3(0, 0, Random.Range(-40, 40)), Random.Range(5, 10))
                .SetEase(Ease.InOutCubic)
                .SetSpeedBased(true);
        }
    }

    public void Show()
    {
        fader.transform.position = Vector3.zero;
        fader.DOFade(1f, timer)
            .OnComplete(() =>
            {
                transform.position = Vector3.zero;
                isShowing = true;
                fader.DOFade(0f, timer).OnComplete(() => fader.transform.position = Vector3.right * 20);
            });
    }

    public void Hide()
    {
        fader.transform.position = Vector3.zero;
        fader.DOFade(1f, timer)
            .OnComplete(() =>
            {
                transform.position = Vector3.right * 20;
                isShowing = false;
                fader.DOFade(0f, timer).OnComplete(() => fader.transform.position = Vector3.right * 20);
            });
    }
}