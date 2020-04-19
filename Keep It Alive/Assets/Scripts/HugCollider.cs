using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HugCollider : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<EnemyController>().isDead = true;
        }
    }
}
