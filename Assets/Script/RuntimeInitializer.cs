using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeInitializer : MonoBehaviour {

	[RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
    private static void InitializeBeforeSceneLoad() {
        // ゲーム中に常に存在するオブジェクトを読み込み、およびシーンの変更時にも破棄されないようにする。
        GameObject audio = GameObject.Instantiate(Resources.Load("Audio")) as GameObject;
        GameObject.DontDestroyOnLoad(audio);
    }
}
