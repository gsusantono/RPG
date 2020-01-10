using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using GamepadInput;
using ObjectData;

public class BaseCharacterNetwork : NetworkBehaviour
{

    #region Variables

    //  プレイヤー用の変数
    [SerializeField]
    [Range(-10f, 10f)] protected float initDirection = 0.5f;    //  プレイヤーの初期方向
    [SerializeField]
    [Range(0f, 20f)] protected float movementSpeed = 10f;    //  移動の速さ
    protected Vector3 moveDirection;  //  移動方向
    protected Rigidbody playerRigidbody;
    protected Material playerMaterial;
    protected OBJECT_COLOR playerColor;

    //  銃用の変数
    [SerializeField]
    protected GameObject bulletSpawnPos;   //  弾の生成位置/場所
    [SerializeField]
    protected GameObject bulletObj;        //  弾の実態
    [SerializeField]
    protected GameObject crosshairObj;     //  十字線の実態
    [SerializeField]
    [Range(4f, 20f)] protected float crosshairDist = 10f;         //  プレイやからの距離

    //  カメラ用の変数
    [SerializeField]
    protected Camera cam;
    protected Vector3 cameraForward;              //  カメラの前方向
    protected Vector3 cameraRight;                //  カメラの右方向

    //  インプット用の変数
    protected bool useJoystick;
    protected string verticalKeyName = "";
    protected string horizontalKeyName = "";
    protected string attackInputName = "";
    protected float verticalKey;
    protected float horizontalKey;
    protected string text;
    protected GamepadState gamepadState;

    //  ステータス用の変数
    protected PLAYER_NUMBER playerNumber;
    [SerializeField]
    protected int maxHealth = 100;
    protected int currentHealth;
    protected bool isDeath;

    //  アニメーション用
    protected Animator anim;

    // SKILL用
    protected const float p_skillPointMax = 1.0f;
    protected const float p_skillPointMin = 0.0f;
    //最初のスキルポイント
    protected float _startSkillPoint = 0.0f;
    //現在のスキルポイント
    //[SerializeField]
    protected float _currrentSkillPoint = 0.0f;
    //スキルポイント消費量
    [SerializeField]
    protected float _skillConsumptionValue = 0.5f;
    //スキルポイント増加率(時間経過）
    [SerializeField]
    protected float _spIncreaseTimeRate = 0.1f;
    //スキルポイント増加率(敵撃破）
    [SerializeField]
    protected float _spIncreaseCrushRate = 0.1f;
    [SerializeField]
    protected SKILL_TYPE skillType;


    #endregion

    #region Getter and Setter

    public int CurrentHealth
    {
        get { return currentHealth; }
    }

    public int MaxHealth
    {
        get { return maxHealth; }
    }

    public bool IsDeath
    {
        get { return isDeath; }
    }

    public OBJECT_COLOR PlayerColor
    {
        get { return playerColor; }
        set { playerColor = value; }
    }

    public PLAYER_NUMBER PlayerNumber
    {
        get { return playerNumber; }
        set { playerNumber = value; }
    }

    public bool UseJoystick
    {
        get { return useJoystick; }
    }

    public SKILL_TYPE SkillType
    {
        get { return skillType; }

        set
        {
            if (value < SKILL_TYPE.SKILL1 || value > SKILL_TYPE.SKILL4) { value = SKILL_TYPE.SKILL1; }
            skillType = value;
        }
    }
    public float currentSkillPoint
    {
        get
        {
            return _currrentSkillPoint;
        }
        set
        {
            if (_currrentSkillPoint < p_skillPointMin) value = p_skillPointMin;
            else if (_currrentSkillPoint > p_skillPointMax) value = p_skillPointMax;
            _currrentSkillPoint = value;
        }
    }
    #endregion
}

