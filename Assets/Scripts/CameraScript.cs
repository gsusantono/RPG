using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamepadInput;
using ObjectData;

//[ExecuteInEditMode]
public class CameraScript : MonoBehaviour
{
    #region Variables
    [SerializeField]
    private Transform targetCamera;     //  カメラのターゲット(プレイヤー)
    private float yaw;            //  回転に使うX座標
    [SerializeField]
    [Range(0f, 60f)] private float pitch;            //  回転に使うY座標
    [SerializeField]
    [Range(0f, 60f)] private float maxPitch;
    [SerializeField]
    [Range(-60f, 0f)] private float minPitch;
    [SerializeField]
    private float rotateCameraSpeed = 200;     //  マウス用の回転の速さ
    [SerializeField]
    [Range(-2f, 2f)]private float followDistanceX;       //  プレイヤーから後ろ方の距離
    [SerializeField]
    [Range(-5f, 0f)] private float followDistanceZ;       //  プレイヤーから後ろ方の距離
    [SerializeField]
    [Range(0f, 10f)] private float followDistanceY;       //  プレイヤーから上方の距離

    private float minNegativeCheck = -0.02f;
    private float maxNegativeCheck = -0.001f;
    private float minPositiveCheck = 0.02f;
    private float maxPositiveCheck = 0.001f;
    private float inputValueX;           //  キー入力から得られた値
    private float inputValueY;           //  キー入力から得られた値
    private string cameraKeyXName;       //   回転カメラのキー入力
    private string cameraKeyYName;       //   回転カメラのキー入力
    private bool useJoystick = false;
    private const string joystickName1 = "Logicool Dual Action";
    private const string joystickName2 = "Controller (Xbox 360 Wireless Receiver for Windows)";
    private const string joystickName3 = "Controller (XBOX 360 For Windows)";



    [SerializeField]
    private LayerMask camOcclusion1;       // 当たり判定に当たるレイヤー
    [SerializeField]
    private LayerMask camOcclusion2;       // 当たり判定に当たるレイヤー
    [SerializeField]
    private LayerMask camOcclusion3;       // 当たり判定に当たるレイヤー
    private Vector3 camMask;
    private Vector3 camPos;

    // インプット用
    private GameObject player;
    GamepadState gamepadState;
    private PlayerNetworkMode playerScript;

    #endregion

    #region Functions

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        player = transform.parent.gameObject;

        playerScript = gameObject.GetComponentInParent<PlayerNetworkMode>();
        useJoystick = playerScript.UseJoystick;
        Debug.Log("aaa" + useJoystick);
        if(!useJoystick)
        {
            cameraKeyXName = "Mouse X";
            cameraKeyYName = "Mouse Y";
            rotateCameraSpeed = 100;
        }
    }

    private void GamePadUpdate()
    {
        switch (playerScript.PlayerNumber)
        {
            case PLAYER_NUMBER.ONE:
                gamepadState = GamePad.GetState(GamePad.Index.One);
                break;
            case PLAYER_NUMBER.TWO:
                gamepadState = GamePad.GetState(GamePad.Index.Two);
                break;
            case PLAYER_NUMBER.THREE:
                gamepadState = GamePad.GetState(GamePad.Index.Three);
                break;
            case PLAYER_NUMBER.FOUR:
                gamepadState = GamePad.GetState(GamePad.Index.Four);
                break;
        }
    }

    void Update()
    {
        //  プレイヤーに向く
        //transform.LookAt(targetCamera);



        //if (player.layer == LayerMask.NameToLayer("Player_Red")) // left
        //{
        //    gamepadState = GamePad.GetState(GamePad.Index.One);
        //}
        //else if (player.layer == LayerMask.NameToLayer("Player_Blue"))
        //{
        //    gamepadState = GamePad.GetState(GamePad.Index.Two);
        //}
    }

    void LateUpdate()
    {
        //if (split.isFinished) return;
        if (!CurrentStatus.instance.isStart) return;

        //  左右方向
        if(useJoystick)
        {
            GamePadUpdate();
            inputValueX = gamepadState.rightStickAxis.x;
        } else
        {
            inputValueX = Input.GetAxis(cameraKeyXName);
        }
        //  コントローラの変な状態で自動にカメラを回転しないように
        if ((inputValueX >= minNegativeCheck && inputValueX < maxNegativeCheck) || (inputValueX <= minPositiveCheck && inputValueX > maxPositiveCheck)) inputValueX = 0;
        yaw += inputValueX * rotateCameraSpeed * Time.deltaTime;

        //  上下方向
        if(useJoystick)
        {
            inputValueY = gamepadState.rightStickAxis.y;
        }
        else
        {
            inputValueY = Input.GetAxis(cameraKeyYName);
        }
        //  コントローラの変な状態で自動にカメラを回転しないように
        if ((inputValueY >= minNegativeCheck && inputValueY < maxNegativeCheck) || (inputValueY <= minPositiveCheck && inputValueY > maxPositiveCheck)) inputValueY = 0;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        pitch += -inputValueY * rotateCameraSpeed * Time.deltaTime;

        // 計算した回転の値を現在の回転に代入する
        Quaternion newRotate = Quaternion.Euler(pitch, yaw, .0f);
        transform.rotation = newRotate;

        //rotationY = Mathf.Clamp(rotationY, -60, 60);
        //transform.eulerAngles = new Vector3(rotationY, rotationX, 0.0f);

        //  壁の当たり判定処理
        //  プレイヤーの中心位置からY座標をちょっと上に上がる(中心座標は足元)
        Vector3 targetOffset = new Vector3(targetCamera.position.x, (targetCamera.position.y + 2f), targetCamera.position.z);
        //Vector3 rotateVector = transform.rotation * Vector3.one;

        camPos = targetCamera.position + (newRotate * new Vector3(followDistanceX, followDistanceY, followDistanceZ));
        camMask = camPos;
        //  当たり判定のチェック。当たれば、新しい位置を計算する
        Reposition(ref targetOffset);
        //  位置を更新する
        transform.position = camPos;

        //Debug.Log(inputValueX + ", " + inputValueY);
    }

    /// <summary>
    /// 壁のあたり判定処理
    /// </summary>
    /// <param name="targetFollow"></param>
    void Reposition(ref Vector3 targetFollow)
    {
        RaycastHit wallHit = new RaycastHit();

        //  プレイヤーとカメラの間に壁があったら、新しいカメラの位置を計算する
        if(Physics.Linecast(targetFollow, camMask, out wallHit, camOcclusion1) ||
            Physics.Linecast(targetFollow, camMask, out wallHit, camOcclusion2) ||
            Physics.Linecast(targetFollow, camMask, out wallHit, camOcclusion3))
        {
            camPos = new Vector3(wallHit.point.x + wallHit.normal.x * 0.5f, wallHit.point.y + wallHit.normal.y * 0.5f, wallHit.point.z + wallHit.normal.z * 0.5f);
        }
    }

    #endregion

}
