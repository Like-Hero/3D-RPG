using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

//[System.Serializable]
//public class EventVector3 : UnityEvent<Vector3> { }

public class MouseManager : MonoBehaviour
{
    private static MouseManager _ins;
    public static MouseManager Ins { get { return _ins; } }

    private RaycastHit hitInfo;
    //public EventVector3 onMouseClicked;//拖拽方式,不太好用
    public event Action<Vector3> OnMouseClicked;//注册方式
    public event Action<GameObject> OnEnemyClicked;


    public Texture2D point;
    public Texture2D doorway;
    public Texture2D attack;
    public Texture2D targer;
    public Texture2D arrow;

    private void Awake()
    {
        if(_ins == null)
        {
            _ins = this;
        }
    }

    private void Update()
    {
        SetCursorTexture();
        MouseControl();
    }

    private void SetCursorTexture()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hitInfo))
        {
            //修改鼠标的图标
            switch (hitInfo.collider.gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(targer, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Enemy":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.ForceSoftware);
                    break;
                default:
                    Cursor.SetCursor(arrow, new Vector2(16, 16), CursorMode.ForceSoftware);
                    break;
            }
        }
    }
    private void MouseControl()
    {
        if (Input.GetMouseButtonDown(0) && hitInfo.collider != null)
        {
            string tag = hitInfo.collider.tag;
            if (tag.Equals("Ground"))
            {
                OnMouseClicked?.Invoke(hitInfo.point);
            }
            if (tag.Equals("Enemy"))
            {
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
            }

        }
    }
}
