using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Unity組み込みのシンプルなフルスクリーンムービープレイヤー
/// 使い勝手は悪いかも
/// ※ Windowsでは動かない
/// </summary>
public class SimpleMoviePlayer : MonoBehaviour {

    /// <summary>
    /// ムービーファイル名（拡張子付）
    /// ムービーファイル名は Assets/StreamingAssets の中にある必要がある
    /// </summary>
    public string movieName;

    /// <summary>
    /// プレイヤーの背景色
    /// </summary>
    public Color backgroudColor = Color.black;

    /// <summary>
    /// コントロールUIボタンの表示設定
    /// </summary>
    public FullScreenMovieControlMode controlMode = FullScreenMovieControlMode.CancelOnInput;

    /// <summary>
    /// 拡大設定
    /// </summary>
    public FullScreenMovieScalingMode scalingMode = FullScreenMovieScalingMode.AspectFit;

    /// <summary>
    /// trueの場合、画面表示時に自動再生開始
    /// </summary>
    public bool playAuto = false;

#if UNITY_EDITOR
    public bool waitForDebug = false;
#endif

    /// <summary>
    /// 再生終了後の呼ばれるコールバック
    /// </summary>
    public MessageReciver onFinish;

    void Start() {
        if(playAuto) Play();
    }

    /// <summary>
    /// 再生開始
    /// </summary>
	public void Play () {
        if(string.IsNullOrEmpty(movieName)) return;

        Debug.Log("movie start");
        Handheld.PlayFullScreenMovie(movieName, backgroudColor, controlMode, scalingMode);
        Debug.Log("movie end");

#if UNITY_EDITOR
        if(waitForDebug){
            StartCoroutine(DelayCallBack());
            return;
        }
#endif
        if(onFinish != null) onFinish.SendMessage();
	}

#if UNITY_EDITOR
    IEnumerator DelayCallBack() {
        yield return new WaitForSeconds(3.0f);
        if(onFinish != null) onFinish.SendMessage();
    }
#endif
}
