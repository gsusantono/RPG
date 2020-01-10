using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;
using UnityEngine;

public class CustomOnline : MonoBehaviour {

    List<GameObject> roomList = new List<GameObject>();
    
    private NetworkManager uNet;

    public GameObject roomPrefab;

    private GameObject connectUI;
    private GameObject charaSelectUI;

    private Transform roomListParent;
    private Transform characters;
    private GameObject charaGO;
    private GameObject gameStatus;

    private int charaIndex = 0;
    // Use this for initialization
    void Start () {

        uNet = NetworkManager.singleton;

        connectUI = GameObject.Find("Connect UI");
        charaSelectUI = GameObject.Find("Character Selection");
        roomListParent = GameObject.Find("Room List Parent").transform;
        characters = GameObject.Find("Characters").transform;
        charaGO = GameObject.Find("Choose Characters");
        gameStatus = GameObject.Find("Game Status");

        connectUI.SetActive(false);
        gameStatus.SetActive(false);

        if (uNet.matchMaker == null)
        {
            uNet.StartMatchMaker();
        }
	}
	
	public void inputRoomName(string roomName)
    {
        if (roomName == "")
        {
            roomName = "default";
        }
        uNet.matchName = roomName;
    }
    public void createRoom()
    {
        uNet.matchMaker.CreateMatch(uNet.matchName, uNet.matchSize, true, "", "", "", 0, 0, OnMatchCreate);
    }
    public void refreshRoomList()
    {
        uNet.matchMaker.ListMatches(0, 10, "", true, 0, 0, OnMatchList);
    }

    public void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        connectUI.SetActive(false);
        charaGO.SetActive(false);
        gameStatus.SetActive(true);
        uNet.OnMatchCreate(success, extendedInfo, matchInfo);
    }
    public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList)
    {
        ClearRoomList();

        foreach (var match in matchList)
        {
            GameObject _roomItem = Instantiate(roomPrefab);
            _roomItem.transform.SetParent(roomListParent);
            _roomItem.transform.localScale = Vector3.one;

            _roomItem.GetComponentInChildren<Text>().text = match.name + "  ( " + match.currentSize + " / " + match.maxSize + " )";
            _roomItem.GetComponent<Button>().onClick.AddListener(() => uNet.matchMaker.JoinMatch
            (match.networkId, "", "", "", 0, 0, OnMatchJoined));

            roomList.Add(_roomItem);
        }
        uNet.OnMatchList(success, extendedInfo, matchList);
    }
    public void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        gameStatus.SetActive(true);
        connectUI.SetActive(false);
        charaGO.SetActive(false);
        uNet.OnMatchJoined(success, extendedInfo, matchInfo);
    }
    void ClearRoomList()
    {
        for(int i = 0; i < roomList.Count; i++)
        {
            Destroy(roomList[i]);
        }
    }
    public void chooseStep(int value)
    {
        int length = characters.childCount;

        charaIndex += value;

        if (charaIndex == length) charaIndex = 0;
        else if (charaIndex < 0) charaIndex = length - 1;

        for(int i = 0; i < length; i++)
            characters.GetChild(i).gameObject.SetActive(i == charaIndex);
    }

    public void choose()
    {
        GetComponent<CustomNetworkManager>().playerPrefabIndex = (short)charaIndex;
        charaSelectUI.SetActive(false);
        connectUI.SetActive(true);
    }
    public void StartHost()
    {
        uNet.StartHost();
    }
    public void StartClient()
    {
        uNet.StartClient();
    }
}
