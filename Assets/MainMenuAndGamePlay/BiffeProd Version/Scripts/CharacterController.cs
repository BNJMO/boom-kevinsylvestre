using Photon.Pun;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class CharacterController : MonoBehaviourPunCallbacks
{
    private Rigidbody rb;
    public FixedJoystick joystick;
    private Animator anim;
    public Transform handTransform;
    public float moveSpeed = 5f;
    public float rotSpeed = 1.5f;
    public float throwForce = 10.0f;
    public float bombeTravelTime = 3f;
    public GameObject bomb;
    [HideInInspector] public float XPos = 0;
    [HideInInspector] public string PlayerLayerMaskName;
    [HideInInspector] public bool IsRobot;
    [HideInInspector] public Vector3 RobotjoystickDirection;
    float PrevTime = 0;
    float DisconnectDurition = 10f;
    int PlayerListlen = 0;

    [Space]
    [Header("Player Name")]
    public Transform nameHolder;
    public float maxDistance = 10f; // The maximum distance at which the canvas should be scaled
    public float scaleFactor = 0.5f; // The amount by which the canvas should be scaled
    [Space]
    [Header("Ball")]
    //ball controller
    [PunRPC]
    public Transform ballTransform;
    private int ballViewID;
    public float shootForce = 10f;
    private Rigidbody ballRB;
    public Transform ballHolder;
    public Transform fakeBall;
    public float ballTravelTime = 0.65f;
    [HideInInspector] public bool hasBall = false;
    [HideInInspector] public bool isMyTeamHaveBall = false;
    [HideInInspector] public bool isMyTeamMate = true;
    public Image indicator;
    private Color indCol;
    public Color BlueCol;
    public Color RedCol;
    public GameObject ControlCanvas;
    public GameObject IndicatorCircleCollider;

    PhotonView PV;
    string firstOwnerID;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        firstOwnerID = PV.Owner.UserId;
        XPos = transform.position.x;
        PrevTime = Time.time;
    }
    private void Start()
    {
        if (!PV.IsMine)
        {
            Destroy(ControlCanvas);
            Destroy(rb);
        }
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        PV.RPC("SetName", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName);

        if (XPos > 0)
        {
            indicator.color = BlueCol;
            nameHolder.GetComponent<Image>().color = BlueCol;
            GameEndController.Instance.TeamColor = "Blue";
        }
        if (XPos < 0)
        {
            indicator.color = RedCol;
            nameHolder.GetComponent<Image>().color = RedCol;
            GameEndController.Instance.TeamColor = "Red";
        }
        indCol = indicator.color;
    }
    private void Update()
    {
        // match end
        if (Timer.IsFinished)
        {
            if (ControlCanvas != null)
            {
                Destroy(ControlCanvas);
            }
            return;
        }
        // Robot preparations
        if (firstOwnerID != PV.Owner.UserId)
        {
            IsRobot = true;
        }
        if (IsRobot && this.GetComponent<RobotController>() == null)
        {
            if (XPos > 0)
            {
                PV.RPC("SetName", RpcTarget.AllBuffered, "Botblue");
            }
            else if (XPos < 0)
            {
                PV.RPC("SetName", RpcTarget.AllBuffered, "Botred");
            }

            this.gameObject.AddComponent<RobotController>();
            Destroy(ControlCanvas);
        }

        PlayerLayerMaskName = LayerMask.LayerToName(gameObject.layer);
        //nameHolder.GetChild(0).GetComponent<Text>().text = PlayerLayerMaskName;

        // Calculate the distance between the player and the canvas
        float distance = Vector3.Distance(nameHolder.position, Camera.main.transform.position);
        // If the distance is greater than the maximum distance
        if (distance > maxDistance)
        {
            // Calculate the scale based on the distance
            float scale = 1 + (distance - maxDistance) * scaleFactor;

            // Set the scale of the canvas
            nameHolder.localScale = new Vector3(scale, scale, scale);
        }
        nameHolder.LookAt(Camera.main.transform.position);
        nameHolder.eulerAngles = new Vector3(nameHolder.eulerAngles.x * -1f, 0f, 0f);

        if (!PV.IsMine)
        {
            return;
        }

        //ball behaviour 
        if (ballTransform && PV.IsMine)
        {
            ballRB = ballTransform.GetComponent<Rigidbody>();
            ballViewID = ballTransform.GetComponent<PhotonView>().ViewID;
        }

        bool isBallIndicatorActive = false;
        if (BallController.Ball.GetComponent<BallController>().prevBallHolder != null)
        {
            Color BallIndicatorColor = BallController.Ball.GetComponent<BallController>().ballIndicatorCol;
            Color MyIndicatorColor = new Color(indicator.color.r, indicator.color.g, indicator.color.b, BallIndicatorColor.a);
            isBallIndicatorActive = MyIndicatorColor != BallIndicatorColor;
        }
        if (!BallController.Ball.activeInHierarchy)
            isBallIndicatorActive = true;
        IndicatorCircleCollider.SetActive(isBallIndicatorActive);

        TeamMate();
    }

    private void FixedUpdate()
    {
        if (!PV.IsMine)
        {
            return;
        }

        Vector3 joystickDirection;
        if (!IsRobot)
        {
            joystickDirection = new Vector3(joystick.Horizontal, 0, joystick.Vertical);
        }
        else
        {
            joystickDirection = RobotjoystickDirection;
        }

        if (anim.GetBool("throwing") == false)
        {
            rb.velocity = new Vector3(joystickDirection.x * moveSpeed, rb.velocity.y, joystickDirection.z * moveSpeed);

            if (joystickDirection.magnitude > 0)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(joystickDirection), rotSpeed * Time.deltaTime);
            }
        }

        if (joystickDirection.x == 0 || joystickDirection.z == 0)
        {
            anim.SetBool("run", false);
        }
        else
        {
            float animationSpeed = Mathf.Abs(joystickDirection.x) + Mathf.Abs(joystickDirection.z);
            anim.speed = animationSpeed;
            anim.SetBool("run", true);
        }
        CheckDisconnectState(joystickDirection);
    }
    [PunRPC]
    void SetName(string name)
    {
        nameHolder.GetChild(0).GetComponent<Text>().text = name;
    }
    [PunRPC]
    void networkThrowBtn()
    {
        anim.SetBool("throwing", true);
    }
    public void throwBTN()
    {
        if (bomb == null)
        {
            PV.RPC("networkThrowBtn", RpcTarget.All);
        }
    }
    [PunRPC]
    void throwRPC(int id)
    {
        GameObject ballRPC = PhotonView.Find(id).gameObject;
        ballRPC.GetComponent<BallController>().currBallHolder = null;
        PV.RPC("UpdatePrevBallHolderInfos", RpcTarget.AllBuffered, id);
        ballRPC.transform.position = ballHolder.transform.position;
        ballRPC.GetComponent<Rigidbody>().velocity = ballHolder.forward * shootForce;
        ballRPC.GetComponent<Rigidbody>().useGravity = false;
        ballRPC.GetComponent<BallController>().callCoroutine(ballTravelTime);
    }
    [PunRPC]
    public void UpdatePrevBallHolderInfos(int id)
    {
        BallController ballControllerRPC = PhotonView.Find(id).gameObject.GetComponent<BallController>();
        ballControllerRPC.prevBallHolder = this.transform;

        Color BallCol = ballControllerRPC.ballIndicatorCol;
        if (ballControllerRPC.prevBallHolder.GetComponent<CharacterController>().XPos > 0)
        {
            BallCol = new Color(RedCol.r, RedCol.g, RedCol.b, ballControllerRPC.ballIndicatorCol.a);
        }
        else if (ballControllerRPC.prevBallHolder.GetComponent<CharacterController>().XPos < 0)
        {
            BallCol = new Color(BlueCol.r, BlueCol.g, BlueCol.b, ballControllerRPC.ballIndicatorCol.a);
        }
        ballControllerRPC.ballIndicatorCol = BallCol;
    }
    public void throwBomb()
    {
        if (PV.IsMine)
        {
            GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < Players.Length; i++)
            {
                if (Players[i].layer == LayerMask.NameToLayer(PlayerLayerMaskName))
                {
                    if (Players[i].GetComponent<CharacterController>().hasBall)
                    {
                        isMyTeamHaveBall = true;
                    }
                }
                else
                {
                    if (Players[i].GetComponent<CharacterController>().hasBall)
                    {
                        isMyTeamHaveBall = false;
                    }
                }
            }
        }


        if (ballTransform && ballRB != null)
        {
            hasBall = false;
            PV.RPC("throwRPC", RpcTarget.All, ballViewID);
            PV.RPC("toggleBall", RpcTarget.All, hasBall);
            ballTransform = null;
        }
        //is don't have a ball to shoot but going to throw a bomb
        else if (PV.IsMine && ballTransform == null && bomb == null && isMyTeamHaveBall == false)
        {
            bomb = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Bomb"), handTransform.position, handTransform.rotation);
            Bombe b = bomb.GetComponent<Bombe>();
            b.thrower = this;
            bomb.GetComponent<Rigidbody>().velocity = ballHolder.forward * throwForce;
            b.callCoroutine(bombeTravelTime);
        }
        anim.SetBool("throwing", false);
    }
    [PunRPC]
    public void toggleBall(bool _hasBall)
    {
        if (_hasBall)
        {
            ballTransform = FindObjectOfType<BallController>().transform;

            if (ballTransform.gameObject.GetComponent<BallController>().ballIsGrounded)
            {
                ballTransform.gameObject.GetComponent<BallController>().currBallHolder = this.transform;
            }
            ballTransform.gameObject.SetActive(false);
            fakeBall.gameObject.SetActive(true);
            camera_follow.target = fakeBall;
            var tempColor = Color.yellow;
            tempColor.a = 0.4f;
            indicator.color = tempColor;
            fakeBall.transform.position = handTransform.position;
            fakeBall.transform.parent = handTransform;
        }
        else
        {
            indicator.color = indCol;
            fakeBall.gameObject.SetActive(false);
            ballTransform = BallController.Ball.transform;
            ballTransform.gameObject.SetActive(true);
            camera_follow.target = ballTransform;
        }
        hasBall = _hasBall;
    }
    public void catchBall()
    {
        PV.RPC("catchBallRPC", RpcTarget.All);
    }
    public void lostBall()
    {
        PV.RPC("lostBallRPC", RpcTarget.All);
    }
    [PunRPC]
    public void catchBallRPC()
    {
        hasBall = true;
        ballTransform = BallController.Ball.transform;
        ballTransform.gameObject.GetComponent<BallController>().currBallHolder = this.transform;
        fakeBall.gameObject.SetActive(true);
        var tempColor = Color.yellow;
        tempColor.a = 0.4f;
        indicator.color = tempColor;
        fakeBall.transform.position = handTransform.position;
        fakeBall.transform.parent = handTransform;
    }
    [PunRPC]
    public void lostBallRPC()
    {
        hasBall = false;
        ballTransform = null;
        indicator.color = indCol;
        fakeBall.gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (PV.IsMine && other.CompareTag("Ball"))
        {
            BallController B = other.GetComponent<BallController>();
            if (!B.ballIsGrounded)
            {
                _toggleBall();
            }
            else
            {
                if (B.prevBallHolder != null)
                {
                    if (B.prevBallHolder.gameObject.layer != gameObject.layer)
                    {
                        _toggleBall();
                    }
                }
                else
                {
                    _toggleBall();
                }
            }
        }
    }

    public void _toggleBall()
    {
        hasBall = true;
        PV.RPC("toggleBall", RpcTarget.All, hasBall);
    }

    public void TeamMate()
    {
        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");

        if (PlayerListlen != Players.Length)
        {
            for (int i = 0; i < Players.Length; i++)
            {
                CharacterController Player = Players[i].GetComponent<CharacterController>();

                if (Player.gameObject.layer == LayerMask.NameToLayer("Character"))
                {
                    if (Player.XPos * XPos > 0)
                    {
                        Player.gameObject.layer = LayerMask.NameToLayer("teamMate");
                        Player.IndicatorCircleCollider.layer = LayerMask.NameToLayer("teamMate Ind");
                    }
                    else if (Player.XPos * XPos < 0)
                    {
                        Player.gameObject.layer = LayerMask.NameToLayer("opponent");
                        Player.IndicatorCircleCollider.layer = LayerMask.NameToLayer("opponent Ind");
                    }
                }
            }
            PlayerListlen = Players.Length;
        }
    }

    void CheckDisconnectState(Vector3 _joystickDirection)
    {
        if (IsRobot || Timer.IsFinished)
            return;

        if (_joystickDirection.magnitude > 0)
        {
            PrevTime = Time.time;
        }

        if (PrevTime + DisconnectDurition < Time.time)
        {
            GameEndController.Instance.IsVirtualDisconnect = true;
        }
    }
}
