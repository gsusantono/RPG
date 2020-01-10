using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using GamepadInput;
using ObjectData;

public class GameManagerScript : MonoBehaviour {

    #region Variables
    public static GameManagerScript instance;

    [SerializeField]
    private GameObject playerObject;
    private GameObject playerOne;
    private PlayerNetworkMode playerOneScript;
    [SerializeField]
    private GameObject respawnPointObject;
    private GameObject[] respawnPoints;
    private bool[] usedRespawnPoints;
    private GAME_STATE currentGameState;
    [SerializeField]
    private Sprite[] playerImgs;
    private bool[] selectedPlayer = { false, false, false, false};
    private int indexLeft = 0;
    private int indexRight = 0;
    [SerializeField]
    private Image playerImgLeft;
    [SerializeField]
    private GameObject selectPanel;

    private bool isOkLeft = false;
    private bool isOkRight = false;

    [SerializeField]
    private Text readyL, readyR;

    private bool isStart = false;

    [SerializeField]
    private Text winText;
    private bool isFinished = false;

    private GamepadState gamepadState1;
    [SerializeField]
    private RectTransform rightPanel;

    //  UI
    [SerializeField]
    private Image player1HpBar;

   
    #endregion

    #region Functions
    private void Awake()
    {
        if (!instance) instance = this;
    }

    // Use this for initialization
    void Start()
    {
        currentGameState = GAME_STATE.SELECT_PLAYER;
        selectPanel.SetActive(true);
        readyL.gameObject.SetActive(false);
        readyR.gameObject.SetActive(false);

        //  Get all respawn points
        respawnPoints = new GameObject[respawnPointObject.transform.childCount];
        usedRespawnPoints = new bool[respawnPoints.Length];
        for (int i = 0; i < respawnPoints.Length; i++)
        {
            respawnPoints[i] = respawnPointObject.transform.GetChild(i).gameObject;
            usedRespawnPoints[i] = false;
        }
    }

    private void SplitMode()
    {
        switch (currentGameState)
        {
            case GAME_STATE.SELECT_PLAYER:
                SelectPlayer();

                if (isStart) currentGameState = GAME_STATE.INITIALIZE_DATA;
                break;
            case GAME_STATE.INITIALIZE_DATA:
                Vector3 player1Pos = GetRespawnPoint().transform.position;
                player1Pos.y = 0;
                Vector3 player2Pos = GetRespawnPoint().transform.position;
                player2Pos.y = 0;
                playerOne = Instantiate(playerObject, player1Pos, Quaternion.identity);

                SKILL_TYPE skill = SKILL_TYPE.SKILL1;
                //  プレイヤー１初期化
                if (playerImgs[indexLeft].name == "RED") skill = SKILL_TYPE.SKILL1;
                else if (playerImgs[indexLeft].name == "BLUE") skill = SKILL_TYPE.SKILL2;
                else if (playerImgs[indexLeft].name == "GREEN") skill = SKILL_TYPE.SKILL3;
                else if (playerImgs[indexLeft].name == "YELLOW") skill = SKILL_TYPE.SKILL4;

                playerOneScript = playerOne.GetComponent<PlayerNetworkMode>();
                //playerOneScript.Create(PLAYER_NUMBER.ONE, skill, false);

                selectPanel.SetActive(false);
                currentGameState = GAME_STATE.IN_GAME;
                break;
            case GAME_STATE.IN_GAME:
                if (Time.frameCount % 2 == 0) UpdateUI();

                if(playerOneScript.IsDeath)
                {
                    winText.text = "Blue Player Win!";
                    isFinished = true;
                }

                if (isFinished)
                {
                    winText.enabled = true;
                    currentGameState = GAME_STATE.END_GAME;
                }
                break;
            case GAME_STATE.END_GAME:
                break;
            default:
                break;
        }
    }

    private void SelectPlayer()
    {
        GamepadState gamepadState1 = GamePad.GetState(GamePad.Index.One);
        GamepadState gamepadState2 = GamePad.GetState(GamePad.Index.Two);
        //  プレイヤー１のキャラー選択
        if (!isOkLeft)
        {
            if (Input.GetKeyDown(KeyCode.A) || gamepadState1.Left)
            {
                do
                {
                    if(Time.frameCount % 8 == 0)indexLeft--;
                    indexLeft = Clamp(indexLeft);
                } while (selectedPlayer[indexLeft]);
            }
            if (Input.GetKeyDown(KeyCode.D) || gamepadState1.Right)
            {
                do
                {
                    if (Time.frameCount % 8 == 0) indexLeft++;
                    indexLeft = Clamp(indexLeft);
                } while (selectedPlayer[indexLeft]);
            }

            while (selectedPlayer[indexLeft])
            {
                indexLeft++;
            }

            playerImgLeft.sprite = playerImgs[indexLeft];
        }
        // キャラーを決めた
        if (Input.GetKeyDown(KeyCode.LeftShift) || gamepadState1.A)
        {
            isOkLeft = true;
            selectedPlayer[indexLeft] = true;
            readyL.gameObject.SetActive(isOkLeft);
        }

        if (isOkLeft) Invoke("StartSplitMode", 3);
    }

    private GameObject GetRespawnPoint()
    {
        int randomIndex = 0;
        while(true)
        {
            randomIndex = Random.Range(0, respawnPoints.Length);
            if (!usedRespawnPoints[randomIndex]) break;
        }

        usedRespawnPoints[randomIndex] = true;
        return respawnPoints[randomIndex];
        
    }

    private void UpdateUI()
    {
        player1HpBar.fillAmount = (float)playerOneScript.CurrentHealth / playerOneScript.MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        SplitMode();
        
    }
    int Clamp(int index)
    {
        if (index < 0) index = playerImgs.Length - 1;
        if (index > playerImgs.Length - 1) index = 0;
        return index;
    }
    void StartSplitMode()
    {
        isStart = true;
    }

    public bool InPlay()
    {
        if (!isFinished) return true;

        return false;
    }

    #endregion
}
