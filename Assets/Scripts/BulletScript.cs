using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectData;

public class BulletScript : MonoBehaviour {

    public bool Red;
    private Material mat;
    public int bulletDamage = 5;
    public ParticleSystem explosionFX;
    // bbbbbb
    /// <summary>
    /// 追加
    /// </summary>
    [SerializeField]
    private OBJECT_COLOR bullectColor;//RED:0 / BLUE:1 / GREEN:2

    
    // aaaaaa

    void aaa()
    {
        int n;//お忙しい中
    }
    
    public void SetColor(OBJECT_COLOR color)
    {
        mat = GetComponent<Renderer>().material;

        //////////////////////////////////////////////
        bullectColor = color;

        switch (bullectColor)
        {
            case OBJECT_COLOR.RED:
                mat.color = Color.red;

                break;
            case OBJECT_COLOR.BLUE:
                mat.color = Color.blue;

                break;
            case OBJECT_COLOR.GREEN:
                mat.color = Color.green;

                break;
            case OBJECT_COLOR.YELLOW:
                mat.color = Color.yellow;

                break;
        }
        explosionFX.startColor = mat.color;
        /////////////////////////////////////////////////

        //mat.color = Red ? Color.red : Color.blue;
    }

    private void OnCollisionEnter(Collision collision)
    {
        explosionFX.Play();
        if (collision.gameObject.tag == "Wall")
        {
            //GameObject wall = collision.gameObject;
            //WallScript wallScript = collision.gameObject.GetComponent<WallScript>();
            //wallScript.TakeDamage(Red);
            ///////////////////////////////////////////
            //wallScript.TakeDamage(Bullet_Color);
        }

        if(collision.gameObject.tag == "Player")
        {
            if (bullectColor == collision.gameObject.GetComponent<PlayerSplitMode>().PlayerColor) return;
            PlayerSplitMode playerScript = collision.gameObject.GetComponent<PlayerSplitMode>();
            playerScript.TakeDamage(bulletDamage);
        }
        //if (collision.gameObject.tag == "Target")
        //{
        //   MovingTargetScript mScript = collision.gameObject.GetComponent<MovingTargetScript>();
        //   mScript.Particle();

        //}
        transform.GetComponent<MeshRenderer>().enabled = false;
        transform.GetComponent<Collider>().enabled = false;
        Destroy(this.gameObject,1);
    }
}
