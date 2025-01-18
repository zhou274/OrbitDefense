////////////////////////////////////////////////////////////////////////////
// bl_PlayerController
//
//
//                    Lovatto Studio 2016
////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class bl_PlayerController : MonoBehaviour
{
    [Header("Settings")]
    [Range(10,1000)]public float Speed = 10;
    [Range(10,1000)]public float SpeedBoost = 10;
    [Header("References")]
    [SerializeField]private RectTransform Player;
    [SerializeField]private RectTransform TrailRect;
    [SerializeField]private AudioClip MoveSound;

    private bool rotateToRight = true;
    private float currentSpeed;
    private float timePress = 0;
    private float TrailSidePos;
    private AudioSource Source;
    private bool isPushBost = false;
    private bl_GameManager Manager;
    private bl_TimeManager TimeManager;
    private bool overPause = false;

    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        Manager = bl_GameManager.Instance;
        TimeManager = bl_TimeManager.Instance;
        currentSpeed = Speed;
        TrailSidePos = TrailRect.anchoredPosition.x;
        Source = GetComponent<AudioSource>();
        Source.clip = MoveSound;
    }

    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        if (Player == null)
            return;
        if (!Manager.isPlaying)
            return;
        if (TimeManager.isPause)
            return;

        if (!overPause)
        {
            InputControl();
        }
        RotateControl();
        SoundControl();
    }

    /// <summary>
    /// 
    /// </summary>
    void RotateControl()
    {
        float next = (isPushBost) ? SpeedBoost : Speed;
        currentSpeed = Mathf.Lerp(currentSpeed, next, Time.deltaTime * bl_Utils.MathfUtils.EaseLerpExtreme(8));
        Vector3 side = (rotateToRight) ? -Vector3.forward : Vector3.forward;
        Player.Rotate(side * Time.deltaTime * currentSpeed, Space.World);
    }

    /// <summary>
    /// 
    /// </summary>
    void InputControl()
    {
        if (Input.GetMouseButton(0))
        {
            timePress += Time.deltaTime;
            if(timePress > 0.2f)
            {
                isPushBost = true;
                Source.volume = 1;
            }
            else { currentSpeed = Speed; isPushBost = false; }
        }
        else { currentSpeed = Speed; timePress = 0; }

        //switch position to rotate when click or touch
        if (Input.GetMouseButtonDown(0))
        {
            rotateToRight = !rotateToRight;
            if (TrailRect != null)
            {
                TrailSidePos = TrailSidePos * -1;
                Vector2 tp = TrailRect.anchoredPosition;
                tp.x = TrailSidePos;
                TrailRect.anchoredPosition = tp;
            }
            Source.Play();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void SoundControl()
    {
        Source.volume = Mathf.Lerp(Source.volume, 0, Time.deltaTime);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="b"></param>
    public void SetOverPause(bool b)
    {
        overPause = b;
    }
}