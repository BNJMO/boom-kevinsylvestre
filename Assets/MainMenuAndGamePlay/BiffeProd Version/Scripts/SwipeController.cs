using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeController : MonoBehaviour
{
    public static SwipeController Instance;
    public GameObject Character;
    public static int CharacterNumber = -1;
    
    Vector2 StartDownPos;
    float PrevPos;
    float CurrPos;
    float RotateAmount;
    public float RotateSpeed = 0.1f;

    bool fingerDown = false;

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (!fingerDown && Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            StartDownPos = Input.touches[0].position;
            CurrPos = StartDownPos.x;
            fingerDown = true;
        }

        if (fingerDown && Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended)
        {
            fingerDown = false;
        }

        if (fingerDown)
        {
            if (StartDownPos.x > Screen.width / 3 && StartDownPos.x < (Screen.width / 3) * 2
                && StartDownPos.y > Screen.height / 4 && StartDownPos.y < (Screen.height / 4) * 3)
            {
                PrevPos = CurrPos;
                CurrPos = Input.mousePosition.x;
                RotateAmount = CurrPos - PrevPos;

                if (Character != null)
                    Character.transform.Rotate(Vector3.up, RotateAmount * -1 * RotateSpeed);
            }
        }

        if (CharacterNumber != -1)
        {
            transform.GetChild(CharacterNumber).gameObject.SetActive(true);
            Character = transform.GetChild(CharacterNumber).gameObject;
            CharacterNumber = -1;
        }
        //PC();
    }

    void PC()
    {
        //Testing for PC
        if (!fingerDown && Input.GetMouseButtonDown(0))
        {
            StartDownPos = Input.mousePosition;
            CurrPos = StartDownPos.x;
            fingerDown = true;
        }

        if (fingerDown && Input.GetMouseButtonUp(0))
        {
            fingerDown = false;
        }
    }
}
