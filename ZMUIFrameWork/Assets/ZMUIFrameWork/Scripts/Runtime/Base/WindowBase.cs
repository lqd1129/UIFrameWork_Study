using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WindowBase : WindowBehavior
{
    private List<Button> mAllButtonList = new List<Button>();//���е�Button�б�
    private List<Toggle> mToggleList = new List<Toggle>();//���е�Toggle�б�
    private List<TMP_InputField> mInputFieldList = new List<TMP_InputField>();//���е������ť

    private CanvasGroup mUIMask;
    private Transform mUIContent;

    private void InitializeBaseComponent()
    {
        mUIMask = transform.Find("UIMask").GetComponent<CanvasGroup>();
        mUIContent = transform.Find("UIContent");
    }

    #region �������ں���
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
        gameObject.SetActive(isVisible);//��ʱ����
        Visible = isVisible;
    }

    #region �¼�����
    /// <summary>
    /// Ϊ��ť����¼�
    /// </summary>
    /// <param name="btn">��ť����</param>
    /// <param name="action">��Ҫ��ӵ��¼�</param>
    public void AddButtonClickListener( Button btn, UnityAction action)
    {
        if (btn != null)
        {
            //���Button�б���û�иð�ť ����ӽ�ȥ
            if (!mAllButtonList.Contains(btn))
            {
                mAllButtonList.Add(btn);
            }
            //�ȰѰ�ť������¼����Ƴ�������ȥ�����Ҫ�������¼�
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(action);
        }
    }
    /// <summary>
    /// ΪToggle����¼�
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
    /// Ϊ���������¼�
    /// </summary>
    /// <param name="input">�����</param>
    /// <param name="onChangeAction">����������ʱ���¼�</param>
    /// <param name="endAction">�������ʱ���¼�</param>
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
    /// �Ƴ����а�ť�󶨵������¼�����
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
