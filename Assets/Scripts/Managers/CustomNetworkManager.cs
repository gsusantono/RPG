using UnityEngine.Networking;
using UnityEngine;

public class MsgTypes
{
    public const short PlayerPrefabSelect = MsgType.Highest + 1;

    public class PlayerPrefabMsg : MessageBase
    {
        public short controllerID;
        public short prefabIndex;
    }
}


public class CustomNetworkManager : NetworkManager
{

    public short playerPrefabIndex;

    public override void OnStartServer()
    {
        NetworkServer.RegisterHandler(MsgTypes.PlayerPrefabSelect, OnResponsePrefab);
        base.OnStartServer();
    }
    public override void OnClientConnect(NetworkConnection conn)
    {
        client.RegisterHandler(MsgTypes.PlayerPrefabSelect, OnRequestPrefab);
        base.OnClientConnect(conn);
    }

    private void OnRequestPrefab(NetworkMessage netMsg)
    {
        MsgTypes.PlayerPrefabMsg msg = new MsgTypes.PlayerPrefabMsg();
        msg.controllerID = netMsg.ReadMessage<MsgTypes.PlayerPrefabMsg>().controllerID;
        msg.prefabIndex = playerPrefabIndex;
        client.Send(MsgTypes.PlayerPrefabSelect, msg);
    }
    private void OnResponsePrefab(NetworkMessage netMsg)
    {
        MsgTypes.PlayerPrefabMsg msg = netMsg.ReadMessage<MsgTypes.PlayerPrefabMsg>();
        playerPrefab = spawnPrefabs[msg.prefabIndex];
        base.OnServerAddPlayer(netMsg.conn, msg.controllerID);
    }
    //  Before Connect
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        MsgTypes.PlayerPrefabMsg msg = new MsgTypes.PlayerPrefabMsg();
        msg.controllerID = playerControllerId;
        NetworkServer.SendToClient(conn.connectionId, MsgTypes.PlayerPrefabSelect, msg);
    }

    public void ChoosePlayer(int number)
    {
        playerPrefabIndex = (short)number;
    }

    //  After Connnect
    //public void SwitchPlayer(SetupLocalPlayer player, int cid)
    //{
    //    GameObject newPlayer = Instantiate(spawnPrefabs[cid],
    //        player.transform.position, player.transform.rotation);
    //    playerPrefab = spawnPrefabs[cid];
    //    Destroy(player.gameObject);
    //    NetworkServer.ReplacePlayerForConnection(player.connectionToClient, newPlayer, 0);
    //}
}
