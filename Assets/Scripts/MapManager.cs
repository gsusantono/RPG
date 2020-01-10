using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectData;

public class MapManager : MonoBehaviour {

    public enum STATE
    {
        WHITE,
        WAY_RED,
        RED,
        WAY_BLUE,
        BLUE,
        WAY_GREEN,
        GREEN,
        WAY_YELLOW,
        YELLOW
    }

    public enum WALLTYPE : int
    {
        THIN_WALL,
        CUBE_WALL
    }

    [SerializeField]
    private int _max_Val;
    public int max_Val
    {     
        get {return _max_Val; }
    } //maxで色変更
    [SerializeField]
    private int _min_Val;
    public int min_Val
    {
        get { return _min_Val; }
    }//minで白色に

    [Header("cant change color timer")]
    public int time = 180;//色変えれない時間
    private int count = 0;//色変え用カウント

    private float[] col_Gradation;
    private Color color_wayWhite = new Color(0.6f, 0.6f, 0.6f);

    //壁とキャラクターの前後左右判定用
    public bool isFront(float f)
    {
        if (f > -0.26 && f < 0.3) return true;
        else return false;
    }
    public bool isRight(float f)
    {
        if (f > 0.34 && f < 0.66) return true;
        else return false;
    }
    public bool isLeft(float f)
    {
        if (f < -0.3 && f > -0.66) return true;
        else return false;
    }
    //壁ワープ移動距離
    [SerializeField]
    private float _thinWarpDist;
    public float thinWarpDist
    {
        get { return _thinWarpDist; }
    }
    [SerializeField]
    private float _cubeWarpDist;
    public float cubeWarpDist
    {
        get { return _cubeWarpDist; }
    }
    //壁の色設定(グラデーション用）
    public Color[] color_wayRed
    {
        get;
        private set;
    }
    public Color[] color_wayBlue
    {
        get;
        private set;
    }
    public Color[] color_wayGreen
    {
        get;
        private set;
    }
    public Color[] color_wayYellow
    {
        get;
        private set;
    }

    //用意する色の数
    public int COLORNUM
    {
        get;
        private set;
    }

    public GameObject Ground;
    public GameObject FireWall;
    public GameObject Column1;
    public GameObject Column2;
    public GameObject Column3;
    public GameObject Tower_Corner;
    public GameObject Wall;
    public GameObject SpawnPosition;

    public Material wallMaterial;
    public Material columnMat;
    public Material statueMat;

    [HideInInspector]
    public List<GameObject> l_fireWalls = new List<GameObject>();
    List<GameObject> l_columns = new List<GameObject>();
    [HideInInspector]
    public List<WallFireScript> l_wallFireScripts = new List<WallFireScript>();

    public int _columnsNum;
    public int _mapWidth;

    public bool fullVer = true;
    public int fireRatio = 3;

    void Awake () {

        if(max_Val > 1)InitializedColor();
       
	}
    private void Start()
    {
        ResourcesLoad();
        SetGround();
        SetOutsideWalls();
        SetObjects();
    }

    private void ResourcesLoad()
    {
        Ground = (GameObject)Resources.Load("Ground");
        FireWall = (GameObject)Resources.Load("WallofFire");
        Column1 = (GameObject)Resources.Load("Column");
        Column2 = (GameObject)Resources.Load("Column2");
        Column3 = (GameObject)Resources.Load("Column3");
        Tower_Corner = (GameObject)Resources.Load("tower_corner");
        Wall = (GameObject)Resources.Load("wall");
        wallMaterial = (Material)Resources.Load("bricks");
        columnMat = (Material)Resources.Load("granite_1_d");
        statueMat = (Material)Resources.Load("Statue01");

    }

