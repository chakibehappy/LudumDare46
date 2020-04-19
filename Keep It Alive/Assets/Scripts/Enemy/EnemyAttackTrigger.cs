using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackTrigger : MonoBehaviour
{
    GameMaster GM;
    Animator playerAnim;

    public EnemyController enemy;

    public float heartRateDamage = 0.005f;
    public float attackRate = 3.0f;
    float attackTime = 0;

    AudioSource sfx;
    public AudioClip sfxClip;

    private void Start()
    {
        playerAnim = GameObject.FindWithTag("Player").GetComponent<Animator>();
        GM = GameObject.Find("Game Master").GetComponent<GameMaster>();
        sfx = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        attackTime += Time.fixedDeltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            AttackPlayer();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
            AttackPlayer();
    }

    void AttackPlayer()
    {
        if (enemy.isDead) return;

        if (attackTime >= attackRate)
        {
            sfx.PlayOneShot(sfxClip);
            enemy.isAttack = true;
            attackTime = 0;
            StartCoroutine("DelayForAttack");
            playerAnim.SetTrigger("gotHit");
            GM.HeartDropRate += heartRateDamage;
            Debug.Log(GM.HeartDropRate);
        }
    }

    IEnumerator DelayForAttack()
    {
        yield return new WaitForSeconds(attackRate);
        enemy.isAttack = false;
    }

}
