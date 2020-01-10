using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine;

public class CurrentStatus : NetworkBehaviour {
    public Transform playersStatus;
    public int currentReady = 0;
    public bool isStart = false;

    public static CurrentStatus instance;
    // Use this for initialization
    private void Awake()
    {
        currentReady = 0;
        isStart = false;

        if (!instance)
            instance = this;
    }

	// Update is called once per frame
	void LateUpdate () {
        SetupLocalPlayer[] players = FindObjectsOfType<SetupLocalPlayer>();
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i])
            {
                playersStatus.GetChild(i).Find("Name").GetComponent<Text>().text = players[i].playerName;
                playersStatus.GetChild(i).Find("Is Ready").GetComponent<Image>().enabled = players[i].isReady;
                playersStatus.GetChild(i).GetComponent<Image>().color = players[i].playerColor;
                countTex = GameObject.Find("Countdown").GetComponent<Text>();
            }
        }
        if (currentReady>0&& currentReady == players.Length)
            GameStart();
        
	}
    public void sendReady()
    {
        currentReady++;
    }
    float count = 3;
    Text countTex;
    void GameStart()
    {
        string countdown = ((int)count).ToString();
        count -= Time.deltaTime;
        if (count <= 0)
        {
            isStart = true;
            countdown = "Start";
            playersStatus.gameObject.SetActive(false);
        }
        countTex.enabled = count > -1;
        countTex.text = countdown;
    }
}
