using System.Collections.Generic;
using UnityEngine;

public class NoJumpDamege : MonoBehaviour
{
    public int damage;
    public float damageRate;
    public float noJumpDuration = 3.0f;

    List<IDamagelbe> things = new List<IDamagelbe>();

    void Start()
    {
        InvokeRepeating("DealDamage", 0, damageRate);
    }

    void DealDamage()
    {
        for (int i = 0; i < things.Count; i++)
        {
            PlayerCondition player = things[i] as PlayerCondition;
            if (player != null && player.GroundedTime(noJumpDuration))
            {
                player.TakePhysicalDamage(damage);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamagelbe damagelbe))
        {
            things.Add(damagelbe);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IDamagelbe damagelbe))
        {
            things.Remove(damagelbe);
        }
    }
}
