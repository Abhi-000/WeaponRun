using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using EZCameraShake;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    float h, width;
    public float speed = 1f;
    [HideInInspector] public bool startRunning = false;
    public Animator anim;
    public CharacterController cc;
    public Slider levelProgress;
    public GameObject mainPlayer, player,losePanel,confetti,winPanel;
    Vector3 pos; Vector3 desiredPos;
    bool gameOver = false;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        width = Screen.width;
        anim = mainPlayer.GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !gameOver)
        {
            startRunning = true;
        }
        if (startRunning)
        {
            //levelProgress.value += 0.1f * Time.deltaTime;
            levelProgress.value = transform.position.z * 0.005f;
            anim.SetBool("run", true);
            cc.Move(new Vector3(0, -12.53f, speed * Time.deltaTime));
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Obstacle"))
        {
            CinemachineShake.instance.Shake(5f,.1f);
            //CameraShaker.Instance.ShakeOnce(10f, 10f, .1f, 1f);
            anim.SetBool("fall", true);
            anim.SetBool("run", false);
            startRunning = false;
            Invoke("GameOver", 1.5f);
        }
        else if(other.transform.CompareTag("End"))
        {
            confetti.transform.GetChild(0).transform.GetComponent<ParticleSystem>().Play();
            confetti.transform.GetChild(1).transform.GetComponent<ParticleSystem>().Play();
            StartCoroutine(WonGame());
            //Invoke("WonGame", 1f);
        }
    }
    IEnumerator WonGame()
    {
        yield return new WaitForSeconds(0.8f);
        anim.SetBool("dancing", true);
        anim.SetBool("run", false);
        startRunning = false;
        gameOver = true;
        yield return new WaitForSeconds(1f);
        winPanel.SetActive(true);
    }
    void GameOver()
    {
        gameOver = true;
        losePanel.SetActive(true);
    }
    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}