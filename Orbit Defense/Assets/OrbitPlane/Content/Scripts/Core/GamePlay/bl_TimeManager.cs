////////////////////////////////////////////////////////////////////////////
// bl_TimeManager
//
//
//                    Lovatto Studio 2016
////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

public class bl_TimeManager : Singleton<bl_TimeManager>
{

    [Header("Reference")]
    [SerializeField]private Animator PauseAnim;

    private bool slowMo = false;
    private float MyDeltaTime;
    private bool m_pause;

    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
        MyDeltaTime = Time.deltaTime;
    }

    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        if (m_pause)
            return;

        float t = (slowMo) ? 0.05f : 1;
        Time.timeScale = Mathf.Lerp(Time.timeScale, t, MyDeltaTime);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="b"></param>
    /// <param name="delay"></param>
    public void SetSlowMotion(bool b,float delay = 0)
    {
        StopAllCoroutines();
        if(delay > 0)
        {
            StartCoroutine(SetSlowDelay(delay, b));
            return;
        }
        slowMo = b;
    }

    /// <summary>
    /// 
    /// </summary>
    public void Pause()
    {
        m_pause = !m_pause;
        if (PauseAnim)
        {
            if (m_pause)
            {
                PauseAnim.gameObject.SetActive(true);
                PauseAnim.SetBool("show", true);
            }
            else
            {
                PauseAnim.SetBool("show", false);
                StartCoroutine(bl_Utils.AnimatorUtils.WaitAnimationLenghtForDesactive(PauseAnim));
            }
        }
        Time.timeScale = (m_pause) ? 0 : 1;
    }

    IEnumerator SetSlowDelay(float delay,bool value)
    {
        yield return new WaitForSeconds(delay);
        slowMo = value;
    }

    public float UnscaledTime
    {
        get
        {
            return MyDeltaTime;
        }
    }

    public bool isPause
    {
        get
        {
            return m_pause;
        }
    }

    public static bl_TimeManager Instance
    {
        get
        {
            return ((bl_TimeManager)mInstance);
        }
        set
        {
            mInstance = value;
        }
    }
}