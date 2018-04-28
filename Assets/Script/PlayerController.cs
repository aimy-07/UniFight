using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	public float flap = 550f;
    public float scroll = 10f;
    float direction = 0f;
	public bool isGround = true;

	Rigidbody rigid;
	Animator animator;
	float enemyPos = 0;
	GameObject enemyPlayer = null;

	PhotonView myPhotonView;
	public bool idleFlg;
	public bool walkFlg;
	public bool jumpDownFlg;
	public bool smallAttackFlg;
	public bool bigAttackFlg;
	public bool skillFlg;
	public bool avoidFlg;
	public bool damageFlg;
	public bool deathFlg;

	[System.NonSerialized] public int chara = -1;
	[System.NonSerialized] public int hairColor = -1;
	[System.NonSerialized] public int eyeColor = -1;
	[System.NonSerialized] public int costumeColor = -1;
	bool isEnemyReady = false;

	public float hp;

	

	void Start () {
		rigid = GetComponent<Rigidbody>();
		myPhotonView = GetComponent<PhotonView>();
		if (myPhotonView.isMine) {
			gameObject.tag = "myPlayer";
			chara = PlayerPrefs.GetInt("chara", 0);
			hairColor = PlayerPrefs.GetInt("hair", 1);
			eyeColor = PlayerPrefs.GetInt("eye", 3);
			costumeColor = PlayerPrefs.GetInt("costume", 6);
			SetChara(chara, hairColor, eyeColor, costumeColor, "myPlayer");
		} else {
			gameObject.tag = "enemyPlayer";
		}
		hp = PhotonManager.MAXHP;
	}

	void SetChara(int chara, int hairColor, int eyeColor, int costumeColor, string tag) {
		GameObject charaObj = Instantiate(Resources.Load("CharaPrefabs/chara" + chara) as GameObject);
		charaObj.transform.parent = GameObject.FindWithTag(tag).transform;
		charaObj.gameObject.name = "UTC_Default";
		charaObj.transform.localPosition = new Vector3(0, 0, 0);
		charaObj.transform.localRotation = new Quaternion(0, 0, 0, 1);
		animator = charaObj.GetComponent<Animator>();
		charaObj.gameObject.AddComponent<FreezeCharaPosition>();
		charaObj.gameObject.GetComponent<AnimationEventScript>().smallAttackCol = transform.root.gameObject.transform.Find("SmallAttackCollider").gameObject.GetComponent<BoxCollider>();
		charaObj.gameObject.GetComponent<AnimationEventScript>().bigAttackCol = transform.root.gameObject.transform.Find("BigAttackCollider").gameObject.GetComponent<BoxCollider>();

		GameObject charaImgObj = null;
		if (transform.position.x < 0) {
			for (int i = 0; i < 7; i++) {
				if (i == chara) {
					charaImgObj = GameObject.Find("charaLeft" + i);
				} else {
					GameObject.Find("charaLeft" + i).SetActive(false);
				}
			}
		} else {
			for (int i = 0; i < 7; i++) {
				if (i == chara) {
					charaImgObj = GameObject.Find("charaRight" + i);
				} else {
					GameObject.Find("charaRight" + i).SetActive(false);
				}
			}
		}

		if (chara == 0) {
			charaObj.transform.Find("costume").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/costume_unity" + costumeColor) as Material;
			charaImgObj.transform.Find("costume").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/costume_unity" + costumeColor) as Material;
		} else {
			charaObj.transform.Find("costume").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/costume_school" + costumeColor) as Material;
			charaImgObj.transform.Find("costume").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/costume_school" + costumeColor) as Material;
		}
		switch(chara) {
			case 0:
			case 1:
			case 2:
			charaObj.transform.Find("hair").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/unity" + hairColor) as Material;
			charaObj.transform.Find("eye").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/unity" + eyeColor) as Material;
			charaImgObj.transform.Find("hair").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/unity" + hairColor) as Material;
			charaImgObj.transform.Find("eye").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/unity" + eyeColor) as Material;
			break;
			case 3:
			case 4:
			charaObj.transform.Find("hair").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/misaki" + hairColor) as Material;
			charaObj.transform.Find("eye").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/misaki" + eyeColor) as Material;
			charaImgObj.transform.Find("hair").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/misaki" + hairColor) as Material;
			charaImgObj.transform.Find("eye").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/misaki" + eyeColor) as Material;
			break;
			case 5:
			case 6:
			charaObj.transform.Find("hair").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/yuko" + hairColor) as Material;
			charaObj.transform.Find("eye").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/yuko" + eyeColor) as Material;
			charaImgObj.transform.Find("hair").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/yuko" + hairColor) as Material;
			charaImgObj.transform.Find("eye").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/yuko" + eyeColor) as Material;
			break;
		}

		Debug.Log(tag + " : キャラのセッティングが完了しました");
	}
	
	void Update () {
		if (gameObject.tag == "enemyPlayer" && chara != -1 && !isEnemyReady) {
			SetChara(chara, hairColor, eyeColor, costumeColor, "enemyPlayer");
			isEnemyReady = true;
		}

		if (hp == 0) {
			GameObject.Find("PhotonManager").SendMessage("BattleEnd", gameObject.tag);
		}

		if (PhotonManager.isEnded) {
			GetComponent<NetworkCharacter>().enabled = false;
			this.enabled = false;
		}

		if (gameObject.tag == "myPlayer") {
			/* ---------------------------------
				敵のいる位置に合わせて向き変更
			---------------------------------- */
			if (transform.position.x > enemyPos) {
				transform.eulerAngles = new Vector3(0, -90, 0);		
			} else {
				transform.eulerAngles = new Vector3(0, 90, 0);	
			}
		}

		if (gameObject.tag == "myPlayer" && PhotonManager.isPlaying) {
			/* ---------------------------------
				左右の移動
			---------------------------------- */
			if (ButtonScript.rightButtonPressing) {
				if (transform.position.x < 3) {
					direction = 1f;
				} else {
					direction = 0;
				}
      		} else if (ButtonScript.leftButtonPressing) {
      	    	if (transform.position.x > -3) {
					direction = -1f;
				} else {
					direction = 0;
				}
        	} else {
        	    direction = 0f;
        	}
        	rigid.velocity = new Vector3(scroll * direction, rigid.velocity.y, 0);

			/* ---------------------------------
				移動アニメーション
			---------------------------------- */
			if (ButtonScript.leftButtonPressed) {
				walkFlg = true;
				animator.SetTrigger("Walk");
				ButtonScript.leftButtonPressed = false;
			}
			if (ButtonScript.rightButtonPressed) {
				walkFlg = true;
				animator.SetTrigger("Walk");
				ButtonScript.rightButtonPressed = false;
			}
			if (ButtonScript.leftButtonUpped) {
				idleFlg = true;
				animator.SetTrigger("Idle");
				ButtonScript.leftButtonUpped = false;
			}
			if (ButtonScript.rightButtonUpped) {
				idleFlg = true;
				animator.SetTrigger("Idle");
				ButtonScript.rightButtonUpped = false;
			}

			/* ---------------------------------
				ジャンプ
			---------------------------------- */
			if (transform.position.y < 0.02f) {
				isGround = true;
			} else {
				isGround = false;
			}
			ButtonScript.isGround = isGround;
			if (ButtonScript.upButtonPressed && isGround) {
        	    rigid.AddForce(Vector3.up * flap);
				ButtonScript.upButtonPressed = false;
        	}

			/* ---------------------------------
				急降下
			---------------------------------- */
			if (ButtonScript.downButtonPressed && !isGround) {
				jumpDownFlg = true;
        	    rigid.AddForce(Vector3.down * flap);
				animator.SetTrigger("JumpDown");
				ButtonScript.downButtonPressed = false;
        	}

			/* ---------------------------------
				弱攻撃
			---------------------------------- */
			if (ButtonScript.smallAttackButtonPressed) {
				smallAttackFlg = true;
				animator.SetTrigger("SmallAttack");
				ButtonScript.smallAttackButtonPressed = false;
       		}

			/* ---------------------------------
				強攻撃
			---------------------------------- */
			if (ButtonScript.bigAttackButtonPressed) {
				bigAttackFlg = true;
				animator.SetTrigger("BigAttack");
				ButtonScript.bigAttackButtonPressed = false;
        	}

			/* ---------------------------------
				スキル
			---------------------------------- */
			if (ButtonScript.skillButtonPressed) {
				skillFlg = true;
				animator.SetTrigger("Skill");
				ButtonScript.skillButtonPressed = false;
       		}

			/* ---------------------------------
				回避
			---------------------------------- */
			if (GameObject.FindWithTag("enemyPlayer") != null) {
				enemyPlayer = GameObject.FindWithTag("enemyPlayer").gameObject;
				enemyPos = enemyPlayer.transform.position.x;
			}
			if (ButtonScript.avoidButtonPressed) {
				avoidFlg = true;
				if (transform.position.x > enemyPos) {
					Instantiate(Resources.Load("AvoidEffect/AvoidEffectRight" + chara) as GameObject, new Vector3(transform.position.x, transform.position.y, -1), Quaternion.identity);
					if (transform.position.x > 3 - 1) {
						transform.position = new Vector3(3, transform.position.y, 0);
					} else {
						transform.position = new Vector3(transform.position.x + 1, transform.position.y, 0);
					}
				} else {
					Instantiate(Resources.Load("AvoidEffect/AvoidEffectLeft" + chara) as GameObject, new Vector3(transform.position.x, transform.position.y, -1), Quaternion.identity);
					if (transform.position.x < -3 + 1) {
						transform.position = new Vector3(-3, transform.position.y, 0);
					} else {
						transform.position = new Vector3(transform.position.x - 1, transform.position.y, 0);
					}
				}
				ButtonScript.avoidButtonPressed = false;
        	}
		}
	}

	void Damaged(int damage) {
		if (gameObject.tag == "myPlayer") {
			hp -= damage;
			if (hp > 0) {
				damageFlg = true;
				animator.SetTrigger("Damage");
			} else {
				hp = 0;
				deathFlg = true;
				animator.SetTrigger("Death");
			}
		}
	}

	void SkillDamaged() {
		if (gameObject.tag == "myPlayer") {
			hp *= 0.9f;
		}
	}
}
