using UnityEngine.Networking;
using UnityEngine;
using ObjectData;

public class SetupLocalPlayer : NetworkBehaviour {

    private GameObject inputUI;
    private GameObject crosshairUI;

    [SyncVar(hook = "OnChangeStatus")]
    public bool isReady = false;

    [SyncVar(hook ="OnChangeName")]
    public string playerName = "";

    [SyncVar(hook = "OnChangeColor")]
    public Color playerColor = Color.red;

    private Color[] colors = { Color.red, Color.blue, Color.green, Color.yellow };
    private int colorIndex = 0;

    private GameObject mainCam,miniCam;

    #region Ready Status
    void OnChangeStatus(bool ready)
    {
        isReady = ready;
        CurrentStatus.instance.sendReady();
    }
    [Command]
    public void CmdReady(bool r)
    {
        isReady = r;
    }
    public void Ready()
    {
        CmdReady(true);
        GetComponent<PlayerNetworkMode>().Create(PLAYER_NUMBER.ONE, GetColor(), false);
        if (playerName == ""||isReady) return;
    }
    #endregion

    #region Name
    public void OnChangeName(string n)
    {
        playerName = n;
    }
    [Command]
    void CmdChangeName(string newName)
    {        
        playerName = newName;
    }
    public void inputName(string n)
    {
        if (isReady) return;
        CmdChangeName(n);
    }
    #endregion

    #region Color
    public void OnChangeColor(Color c)
    {
        playerColor = c;
    }
    [Command]
    void CmdChangeColor(Color newColor)
    {
        playerColor = newColor;
        playerColor.a = 0.5f;
    }
    public void ChangeColor()
    {
        if (isReady) return;
        colorIndex++;
        if (colorIndex > colors.Length - 1)
            colorIndex = 0;
        CmdChangeColor(colors[colorIndex]);
    }

    /// <summary>
    /// プレイヤーが選択した色からOBJECT_COLORを取得する
    /// </summary>
    /// <returns></returns>
    private OBJECT_COLOR GetColor()
    {
        if (playerColor == new Color(1, 0, 0, 0.5f)) return OBJECT_COLOR.RED;
        else if (playerColor == new Color(0, 0, 1, 0.5f)) return OBJECT_COLOR.BLUE;
        else if (playerColor == new Color(0, 1, 0, 0.5f)) return OBJECT_COLOR.GREEN;
        else if (playerColor == new Color(1, 0.92f, 0.016f, 0.5f)) return OBJECT_COLOR.YELLOW;

        return OBJECT_COLOR.NONE;
    }
    #endregion

    private void Start()
    {
        inputUI = transform.Find("Chara UI").Find("Input UI").gameObject;
        inputUI.SetActive(isLocalPlayer);

        mainCam = transform.Find("Camera").gameObject;
        mainCam.SetActive(isLocalPlayer);

        miniCam = transform.Find("Minicam").gameObject;
        miniCam.SetActive(isLocalPlayer);

        crosshairUI = transform.Find("Crosshair UI").gameObject;
        crosshairUI.SetActive(false);

    }
    private void Update()
    {
        if (isLocalPlayer)
            if (CurrentStatus.instance.isStart)
            {
                inputUI.SetActive(false);
                crosshairUI.SetActive(true);
            }

        //Debug.Log(CurrentStatus.instance.isStart);
    }
}
