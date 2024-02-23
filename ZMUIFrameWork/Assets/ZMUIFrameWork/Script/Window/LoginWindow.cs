using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginWindow : WindowBase
{
    public override void OnAwake()
    {
        base.OnAwake();
        Debug.Log("LoginWindow OnAwake");
    }

    public override void OnShow()
    {
        base.OnShow();  
    }

    public override void OnHide()
    {
        base.OnHide();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }
    public void Test1()
    {
        Debug.Log("GetWindow≤‚ ‘");
    }
}
