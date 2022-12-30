using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    //타일을 담아줄 배열
    //[0][3][6]
    //[1][4][7]
    //[2][5][8]
    public GameObject[] landArray;
    //플레이어
    public GameObject player;
    PlayerMove playerLogic;

    //타일 하나의 크기 = 5
    public float unitSizeX;
    public float unitSizeY;
    //시야 (이 시야 밖에 타일이 없으면 타일을 갱신)
    float halfSightX = 8;
    float halfSightY = 10;
    //타일 전체 크기(순서대로 왼쪽-위 좌표, 오른쪽-아래 좌표)
    Vector2[] border;

    void Start()
    {
        //초기화
        border = new Vector2[]{
            new Vector2(-unitSizeX * 1.5f, unitSizeY * 1.5f),
            new Vector2(unitSizeX * 1.5f, -unitSizeY *1.5f)
        };
    }
    void Awake()
    {
        playerLogic = player.GetComponent<PlayerMove>();
    }
    void Update()
    {
        BoundaryCheck();
    }
    void BoundaryCheck()
    {
        if(border[1].x < player.transform.position.x + halfSightX){//오른쪽 시야 영역 중 타일이 없을 때
            border[0] += Vector2.right * unitSizeX;
            border[1] += Vector2.right * unitSizeX;

            MoveWorld(0);
        } else if (border[0].x > player.transform.position.x - halfSightX){//왼쪽 시야 영역 중 타일이 없을때
            border[0] -= Vector2.right * unitSizeX;
            border[1] -= Vector2.right * unitSizeX;

            MoveWorld(2);
        } else if (border[0].y < player.transform.position.y + halfSightY){//위쪽 시야 영역 중 타일이 없을때
            border[0] += Vector2.up * unitSizeY;
            border[1] += Vector2.up * unitSizeY;

            MoveWorld(1);
        } else if (border[1].y > player.transform.position.y - halfSightY){//아래쪽 시야 영역 중 타일이 없을때
            border[0] -= Vector2.up * unitSizeY;
            border[1] -= Vector2.up * unitSizeY;

            MoveWorld(3);
        }
    }
    void MoveWorld(int dir)
    {
        GameObject[] _landArray = new GameObject[9];
        System.Array.Copy(landArray, _landArray, 9);

        switch(dir){
            case 0:
                for(int i=0;i<9;i++){
                    int revise = i - 3;
                    if(revise < 0){
                        landArray[9 + revise] = _landArray[i];
                        _landArray[i].transform.position += Vector3.right * unitSizeX * 3;
                    } else {
                        landArray[revise] = _landArray[i];
                    }
                }
            break;
            case 1:
                for(int i=0;i<9;i++){
                    int revise = i % 3;
                    if(revise == 2){
                        landArray[i - 2] = _landArray[i];
                        _landArray[i].transform.position += Vector3.up * unitSizeY * 3;
                    } else {
                        landArray[i + 1] = _landArray[i];
                    }
                }
            break;
            case 2:
                for(int i=0;i<9;i++){
                    int revise = i + 3;
                    if(revise > 8){
                        landArray[revise - 9] = _landArray[i];
                        _landArray[i].transform.position -= Vector3.right * unitSizeX * 3;
                    } else {
                        landArray[revise] = _landArray[i];
                    }
                }
            break;
            case 3:
                for(int i=0;i<9;i++){
                    int revise = i % 3;
                    if(revise == 0){
                        landArray[i + 2] = _landArray[i];
                        _landArray[i].transform.position -= Vector3.up * unitSizeY * 3;
                    } else {
                        landArray[i - 1] = _landArray[i];
                    }
                }
            break;
        }
    }
}
