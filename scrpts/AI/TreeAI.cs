using System;
using UnityEngine;
using UnityEngine.UIElements;

public class TreeAI : MonoBehaviour
{
    public float hp = 60;
    void Start()
    {
    }

    void Update()
    {
        if (hp <= 0)
        {
            killTree();
        }
    }

    public Boolean removeHP(float amount)
    {
        hp -= amount;
        if (hp <= 0)
        {
            killTree();
            return true;
        }
        return false;
    }

    public void killTree()
    {
        Destroy(gameObject);
    }
}
