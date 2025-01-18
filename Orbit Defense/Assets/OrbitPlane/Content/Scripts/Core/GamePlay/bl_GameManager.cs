
using UnityEngine;
using UnityEngine.UI;
using TTSDK.UNBridgeLib.LitJson;
using TTSDK;
using StarkSDKSpace;
using System.Collections.Generic;

public class bl_GameManager : Singleton<bl_GameManager>
{
    [HideInInspector]public bool isPlaying = false;

    [Header("References")]
    [SerializeField]private Animator MainAnim;
    [SerializeField]private Image AudioIconImage;
    [SerializeField]private Sprite AudioOnSprite;
    [SerializeField]private Sprite AudioOffSprite;
    [SerializeField]private Animator GameOverAnim;
    [SerializeField]private GameObject PlayButton;//we need show only one time this, so will desactive after use it.
    [SerializeField]private GameObject[] ExtraDefenses;
    public Transform FloatingParent;

    [Header("Backgrounds")]
    [SerializeField]private SpriteRenderer BackgroundRender;
    [SerializeField]private Color[] BackgroundsColors;

    private bool audioOn = true;
    public string clickid;
    private StarkAdManager starkAdManager;

    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        LoadSettings();
        ChangeBackground();
    }

    /// <summary>
    /// 
    /// </summary>
    void LoadSettings()
    {
        audioOn = bl_Utils.PlayerPrefsX.GetBool(KeyMaster.AudioEnable, true);
        AudioIconImage.sprite = (audioOn) ? AudioOnSprite : AudioOffSprite;
        AudioListener.pause = !audioOn;

        MainAnim.gameObject.SetActive(true);
        MainAnim.SetBool("show", true);
    }

    /// <summary>
    /// 
    /// </summary>
    public void Share()
    {
        StartCoroutine(bl_LovattoMobileUtils.TakeScreenShotAndShare(0));
    }

    /// <summary>
    /// 
    /// </summary>
    public void OnGameOver()
    {
        isPlaying = false;
        PlayButton.SetActive(false);
        MainAnim.gameObject.SetActive(true);
        MainAnim.SetBool("show", true);
        GameOverAnim.gameObject.SetActive(true);
        GameOverAnim.SetBool("show", true);
        bl_SpawnerManager.Instance.HideAll();
        //bl_ScoreManager.Instance.Reset();
        ShowInterstitialAd("1lcaf5895d5l1293dc",
            () => {
                Debug.LogError("--插屏广告完成--");

            },
            (it, str) => {
                Debug.LogError("Error->" + str);
            });
    }
    public void Continue()
    {
        ShowVideoAd("192if3b93qo6991ed0",
            (bol) => {
                if (bol)
                {
                    isPlaying = true;
                    PlayButton.SetActive(true);
                    MainAnim.gameObject.SetActive(false);
                    GameOverAnim.gameObject.SetActive(false);
                    bl_SpawnerManager.Instance.ResumeSpawn();



                    clickid = "";
                    getClickid();
                    apiSend("game_addiction", clickid);
                    apiSend("lt_roi", clickid);


                }
                else
                {
                    StarkSDKSpace.AndroidUIManager.ShowToast("观看完整视频才能获取奖励哦！");
                }
            },
            (it, str) => {
                Debug.LogError("Error->" + str);
                //AndroidUIManager.ShowToast("广告加载异常，请重新看广告！");
            });
        
    }

    /// <summary>
    /// /
    /// </summary>
    public void TryAgain()
    {
        if (isPlaying)
            return;
        //todo
        bl_ScoreManager.Instance.Reset();
        //
        bl_TimeManager.Instance.SetSlowMotion(false);
        MainAnim.SetBool("show", false);
        StartCoroutine(bl_Utils.AnimatorUtils.WaitAnimationLenghtForDesactive(MainAnim));
        GameOverAnim.SetBool("show", false);
        StartCoroutine(bl_Utils.AnimatorUtils.WaitAnimationLenghtForDesactive(GameOverAnim));
        ChangeBackground();
        bl_Planet.Instance.Reset();
        bl_SpawnerManager.Instance.Spawn();
        foreach(GameObject g in ExtraDefenses) { g.SetActive(true); }

        isPlaying = true;
    }

    void ChangeBackground()
    {
        BackgroundRender.color = BackgroundsColors[Random.Range(0, BackgroundsColors.Length)];
    }

    /// <summary>
    /// 
    /// </summary>
    public void StartGame()
    {
        isPlaying = true;
    }

    /// <summary>
    /// 
    /// </summary>
    public void SwitchAudio()
    {
        audioOn = !audioOn;
        AudioIconImage.sprite = (audioOn) ? AudioOnSprite : AudioOffSprite;
        AudioListener.pause = !audioOn;
        bl_Utils.PlayerPrefsX.SetBool(KeyMaster.AudioEnable, audioOn);
    }

    public static bl_GameManager Instance
    {
        get
        {
            return ((bl_GameManager)mInstance);
        }
        set
        {
            mInstance = value;
        }
    }


    public void getClickid()
    {
        var launchOpt = StarkSDK.API.GetLaunchOptionsSync();
        if (launchOpt.Query != null)
        {
            foreach (KeyValuePair<string, string> kv in launchOpt.Query)
                if (kv.Value != null)
                {
                    Debug.Log(kv.Key + "<-参数-> " + kv.Value);
                    if (kv.Key.ToString() == "clickid")
                    {
                        clickid = kv.Value.ToString();
                    }
                }
                else
                {
                    Debug.Log(kv.Key + "<-参数-> " + "null ");
                }
        }
    }

    public void apiSend(string eventname, string clickid)
    {
        TTRequest.InnerOptions options = new TTRequest.InnerOptions();
        options.Header["content-type"] = "application/json";
        options.Method = "POST";

        JsonData data1 = new JsonData();

        data1["event_type"] = eventname;
        data1["context"] = new JsonData();
        data1["context"]["ad"] = new JsonData();
        data1["context"]["ad"]["callback"] = clickid;

        Debug.Log("<-data1-> " + data1.ToJson());

        options.Data = data1.ToJson();

        TT.Request("https://analytics.oceanengine.com/api/v2/conversion", options,
           response => { Debug.Log(response); },
           response => { Debug.Log(response); });
    }


    /// <summary>
    /// </summary>
    /// <param name="adId"></param>
    /// <param name="closeCallBack"></param>
    /// <param name="errorCallBack"></param>
    public void ShowVideoAd(string adId, System.Action<bool> closeCallBack, System.Action<int, string> errorCallBack)
    {
        starkAdManager = StarkSDK.API.GetStarkAdManager();
        if (starkAdManager != null)
        {
            starkAdManager.ShowVideoAdWithId(adId, closeCallBack, errorCallBack);
        }
    }
    /// <summary>
    /// 播放插屏广告
    /// </summary>
    /// <param name="adId"></param>
    /// <param name="errorCallBack"></param>
    /// <param name="closeCallBack"></param>
    public void ShowInterstitialAd(string adId, System.Action closeCallBack, System.Action<int, string> errorCallBack)
    {
        starkAdManager = StarkSDK.API.GetStarkAdManager();
        if (starkAdManager != null)
        {
            var mInterstitialAd = starkAdManager.CreateInterstitialAd(adId, errorCallBack, closeCallBack);
            mInterstitialAd.Load();
            mInterstitialAd.Show();
        }
    }
}