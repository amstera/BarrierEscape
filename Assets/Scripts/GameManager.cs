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
    public Text ScoreText;
    public int Score;
    public bool IsMoving = true;

    public float RoadSpeed = 5f;
    public float RoadLength = 250f;

    void Start()
    {
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
    }

    public void RestartLevel()
    {
        GameAgent.Instance.GetComponent<GameAgent>().AddReward(-1f);
        GameAgent.Instance.EndEpisode();
        StartCoroutine(Restart());
    }

    public void AddScore()
    {
        GameAgent.Instance.GetComponent<GameAgent>().AddReward(1f);
        Score++;
        ScoreText.text = Score.ToString();
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

        IsMoving = true;
        RoadSpeed = 5f;
        Score = 0;

        StartGame();
    }

    private void StartGame()
    {
        Player.GetComponent<MeshRenderer>().enabled = true;
        RoadPieces[0].transform.position = new Vector3(0, 0, 118.8f);
        RoadPieces[1].transform.position = new Vector3(0, 0, 368.8f);
        PopulateBarriers(0);
        PopulateBarriers(1);
        ScoreText.text = Score.ToString();
    }
}