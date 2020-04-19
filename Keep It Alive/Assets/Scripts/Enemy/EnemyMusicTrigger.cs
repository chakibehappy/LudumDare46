using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMusicTrigger : MonoBehaviour
{
    LevelManager LM;
    bool playerIn;

    void Start()
    {
        LM = GameObject.Find("Level Manager").GetComponent<LevelManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(LM.ChangeToMonsterBgm());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(LM.ChangeToLevelBgm());
        }
    }
}
