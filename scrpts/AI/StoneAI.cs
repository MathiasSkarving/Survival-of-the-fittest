using System;
using UnityEngine;
using UnityEngine.UIElements;

public class StoneAI : MonoBehaviour
{
    public float hp = 100;
    void Start()
    {
    }

    void Update()
    {
        if (hp <= 0)
        {
            KillStone();
        }
    }
    public Boolean RemoveHP(float amount)
    {
        hp -= amount;
        if (hp <= 0)
        {
            KillStone();
            return true;
        }
        return false;
    }

    public void KillStone()
    {
        Destroy(gameObject);
    }
}
