using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEffect : MonoBehaviour
{
    public Sprite[] sprites; // 네모, 세모, 동그라미

    void OnEnable()
    {
        Invoke("ActiveOff", 2f);
    }
    void ActiveOff()
    {
        gameObject.SetActive(false);
    }
}
