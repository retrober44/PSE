using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointDisplay : MonoBehaviour
{
    public TextMeshProUGUI OpponentScore;
    public TextMeshProUGUI MyScore;
    public GameObject BlueCube;
    public GameObject RedCube;
    public Transform SpawnBlue;
    public Transform SpawnRed;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.PointChange.AddListener(handleEvent);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void handleEvent(PointChangeEventData arg)
    {
        Player player = GameManager.Instance.Me;
        if (arg.player == GameManager.Instance.Me && arg.points!=0)
        {
            ShowMyScore(arg.player.Points());
            getDifference();
        }
        else if(arg.player == GameManager.Instance.Opponent && arg.points != 0)
        {
            ShowOpponentScore(arg.player.Points());
            getDifference();
        }
    }


    private void getDifference()
    {
        int newCubes=0;
        int difference;
        int myPoints = GameManager.Instance.Me.Points();
        int opponentPoints = GameManager.Instance.Opponent.Points();

        if (myPoints > opponentPoints)
        {
            difference =  myPoints - opponentPoints;
            while(difference >= 60)
            {
                difference -= 60;
                Destroy(GameObject.Find("Roter_Würfel_M3"));
                GameObject newBlueCube = Instantiate(BlueCube, SpawnBlue.position, SpawnBlue.rotation);
                newCubes++;
            }
            GameManager.Instance.Me.SetCubes(newCubes + GameManager.Instance.Me.GetCubes());
            GameManager.Instance.Opponent.SetCubes(GameManager.Instance.Opponent.GetCubes() - newCubes);
        }
        else
        {
            difference = opponentPoints - myPoints;
            while (difference >= 60)
            {
                difference -= 60;
                Destroy(GameObject.Find("Blauer_Würfel_M3"));
                GameObject newRedCube = Instantiate(RedCube, SpawnRed.position, SpawnRed.rotation);
                newCubes++;
            }
            GameManager.Instance.Opponent.SetCubes(newCubes + GameManager.Instance.Opponent.GetCubes());
            GameManager.Instance.Me.SetCubes(GameManager.Instance.Me.GetCubes() - newCubes);
        }
    }

    public void ShowMyScore(int points)
    {
        MyScore.text = points.ToString();
    }
    public void ShowOpponentScore(int points)
    {
        OpponentScore.text = points.ToString();
    }
}
