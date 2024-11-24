using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public GameObject chesspiece;

    private GameObject[,] positions = new GameObject[8, 8];
    private GameObject[] playerWhite;
    private GameObject[] playerBlack;

    private string currentPlayer = "white";
    private bool gameOver = false;

    void Start()
    {
        playerWhite = new GameObject[] {
            Create("RookW", 0, 0), Create("KnightW", 1, 0),
            Create("BishopW", 2, 0), Create("QueenW", 3, 0), Create("KingW", 4, 0),
            Create("BishopW", 5, 0), Create("KnightW", 6, 0), Create("RookW", 7, 0),
            Create("PawnW", 0, 1), Create("PawnW", 1, 1), Create("PawnW", 2, 1),
            Create("PawnW", 3, 1), Create("PawnW", 4, 1), Create("PawnW", 5, 1),
            Create("PawnW", 6, 1), Create("PawnW", 7, 1)
        };

        playerBlack = new GameObject[] {
            Create("RookB", 0, 7), Create("KnightB", 1, 7),
            Create("BishopB", 2, 7), Create("QueenB", 3, 7), Create("KingB", 4, 7),
            Create("BishopB", 5, 7), Create("KnightB", 6, 7), Create("RookB", 7, 7),
            Create("PawnB", 0, 6), Create("PawnB", 1, 6), Create("PawnB", 2, 6),
            Create("PawnB", 3, 6), Create("PawnB", 4, 6), Create("PawnB", 5, 6),
            Create("PawnB", 6, 6), Create("PawnB", 7, 6)
        };

        for (int i = 0; i < playerBlack.Length; i++)
        {
            SetPosition(playerBlack[i]);
            SetPosition(playerWhite[i]);
        }
    }

    public GameObject Create(string name, int x, int y)
    {
        if (chesspiece == null)
        {
            Debug.LogError("Chesspiece prefab is not assigned in the Inspector!");
            return null;
        }

        GameObject obj = Instantiate(chesspiece, new Vector3(0, 0, -1), Quaternion.identity);
        Chessman cm = obj.GetComponent<Chessman>();

        if (cm == null)
        {
            Debug.LogError("Chesspiece prefab must have a Chessman script attached!");
            return null;
        }

        cm.name = name;
        cm.SetXBoard(x);
        cm.SetYBoard(y);
        cm.Activate();
        return obj;
    }

    public void SetPosition(GameObject obj)
    {
        Chessman cm = obj.GetComponent<Chessman>();
        if (cm != null)
        {
            positions[cm.GetXBoard(), cm.GetYBoard()] = obj;
        }
        else
        {
            Debug.LogError("GameObject does not have a Chessman script!");
        }
    }

    public void SetPositionEmpty(int x, int y)
    {
        positions[x, y] = null;
    }

    public GameObject GetPosition(int x, int y)
    {
        return positions[x, y];
    }

    public bool PositionOnBoard(int x, int y)
    {
        if (x < 0 || y < 0 || x >= positions.GetLength(0) || y >= positions.GetLength(1)) return false;
        return true;
    }

    public string GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public bool IsGameOver()
    {
        return gameOver;
    }

    public void NextTurn()
    {
        if (currentPlayer == "white")
        {
            currentPlayer = "black";
        }
        else
        {
            currentPlayer = "white";
        }
    }

    void Update()
    {
        if (gameOver == true && Input.GetMouseButtonDown(0))
        {
            gameOver = false;
            SceneManager.LoadScene("Game");
        }
    }

    public void Winner(string playerWinner)
    {
        gameOver = true;

        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().enabled = true;
        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().text = playerWinner + " is the winner";

        GameObject.FindGameObjectWithTag("RestartText").GetComponent<Text>().enabled = true;
    }
}
