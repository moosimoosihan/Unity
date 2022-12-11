using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomEffect : MonoBehaviour
{
    Animator anim;
    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void OnEnable()
    {
        anim.SetTrigger("isBoom");
        Invoke("ActiveOff", 0.5f);
    }
    void ActiveOff()
    {
        gameObject.SetActive(false);
    }
}
