using UnityEngine.Networking;
using UnityEngine;

public class PlayerOn : NetworkBehaviour {
    private CharacterController motor;
    private Animator anim;
    private Camera mainCam;

    public float speed = 6f;
	// Use this for initialization
	void Start () {
        motor = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        mainCam = transform.Find("TPS Camera").GetComponentInChildren<Camera>();

        mainCam.gameObject.SetActive(isLocalPlayer);

    }

    // Update is called once per frame
    void Update () {
        if (!isLocalPlayer)
            return;
        if (!CurrentStatus.instance.isStart)
            return;
        movement();
        attack();
	}
    void movement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 dir = transform.right * h + transform.forward * v;

        float rot = mainCam.transform.eulerAngles.y;

        if (dir.magnitude != 0)
            transform.rotation = Quaternion.Euler(Vector3.up * rot);

        motor.SimpleMove(dir * speed);

        anim.SetFloat("speed", dir.magnitude);

        
    }
    void attack()
    {
        setSkill(KeyCode.Alpha1, 1);
        setSkill(KeyCode.Alpha2, 2);
        setSkill(KeyCode.Alpha3, 3);
        setSkill(KeyCode.Alpha4, 4);
        setSkill(KeyCode.Alpha5, 5);
        setSkill(KeyCode.Alpha6, 6);
        setSkill(KeyCode.Alpha7, 7);
        setSkill(KeyCode.Alpha8, 8);
    }
    void setSkill(KeyCode key, int no)
    {
        if (Input.GetKeyDown(key))
            anim.SetInteger("atk", no);
        if (Input.GetKeyUp(key))
            anim.SetInteger("atk", 0);
    }
}
