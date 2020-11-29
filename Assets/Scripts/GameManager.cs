﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject Player;
    public GameObject Barrier;
    public GameObject MiddleBarrier;
    public GameObject[] RoadPieces = new GameObject[2];
    public Material Skybox;
    public Text ScoreText;
    public Text HighScoreText;
    public Color StartScoreColor;
    public Color EndScoreColor;
    public Color[] StartSkyColors = new Color[2];
    public Color[] EndSkyColors = new Color[2];
    public int Score;
    public int HighScore;
    public bool IsMoving = true;

    public AudioSource Blip;
    public AudioSource Crash;
    public AudioSource Rolling;

    public float RoadSpeed = 5f;
    public float RoadLength = 250f;

    private bool _scaleScoreText;
    private float _elapsedTime;

    void Start()
    {
        HighScore = PlayerPrefs.HasKey("HighScore") ? 0 : PlayerPrefs.GetInt("HighScore");
        StartGame();

        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Update()
    {
        if (!IsMoving)
        {
            return;
        }

        _elapsedTime += Time.deltaTime;

        RoadSpeed = Mathf.Clamp(RoadSpeed + Time.deltaTime / 4, 5, 15.5f);

        for(int i = 0; i < RoadPieces.Length; i++)
        {
            Vector3 newRoadPos = RoadPieces[i].transform.position;
            newRoadPos.z -= RoadSpeed * Time.deltaTime;
            if (newRoadPos.z < -RoadLength/2 - 50)
            {
                RoadPieces[i].transform.position = new Vector3(0, 0, RoadPieces[(i + 1) % RoadPieces.Length].transform.position.z + RoadLength);
                PopulateBarriers(i);
            }
            else
            {
                RoadPieces[i].transform.position = newRoadPos;
            }
        }

        if (_scaleScoreText)
        {
            ScoreText.transform.localScale += Vector3.one * Time.deltaTime;
            if (ScoreText.transform.localScale.x >= 1.15f)
            {
                _scaleScoreText = false;
            }
        }
        else if (ScoreText.transform.localScale.x > 1)
        {
            ScoreText.transform.localScale -= Vector3.one * Time.deltaTime;
        }

        Skybox.SetColor("_SkyGradientTop", Color.Lerp(StartSkyColors[0], EndSkyColors[0], Mathf.Clamp(_elapsedTime / 35, 0, 35)));
        Skybox.SetColor("_SkyGradientBottom", Color.Lerp(StartSkyColors[1], EndSkyColors[1], Mathf.Clamp(_elapsedTime / 35, 0, 35)));
    }

    public void RestartLevel()
    {
        GameAgent.Instance.GetComponent<GameAgent>().AddReward(-1f);
        GameAgent.Instance.EndEpisode();
        Rolling.Stop();
        Crash.Play();
        StartCoroutine(Restart());
    }

    public void AddScore()
    {
        GameAgent.Instance.GetComponent<GameAgent>().AddReward(1f);
        Score++;
        ScoreText.text = Score.ToString();
        float scorePercentage = Mathf.Clamp((float)Score / 35, 0, 1);
        ScoreText.color = Color.Lerp(StartScoreColor, EndScoreColor, scorePercentage);
        _scaleScoreText = true;
        Blip.Play();
    }

    private void PopulateBarriers(int index)
    {
        foreach (Transform child in RoadPieces[index].transform)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < RoadLength/8.25f; i++)
        {
            float zPos = RoadPieces[index].transform.position.z - RoadLength/2 + 6.2f + 8f * i;
            GameObject barrier;
            if (Random.Range(0, 3) == 1)
            {
                barrier = Instantiate(MiddleBarrier, new Vector3(0, 0, zPos), Quaternion.identity);
            }
            else
            {
                barrier = Instantiate(Barrier, new Vector3(i % 2 == 0 ? -1 : 1, 0.6f, zPos), Quaternion.identity);
            }
            barrier.transform.parent = RoadPieces[index].transform;
        }
    }

    private IEnumerator Restart()
    {
        yield return new WaitForSeconds(1);

        if (Score > HighScore)
        {
            HighScore = Score;
            PlayerPrefs.SetInt("HighScore", HighScore);
            PlayerPrefs.Save();
        }

        IsMoving = true;
        RoadSpeed = 5f;
        Score = 0;
        _elapsedTime = 0;

        StartGame();
    }

    private void StartGame()
    {
        Rolling.Play();
        Player.GetComponent<MeshRenderer>().enabled = true;
        Player.transform.position = new Vector3(0, 0.5f, -5.5f);
        RoadPieces[0].transform.position = new Vector3(0, 0, 118.8f);
        RoadPieces[1].transform.position = new Vector3(0, 0, 368.8f);
        PopulateBarriers(0);
        PopulateBarriers(1);
        ScoreText.text = Score.ToString();
        ScoreText.color = StartScoreColor;
        HighScoreText.text = HighScore.ToString();
    }
}
