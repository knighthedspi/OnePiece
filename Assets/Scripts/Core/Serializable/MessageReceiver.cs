using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// インスペクタでコールバックを設定したかったので、
/// NGUIのEventDelegateを参考に作成。
/// </summary>
[Serializable]
public class MessageReciver {

    /// <summary>
    /// 呼ばれるコールバックメソッドが実装されているスクリプトが
    /// アタッチされているGameObject
    /// </summary>
    public MonoBehaviour target;

    /// <summary>
    /// 呼ばれるコールバックメソッドの名前
    /// </summary>
    public string methodName;

    /// <summary>
    /// この <see cref="MessageReciver"/> にちゃんとメンバーが設定されているかどうか
    /// </summary>
    public bool isValid { get { return target != null && !string.IsNullOrEmpty(methodName); } }

    /// <summary>
    /// コールバックメソッド呼び出し
    /// </summary>
    public void SendMessage() {
        if(!isValid) return;
        target.SendMessage(methodName);
    }

    /// <summary>
    /// コールバックメソッド呼び出し
    /// </summary>
    /// <param name="value">Value.</param>
    public void SendMessage(object value) {
        if(!isValid) return;
        target.SendMessage(methodName, value);
    }
}
