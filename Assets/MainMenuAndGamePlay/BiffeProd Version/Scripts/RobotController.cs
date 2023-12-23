using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    public Vector2 FarPlaygroundDistance = new Vector2(7.5f, 3.5f);
    public float MinDistance = 0.7f;
    public float AngleRange = 10f;
    public float TheDelay = 0.5f;
    public float RobotSlowdownFactor = 0.8f;
    public float BetweenThrowDuration = 1f;
    float PreviousTime;

    GameObject PlayerTargetToThrow;
    Vector3 Target;
    string MyTeamLayer;
    bool IsHasBall;
    bool IsMyTeamHaveBall;
    bool SetNewTarget;
    bool SetNewRandomTarget;
    bool IsMyTeamCouldCatchBall;
    float XPos;

    private void Start()
    {
        SetNewTarget = true;
        SetNewRandomTarget = true;
        IsMyTeamCouldCatchBall = true;
        XPos = this.GetComponent<CharacterController>().XPos;
    }

    private void Update()
    {
        RobotActions();
    }

    void RobotActions()
    {
        GetInfo();

        bool IsBallFree = BallController.Ball.activeInHierarchy;
        if (!IsBallFree)
        {
            if (IsMyTeamHaveBall)
            {
                if (IsHasBall)
                {
                    if (SetNewTarget)
                    {
                        PlayerTargetToThrow = GetNearestPlayerWithLayer(MyTeamLayer);
                        SetNewTarget = false;
                    }
                    if (PlayerTargetToThrow != null)
                    {
                        Follow(PlayerTargetToThrow.transform.position, MinDistance, 1f);
                    }
                    //PressThrowWithAngle(PlayerTargetToThrow.transform.position, AngleRange);
                    PressThrowWithDelay(TheDelay * 3);
                }
                else
                {
                    if (IsNeedNewRandomTarget(Target, SetNewRandomTarget))
                    {
                        Target = GetRandomTeamPosition(XPos);
                        SetNewRandomTarget = false;
                    }
                    Follow(Target, MinDistance, 1f);
                }
            }
            else
            {
                Target = GetBallPlayerPos();
                Follow(Target, MinDistance, 1f);
                PressThrowWithDelay(TheDelay);
            }
        }
        else
        {
            SetNewTarget = true;
            SetNewRandomTarget = true;
            if (IsMyTeamCouldCatchBall)
            {
                Follow(BallController.Ball.transform.position, 0f, 1f);
            }
            else
            {
                Follow(BallController.Ball.transform.position, 4f, 1f);
            }
        }
    }

    void GetInfo()
    {
        IsHasBall = this.transform.GetComponent<CharacterController>().hasBall;
        MyTeamLayer = this.transform.GetComponent<CharacterController>().PlayerLayerMaskName;

        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < Players.Length; i++)
        {
            if (Players[i].layer == LayerMask.NameToLayer(MyTeamLayer))
            {
                if (Players[i].GetComponent<CharacterController>().hasBall)
                {
                    IsMyTeamHaveBall = true;
                }
            }
            else
            {
                if (Players[i].GetComponent<CharacterController>().hasBall)
                {
                    IsMyTeamHaveBall = false;
                }
            }
        }

        Transform PBH = BallController.Ball.GetComponent<BallController>().prevBallHolder;
        if (PBH != null)
        {
            if (PBH.GetComponent<CharacterController>().XPos * XPos > 0)
            {
                IsMyTeamCouldCatchBall = false;
            }
            else
            {
                IsMyTeamCouldCatchBall = true;
            }
        }
    }

    Vector3 GetBallPlayerPos()
    {
        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < Players.Length; i++)
        {
            if (Players[i].GetComponent<CharacterController>().hasBall)
            {
                return Players[i].transform.position;
            }
        }
        return Vector3.zero;
    }
    
    GameObject GetNearestPlayerWithLayer(string _TargetLayerName)
    {
        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
        GameObject _Target = null;
        float Distance = 0;

        for (int i = 0; i < Players.Length; i++)
        {
            if (Players[i].layer == LayerMask.NameToLayer(_TargetLayerName))
            {
                float _Distance = Vector3.Distance(this.transform.position, Players[i].transform.position);
                if (Distance < _Distance)
                {
                    _Target = Players[i];
                    Distance = _Distance;
                }
            }
        }
        return _Target;
    }

    Vector3 GetRandomTeamPosition(float _XPos)
    {
        Vector3 RandomPos = new Vector3();
        if (_XPos > 0)
            RandomPos.x = Random.Range(1, FarPlaygroundDistance.x);
        if (_XPos < 0)
            RandomPos.x = Random.Range(-1, -FarPlaygroundDistance.x);

        RandomPos.z = Random.Range(FarPlaygroundDistance.y, -FarPlaygroundDistance.y);

        return RandomPos;
    }

    bool IsNeedNewRandomTarget(Vector3 _Target, bool _SetNewRandomTarget)
    {
        if (!_SetNewRandomTarget)
        {
            float _Distance = Vector3.Distance(this.transform.position, _Target);
            if (_Distance < MinDistance)
            {
                return true;
            }
            return false;
        }
        return true;
    }

    void Follow(Vector3 _Target, float _MinDistance, float SpeedPresent)
    {
        Vector3 TargetDirection = _Target - this.transform.position;
        Vector2 JoyVec = new Vector2(TargetDirection.x, TargetDirection.z);
        JoyVec = JoyVec.normalized * RobotSlowdownFactor * SpeedPresent;

        float Distance = Vector3.Distance(this.transform.position, _Target);
        if (Distance > _MinDistance)
        {
            this.transform.GetComponent<CharacterController>().RobotjoystickDirection = new Vector3(JoyVec.x, 0, JoyVec.y);
        }
        else
        {
            this.transform.GetComponent<CharacterController>().RobotjoystickDirection = Vector2.zero;
        }
    }

    void PressThrowWithAngle(Vector3 _Target, float AngleRange)
    {
        float Angle = Vector3.Angle(this.transform.forward, _Target - this.transform.position);
        if (Angle < AngleRange)
        {
            Throw();
        }
    }

    void PressThrowWithDelay(float _TheDelay)
    {
        Invoke("Throw", _TheDelay);
    }

    void Throw()
    {
        if (Time.time > PreviousTime + BetweenThrowDuration)
        {
            this.transform.GetComponent<CharacterController>().throwBTN();
            PreviousTime = Time.time;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            SetNewTarget = true;
            SetNewRandomTarget = true;
        }
    }
}