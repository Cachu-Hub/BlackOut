using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    Player playerScript;
    public AudioClip bgm;

    AudioSource audioSource;
    public GameObject player;
    public GameObject[] Stages;
    public GameObject[] Finishs;
    public Image[] health;
    public Image[] menuImage;
    public Text stage;
    public Text score;
    public int stageNumber;
    public bool GameIsPaused = false; 
    public GameObject pauseMenuCanvas;
    public GameObject retryCanvas;
    int menuCheck = 0;


    Vector3 startPos = new Vector3(-9, -8, 0);
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
                Resume();
            else
            {
                menuCheck = 0;
                menuImage[0].gameObject.SetActive(true);
                menuImage[1].gameObject.SetActive(false);
                Pause();
            }
        }
        if (GameIsPaused)
        {
            if (pauseMenuCanvas.activeSelf)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (menuCheck == 0)
                        Resume();
                    else
                        QuitGame();
                }
                if (Input.GetKeyDown(KeyCode.UpArrow) ||
                    Input.GetKeyDown(KeyCode.DownArrow))
                {
                    if (menuCheck == 0)
                    {
                        menuImage[1].gameObject.SetActive(true);
                        menuImage[0].gameObject.SetActive(false);
                        menuCheck = 1;
                    }
                    else
                    {
                        menuImage[0].gameObject.SetActive(true);
                        menuImage[1].gameObject.SetActive(false);
                        menuCheck = 0;
                    }
                }
            }
            else if (retryCanvas.activeSelf)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Retry();
                }
            }
        }
        
    }
    public void Resume()
    {
        pauseMenuCanvas.SetActive(false);
        playerScript.DontJump = true;
        Time.timeScale = 1f; 
        GameIsPaused = false; 
    }
    public void Pause()
    {
        pauseMenuCanvas.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }
    public void QuitGame() 
    { 
        Application.Quit(); 
    }
    public void Retry()
    {
        Init();
        GameIsPaused = false;
    }

    public void PlayerScoreUpdate()
    {
        score.text = playerScript.Score.ToString();
    }
    public void PlayerHealthUpdate()
    {
        for (int i = 0; i < playerScript.Health && i < 3; i++)
            health[i].gameObject.SetActive(true);
        for (int i = playerScript.Health; i < 3; i++)
            health[i].gameObject.SetActive(false);
    }
    public void StageChange(Finish finish)
    {
        SetStage(finish.nextStage);
        playerScript.Teleport(Finishs[finish.nextFinish].transform.position
            + Vector3.up * 3 +Vector3.forward);
    }
    public void PlayerDead()
    {
        Debug.Log("Player Dead!!");
        Time.timeScale = 0f;
        GameIsPaused = true;
        retryCanvas.SetActive(true);
    }
    void SetStage(int num)
    {
        for (int i = 0; i < Stages.Length; i++)
            Stages[i].SetActive(false);
        //player.transform.parent = Stages[num].transform;
        Stages[num].SetActive(true);
        if (num == 0)
            stage.text = "Practice";
        else
            stage.text = "Stage " + num.ToString();
    }

    void Init()
    {
        retryCanvas.SetActive(false);
        pauseMenuCanvas.SetActive(false);
        audioSource = GetComponent<AudioSource>();
        playerScript = player.GetComponent<Player>();
        audioSource.clip = bgm;
        audioSource.Play();
        SetStage(0);
        score.text = playerScript.Score.ToString();
        player.SetActive(true);
        playerScript.IsDead = false;
        playerScript.Health = 3;
        playerScript.Score = 0;
        playerScript.Teleport(startPos);
        playerScript.Init();
        PlayerHealthUpdate();
        PlayerScoreUpdate();
        Time.timeScale = 1f;
    }
    private void Awake()
    {
        Init();
    }

}
