using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class JoyStick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public RectTransform backJoystic;
    public RectTransform Joystic;
    public GameObject player;
    public PlayerMove playerMove;
    RectTransform m_rectBack;
    RectTransform m_rectJoystick;
    Vector2 vecNor; 
    Transform m_Player;
    float m_fRadius;
    float m_fSpeed;
    //public float fSqr;
 
    Vector3 m_vecMove;
 
    public bool m_bTouch = false;

    void Start()
    {
        m_fSpeed = playerMove.speed;
        m_rectBack = backJoystic.GetComponent<RectTransform>();
        m_rectJoystick = Joystic.GetComponent<RectTransform>();
 
        m_Player = player.transform;
 
        // JoystickBackground의 반지름입니다.
        m_fRadius = m_rectBack.rect.width * 0.5f;
    }
 
    void FixedUpdate()
    {
        if(playerMove.playerDead)
            return;
    
        if (m_bTouch)
        {
            playerMove.inputVec = vecNor;
        } else {
            playerMove.inputVec = Vector2.zero;
        }
    }
 
    void OnTouch(Vector2 vecTouch)
    {
        if(playerMove.playerDead)
            return;

        Vector2 vec = new Vector2(vecTouch.x - m_rectBack.position.x, vecTouch.y - m_rectBack.position.y);
 
        // vec값을 m_fRadius 이상이 되지 않도록 합니다.
        vec = Vector2.ClampMagnitude(vec, m_fRadius);
        m_rectJoystick.localPosition = vec;
 
        // 조이스틱 배경과 조이스틱과의 거리 비율로 이동합니다.
        //fSqr = (m_rectBack.position - m_rectJoystick.position).sqrMagnitude / (m_fRadius * m_fRadius);
 
        // 터치위치 정규화(nomalized)
        vecNor = new Vector2(vec.normalized.x , vec.normalized.y);
    }
 
    public void OnDrag(PointerEventData eventData)
    {
        OnTouch(eventData.position);
        m_bTouch = true;
    }
 
    public void OnPointerDown(PointerEventData eventData)
    {
        OnTouch(eventData.position);
        m_bTouch = true;
        //playerMove.joyStickOn = true;
    }
 
    public void OnPointerUp(PointerEventData eventData)
    {
        // 원래 위치로 되돌립니다.
        m_rectJoystick.localPosition = Vector2.zero;
        m_bTouch = false;
        //playerMove.joyStickOn = false;
    }
}
