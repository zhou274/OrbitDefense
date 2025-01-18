////////////////////////////////////////////////////////////////////////////
// bl_Planet
//
//
//                    Lovatto Studio 2016
////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

public class bl_Planet : Singleton<bl_Planet>
{

    public int Lives = 1;
    [Header("References")]
    [SerializeField]private Animator PlanetAnim;  
    [SerializeField]private GameObject DestroyParticle;

    private int DefaultLives;
    private GameObject cacheExplosion;

    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
        DefaultLives = Lives;
    }

    /// <summary>
    /// 
    /// </summary>
    public void DoDamage()
    {
        Lives--;

        PlanetAnim.Play("hit", 0, 0);
        if (Lives > 0)
        {       
        }
        else if(Lives <= 0)
        {
            GameOver();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void GameOver()
    {
        cacheExplosion = Instantiate(DestroyParticle, transform.position, Quaternion.identity) as GameObject;
        //bl_TimeManager.Instance.SetSlowMotion(true, 1);
        bl_ScoreManager.Instance.SaveScore();
        bl_GameManager.Instance.OnGameOver();
    }
    

    /// <summary>
    /// 
    /// </summary>
    public void Reset()
    {
        Lives = DefaultLives;
        if(cacheExplosion != null)
        {
            Destroy(cacheExplosion);
            cacheExplosion = null;
        }
    }

    public static bl_Planet Instance
    {
        get
        {
            return ((bl_Planet)mInstance);
        }
        set
        {
            mInstance = value;
        }
    }
}