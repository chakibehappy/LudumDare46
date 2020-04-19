using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    LevelManager LM;
    public NavMeshAgent agent;
    Transform target;
    public Vector3 targetPos;

    public bool playerIn;
    public bool isDead;
    public float deadDelayTime = 6f;

    bool runOnce;
    public bool isAttack;

    AudioSource enemySfx;
    public AudioClip sfxClip;

    
    void Start()
    {
        LM = GameObject.Find("Level Manager").GetComponent<LevelManager>();
        target = GameObject.FindWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        enemySfx = GetComponent<AudioSource>();
    }

    void Update()
    {
        EnemyBehaviour();
    }

    void EnemyBehaviour()
    {
        if (isDead)
        {
            DestroyEnemy();
            return;
        }

        if (!playerIn)
        {
            if (!runOnce) StartCoroutine("EnemyPatrol");
        }
        else
        {
            if (!isAttack)
            {
                EnemyChasing();
            }
        }
    }

    IEnumerator EnemyPatrol()
    {
        runOnce = true;
        float x, z;
        x = transform.position.x + Random.Range(-2.5f, 2.5f);
        z = transform.position.z + Random.Range(-2.5f, 2.5f);
        targetPos = new Vector3(x, transform.position.y, z);
        agent.SetDestination(targetPos);
        yield return new WaitForSeconds(Random.Range(1.0f, 2.0f));
        runOnce = false;
    }

    void EnemyChasing()
    {
        targetPos = target.position;
        agent.SetDestination(targetPos);
    }

    void DestroyEnemy()
    {
        if(!runOnce)
        {
            runOnce = true;
            agent.isStopped = true;
            enemySfx.PlayOneShot(sfxClip);
            StartCoroutine(LM.ChangeToLevelBgm());
            Destroy(gameObject, deadDelayTime);
        }
    }
    
}
