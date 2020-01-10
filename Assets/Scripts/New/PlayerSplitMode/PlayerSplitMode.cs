using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GamepadInput;
using ObjectData;

//[ExecuteInEditMode]
[RequireComponent(typeof(Rigidbody))]
public class PlayerSplitMode : BaseCharacterScript
{

    #region Variables
    
    public bool playerColorIsRed = true;
    public float bulletSpeed = 30;
    public bool canShot = true;
    private int time = 0;
    public int rate = 15;
    private float randomX = 0.0f;
    private float randomY = 0.0f;
    private float randamValue = 0.01f;

    // UI用
    private RectTransform rightPanel;

    //  他
    //private SplitMode split;

    // FX
    [Header("FX")]
    [SerializeField]
    private ParticleSystem warpFX;
    [SerializeField]
    public ParticleSystem gunFX;
    [SerializeField]
    private Color playerColorFX;

    LineRenderer lineRenderer;


    #endregion

    #region Functions

    void Start()
    {
        //split = GameObject.Find("Game Manager").gameObject.GetComponent<SplitMode>();
        anim = GetComponentInChildren<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();

        //if (gameObject.layer == LayerMask.NameToLayer("Player_Red"))//left
        //{
            text = "Blue Player Win!";
        //}

        //if (gameObject.layer == LayerMask.NameToLayer("Player_Blue"))//left
        //{
        //    text = "Red Player Win!";
        //}

       
        //gunFX.startColor = playerColorFX;
        //warpFX.startColor = playerColorFX;

        currentSkillPoint = 0f;
    }

    /// <summary>
    /// プレイヤーのステータスの初期の処理
    /// </summary>
    /// <param name="color"></param>
    public void Create(PLAYER_NUMBER number, SKILL_TYPE skill, bool joystickFlg = true)
    {
        currentHealth = maxHealth;
        isDeath = false;

        playerNumber = number;
        switch (playerNumber)
        {
            case PLAYER_NUMBER.ONE:
                gameObject.layer = LayerMask.NameToLayer("Player_Red");
                playerColorFX = Color.red;
                playerColor = OBJECT_COLOR.RED;
                break;
            case PLAYER_NUMBER.TWO:
                gameObject.layer = LayerMask.NameToLayer("Player_Blue");
                playerColorFX = Color.blue;
                playerColor = OBJECT_COLOR.BLUE;
                cam.rect = new Rect(0.5f, 0, 0.5f, 1);
                break;
            case PLAYER_NUMBER.THREE:
                gameObject.layer = LayerMask.NameToLayer("Player_Green");
                playerColorFX = Color.green;
                playerColor = OBJECT_COLOR.GREEN;
                break;
            case PLAYER_NUMBER.FOUR:
                gameObject.layer = LayerMask.NameToLayer("Player_Yellow");
                playerColorFX = Color.yellow;
                playerColor = OBJECT_COLOR.YELLOW;
                break;
        }

        skillType = skill;

        useJoystick = joystickFlg;
        if (!useJoystick)
        {
            verticalKeyName = "Vertical Move";
            horizontalKeyName = "Horizontal Move";
            attackInputName = "Normal Attack";
        }
    }


    private void GamePadUpdate()
    {
        switch (playerNumber)
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
        
        //if (!GameManagerScript.instance.InPlay())
        //{
        //    return;
        //}

        //  コントローラー更新
        GamePadUpdate();
        //  攻撃
        Shot();
        //  移動
        Move();

        //  十字線の実態の位置を更新する
        Vector3 forward = cam.transform.TransformDirection(Vector3.forward);
        var offset = cam.transform.position + forward * crosshairDist;
        crosshairObj.transform.position = new Vector3(Screen.width / 3, offset.y, Screen.height / 2);

        //時間経過でスキルポイント回復
        currentSkillPoint += _spIncreaseTimeRate * Time.deltaTime;
        //敵倒してスキルポイント回復

        //スキル使用
        if (Input.GetKeyDown(KeyCode.LeftShift) || gamepadState.LeftShoulder)
        {

            SkillCasting(skillType);
        }
    }

    private void SkillCasting(SKILL_TYPE skillType)
    {
        if (currentSkillPoint < _skillConsumptionValue) return;

        switch (skillType)
        {
            case SKILL_TYPE.SKILL1:

                currentSkillPoint -= _skillConsumptionValue;

                GameObject lr = new GameObject("SkillEffect1");
                Color color1 = Color.red;
                Color color2 = Color.red;
                Vector3[] positions;


                lineRenderer = lr.AddComponent<LineRenderer>();
                lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
                lineRenderer.startColor = color1;
                lineRenderer.endColor = color2;
                positions = new Vector3[2];


                positions[0] = this.transform.position + new Vector3(0, 1, 0);
                positions[1] = this.transform.position + this.transform.forward * 10.0f + new Vector3(0, 1, 0);

                lineRenderer.startWidth = 0.5f;
                lineRenderer.endWidth = 0.5f;
                lineRenderer.positionCount = positions.Length;
                lineRenderer.SetPositions(positions);

                Destroy(lr, 0.5f);

                break;
            case SKILL_TYPE.SKILL2:

                currentSkillPoint -= _skillConsumptionValue;

                GameObject lr2 = new GameObject("SkillEffect2");
                Color color12 = Color.blue;
                Color color22 = Color.blue;
                Vector3[] positions2;


                lineRenderer = lr2.AddComponent<LineRenderer>();
                lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
                lineRenderer.startColor = color12;
                lineRenderer.endColor = color22;
                positions2 = new Vector3[2];


                positions2[0] = this.transform.position + new Vector3(0, 1, 0);
                positions2[1] = this.transform.position + this.transform.forward * 10.0f + new Vector3(0, 1, 0);

                lineRenderer.startWidth = 0.5f;
                lineRenderer.endWidth = 0.5f;
                lineRenderer.positionCount = positions2.Length;
                lineRenderer.SetPositions(positions2);

                Destroy(lr2, 0.5f);

                break;

            case SKILL_TYPE.SKILL3:
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = this.transform.position + this.transform.forward * 3.0f;
                cube.transform.localScale = new Vector3(3.0f, 6.0f, 3.0f);
                Destroy(cube, 1.0f);
                break;
        }


    }

