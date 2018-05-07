using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventScript : MonoBehaviour {

	public BoxCollider smallAttackCol;
	public BoxCollider bigAttackCol;

	GameObject myPlayer;
	GameObject enemyPlayer;
	AudioSource myDamageSound;
	AudioSource enemyDamageSound;


	void Start () {
		if (PhotonManager.phase == PhotonManager.PHASE.other) {
			myPlayer = GameObject.FindWithTag("myPlayer").gameObject;
			enemyPlayer = GameObject.FindWithTag("enemyPlayer").gameObject;
			myDamageSound = myPlayer.transform.Find("audio").gameObject.GetComponents<AudioSource>()[6];
			enemyDamageSound = enemyPlayer.transform.Find("audio").gameObject.GetComponents<AudioSource>()[6];
		}
	}
	
	void Update () {
		if (PhotonManager.phase != PhotonManager.PHASE.other) {
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
