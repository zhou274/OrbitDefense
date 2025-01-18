////////////////////////////////////////////////////////////////////////////
// bl_ScoreManager
//
//
//                    Lovatto Studio 2016
////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class bl_ScoreManager : Singleton<bl_ScoreManager>
{
    public List<bl_LevelInfo> Levels = new List<bl_LevelInfo>();
    [Header("References")]
    [SerializeField]private TextMeshProUGUI ScoreText;
    [SerializeField]private Text LastScoreText;
    [SerializeField]private TextMeshProUGUI NewLevelText;
    [SerializeField]private TextMeshProUGUI BestScoreMenuText;
    [SerializeField]private Animator ScoreAnim;
    [SerializeField]private Animator BestScorePlayAnim;
    [SerializeField]private Animator NewLevelAnim;

    private int m_currentScore;
    private bool reachedBestOnPlay = false;
    private int m_CurrentLevel = 0;
    private int m_LastLevel = 0;

    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
        int best = PlayerPrefs.GetInt(KeyMaster.BestScore, 0);
        BestScoreMenuText.text = string.Format("最高分: {0}", best);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="add"></param>
    public void AddScore(int add = 1)
    {
        m_currentScore += add;
        ScoreAnim.Play("point", 0, 0);
        ScoreText.text = string.Format("得分:{0}", m_currentScore);
        CheckBestPlaytime();
        CheckLevels();
    }

    /// <summary>
    /// 
    /// </summary>
    void CheckBestPlaytime()
    {
        int best = PlayerPrefs.GetInt(KeyMaster.BestScore, 0);
        if (m_currentScore > best && !reachedBestOnPlay && m_currentScore > 1)
        {
            //NewScore
            reachedBestOnPlay = true;
            BestScorePlayAnim.gameObject.SetActive(true);
            BestScorePlayAnim.SetBool("show", true);
            StartCoroutine(bl_Utils.AnimatorUtils.WaitAnimationReverseForDesactive(BestScorePlayAnim, "show", 4));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void Reset()
    {
        m_currentScore = 0;
        ScoreText.text = string.Format("得分:{0}", m_currentScore);
        reachedBestOnPlay = false;
        m_CurrentLevel = 0;
        m_LastLevel = 0;
    }

    /// <summary>
    /// 
    /// </summary>
    public void SaveScore()
    {
        int best = PlayerPrefs.GetInt(KeyMaster.BestScore, 0);
        if(m_currentScore > best)
        {
            PlayerPrefs.SetInt(KeyMaster.BestScore, m_currentScore);
            best = m_currentScore;
        }
        BestScoreMenuText.text = string.Format("最高分: {0}", best);
        LastScoreText.text = m_currentScore.ToString();
    }

    /// <summary>
    /// 
    /// </summary>
    void CheckLevels()
    {
        if (m_CurrentLevel - 1 <= Levels.Count)
        {
            for (int i = 0; i < Levels.Count; i++)
            {
                if (m_CurrentLevel + 1 < Levels.Count - 1)
                {
                    if (m_currentScore >= Levels[i].PoinNeeded && m_currentScore < Levels[i + 1].PoinNeeded)
                    {
                        m_CurrentLevel = i + 1;

                        if (m_LastLevel != m_CurrentLevel)
                        {
                            //New Level
                            if (m_LastLevel > 0)
                            {
                                if (NewLevelAnim)
                                {
                                    NewLevelText.text = string.Format("新阶段: {0}", m_CurrentLevel);
                                    NewLevelAnim.gameObject.SetActive(true);
                                    NewLevelAnim.SetBool("show", true);
                                    StartCoroutine(bl_Utils.AnimatorUtils.WaitAnimationReverseForDesactive(NewLevelAnim, "show", 2));
                                }
                            }
                            m_LastLevel = m_CurrentLevel;
                        }
                    }
                }
                else
                {
                    m_CurrentLevel = Levels.Count;
                }
            }
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public float SpeedIncrement
    {
        get
        {
            return Levels[m_CurrentLevel].SpeedIncrement;
        }
    }

    public static bl_ScoreManager Instance
    {
        get
        {
            return ((bl_ScoreManager)mInstance);
        }
        set
        {
            mInstance = value;
        }
    }
}