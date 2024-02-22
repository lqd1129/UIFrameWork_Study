using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowBehavior
{
    public GameObject gameObject { get; set; }//当前窗口物体
    public Transform transform { get; set; }//代表自己
    public Canvas Canvas { get; set; }
    public string Name {  get; set; }
    public bool Visible { get; set; }
    public virtual void OnAwake() { } //只会在物体创建的时候执行一次
    public virtual void OnShow() { } //在物体显示的时候执行一次
    public virtual void OnUpdate() { }
    public virtual void OnHide() { } //在物体隐藏的时候执行一次
    public virtual void OnDestroy() { } //在物体销毁的时候执行一次
    public virtual void SetVisible(bool isVisible) { } //设置物体的可见性

}
