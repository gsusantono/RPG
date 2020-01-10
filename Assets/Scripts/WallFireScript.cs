using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallFireScript : MonoBehaviour {

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

    [Header("State")]
    public STATE currentState;

    [Header("Parameter")]
    [SerializeField]
    private int current_Val;
    //[SerializeField]
    private int max_Val;//maxで色変更
    //[SerializeField]
    private int min_Val;//minで白色に
    private Material mat;
    //private float emission = 0.0f;
    public bool canChange = true;//色変えれるかどうか
    private int count = 0;//色変え用カウント

    public Color color_wayWhite = new Color(0.6f, 0.6f, 0.6f);
    private Color[] color_wayRed;
    private Color[] color_wayBlue;
    private Color[] color_wayGreen;
    private Color[] color_wayYellow;

    const int RED = 0;
    const int BLUE = 1;
    const int GREEN = 2;
    const int YELLOW = 3;

    ParticleSystem.MinMaxGradient color;
    ParticleSystem.MainModule main;
    ParticleSystem.MainModule sub;

    public enum ParticleType : int
    {
        Main,
        Sub
    }

    public enum WALLTYPE : int
    {
        THIN_WALL,
        CUBE_WALL
    }
    public WALLTYPE _wallType;

    private Vector3[] positions;

    MapManager mapManager;

    public ParticleSystem[] _particleSystems;

    private void StartParticleSystems(int num)
    {
        //for (int i = 0; i < _particleSystems.Length; i++)
        //{        
        //    _particleSystems[i].Play();
        //}

        _particleSystems[num].Play();
    }
    private void StopParticleSystems(int num)
    {
        _particleSystems[num].Stop();
    }


    void InitializedColor()
    {
        mapManager = GameObject.Find("Map2").GetComponent<MapManager>();

        if (mapManager.max_Val > 1)
        {
            color_wayRed = new Color[mapManager.COLORNUM];
            color_wayBlue = new Color[mapManager.COLORNUM];
            color_wayGreen = new Color[mapManager.COLORNUM];
            color_wayYellow = new Color[mapManager.COLORNUM];

            for (int i = 0; i < mapManager.COLORNUM; i++)
            {
                color_wayRed[i] = mapManager.color_wayRed[i];
                color_wayBlue[i] = mapManager.color_wayBlue[i];
                color_wayGreen[i] = mapManager.color_wayGreen[i];
                color_wayYellow[i] = mapManager.color_wayYellow[i];
            }
        }
       
        this.max_Val = mapManager.max_Val;
        this.min_Val = mapManager.min_Val;

        /////
        color = new ParticleSystem.MinMaxGradient();
        color.mode = ParticleSystemGradientMode.Color;   

        //for (int i = 0; i < _particleSystems.Length; i++)
        //{
        //    main = _particleSystems[i].main;
        //}
        main = _particleSystems[0].main;
        sub = _particleSystems[1].main;
        ChangeState(STATE.WHITE);

    }

    public void ChangeState(STATE state)
    {
        currentState = state;

        switch (currentState)
        {
            case STATE.WHITE:

                gameObject.layer = LayerMask.NameToLayer("Default");             
                main.startColor = Color.white;           
                break;
            case STATE.WAY_RED:

                gameObject.layer = LayerMask.NameToLayer("Default");                           
                //main.startColor = color_wayRed[current_Val];
                break;
            case STATE.RED:

                gameObject.layer = LayerMask.NameToLayer("Red");                      
                main.startColor = Color.red;
                break;
            case STATE.WAY_BLUE:

                gameObject.layer = LayerMask.NameToLayer("Default");                          
                //main.startColor = color_wayBlue[current_Val];
                break;
            case STATE.BLUE:

                gameObject.layer = LayerMask.NameToLayer("Blue");
                StartParticleSystems(1);
                main.startColor = Color.blue;
                break;
            case STATE.WAY_GREEN:

                gameObject.layer = LayerMask.NameToLayer("Default");            
                //main.startColor = color_wayGreen[current_Val];
                break;
            case STATE.GREEN:
            
                gameObject.layer = LayerMask.NameToLayer("Green");               
                main.startColor = Color.green;
                break;
            case STATE.WAY_YELLOW:

                gameObject.layer = LayerMask.NameToLayer("Default");                          
                //main.startColor = color_wayYellow[current_Val];
                break;
            case STATE.YELLOW:
             
                gameObject.layer = LayerMask.NameToLayer("Yellow");             
                main.startColor = Color.yellow;
                break;

        }
    }

    void Start () {
        StartParticleSystems((int)ParticleType.Main);
        InitializedColor();
    }

    void FixedUpdate()
    {
        if (canChange) return;
        if (!canChange)
        {          
            SetBoolCanChange();
        }

    }

    void SetBoolCanChange()
    {
        canChange = false;    
        count++;
        if (count >= mapManager.time)
        {
          
            StopParticleSystems(1);
            canChange = true;
            count = 0;
        }
    }

    /// <summary>
    /// 壁の色パラメータ変更
    /// </summary>
    /// <param name="col">当たった弾の色</param>
    public void TakeDamage(int col)
    {    
        if (canChange)
        {
            if (mapManager.max_Val > 1)
            {
                switch (col)//当たった弾の色
                {
                    case RED://赤色の弾が当たった時

                        switch (currentState)//現在の壁の色
                        {
                            case STATE.WHITE:
                                current_Val++;
                                ChangeState(STATE.WAY_RED);

                                break;
                            case STATE.WAY_RED:
                                current_Val++;
                                main.startColor = color_wayRed[current_Val];
                                if (current_Val >= max_Val)
                                {
                                    ChangeState(STATE.RED);
                                    canChange = false;
                                    //StartCoroutine(SetBoolCanChange());
                                    //SetBoolCanChange();
                                }
                                break;
                            case STATE.RED:

                                break;
                            case STATE.WAY_BLUE:
                                current_Val--;
                                main.startColor = color_wayBlue[current_Val];

                                if (current_Val == min_Val)
                                {
                                    ChangeState(STATE.WHITE);
                                }
                                break;
                            case STATE.BLUE:
                                current_Val--;
                                ChangeState(STATE.WAY_BLUE);
                                break;

                            case STATE.WAY_GREEN:
                                current_Val--;
                                main.startColor = color_wayGreen[current_Val];

                                if (current_Val == min_Val)
                                {
                                    ChangeState(STATE.WHITE);
                                }
                                break;
                            case STATE.GREEN:
                                current_Val--;
                                ChangeState(STATE.WAY_GREEN);
                                break;
                            case STATE.WAY_YELLOW:
                                current_Val--;
                                main.startColor = color_wayYellow[current_Val];

                                if (current_Val == min_Val)
                                {
                                    ChangeState(STATE.WHITE);
                                }
                                break;
                            case STATE.YELLOW:
                                current_Val--;
                                ChangeState(STATE.WAY_YELLOW);
                                break;
                        }
                        break;

                    case BLUE://青色の弾が当たった時
                        switch (currentState)//現在の壁の色
                        {
                            case STATE.WHITE:
                                current_Val++;
                                ChangeState(STATE.WAY_BLUE);

                                break;
                            case STATE.WAY_RED:
                                current_Val--;
                                main.startColor = color_wayRed[current_Val];

                                if (current_Val == min_Val)
                                {
                                    ChangeState(STATE.WHITE);
                                }
                                break;
                            case STATE.RED:
                                current_Val--;
                                ChangeState(STATE.WAY_RED);
                                break;

                            case STATE.WAY_BLUE:
                                current_Val++;
                                main.startColor = color_wayBlue[current_Val];

                                if (current_Val >= max_Val)
                                {
                                    //SetBoolCanChange();
                                    //StartCoroutine(SetBoolCanChange());
                                    ChangeState(STATE.BLUE);
                                    canChange = false;
                                }
                                break;
                            case STATE.BLUE:

                                break;
                            case STATE.WAY_GREEN:
                                current_Val--;
                                main.startColor = color_wayGreen[current_Val];

                                if (current_Val == min_Val)
                                {
                                    ChangeState(STATE.WHITE);
                                }
                                break;

                            case STATE.GREEN:
                                current_Val--;
                                ChangeState(STATE.WAY_BLUE);
                                break;

                            case STATE.WAY_YELLOW:
                                current_Val--;
                                main.startColor = color_wayYellow[current_Val];

                                if (current_Val == min_Val)
                                {
                                    ChangeState(STATE.WHITE);
                                }
                                break;
                            case STATE.YELLOW:
                                current_Val--;
                                ChangeState(STATE.WAY_YELLOW);
                                break;
                        }
                        break;
                    case GREEN://緑色の弾が当たった時
                        switch (currentState)//現在の壁の色
                        {
                            case STATE.WHITE:
                                current_Val++;
                                ChangeState(STATE.WAY_GREEN);
                                break;
                            case STATE.WAY_RED:
                                current_Val--;
                                main.startColor = color_wayRed[current_Val];

                                if (current_Val == min_Val)
                                {
                                    ChangeState(STATE.WHITE);
                                }
                                break;
                            case STATE.RED:
                                current_Val--;
                                ChangeState(STATE.WAY_RED);
                                break;
                            case STATE.WAY_BLUE:
                                current_Val--;
                                main.startColor = color_wayBlue[current_Val];

                                if (current_Val == min_Val)
                                {
                                    ChangeState(STATE.WHITE);
                                }
                                break;
                            case STATE.BLUE:
                                current_Val--;
                                ChangeState(STATE.WAY_BLUE);
                                break;
                            case STATE.WAY_GREEN:
                                current_Val++;
                                main.startColor = color_wayGreen[current_Val];

                                if (current_Val >= max_Val)
                                {
                                    //SetBoolCanChange();
                                    ChangeState(STATE.GREEN);
                                    //StartCoroutine(SetBoolCanChange());
                                    canChange = false;
                                }
                                break;
                            case STATE.GREEN:

                                break;
                            case STATE.WAY_YELLOW:
                                current_Val--;
                                main.startColor = color_wayYellow[current_Val];

                                if (current_Val == min_Val)
                                {
                                    ChangeState(STATE.WHITE);
                                }
                                break;
                            case STATE.YELLOW:
                                current_Val--;
                                ChangeState(STATE.WAY_YELLOW);
                                break;

                        }
                        break;
                    case YELLOW://黄色の弾が当たった時
                        switch (currentState)//現在の壁の色
                        {
                            case STATE.WHITE:
                                current_Val++;
                                ChangeState(STATE.WAY_YELLOW);
                                break;
                            case STATE.WAY_RED:
                                current_Val--;
                                main.startColor = color_wayRed[current_Val];

                                if (current_Val == min_Val)
                                {
                                    ChangeState(STATE.WHITE);
                                }
                                break;
                            case STATE.RED:
                                current_Val--;
                                ChangeState(STATE.WAY_RED);
                                break;
                            case STATE.WAY_BLUE:
                                current_Val--;
                                main.startColor = color_wayBlue[current_Val];

                                if (current_Val == min_Val)
                                {
                                    ChangeState(STATE.WHITE);
                                }
                                break;
                            case STATE.BLUE:
                                current_Val--;
                                ChangeState(STATE.WAY_BLUE);
                                break;
                            case STATE.WAY_GREEN:
                                current_Val--;
                                main.startColor = color_wayGreen[current_Val];

                                if (current_Val == min_Val)
                                {
                                    ChangeState(STATE.WHITE);
                                }
                                break;
                            case STATE.GREEN:
                                current_Val--;
                                ChangeState(STATE.WAY_GREEN);
                                break;
                            case STATE.WAY_YELLOW:
                                current_Val++;
                                main.startColor = color_wayYellow[current_Val];

                                if (current_Val >= max_Val)
                                {
                                    //SetBoolCanChange();
                                    ChangeState(STATE.YELLOW);
                                    //StartCoroutine(SetBoolCanChange());
                                    canChange = false;
                                }
                                break;
                            case STATE.YELLOW:

                                break;
                        }
                        break;
                }
            }
            else
            {
                switch (col)//当たった弾の色
                {
                    case RED://赤色の弾が当たった時

                        switch (currentState)//現在の壁の色
                        {
                            case STATE.WHITE:
                                ChangeState(STATE.RED);
                                break;
                            case STATE.RED:
                                break;
                            case STATE.BLUE:
                                ChangeState(STATE.WHITE);
                                break;
                            case STATE.GREEN:
                                ChangeState(STATE.WHITE);
                                break;
                            case STATE.YELLOW:
                                ChangeState(STATE.WHITE);
                                break;
                        }
                        break;

                    case BLUE://青色の弾が当たった時
                        switch (currentState)//現在の壁の色
                        {
                            case STATE.WHITE:
                                ChangeState(STATE.BLUE);
                                break;
                            case STATE.RED:
                                ChangeState(STATE.WHITE);
                                break;
                            case STATE.BLUE:

                                break;
                            case STATE.GREEN:
                                ChangeState(STATE.WHITE);
                                break;
                            case STATE.YELLOW:
                                ChangeState(STATE.WHITE);
                                break;
                        }
                        break;
                    case GREEN://緑色の弾が当たった時
                        switch (currentState)//現在の壁の色
                        {
                            case STATE.WHITE:
                                ChangeState(STATE.WHITE);
                                break;
                            case STATE.RED:
                                ChangeState(STATE.WHITE);
                                break;
                            case STATE.BLUE:
                                ChangeState(STATE.WHITE);
                                break;
                            case STATE.GREEN:

                                break;
                            case STATE.YELLOW:
                                ChangeState(STATE.WHITE);
                                break;
                        }
                        break;
                    case YELLOW://黄色の弾が当たった時
                        switch (currentState)//現在の壁の色
                        {
                            case STATE.WHITE:
                                ChangeState(STATE.WHITE);
                                break;
                            case STATE.RED:
                                ChangeState(STATE.WHITE);
                                break;
                            case STATE.BLUE:
                                ChangeState(STATE.WHITE);
                                break;
                            case STATE.GREEN:
                                ChangeState(STATE.WHITE);
                                break;
                            case STATE.YELLOW:

                                break;
                        }
                        break;
                }
            }

        }
        current_Val = current_Val > max_Val ? max_Val : current_Val;
        current_Val = current_Val < min_Val ? min_Val : current_Val;

    }



   
}
