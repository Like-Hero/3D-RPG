using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{

    public KeyCode changeCameraKey;
    public List<GameObject> cameras;
    public List<String> camerasName;
    private int nowCameraIndex;

    private string nowCameraName;
    private GameObject nowCamera;
    public string NowCameraName { get { return nowCameraName; } }
    public GameObject NowCamera { get { return nowCamera; } }
    protected override void Awake()
    {
        base.Awake();
        //DontDestroyOnLoad();
    }
    private void Update()
    {
        if (Input.GetKeyDown(changeCameraKey))
        {
            ChangeCamera();
        }
        nowCameraName = camerasName[nowCameraIndex];
    }

    private void ChangeCamera()
    {
        nowCameraIndex++;
        if (nowCameraIndex == cameras.Count)
        {
            nowCameraIndex = 0;
        }
        nowCamera = cameras[nowCameraIndex];
        for(int i = 0; i < cameras.Count; i++)
        {
            if (i == nowCameraIndex)
            {
                cameras[nowCameraIndex].SetActive(true);
            }
            else
            {
                cameras[i].SetActive(false);
            }
        }
    }
}
