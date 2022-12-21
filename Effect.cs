using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    public float power;
    public string effectName;
    private float destroyTime = 0.5f;
    public string elementalType;
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
