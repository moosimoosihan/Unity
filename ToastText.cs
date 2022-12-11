using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToastText : MonoBehaviour
{
    private float destroyTime = 2f;
    Animator anim;
    void Awake()
    {
        anim = GetComponent<Animator>();
    }
    void OnEnable()
    {
        anim.SetBool("IsOn",gameObject.activeSelf);
        Invoke("DestroyObject", destroyTime);
    }
    private void DestroyObject()
    {
        gameObject.SetActive(false);
    }
}
