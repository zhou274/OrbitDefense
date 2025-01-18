
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class bl_EnemyController : MonoBehaviour
{
    public float Speed = 10;
    [SerializeField]private GameObject ParticleHit;
    [SerializeField]private AudioClip[] DestroySound;
    [SerializeField]private AudioClip[] PlanelHitSound;
    [SerializeField]private SpriteRenderer TrailImage;
    [SerializeField]private GameObject FloatingText;

    private Transform Player;
    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        Speed =  Speed + bl_ScoreManager.Instance.SpeedIncrement;
    }

    /// <summary>
    /// 
    /// </summary>
    void OnEnable()
    {
        if (TrailImage)
        {
            Color c = TrailImage.color;
            c.a = 0;
            TrailImage.color = c;

            StopAllCoroutines();
            StartCoroutine(FadeTrail(c));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        if (Player == null)
            return;

        Move();
    }

    /// <summary>
    /// 
    /// </summary>
    void Move()
    {
        //Lookat 2D
        Vector3 dir = Player.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        //move towards the player
        if (Vector3.Distance(transform.position, Player.position) > 0.5f)
        {//move if distance from target is greater than 1
            transform.Translate(new Vector3(Speed * Time.deltaTime, 0, 0));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="c"></param>
    void OnTriggerEnter2D(Collider2D c)
    {
        if (c.gameObject.GetComponent<bl_PlayerController>() != null)
        {
            PlayerHit();
        }
        else if(c.gameObject.GetComponent<bl_Planet>() != null)
        {
            PlanetHit(c.gameObject);
        }
        else if(c.transform.parent.GetComponent<bl_AutoRotation>() != null)
        {
            bl_Shaker.Instance.Do(1);
            Vector3 mypos = transform.position;
            mypos.z = 0;
            GameObject par = Instantiate(ParticleHit, mypos, Quaternion.identity) as GameObject;
            Destroy(par, 1.5f);
            AudioSource.PlayClipAtPoint(DestroySound[Random.Range(0, DestroySound.Length)], Camera.main.transform.position);
            c.transform.parent.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void PlayerHit()
    {
        bl_Shaker.Instance.Do(0);
        bl_ScoreManager.Instance.AddScore();
        Vector3 mypos = transform.position;
        mypos.z = 0;
        GameObject par = Instantiate(ParticleHit, mypos, Quaternion.identity) as GameObject;
        Destroy(par, 1.5f);
        AudioSource.PlayClipAtPoint(DestroySound[Random.Range(0, DestroySound.Length)], Camera.main.transform.position);

        GameObject ft = Instantiate(FloatingText) as GameObject;
        ft.transform.SetParent(bl_GameManager.Instance.FloatingParent, false);
        bl_FloatingText.Info ftinfo = new bl_FloatingText.Info();
        Vector3 vp = Camera.main.ViewportToScreenPoint(transform.position);
        bool vpup = (vp.y > 0.5f) ? true : false;
        float vertical = (vpup) ? 1 : -1;
        ftinfo.FloatDirection = new Vector3(Random.Range(-0.1f, 0.1f), vertical, 0);
        ftinfo.Delay = 2;
        ftinfo.InitPosition = transform.position;
        ftinfo.Speed = 75;
        ftinfo.Text = "+1";
        ft.GetComponent<bl_FloatingText>().Instance(ftinfo);

        gameObject.SetActive(false);
    }

    /// <summary>
    /// 
    /// </summary>
    void PlanetHit(GameObject c)
    {
        bl_Shaker.Instance.Do(1);

        c.GetComponent<bl_Planet>().DoDamage();
        AudioSource.PlayClipAtPoint(PlanelHitSound[Random.Range(0, DestroySound.Length)], Camera.main.transform.position);
        gameObject.SetActive(false);
    }


    IEnumerator FadeTrail(Color targetColor)
    {
        yield return new WaitForSeconds(1f);
        while(targetColor.a < 1)
        {
            targetColor.a += Time.deltaTime / 2;
            TrailImage.color = targetColor;
            yield return new WaitForEndOfFrame();
        }
    }
}