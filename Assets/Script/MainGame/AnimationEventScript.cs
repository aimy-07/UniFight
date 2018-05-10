using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimationEventScript : MonoBehaviour {

	public BoxCollider smallAttackCol;
	public BoxCollider bigAttackCol;

	GameObject myPlayer;
	GameObject enemyPlayer;
	AudioSource myDamageSound;
	AudioSource enemyDamageSound;


	void Start () {
		if (SceneManager.GetActiveScene().name == "OfflineMain") {
			myPlayer = GameObject.FindWithTag("myPlayer").gameObject;
			enemyPlayer = GameObject.FindWithTag("enemyPlayer").gameObject;
			myDamageSound = myPlayer.transform.Find("audio").gameObject.GetComponents<AudioSource>()[6];
			enemyDamageSound = enemyPlayer.transform.Find("audio").gameObject.GetComponents<AudioSource>()[6];
		} else if (SceneManager.GetActiveScene().name == "Title"
				|| SceneManager.GetActiveScene().name == "Result"
				|| SceneManager.GetActiveScene().name == "OfflineResult"
				|| SceneManager.GetActiveScene().name == "SelectCostume") {
			Destroy(this);
		}
	}
	
	void Update () {
		if (SceneManager.GetActiveScene().name == "Main") {
			myPlayer = GameObject.FindWithTag("myPlayer").gameObject;
			myDamageSound = myPlayer.transform.Find("audio").gameObject.GetComponents<AudioSource>()[6];
		}
	}

	void SmallAttackStart() {
		smallAttackCol.enabled = true;
	}

	void SmallAttackEnd() {
		smallAttackCol.enabled = false;
	}

	void BigAttackStart() {
		bigAttackCol.enabled = true;
	}

	void BigAttackEnd() {
		bigAttackCol.enabled = false;
	}

	void Skill() {
		if (PhotonManager.phase == PhotonManager.PHASE.isPlaying) {
			// 通信対戦モード
			if (Mathf.Abs(Vector3.Distance(myPlayer.transform.position, GameObject.FindWithTag("enemyPlayer").gameObject.transform.position)) < 4) {
				if (transform.root.gameObject.tag == "myPlayer") {
					Debug.Log("スキル 成功！");
				} else {
					Debug.Log("ダメージ！スキル!!");
					myDamageSound.Play();
					myPlayer.SendMessage("Damaged", PhotonManager.SkillDamage);
				}
			}
		} else if (OfflineManager.isPlaying) {
			// シングルプレイモード
			if (Mathf.Abs(Vector3.Distance(myPlayer.transform.position, enemyPlayer.transform.position)) < 4) {
				if (transform.root.gameObject.tag == "myPlayer") {
					Debug.Log("スキル 成功！");
					enemyDamageSound.Play();
					enemyPlayer.SendMessage("Damaged", PhotonManager.SkillDamage);
				} else {
					Debug.Log("ダメージ！スキル!!");
					myDamageSound.Play();
					myPlayer.SendMessage("Damaged", PhotonManager.SkillDamage);
				}
			}
		}
	}
}
