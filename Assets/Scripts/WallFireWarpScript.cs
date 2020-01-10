using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallFireWarpScript : MonoBehaviour {

    public enum WALLTYPE : int
    {
        THIN_WALL,
        CUBE_WALL
    }
    public WALLTYPE _wallType;

    public GameObject FireWall;
    WallFireScript wfScript;

    private Vector3[] positions;

    public MapManager mapManager;

    void Start()
    {
        mapManager = GameObject.Find("Map2").GetComponent<MapManager>();
        wfScript = FireWall.GetComponent<WallFireScript>();
    }

    private void CulcWarpPos(GameObject other)
    {
        var target = new Vector3(other.transform.position.x, 0, other.transform.position.z);
        var self = new Vector3(this.transform.position.x, 0, this.transform.position.z);

        var diff = target - self;     
        var axis = Vector3.Cross(this.transform.right.normalized, diff);     
        var angle = Vector3.Angle(this.transform.right.normalized, diff) * (axis.y < 0 ? -1 : 1) / 180.0f;

        PlayerOn playerO = other.GetComponent<PlayerOn>();

        if (_wallType == WALLTYPE.THIN_WALL)
        {
            //Debug.Log(angle);
            if (angle > 0)
            {
                var destPos = this.gameObject.transform.position + transform.forward * mapManager.thinWarpDist;
                other.gameObject.transform.position = new Vector3(destPos.x, other.transform.position.y, destPos.z);
                //Debug.Log(transform.up);
            }
            else if (angle <= 0)
            {
                var destPos = this.gameObject.transform.position - transform.forward * mapManager.thinWarpDist;
                other.gameObject.transform.position = new Vector3(destPos.x, other.transform.position.y, destPos.z);
                //Debug.Log(transform.up);
            }
        }
        if (_wallType == WALLTYPE.CUBE_WALL)
        {
            if (mapManager.isLeft(angle))
            {
                var destPos = this.gameObject.transform.position + transform.right * mapManager.cubeWarpDist;
                other.gameObject.transform.position = new Vector3(destPos.x, other.transform.position.y, destPos.z);
            }
            else if (mapManager.isRight(angle))
            {
                var destPos = this.gameObject.transform.position - transform.right * mapManager.cubeWarpDist;
                other.gameObject.transform.position = new Vector3(destPos.x, other.transform.position.y, destPos.z);
            }
            else if (mapManager.isFront(angle))
            {
                var destPos = this.gameObject.transform.position - transform.forward * mapManager.cubeWarpDist;
                other.gameObject.transform.position = new Vector3(destPos.x, other.transform.position.y, destPos.z);
            }
            else
            {
                var destPos = this.gameObject.transform.position + transform.forward * mapManager.cubeWarpDist;
                other.gameObject.transform.position = new Vector3(destPos.x, other.transform.position.y, destPos.z);
            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {

            PlayerOn playerSplitScript = other.gameObject.GetComponent<PlayerOn>();

            if (other.gameObject.layer == LayerMask.NameToLayer("Player_Red") && wfScript.currentState == WallFireScript.STATE.RED)
            {
                CulcWarpPos(other.gameObject);
               
            }
            if (other.gameObject.layer == LayerMask.NameToLayer("Player_Blue") && wfScript.currentState == WallFireScript.STATE.BLUE)
            {
                CulcWarpPos(other.gameObject);
              

            }
            if (other.gameObject.layer == LayerMask.NameToLayer("Player_Green") && wfScript.currentState == WallFireScript.STATE.GREEN)
            {
                CulcWarpPos(other.gameObject);
               
            }
            if (other.gameObject.layer == LayerMask.NameToLayer("Player_Yellow") && wfScript.currentState == WallFireScript.STATE.YELLOW)
            {
                CulcWarpPos(other.gameObject);
              
            }

            //if (playerSplitScript.isWarp == false)
            //{

            //    if (other.gameObject.layer == LayerMask.NameToLayer("Player_Red") && wfScript.currentState == WallFireScript.STATE.RED)
            //    {
            //        CulcWarpPos(other.gameObject);
            //        playerSplitScript.isWarp = true;
            //    }
            //    if (other.gameObject.layer == LayerMask.NameToLayer("Player_Blue") && wfScript.currentState == WallFireScript.STATE.BLUE)
            //    {
            //        CulcWarpPos(other.gameObject);
            //        playerSplitScript.isWarp = true;

            //    }
            //    if (other.gameObject.layer == LayerMask.NameToLayer("Player_Green") && wfScript.currentState == WallFireScript.STATE.GREEN)
            //    {
            //        CulcWarpPos(other.gameObject);
            //        playerSplitScript.isWarp = true;
            //    }
            //    if (other.gameObject.layer == LayerMask.NameToLayer("Player_Yellow") && wfScript.currentState == WallFireScript.STATE.YELLOW)
            //    {
            //        CulcWarpPos(other.gameObject);
            //        playerSplitScript.isWarp = true;
            //    }
            //}
        }

    }
    //private void OnTriggerExit(Collider other)
    //{
    //    //if (other.tag == "Player")
    //    //{
    //    //    PlayerOn playerSplitScript = other.gameObject.GetComponent<PlayerOn>();

    //    //    playerSplitScript.isWarp = playerSplitScript.isWarp = true ? false : true;
    //    //}


    //}


}
