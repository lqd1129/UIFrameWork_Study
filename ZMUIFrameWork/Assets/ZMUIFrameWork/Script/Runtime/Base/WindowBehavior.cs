using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowBehavior
{
    public GameObject gameObject { get; set; }//��ǰ��������
    public Transform transform { get; set; }//�����Լ�
    public Canvas Canvas { get; set; }
    public string Name {  get; set; }
    public bool Visible { get; set; }
    public virtual void OnAwake() { } //ֻ�������崴����ʱ��ִ��һ��
    public virtual void OnShow() { } //��������ʾ��ʱ��ִ��һ��
    public virtual void OnUpdate() { }
    public virtual void OnHide() { } //���������ص�ʱ��ִ��һ��
    public virtual void OnDestroy() { } //���������ٵ�ʱ��ִ��һ��
    public virtual void SetVisible(bool isVisible) { } //��������Ŀɼ���

}
