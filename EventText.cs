using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventText : MonoBehaviour
{
    Animator anim;
    void Awake()
    {
        anim = GetComponent<Animator>();
    }
    void OnEnable()
    {
        anim.SetBool("isOn",gameObject.activeSelf);
        Invoke("ActiveOff",3f);
    }
    void ActiveOff()
    {
        gameObject.SetActive(false);
    }
}
