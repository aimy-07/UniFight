using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OfflineEnemyController : MonoBehaviour {

	public float flap = 550f;
    public float scroll = 3f;
    float direction = 0f;
	public bool isGround = true;
	bool isOver;

	Rigidbody rigid;
	Animator animator;
	GameObject myPlayer;
	float myPos = 0;
	public int chara;

	[SerializeField] ParticleSystem psSmallLeft;
	[SerializeField] ParticleSystem psSmallRight;
	[SerializeField] ParticleSystem psBigLeft;
	[SerializeField] ParticleSystem psBigRight;
	[SerializeField] GameObject psSkill;
	[SerializeField] GameObject psJump;
	[SerializeField] ParticleSystem psHit;
	AudioSource[] audios_SE = new AudioSource[7];

	public float hp;
	[SerializeField] Image hpbar;
	[SerializeField] Text hpbarText;
	public int ap;
	[SerializeField] Image apbar;
	[SerializeField] Text apbarText;

	MOVE_STATE moveState = MOVE_STATE.stopping;
	enum MOVE_STATE {
		chasing, fleeing, stopping
	}
	bool walkFlg;
	bool idleFlg;

	float timer;
	public bool myAttacked;
	bool skillFlg;
	float AP_PROB = 1.0f;
	float JUMP_PROB = 0.3f; // ジャンプ率40%
	float ATTACK_PROB = 0.7f; // 攻撃積極性70%
	float SKILL_PROB = 0.2f; // スキル発生率20%(素点)
	float AVOID_PROB = 0.3f; // 回避率30%
	


	void Start () {
		rigid = GetComponent<Rigidbody>();
		animator = transform.Find("UTC_Default").gameObject.GetComponent<Animator>();
		myPlayer = GameObject.Find("myPlayer");

		audios_SE = transform.Find("audio").gameObject.GetComponents<AudioSource>();
		// for (int i = 0; i < 7; i++) {
		// 	audios_SE[i].volume = 1.0f;
		// }

		hp = PhotonManager.MAXHP;
		ap = PhotonManager.MAXAP;
	}

	void Update () {
		if (OfflineManager.isPlaying) {
			/* ---------------------------------
				AP, HP関係
			---------------------------------- */
			if (OfflineManager.apPlus) {
				ap++;
			}
			if (ap > PhotonManager.MAXAP) {
				ap = PhotonManager.MAXAP;
			} else if (ap < 0) {
				ap = 0;
			}
			if (hp == 0) {
				GameObject.Find("OfflineManager").SendMessage("BattleEnd", gameObject.tag);
			}
			hpbar.fillAmount = (float)hp / PhotonManager.MAXHP;
			hpbarText.text = "HP " + hp + " / " + PhotonManager.MAXHP;
			apbar.fillAmount = (float)ap / PhotonManager.MAXAP;
			apbarText.text = "AP " + ap + " / " + PhotonManager.MAXAP;
			AP_PROB = (float)ap / PhotonManager.MAXAP;

			/* ---------------------------------
				敵のいる位置に合わせて向き変更
			---------------------------------- */
			myPos = myPlayer.transform.position.x;
			// 位置が重なってる時向きが変わりまくってガタガタするのを防止
			if (Mathf.Abs(transform.position.x - myPos) > 0.2f) {
				if (transform.position.x > myPos) {
					transform.eulerAngles = new Vector3(0, -90, 0);		
				} else {
					transform.eulerAngles = new Vector3(0, 90, 0);	
				}
			}

			/* ---------------------------------
				キャラ同士が乗っかりあってしまった時、AIが動くことで重なりを回避
			---------------------------------- */
			if (Mathf.Abs(transform.position.x - myPos) < 0.3f && Mathf.Abs(transform.position.y - myPlayer.transform.position.y) < 1.03f) {
				isOver = true;
			} else {
				isOver = false;
			}

			/* ---------------------------------
				基本は毎秒行動選択を更新
				相手の攻撃モーション時には即更新が入る
			---------------------------------- */
			if (isOver) {
				/* ---------------------------------
					キャラの重なり合いが起きている時は回避を行う
				--------------------------------- */
				if (ap >= PhotonManager.AvoidAP) {
					ap -= PhotonManager.AvoidAP;
					if (transform.position.x > 0) { 
						transform.position = new Vector3(-3, transform.position.y, 0);
						Instantiate(Resources.Load("AvoidEffect/AvoidEffectLeft" + chara) as GameObject, new Vector3(-2, transform.position.y, -1), Quaternion.identity);
					} else {
						transform.position = new Vector3(3, transform.position.y, 0);
						Instantiate(Resources.Load("AvoidEffect/AvoidEffectRight" + chara) as GameObject, new Vector3(2, transform.position.y, -1), Quaternion.identity);
					}
					audios_SE[5].Play();
					moveState = MOVE_STATE.stopping;
					idleFlg = true;
				} else {
					moveState = MOVE_STATE.fleeing;
					walkFlg = true;
				}
				if (timer == 0) {
					timer = 1.1f;
				}
				// ---------------------------------
			} else {
				if (myAttacked) {
					timer = 0;
				} else {
					if (timer > 1) {
						timer = 0;
					} else {
						timer += Time.deltaTime;
					}
				}
			}

			/* ---------------------------------
				timer == 0の時、行動選択を更新
			---------------------------------- */
			if (timer == 0) {

				/* ---------------------------------
					相手の攻撃モーション発生時
				---------------------------------- */
				if (myAttacked) {
					// 回避率と距離によって回避が発生
					if (ap >= PhotonManager.AvoidAP && Random.Range(0.0f, 1.0f) < AVOID_PROB && Mathf.Abs(transform.position.x - myPos) < 1f) {
						ap -= PhotonManager.AvoidAP;
						if (transform.position.x > 0) { 
							transform.position = new Vector3(-3, transform.position.y, 0);
							Instantiate(Resources.Load("AvoidEffect/AvoidEffectLeft" + chara) as GameObject, new Vector3(-2, transform.position.y, -1), Quaternion.identity);
						} else {
							transform.position = new Vector3(3, transform.position.y, 0);
							Instantiate(Resources.Load("AvoidEffect/AvoidEffectRight" + chara) as GameObject, new Vector3(2, transform.position.y, -1), Quaternion.identity);
						}
						audios_SE[5].Play();
						moveState = MOVE_STATE.stopping;
						idleFlg = true;
					} else {
						// 回避が発生しなかった場合は一定確率でジャンプ発生
						if (isGround) {
							if (Random.Range(0.0f, 1.0f) < JUMP_PROB) {
        	   					Jump();
							}
						}
					}
					timer = 0.5f;
					// ---------------------------------
					
				/* ---------------------------------
					通常行動
				---------------------------------- */
				} else {

					/* ---------------------------------
						残りHPを考慮した確率と距離によってスキル発動
					---------------------------------- */
					if (hp > PhotonManager.SkillOwnDamage && 2.5f < Mathf.Abs(transform.position.x - myPos) && Mathf.Abs(transform.position.x - myPos) < 4.5f) {
						// 残りHPの割合分だけ、スキル発生確率が下がる
						// HP最大時 => スキル確率20 * 1.0 = 20%
						// HP半分時 => スキル確率20 * 0.5 = 10%
						float HP_PROB = (float)hp / PhotonManager.MAXHP;
						if (Random.Range(0.0f, 1.0f) < SKILL_PROB * HP_PROB) {
							hp -= PhotonManager.SkillOwnDamage;
							animator.SetTrigger("Skill");
							psSkill.transform.position = new Vector3(transform.position.x, 0, 0);
							psSkill.GetComponent<ParticleSystem>().Play();
							audios_SE[3].Play();
							audios_SE[4].PlayDelayed(0.5f);
							skillFlg = true;
							moveState = MOVE_STATE.stopping;
							idleFlg = true;
						}
					}
					// ---------------------------------

					/* ---------------------------------
						スキルが発動しなければ移動のみか通常攻撃
					---------------------------------- */
					if (!skillFlg) {

						/* ---------------------------------
							距離が離れている時は攻撃積極性により追いかける
						---------------------------------- */
						if (Mathf.Abs(transform.position.x - myPos) > 1) {
							// 一定確率でジャンプ発生
							if (isGround) {
								if (Random.Range(0.0f, 1.0f) < JUMP_PROB) {
        	   						Jump();
								}
							}
							if (Random.Range(0.0f, 1.0f) < ATTACK_PROB) {
								moveState = MOVE_STATE.chasing;
								walkFlg = true;
							} else {
								moveState = MOVE_STATE.stopping;
								idleFlg = true;
							}
							// ---------------------------------

						/* ---------------------------------
							距離が近い時、近距離攻撃or移動のみ
						---------------------------------- */
						} else {
							// 一定確率でジャンプ発生
							if (isGround) {
								if (Random.Range(0.0f, 1.0f) < JUMP_PROB) {
									Jump();
								}
							}

							/* ---------------------------------
								攻撃積極性によってさらに近づくか一度離れるかが決まる
							---------------------------------- */
							if (Random.Range(0.0f, 1.0f) < ATTACK_PROB) {
								moveState = MOVE_STATE.chasing;
								walkFlg = true;
								/* ---------------------------------
									残りAPを考慮した確率によって攻撃
									攻撃をしなかったら攻撃積極性により後退
								---------------------------------- */
								float rand = Random.Range(0.0f, 1.0f);
								// 60%でSmallAttack
								if (rand < 0.6f) {
									if (ap >= PhotonManager.SmallAttackAP && Random.Range(0.0f, 1.0f) < AP_PROB) {
										ap -= PhotonManager.SmallAttackAP;
										animator.SetTrigger("SmallAttack");
										if (transform.position.x > myPos) {
											psSmallLeft.Play();
										} else {
											psSmallRight.Play();
										}
										audios_SE[2].Play();
									} else {
										if (Random.Range(0.0f, 1.0f) > ATTACK_PROB) {
											moveState = MOVE_STATE.fleeing;
										}
									}
								// 40%でBigAttack
								} else {
									if (ap >= PhotonManager.BigAttackAP && Random.Range(0.0f, 1.0f) < AP_PROB) {
										ap -= PhotonManager.BigAttackAP;
										animator.SetTrigger("BigAttack");
										if (transform.position.x > myPos) {
											psBigLeft.Play();
										} else {
											psBigRight.Play();
										}
										audios_SE[2].Play();
									} else {
										if (Random.Range(0.0f, 1.0f) > ATTACK_PROB) {
											moveState = MOVE_STATE.fleeing;
										}
									}
								}
								// ---------------------------------
							} else {
								moveState = MOVE_STATE.fleeing;
								walkFlg = true;
							}
							// ---------------------------------
						}
					}
				}
				myAttacked = false;
				skillFlg = false;
			}

			// 移動範囲外に出たら止まる
			if (transform.position.x < -3) {
				moveState = MOVE_STATE.stopping;
				idleFlg = true;
				transform.position = new Vector3(-3, transform.position.y, transform.position.z);
      		} else if (3 < transform.position.x) {
				moveState = MOVE_STATE.stopping;
				idleFlg = true;
				transform.position = new Vector3(3, transform.position.y, transform.position.z);
      		}
			
			switch(moveState) {
				case MOVE_STATE.chasing:
					if (transform.position.x - myPos < 0) {
						direction = 1f;
					} else {
					direction = -1f;
					}
					animator.SetTrigger("Walk");
					if (walkFlg) {
						audios_SE[0].Play();
						walkFlg = false;
					}
					break;
				case MOVE_STATE.fleeing:
					if (transform.position.x - myPos < 0) {
						direction = -1f;
					} else {
						direction = 1f;
					}
					animator.SetTrigger("Walk");
					if (walkFlg) {
						audios_SE[0].Play();
						walkFlg = false;
					}
					break;
				case MOVE_STATE.stopping:
					direction = 0;
					animator.SetTrigger("Idle");
					if (idleFlg) {
						audios_SE[0].Stop();
						idleFlg = false;
					}
					break;
			}
			rigid.velocity = new Vector3(scroll * direction, rigid.velocity.y, 0);
		}
	}

	void Jump() {
		rigid.AddForce(Vector3.up * flap);
		isGround = false;
		psJump.transform.position = new Vector3(transform.position.x, -0.2f, 0);
		psJump.GetComponent<ParticleSystem>().Play();
		audios_SE[1].Play();
	}

	void OnCollisionEnter(Collision c) {
		if (c.gameObject.tag == "floor") {
			isGround = true;
		}
	}

	void OnCollisionExit(Collision c) {
		if (c.gameObject.tag == "floor") {
			isGround = false;
		}
	}

	void Damaged(int damage) {
		hp -= damage;
		psHit.Play();
		if (hp > 0) {
			animator.SetTrigger("Damage");
		} else {
			hp = 0;
			animator.SetTrigger("Death");
		}
	}
}