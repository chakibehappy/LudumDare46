using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySightRange : MonoBehaviour
{
    public EnemyController enemy;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemy.playerIn = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemy.playerIn = false;
        }
    }
}