    /// <summary>
    /// 回転処理
    /// </summary>
    private void UpdateRotation(Camera cam)
    {
        // カメラの前方向のベクトルを取得
        cameraForward = cam.transform.TransformDirection(Vector3.forward);
        cameraForward.y = 0;
        cameraForward = cameraForward.normalized;
      
        //  前方向ベクトルからの右方向のベクトルを取得
        cameraRight = new Vector3(cameraForward.z, 0.0f, -cameraForward.x);

        //  移動方向を計算
        Vector3 targetDirection = cameraForward + (cameraRight * initDirection);

        //  方向が変わったら、方向を更新
        if (targetDirection != Vector3.zero)
        {
            moveDirection = targetDirection;
        }
    }

    /// <summary>
    /// 移動処理
    /// </summary>
    private void Move()
    {
        //  移動ボタンを取得 
        if (!useJoystick)
        {
            verticalKey = Input.GetAxis(verticalKeyName);
            horizontalKey = Input.GetAxis(horizontalKeyName);
        }
        else
        {
            verticalKey = gamepadState.LeftStickAxis.y;
            horizontalKey = gamepadState.LeftStickAxis.x;
        }

        //  移動方向を計算する
        UpdateRotation(cam);

        //  プレイヤーの方向を変える
        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }

        //  移動
        Vector3 moveDir = cameraForward * verticalKey + cameraRight * horizontalKey;
        moveDir *= movementSpeed;
        playerRigidbody.MovePosition(transform.position + moveDir * Time.deltaTime);
        //  念のためUnityが加速度をゼロにしなかったら
        playerRigidbody.velocity = new Vector3(0, playerRigidbody.velocity.y, 0);

        //  アニメーション
        //anim.SetFloat("v", verticalKey);
        //anim.SetFloat("h", horizontalKey);
    }

    /// <summary>
    /// 攻撃処理
    /// </summary>
    void Shot()
    {
        bool shotFlg;
        if (!useJoystick) shotFlg = Input.GetButton(attackInputName);
        else shotFlg = gamepadState.RightShoulder;

        if (shotFlg)
        {
            if (time % rate == 0)
            {
                SpawnBullet();
            }

            time++;
        }else
        {
            time = 0;
        }
    }

    void SpawnBullet()
    {
        RaycastHit hit;
        Camera mainCam = cam;
        Vector3 bulletDir = Vector3.zero;

        randomX = Random.Range(-randamValue, randamValue);
        randomY = Random.Range(-randamValue, randamValue);
        Ray ray;

        switch (playerNumber)
        {
            case PLAYER_NUMBER.ONE:
                ray = cam.ScreenPointToRay(new Vector3(Screen.width / 3, Screen.height / 2));

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform)
                        bulletDir = (hit.point - bulletSpawnPos.transform.position).normalized;
                }
                else
                {
                    bulletDir = cam.transform.forward.normalized;
                }
                break;
            case PLAYER_NUMBER.TWO:
                ray = cam.ScreenPointToRay(new Vector3(Screen.width - cam.pixelWidth / 3, Screen.height / 2));

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform)
                        bulletDir = (hit.point - bulletSpawnPos.transform.position).normalized;
                }
                else
                {
                    bulletDir = cam.transform.forward.normalized;
                }
                break;
            case PLAYER_NUMBER.THREE:
                break;
            case PLAYER_NUMBER.FOUR:
                break;
        }

        GameObject bullet = (GameObject)Instantiate(bulletObj, bulletSpawnPos.transform.position, Quaternion.identity);
        BulletScript bulletScript = bullet.GetComponent<BulletScript>();
        bulletScript.SetColor(playerColor);
        bulletDir = new Vector3(bulletDir.x + randomX, bulletDir.y + randomY, bulletDir.z);
        bullet.GetComponent<Rigidbody>().velocity = bulletDir * bulletSpeed;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if(currentHealth < 0)
        {
            //currentHealth = 0;
            //isDeath = true;
            //split.winText.text = text;
            //split.isFinished = true;
            //if (split.isFinished)
            //    anim.SetTrigger("die");
        }
    }

    public void WarpFX()
    {
        warpFX.Play();
    }

    public RectTransform RightPanel
    {
        set { rightPanel = value; }
    }
    #endregion
}
