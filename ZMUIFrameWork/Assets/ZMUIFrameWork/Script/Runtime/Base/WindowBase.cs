using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WindowBase : WindowBehavior
{
    private List<Button> mAllButtonList = new List<Button>();//所有的Button列表
    private List<Toggle> mToggleList = new List<Toggle>();//所有的Toggle列表
    private List<TMP_InputField> mInputFieldList = new List<TMP_InputField>();//所有的输入框按钮

    private CanvasGroup mUIMask;
    private Transform mUIContent;

    private void InitializeBaseComponent()
    {
        mUIMask = transform.Find("UIMask").GetComponent<CanvasGroup>();
        mUIContent = transform.Find("UIContent");
    }

    #region 生命周期函数
    public override void OnAwake()
    {
        base.OnAwake();
        InitializeBaseComponent();
    }
    public override void OnShow()
    {
        base.OnShow();
    }
    public override void OnHide()
    {
        base.OnHide();
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        RemoveAllButtonListener();
        RemoveAllInputListener();
        RemoveAllToggleListener();
        mAllButtonList.Clear();
        mToggleList.Clear();
        mInputFieldList.Clear();
    }
    #endregion

    public void SetMaskVisible(bool isVisible)
    {
        if (!UISetting.Instance.SINGMASK_SYSTEM)
        {
            return;
        }
        mUIMask.alpha = isVisible ? 1f : 0f;
    }
    public override void SetVisible(bool isVisible)
    {
        gameObject.SetActive(isVisible);//临时代码
        Visible = isVisible;
    }

    #region 事件管理
    /// <summary>
    /// 为按钮添加事件
    /// </summary>
    /// <param name="btn">按钮本身</param>
    /// <param name="action">需要添加的事件</param>
    public void AddButtonClickListener( Button btn, UnityAction action)
    {
        if (btn != null)
        {
            //如果Button列表中没有该按钮 就添加进去
            if (!mAllButtonList.Contains(btn))
            {
                mAllButtonList.Add(btn);
            }
            //先把按钮本身的事件先移除掉，再去添加需要监听的事件
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(action);
        }
    }
    /// <summary>
    /// 为Toggle添加事件
    /// </summary>
    /// <param name="toggle"></param>
    /// <param name="action"></param>
    public void AddToggleClickListener(Toggle toggle , UnityAction<bool , Toggle> action )
    {
        if(toggle != null)
        {
            if (!mToggleList.Contains(toggle))
            {
                mToggleList.Add(toggle);
            }
        }
        toggle.onValueChanged.RemoveAllListeners();
        toggle.onValueChanged.AddListener((isOn) => {
            action?.Invoke(isOn , toggle);
        });
    }
    /// <summary>
    /// 为输入框添加事件
    /// </summary>
    /// <param name="input">输入框</param>
    /// <param name="onChangeAction">当正在输入时的事件</param>
    /// <param name="endAction">输入结束时的事件</param>
    public void AddInputFleldListener(TMP_InputField input , UnityAction<string> onChangeAction , UnityAction<string> endAction )
    {
        if(input != null)
        {
            if(!mInputFieldList.Contains(input))
            {
                mInputFieldList.Add(input);
            }
            input.onValueChanged.RemoveAllListeners();
            input.onEndEdit.RemoveAllListeners();
            input.onValueChanged.AddListener(onChangeAction);
            input.onEndEdit.AddListener(endAction);

        }
    }
    /// <summary>
    /// 移除所有按钮绑定的所有事件监听
    /// </summary>
    public void RemoveAllButtonListener()
    {
        foreach (var item in mAllButtonList)
        {
            item.onClick.RemoveAllListeners();
        }
    }

    public void RemoveAllToggleListener()
    {
        foreach (var item in mToggleList)
        {
            item.onValueChanged.RemoveAllListeners();
        }
    }
    public void RemoveAllInputListener()
    {
        foreach (var item in mInputFieldList)
        {
            item.onValueChanged.RemoveAllListeners();
            item.onEndEdit.RemoveAllListeners();
        }
    }
    #endregion
}
