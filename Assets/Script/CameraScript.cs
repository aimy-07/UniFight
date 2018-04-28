using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

	Transform myPlayer;
	Transform enemyPlayer;
	bool isReady = false;
	Vector2 offset = new Vector2(2, 2);

	private float screenAspect = 0; 
	private Camera cam;

	void Awake() {
		screenAspect = (float)Screen.height / Screen.width;
		cam = GetComponent<Camera> ();
	}

	void Update () {
		if (!isReady) {
			if (GameObject.FindWithTag("myPlayer") != null && GameObject.FindWithTag("enemyPlayer") != null) {
				myPlayer = GameObject.FindWithTag("myPlayer").transform;
				enemyPlayer = GameObject.FindWithTag("enemyPlayer").transform;
				isReady = true;
			}
		}
		if (PhotonManager.isPlaying) {
			UpdateCameraPosition ();
			UpdateOrthographicSize ();
		}
	}

	void UpdateCameraPosition() {
		// 2点間の中心点からカメラの位置を更新
		Vector3 center = Vector3.Lerp (myPlayer.position, enemyPlayer.position, 0.5f);
		transform.position = center + new Vector3(0, 1.6f, -10);
	}

	void UpdateOrthographicSize() {
		// ２点間のベクトルを取得
		Vector3 targetsVector = AbsPositionDiff (myPlayer, enemyPlayer) + (Vector3)offset;

		// アスペクト比が縦長ならyの半分、横長ならxとアスペクト比でカメラのサイズを更新
		float targetsAspect = targetsVector.y / targetsVector.x;
		float targetOrthographicSize = 0;
		if ( screenAspect < targetsAspect) {
			targetOrthographicSize = targetsVector.y * 0.5f;
		} else {
			targetOrthographicSize = targetsVector.x * (1/cam.aspect) * 0.5f;
		}
		cam.orthographicSize =  targetOrthographicSize;
	}

	Vector3 AbsPositionDiff(Transform target1, Transform target2) {
		Vector3 targetsDiff = myPlayer.position - enemyPlayer.position;
		return new Vector3(Mathf.Abs(targetsDiff.x), Mathf.Abs(targetsDiff.y));
	}
}
