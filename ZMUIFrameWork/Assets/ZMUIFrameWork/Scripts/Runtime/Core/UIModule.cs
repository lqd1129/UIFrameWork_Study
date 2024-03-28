using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIModule
{
    private static UIModule _instance;
    public  static UIModule Instance { get{ if (_instance == null) {_instance = new UIModule(); } return _instance; } }

    private Camera mUICamera;
    private Transform mUIRoot;

    private Dictionary<string , WindowBase> mAllWindowDic = new Dictionary<string, WindowBase>();//用字典存所有的窗口
    private List<WindowBase> mAllWindowList = new List<WindowBase>(); //所有窗口列表
    private List<WindowBase> mVisibleWindowList = new List<WindowBase>(); //所有可见窗口列表

    public  void Initialize()
    {
        mUICamera = GameObject.Find("UICamera").GetComponent<Camera>();
        mUIRoot = GameObject.Find("UIRoot").transform;
    }

    /// <summary>
    /// 弹出窗口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public  T PopUpWindow<T>() where T : WindowBase, new()
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
    /// 获取已经弹出的弹窗
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public  T GetWindow<T>()where T : WindowBase
    {
        System.Type type = typeof(T);
        foreach (var item in mVisibleWindowList)
        {
            if(item.Name == type.Name)
            {
                return (T)item;
            }
        }
        Debug.LogError("该窗口没有获取到" +  type.Name);
        return null;


    }

    /// <summary>
    /// 隐藏窗口
    /// </summary>
    /// <param name="wndName"></param>
    public void HideWindow(string wndName)
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
            SetWidnowMaskVisible();
            window.OnHide();
        }
    }
    public  void HideWindow<T>() where T: WindowBase
    {
        HideWindow(typeof(T).Name);
    }

    private void DestroyWindow(string wndName)
    {
        WindowBase window = GetWindow(wndName);
        DestroyWindow(window);

    }
    public  void DestroyWindow<T>() where T : WindowBase
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
            SetWidnowMaskVisible();
            window.OnHide();
            window.OnDestroy();
            GameObject.Destroy(window.gameObject);
            
        }
    }

    public  void DestoryAllWindow(List<string> filterlist = null)
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
        //1.生成对应窗口预制体
        GameObject nWnd = TempLoadWindow(wndName);
        //2.初始化对应管理类
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
            SetWidnowMaskVisible();
            return windowBase;
        }
        Debug.LogError("没有加载到对应的窗口 窗口名字" + wndName);
        return null;
    }
    /// <summary>
    /// 显示窗口
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
                SetWidnowMaskVisible();
                window.OnShow();
            }
            return window;
        }
        else
            Debug.LogError(wndName + "窗口不存在，请调用PopUpWindow 进行弹出");
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
    private void SetWidnowMaskVisible()
    {
        if (!UISetting.Instance.SINGMASK_SYSTEM)
        {
            return;
        }
        WindowBase maxOrderWndBase = null;//最大渲染层级的窗口
        int maxOrder = 0;//最大渲染层级
        int maxIndex = 0;//最大排序下标 在相同父节点下的位置下标
        //1.关闭所有窗口的Mask 设置为不可见
        //2.从所有可见窗口中找到一个层级最大的窗口，把Mask设置为可见
        for (int i = 0; i < mVisibleWindowList.Count; i++)
        {
            WindowBase window = mVisibleWindowList[i];
            if (window != null && window.gameObject != null)
            {
                window.SetMaskVisible(false);
                if (maxOrderWndBase == null)
                {
                    maxOrderWndBase = window;
                    maxOrder = window.Canvas.sortingOrder;
                    maxIndex = window.transform.GetSiblingIndex();
                }
                else
                {
                    //找到最大渲染层级的窗口，拿到它
                    if (maxOrder < window.Canvas.sortingOrder)
                    {
                        maxOrderWndBase = window;
                        maxOrder = window.Canvas.sortingOrder;
                    }
                    //如果两个窗口的渲染层级相同，就找到同节点下最靠下一个物体，优先渲染Mask
                    else if (maxOrder == window.Canvas.sortingOrder && maxIndex < window.transform.GetSiblingIndex())
                    {
                        maxOrderWndBase = window;
                        maxIndex = window.transform.GetSiblingIndex();
                    }
                }
            }
        }
        if (maxOrderWndBase != null)
        {
            maxOrderWndBase.SetMaskVisible(true);
        }
    }

    /// <summary>
    /// 加载窗口
    /// </summary>
    /// <param name="wndName"></param>
    /// <returns></returns>
    public  GameObject TempLoadWindow(string wndName)
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
