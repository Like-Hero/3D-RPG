using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : Singleton<CanvasManager>
{
    private Text cameraNameText;
    protected override void Awake()
    {
        base.Awake();
        cameraNameText = transform.Find("CameraNameText").GetComponent<Text>();
        //DontDestroyOnLoad();
    }
    private void Update()
    {
        SetCameraNameText();
    }

    private void SetCameraNameText()
    {
        cameraNameText.text = CameraManager.Ins.NowCameraName;
    }
}
