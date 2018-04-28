using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class NetworkCharacter : Photon.MonoBehaviour {

	float myPos = 0;
 

	void Update() {
		if (gameObject.tag == "enemyPlayer") {
			myPos = GameObject.FindWithTag("myPlayer").gameObject.transform.position.x;
		}
	}
 

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if (stream.isWriting) {
			PlayerController playerController = GetComponent<PlayerController>();

			//キャラの見た目
			stream.SendNext(playerController.chara);
			stream.SendNext(playerController.hairColor);
			stream.SendNext(playerController.eyeColor);
			stream.SendNext(playerController.costumeColor);
			//位置と回転
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
			stream.SendNext(GetComponent<Rigidbody>().velocity);
			//HP
			stream.SendNext(playerController.hp);
			
			if (PhotonManager.isPlaying) {
				//アニメーション
				stream.SendNext(playerController.idleFlg);
				stream.SendNext(playerController.walkFlg);
				stream.SendNext(playerController.jumpDownFlg);
				stream.SendNext(playerController.smallAttackFlg);
				stream.SendNext(playerController.bigAttackFlg);
				stream.SendNext(playerController.skillFlg);
				stream.SendNext(playerController.avoidFlg);
				stream.SendNext(playerController.damageFlg);
				stream.SendNext(playerController.deathFlg);
				playerController.idleFlg = false;
				playerController.walkFlg = false;
				playerController.jumpDownFlg = false;
				playerController.smallAttackFlg = false;
				playerController.bigAttackFlg = false;
				playerController.skillFlg = false;
				playerController.avoidFlg = false;
				playerController.damageFlg = false;
				playerController.deathFlg = false;
			}

		} else {
			Debug.Log("Receivred : " + gameObject.tag);
			PlayerController playerController = GetComponent<PlayerController>();

			//キャラの見た目
			playerController.chara = (int)stream.ReceiveNext();
			playerController.hairColor = (int)stream.ReceiveNext();
			playerController.eyeColor = (int)stream.ReceiveNext();
			playerController.costumeColor = (int)stream.ReceiveNext();
			//位置と回転
			transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
			GetComponent<Rigidbody>().velocity = (Vector3)stream.ReceiveNext();
			//HP
			playerController.hp = (float)stream.ReceiveNext();
			
			if (PhotonManager.isPlaying) {
				//アニメーション
				Animator animator = transform.Find("UTC_Default").gameObject.GetComponent<Animator>();
				bool idleFlg = (bool)stream.ReceiveNext();
				bool walkFlg = (bool)stream.ReceiveNext();
				bool jumpDownFlg = (bool)stream.ReceiveNext();
				bool smallAttackFlg = (bool)stream.ReceiveNext();
				bool bigAttackFlg = (bool)stream.ReceiveNext();
				bool skillFlg = (bool)stream.ReceiveNext();
				bool avoidFlg = (bool)stream.ReceiveNext();
				bool damageFlg = (bool)stream.ReceiveNext();
				bool deathFlg = (bool)stream.ReceiveNext();
				if (idleFlg) {
        			animator.SetTrigger("Idle");
      			}
				if (walkFlg) {
        			animator.SetTrigger("Walk");
      			}
				if (jumpDownFlg) {
        			animator.SetTrigger("JumpDown");
      			}
      			if (smallAttackFlg) {
        			animator.SetTrigger("SmallAttack");
      			}
				if (bigAttackFlg) {
        			animator.SetTrigger("BigAttack");
      			}
				if (skillFlg) {
        			animator.SetTrigger("Skill");
      			}
				if (avoidFlg) {
        			if (transform.position.x > myPos) {
						Instantiate(Resources.Load("AvoidEffect/AvoidEffectRight" + playerController.chara) as GameObject, new Vector3(transform.position.x, transform.position.y, -1), Quaternion.identity);
						if (transform.position.x > 3 - 1) {
							transform.position = new Vector3(3, transform.position.y, 0);
						} else {
							transform.position = new Vector3(transform.position.x + 1, transform.position.y, 0);
						}
					} else {
						Instantiate(Resources.Load("AvoidEffect/AvoidEffectLeft" + playerController.chara) as GameObject, new Vector3(transform.position.x, transform.position.y, -1), Quaternion.identity);
						if (transform.position.x < -3 + 1) {
							transform.position = new Vector3(-3, transform.position.y, 0);
						} else {
							transform.position = new Vector3(transform.position.x - 1, transform.position.y, 0);
						}
					}
      			}
				if (damageFlg) {
        			animator.SetTrigger("Damage");
      			}
				if (deathFlg) {
        			animator.SetTrigger("Death");
      			}
			}
		}
	}
}