    private void CombineMeshes(GameObject obj)
    {
        MeshFilter[] meshFilters = obj.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.active = false;
            i++;
        }
        obj.transform.GetComponent<MeshFilter>().mesh = new Mesh();
        obj.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        obj.transform.gameObject.active = true;
    }

    void SetGround()
    {
        GameObject ground_instance = Instantiate(Ground, new Vector3(_mapWidth *10 / 2, 0, _mapWidth * 10 / 2), Quaternion.identity);
        ground_instance.transform.localScale = new Vector3(_mapWidth, 1, _mapWidth);
    }

    void SetOutsideWalls()
    {
        var mapSize = _mapWidth * 10;
        var wallSize = 2;

        GameObject outsidewalls = new GameObject();
        outsidewalls.name = "WALLS";
        outsidewalls.AddComponent<MeshFilter>();
        outsidewalls.AddComponent<MeshRenderer>();
        outsidewalls.transform.position = Vector3.zero;

        GameObject tower_cornerInstance1 = Instantiate(Tower_Corner,new Vector3(0, 0, 0),Quaternion.identity);
        tower_cornerInstance1.transform.eulerAngles = new Vector3(-90, 0, 90);       

        GameObject tower_cornerInstance2 = Instantiate(Tower_Corner, new Vector3(mapSize, 0, 0), Quaternion.identity);
        tower_cornerInstance2.transform.eulerAngles = new Vector3(-90, 0, 0);
        
        GameObject tower_cornerInstance3 = Instantiate(Tower_Corner, new Vector3(mapSize, 0, mapSize), Quaternion.identity);
        tower_cornerInstance3.transform.eulerAngles = new Vector3(-90, 0, -90);
      
        GameObject tower_cornerInstance4 = Instantiate(Tower_Corner, new Vector3( 0, 0, mapSize), Quaternion.identity);
        tower_cornerInstance4.transform.eulerAngles = new Vector3(-90, 0, -180);
       
        var wallNum = mapSize / wallSize;

        for(int i = 0; i < wallNum; i++)
        {
            GameObject wall_instance = Instantiate(Wall, new Vector3(0, 0, i * wallSize + 3),Quaternion.identity);           
            wall_instance.transform.eulerAngles = new Vector3(-90,180,0);
            wall_instance.transform.parent = outsidewalls.transform;
        }
        for (int i = 0; i < wallNum; i++)
        {
            GameObject wall_instance = Instantiate(Wall, new Vector3(i * wallSize + 3, 0, 0), Quaternion.identity);         
            wall_instance.transform.eulerAngles = new Vector3(-90, 180, 270);
            wall_instance.transform.parent = outsidewalls.transform;
        }
        for (int i = 0; i < wallNum; i++)
        {
            GameObject wall_instance = Instantiate(Wall, new Vector3(mapSize, 0, i * wallSize + 3), Quaternion.identity);          
            wall_instance.transform.eulerAngles = new Vector3(-90, 180, -180);
            wall_instance.transform.parent = outsidewalls.transform;
        }
        for (int i = 0; i < wallNum; i++)
        {
            GameObject wall_instance = Instantiate(Wall, new Vector3(i * wallSize + 3, 0, mapSize), Quaternion.identity);           
            wall_instance.transform.eulerAngles = new Vector3(-90, 180, 90);
            wall_instance.transform.parent = outsidewalls.transform;
        }

        CombineMeshes(outsidewalls);
        outsidewalls.GetComponent<Renderer>().material = wallMaterial;
        outsidewalls.AddComponent<MeshCollider>();


        tower_cornerInstance1.transform.parent = outsidewalls.transform;
        tower_cornerInstance2.transform.parent = outsidewalls.transform;
        tower_cornerInstance3.transform.parent = outsidewalls.transform;
        tower_cornerInstance4.transform.parent = outsidewalls.transform;
    }
     
    void SetObjects()
    {
        _columnsNum = _columnsNum < 2 ? 2 : _columnsNum;
        _mapWidth = _mapWidth < 5 ? 5 : _mapWidth;

        //space from outside_walls
        var space = _mapWidth;        
        //interval  for columns
        //var interval = _mapWidth * 10 / _columnsNum;
        var interval = ((_mapWidth * 10) - (space * 2)) / (_columnsNum - 1);
        //real fireWall_size
        var fireWallSize = interval / 5;

        //columns
        for (int z = 0; z < _columnsNum; z++)
        {
            for(int x = 0; x < _columnsNum; x++)
            {
                //int num = x + _columnsNum * z;
                int rand = Random.Range(0, 3);
                if (rand == 0) SetColumns(/*Column1,*/ new Vector3(interval * x + space, 0, interval * z + space));
                if (rand == 1) SetColumns(/*Column2,*/ new Vector3(interval * x + space, 0, interval * z + space));
                if (rand == 2) SetColumns(/*Column3,*/ new Vector3(interval * x + space, 0, interval * z + space));

            }
        }

        //fire_walls x_buffa
        for (int z = 0; z < _columnsNum; z++)
        {
            for (int x = 0; x < _columnsNum - 1; x++)
            {
                if (!fullVer)
                {
                    int rand = Random.Range(0, fireRatio);
                    if (rand != 0) continue;
                }               

                GameObject obj_instance = Instantiate(FireWall, new Vector3(interval * x + (interval / 2) + space, 0, interval * z + space), Quaternion.identity);
                obj_instance.transform.localScale = new Vector3(fireWallSize, fireWallSize/2, 1);
                l_fireWalls.Add(obj_instance);
                l_wallFireScripts.Add(obj_instance.GetComponentInChildren<WallFireScript>());

            }
        }
        //fire_walls z_buffa
        for (int z = 0; z < _columnsNum -1; z++)
        {
            for (int x = 0; x < _columnsNum; x++)
            {
                if (!fullVer)
                {
                    int rand = Random.Range(0, fireRatio);
                    if (rand != 0) continue;
                }               

                GameObject obj_instance = Instantiate(FireWall, new Vector3(interval * x + space, 0, interval * z + (interval / 2) + space), Quaternion.identity);
                obj_instance.transform.localScale = new Vector3(fireWallSize, fireWallSize/2, 1);
                obj_instance.transform.eulerAngles = new Vector3(0, 90, 0);
                l_fireWalls.Add(obj_instance);
                l_wallFireScripts.Add(obj_instance.GetComponentInChildren<WallFireScript>());
            }
        }   
    }
    void SetColumns(/*GameObject obj,*/ Vector3 pos)
    {
        int rand = Random.Range(0, 4);
        int rand2 = Random.Range(0, 3);
        GameObject c = null;
        if (rand2 == 0) { c = Column1; }
        if (rand2 == 1) { c = Column2; }
        if (rand2 == 2) { c = Column3; }

        GameObject column = new GameObject();
        column.AddComponent<MeshFilter>();
        column.AddComponent<MeshRenderer>();

        GameObject obj_instance = Instantiate(c, new Vector3(pos.x, 0.0f, pos.z), Quaternion.identity);
        //column.transform.position = new Vector3(pos.x, 0.0f, pos.z);

        if (rand == 0) obj_instance.transform.eulerAngles = new Vector3(0, 0, 0);
        if (rand == 1) obj_instance.transform.eulerAngles = new Vector3(0, 90, 0);
        if (rand == 2) obj_instance.transform.eulerAngles = new Vector3(0, 180, 0);
        if (rand == 3) obj_instance.transform.eulerAngles = new Vector3(0, 270, 0);

        obj_instance.transform.parent = column.transform;

        CombineMeshes(column);
        if (rand2 == 0||rand2 == 2)column.GetComponent<Renderer>().material = columnMat;
        if (rand2 == 1) column.GetComponent<Renderer>().material = statueMat;
        column.name = "Column";
    }
   

    void InitializedColor()
    {
        if (max_Val > 1)
        {
            COLORNUM = max_Val + 1;

            col_Gradation = new float[COLORNUM];
            var col_g = 1.0f;
            var col_v = col_g / (COLORNUM - 1) * 0.8f;

            col_Gradation[0] = 1.0f;
            for (int i = 1; i < COLORNUM; i++)
            {
                col_Gradation[i] = col_g;
                col_g -= col_v;
            }

            color_wayRed = new Color[COLORNUM];
            color_wayBlue = new Color[COLORNUM];
            color_wayGreen = new Color[COLORNUM];
            color_wayYellow = new Color[COLORNUM];

            for (int i = 0; i < COLORNUM; i++)
            {
                color_wayRed[i] = new Color(col_Gradation[0], col_Gradation[i], col_Gradation[i]);
                color_wayBlue[i] = new Color(col_Gradation[i], col_Gradation[i], col_Gradation[0]);
                color_wayGreen[i] = new Color(col_Gradation[i], col_Gradation[0], col_Gradation[i]);
                color_wayYellow[i] = new Color(col_Gradation[0], col_Gradation[0], col_Gradation[i]);
            }
        }
           
    }

    public void ChangeColor(OBJECT_COLOR o_color)
    {
        for(int i = 0; i < l_fireWalls.Count; i++)
        {
            switch (o_color)
            {
                case OBJECT_COLOR.BLUE:
                    l_wallFireScripts[i].ChangeState(WallFireScript.STATE.BLUE);
                    break;
                case OBJECT_COLOR.RED:
                    l_wallFireScripts[i].ChangeState(WallFireScript.STATE.RED);
                    break;
                case OBJECT_COLOR.GREEN:
                    l_wallFireScripts[i].ChangeState(WallFireScript.STATE.GREEN);
                    break;
                case OBJECT_COLOR.YELLOW:
                    l_wallFireScripts[i].ChangeState(WallFireScript.STATE.YELLOW);
                    break;
            }
           
            
        }
    }
   
}
