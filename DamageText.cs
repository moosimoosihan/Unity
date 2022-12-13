using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class DamageText : MonoBehaviour
{
    public string language;
    private float moveSpeed;
    private float destroyTime;
    TextMeshPro damageText;
    Rigidbody2D rigid;

    void Awake()
    {
        //초기화
        damageText = GetComponent<TextMeshPro>();
        rigid = GetComponent<Rigidbody2D>();
        moveSpeed = 0.4f;
        destroyTime = 1.2f;
    }
    void OnEnable()
    {        
        Invoke("DestroyObject", destroyTime);
    }

    public void StartDamageText(Vector3 pos, float dmg, string type, bool criticalCheck)
    {
        if(criticalCheck){
            gameObject.transform.localScale = new Vector3(2,2,1);
        }
        if(type == "Wind"){
            damageText.color = new Color(0,1,0);
        } else if (type == "Fire"){
            damageText.color = new Color(1,0,0);
        } else if (type == "Stone"){
            damageText.color = new Color(1,1,0);
        } else if (type == "Ice"){
            damageText.color = new Color(0,0,1);
        } else if (type == "Lightning"){
            damageText.color = new Color(0.5f,0,1);
        } else if (type == "Water"){
            damageText.color = new Color(0.3f,0.7f,0.8f);
        } else if (type == "Healing"){
            damageText.color = new Color(0,1,0);
        } else if (type == "Light"){
            damageText.color = new Color(1,0.8f,0);
        } else if (type == "Poison"){
            damageText.color = new Color(0,0.5f,0);
        }
        
        damageText.text = string.Format("{0:n0}", dmg); //000,000 으로 출력
        gameObject.transform.position = pos;
        rigid.AddForce(Vector2.up * moveSpeed, ForceMode2D.Impulse);
    }
    public void StartStateText(Vector3 pos, string type)
    {
        Vector3 plusPos = new Vector3(0,0.2f,0);
        if(language=="English"){
            if(type == "Diffusion"){//확산
                damageText.color = new Color(0,1,0);
                damageText.text = "Diffusion";
                gameObject.transform.position = pos + plusPos;
                rigid.AddForce(Vector2.up * moveSpeed, ForceMode2D.Impulse);
            } else if (type == "Melting"){//융해
                damageText.color = new Color(1,0,1);
                damageText.text = "Melting";
                gameObject.transform.position = pos + plusPos;
                rigid.AddForce(Vector2.up * moveSpeed, ForceMode2D.Impulse);
            } else if (type == "Overload"){//과부하
                damageText.color = new Color(1,0,0);
                damageText.text = "Overload";
                gameObject.transform.position = pos + plusPos;
                rigid.AddForce(Vector2.up * moveSpeed, ForceMode2D.Impulse);
            } else if(type=="Superconductivity"){//초전도
                damageText.color = new Color(0,0,1);
                damageText.text = "Superconductivity";
                gameObject.transform.position = pos + plusPos;
                rigid.AddForce(Vector2.up * moveSpeed, ForceMode2D.Impulse);
            } else if(type=="Evaporation"){//증발
                damageText.color = new Color(0.2f,0.5f,0.8f);
                damageText.text = "Evaporation";
                gameObject.transform.position = pos + plusPos;
                rigid.AddForce(Vector2.up * moveSpeed, ForceMode2D.Impulse);
            } else if(type=="Freezing"){//빙결
                damageText.color = new Color(0.4f,0.6f,0.9f);
                damageText.text = "Freezing";
                gameObject.transform.position = pos + plusPos;
                rigid.AddForce(Vector2.up * moveSpeed, ForceMode2D.Impulse);
            } else if(type=="ElectricShock"){//감전
                damageText.color = new Color(0.5f,0,1f);
                damageText.text = "ElectricShock";
                gameObject.transform.position = pos + plusPos;
                rigid.AddForce(Vector2.up * moveSpeed, ForceMode2D.Impulse);
            } else if(type=="InDeath"){//즉사
                damageText.color = new Color(1f,1f,1f);
                damageText.text = "Instant Death";
                gameObject.transform.position = pos + plusPos;
                rigid.AddForce(Vector2.up * moveSpeed, ForceMode2D.Impulse);
                gameObject.transform.localScale = new Vector3(2,2,1);
            }
        } else if(language=="Korean"){
            if(type == "Diffusion"){//확산
                damageText.color = new Color(0,1,0);
                damageText.text = "확산";
                gameObject.transform.position = pos + plusPos;
                rigid.AddForce(Vector2.up * moveSpeed, ForceMode2D.Impulse);
            } else if (type == "Melting"){//융해
                damageText.color = new Color(1,0,1);
                damageText.text = "융해";
                gameObject.transform.position = pos + plusPos;
                rigid.AddForce(Vector2.up * moveSpeed, ForceMode2D.Impulse);
            } else if (type == "Overload"){//과부화
                damageText.color = new Color(1,0,0);
                damageText.text = "과부화";
                gameObject.transform.position = pos + plusPos;
                rigid.AddForce(Vector2.up * moveSpeed, ForceMode2D.Impulse);
            } else if(type=="Superconductivity"){//초전도
                damageText.color = new Color(0,0,1);
                damageText.text = "초전도";
                gameObject.transform.position = pos + plusPos;
                rigid.AddForce(Vector2.up * moveSpeed, ForceMode2D.Impulse);
            } else if(type=="Evaporation"){//증발
                damageText.color = new Color(0.2f,0.5f,0.8f);
                damageText.text = "증발";
                gameObject.transform.position = pos + plusPos;
                rigid.AddForce(Vector2.up * moveSpeed, ForceMode2D.Impulse);
            } else if(type=="Freezing"){//빙결
                damageText.color = new Color(0.4f,0.6f,0.9f);
                damageText.text = "빙결";
                gameObject.transform.position = pos + plusPos;
                rigid.AddForce(Vector2.up * moveSpeed, ForceMode2D.Impulse);
            } else if(type=="ElectricShock"){//감전
                damageText.color = new Color(0.5f,0,1f);
                damageText.text = "감전";
                gameObject.transform.position = pos + plusPos;
                rigid.AddForce(Vector2.up * moveSpeed, ForceMode2D.Impulse);
            } else if(type=="InDeath"){//즉사
                damageText.color = new Color(1f,0,0);
                damageText.text = "즉사";
                gameObject.transform.position = pos + plusPos;
                rigid.AddForce(Vector2.up * moveSpeed, ForceMode2D.Impulse);
                gameObject.transform.localScale = new Vector3(2,2,1);
            }        
        }
    }
    private void DestroyObject()
    {
        gameObject.SetActive(false);
        damageText.color = new Color(1,1,1);
        gameObject.transform.localScale = new Vector3(1,1,1);
    }
}
