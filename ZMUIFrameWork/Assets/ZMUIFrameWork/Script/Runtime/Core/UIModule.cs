using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIModule
{
    private static UIModule _instance;
    public static UIModule Instance { get{ if (_instance == null) {_instance = new UIModule(); } return _instance; } }

    private Camera mUICamera;
    private Transform mUIRoot;

    private Dictionary<string , WindowBase> mAllWindowDic = new Dictionary<string, WindowBase>();//���ֵ�����еĴ���
    private List<WindowBase> mAllWindowList = new List<WindowBase>(); //���д����б�
    private List<WindowBase> mVisibleWindowList = new List<WindowBase>(); //���пɼ������б�

    public void Initialize()
    {
        mUICamera = GameObject.Find("UICamera").GetComponent<Camera>();
        mUIRoot = GameObject.Find("UIRoot").transform;
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T PopUpWindow<T>() where T : WindowBase, new()
    {
        System.Type type = typeof(T);
        string wndName = type.Name;
        WindowBase wnd =  GetWindow(wndName);
        if(wnd != null )
        {
           return  ShowWindow(wndName) as T ;
        }
        T t = new T();
        return InitializeWindow(t,wndName) as T ;

    }

    /// <summary>
    /// ��ȡ�Ѿ������ĵ���
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetWindow<T>()where T : WindowBase
    {
        System.Type type = typeof(T);
        foreach (var item in mVisibleWindowList)
        {
            if(item.Name == type.Name)
            {
                return (T)item;
            }
        }
        Debug.LogError("�ô���û�л�ȡ��" +  type.Name);
        return null;


    }

    /// <summary>
    /// ���ش���
    /// </summary>
    /// <param name="wndName"></param>
    private void HideWindow(string wndName)
    {
        WindowBase window = GetWindow(wndName);
        HideWindow(window);
        
    }
    private void HideWindow(WindowBase window)
    {
        if(window != null && window.Visible)
        {
            mVisibleWindowList.Remove(window);
            window.SetVisible(false);
            window.OnHide();
        }
    }
    public void HideWindow<T>() where T: WindowBase
    {
        HideWindow(typeof(T).Name);
    }

    private void DestroyWindow(string wndName)
    {
        WindowBase window = GetWindow(wndName);
        DestroyWindow(window);

    }
    public void DestroyWindow<T>() where T : WindowBase
    {
        DestroyWindow(typeof(T).Name);
    }
    private void DestroyWindow(WindowBase window)
    {
        if(window != null)
        {
            if (mAllWindowDic.ContainsKey(window.Name))
            {
                mAllWindowDic.Remove(window.Name);
                mAllWindowList.Remove(window);
                mVisibleWindowList.Remove(window);
            }
            window.SetVisible(false);
            window.OnHide();
            window.OnDestroy();
            GameObject.Destroy(window.gameObject);
            
        }
    }

    public void DestoryAllWindow(List<string> filterlist = null)
    {
        for(int i = mAllWindowList.Count-1; i >= 0;i--)
        {
            WindowBase window = mAllWindowList[i];
            if(window == null ||(filterlist!= null && filterlist.Contains(window.Name)))
            {
                continue;
            }
            DestroyWindow(window);
            
        }
        Resources.UnloadUnusedAssets();
    }



    private WindowBase InitializeWindow(WindowBase windowBase , string wndName)
    {
        //1.���ɶ�Ӧ����Ԥ����
        GameObject nWnd = TempLoadWindow(wndName);
        //2.��ʼ����Ӧ������
        if(nWnd != null )
        {
            windowBase.gameObject = nWnd;
            windowBase.transform = nWnd.transform;
            windowBase.Canvas = nWnd.GetComponent<Canvas>();
            windowBase.Canvas.worldCamera = mUICamera;
            windowBase.transform.SetAsLastSibling();
            windowBase.Name = nWnd.name;
            windowBase.OnAwake();
            windowBase.SetVisible(true);
            windowBase.OnShow();
            RectTransform rectTrans  = nWnd.GetComponent<RectTransform>();
            rectTrans.anchorMax = Vector2.one;
            rectTrans.offsetMax = Vector2.zero;
            rectTrans.offsetMin = Vector2.zero;
            mAllWindowDic.Add(wndName, windowBase);
            mAllWindowList.Add(windowBase);
            mVisibleWindowList.Add(windowBase);
            return windowBase;
        }
        Debug.LogError("û�м��ص���Ӧ�Ĵ��� ��������" + wndName);
        return null;
    }
    /// <summary>
    /// ��ʾ����
    /// </summary>
    /// <param name="wndName"></param>
    /// <returns></returns>
    private WindowBase ShowWindow(string wndName)
    {
        WindowBase window = null;
        if (mAllWindowDic.ContainsKey(wndName))
        {
            window = mAllWindowDic[wndName];
            if (window.gameObject != null && window.Visible == false)
            {
                mVisibleWindowList.Add(window);
                window.transform.SetAsLastSibling();
                window.SetVisible(true);
                window.OnShow();
            }
            return window;
        }
        else
            Debug.LogError(wndName + "���ڲ����ڣ������PopUpWindow ���е���");
        return null;
    }

    private WindowBase GetWindow(string wndName)
    {
        if (mAllWindowDic.ContainsKey(wndName))
        { 
            return mAllWindowDic[wndName];
        }
        return null;    
    }
    /// <summary>
    /// ���ش���
    /// </summary>
    /// <param name="wndName"></param>
    /// <returns></returns>
    public GameObject TempLoadWindow(string wndName)
    {
        GameObject window = GameObject.Instantiate(Resources.Load<GameObject>("Window/" + wndName));
        window.transform.SetParent(mUIRoot);
        window.transform.localScale = Vector3.one;
        window.transform.localPosition = Vector3.zero;
        window.transform.localRotation = Quaternion.identity;
        window.name = wndName;
        return window;
    }
}
