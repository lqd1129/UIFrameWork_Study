using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZMUIMain : MonoBehaviour
{
    private void Awake()
    {
        UIModule.Instance.Initialize();
    }
    void Start()
    {
        UIModule.Instance.PopUpWindow<LoginWindow>();

        LoginWindow loginWindow = UIModule.Instance.GetWindow<LoginWindow>();
        loginWindow.Test1();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            UIModule.Instance.HideWindow<LoginWindow>();    
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            UIModule.Instance.DestoryAllWindow(new List<string> { "LoginWindow"});
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            UIModule.Instance.DestroyWindow<LoginWindow>();
        }
    }
}
