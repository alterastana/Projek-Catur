using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    // Reference from Unity IDE
    public GameObject chesspiece;

    // Matrices for positions and players
    private GameObject[,] positions = new GameObject[8, 8];
    private GameObject[] playerBlack = new GameObject[16];
    private GameObject[] playerWhite = new GameObject[16];

    // Current turn and game-over state
    private string currentPlayer = "white";
    private bool gameOver = false;

    public void Start()
    {
        playerWhite = new GameObject[] {
            Create("white_rook", 0, 0), Create("white_knight", 1, 0), Create("white_bishop", 2, 0),
            Create("white_queen", 3, 0), Create("white_king", 4, 0), Create("white_bishop", 5, 0),
            Create("white_knight", 6, 0), Create("white_rook", 7, 0),
            Create("white_pawn", 0, 1), Create("white_pawn", 1, 1), Create("white_pawn", 2, 1),
            Create("white_pawn", 3, 1), Create("white_pawn", 4, 1), Create("white_pawn", 5, 1),
            Create("white_pawn", 6, 1), Create("white_pawn", 7, 1)
        };
        playerBlack = new GameObject[] {
            Create("black_rook", 0, 7), Create("black_knight", 1, 7), Create("black_bishop", 2, 7),
            Create("black_queen", 3, 7), Create("black_king", 4, 7), Create("black_bishop", 5, 7),
            Create("black_knight", 6, 7), Create("black_rook", 7, 7),
            Create("black_pawn", 0, 6), Create("black_pawn", 1, 6), Create("black_pawn", 2, 6),
            Create("black_pawn", 3, 6), Create("black_pawn", 4, 6), Create("black_pawn", 5, 6),
            Create("black_pawn", 6, 6), Create("black_pawn", 7, 6)
        };

        for (int i = 0; i < playerBlack.Length; i++)
        {
            SetPosition(playerBlack[i]);
            SetPosition(playerWhite[i]);
        }
    }

    public GameObject Create(string name, int x, int y)
    {
        GameObject obj = Instantiate(chesspiece, new Vector3(0, 0, -1), Quaternion.identity);
        Chessman cm = obj.GetComponent<Chessman>();
        cm.name = name;
        cm.SetXBoard(x);
        cm.SetYBoard(y);
        cm.Activate();
        return obj;
    }

    public void SetPosition(GameObject obj)
    {
        Chessman cm = obj.GetComponent<Chessman>();
        positions[cm.GetXBoard(), cm.GetYBoard()] = obj;
    }

    public void SetPositionEmpty(int x, int y)
    {
        GameObject obj = positions[x, y];
        if (obj != null && obj.name.EndsWith("king"))
        {
            Winner(obj.GetComponent<Chessman>().GetPlayer() == "white" ? "black" : "white");
        }
        positions[x, y] = null;
    }

    public GameObject GetPosition(int x, int y)
    {
        return positions[x, y];
    }

    public bool PositionOnBoard(int x, int y)
    {
        return x >= 0 && y >= 0 && x < 8 && y < 8;
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
        currentPlayer = currentPlayer == "white" ? "black" : "white";
    }

    public void Update()
    {
        if (gameOver && Input.GetMouseButtonDown(0))
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

    public void EndTurn()
    {
        if (IsKingInCheck(currentPlayer))
        {
            if (IsCheckmate(currentPlayer))
            {
                Winner(currentPlayer == "white" ? "black" : "white");
            }
            else
            {
                if (CanEscapeCheck(currentPlayer))
                {
                    // King can escape check or be protected, continue the game
                }
                else
                {
                    Winner(currentPlayer == "white" ? "black" : "white");
                }
            }
        }
        else
        {
            NextTurn();
        }
    }

    public bool IsCheckmate(string player)
    {
        GameObject king = null;
        foreach (GameObject piece in player == "white" ? playerWhite : playerBlack)
        {
            if (piece != null && piece.name == player + "_king")
            {
                king = piece;
                break;
            }
        }

        if (king == null) return true;

        Chessman kingCm = king.GetComponent<Chessman>();
        int kingX = kingCm.GetXBoard();
        int kingY = kingCm.GetYBoard();

        if (IsKingInCheck(player))
        {
            return !CanEscapeCheck(player);
        }

        return false;
    }

    public bool CanEscapeCheck(string player)
    {
        GameObject king = null;
        foreach (GameObject piece in player == "white" ? playerWhite : playerBlack)
        {
            if (piece != null && piece.name == player + "_king")
            {
                king = piece;
                break;
            }
        }

        if (king == null) return false;

        Chessman kingCm = king.GetComponent<Chessman>();
        int kingX = kingCm.GetXBoard();
        int kingY = kingCm.GetYBoard();

        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        return DFSKingEscape(player, kingX, kingY, visited);
    }

    private bool DFSKingEscape(string player, int x, int y, HashSet<Vector2Int> visited)
    {
        if (visited.Contains(new Vector2Int(x, y))) return false;
        visited.Add(new Vector2Int(x, y));

        if (!PositionOnBoard(x, y)) return false;

        if (WouldBeInCheck(player, new Vector2Int(x, y))) return false;

        Vector2Int[] directions = new Vector2Int[] {
            new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1),
            new Vector2Int(1, 1), new Vector2Int(-1, -1), new Vector2Int(1, -1), new Vector2Int(-1, 1)
        };

        foreach (Vector2Int dir in directions)
        {
            int newX = x + dir.x;
            int newY = y + dir.y;
            if (DFSKingEscape(player, newX, newY, visited)) return true;
        }

        return false;
    }

    public bool WouldBeInCheck(string player, Vector2Int moveTo)
    {
        GameObject[,] backupPositions = (GameObject[,])positions.Clone();
        GameObject piece = GetPosition(moveTo.x, moveTo.y);

        SetPositionEmpty(moveTo.x, moveTo.y);
        SetPosition(piece);

        bool inCheck = IsKingInCheck(player);

        positions = backupPositions;
        return inCheck;
    }

    public bool IsKingInCheck(string player)
    {
        GameObject king = null;
        foreach (GameObject piece in player == "white" ? playerWhite : playerBlack)
        {
            if (piece != null && piece.name == player + "_king")
            {
                king = piece;
                break;
            }
        }

        if (king == null ) return false;

        Chessman kingCm = king.GetComponent<Chessman>();
        int kingX = kingCm.GetXBoard();
        int kingY = kingCm.GetYBoard();

        foreach (GameObject opponent in player == "white" ? playerBlack : playerWhite)
        {
            Chessman opponentCm = opponent.GetComponent<Chessman>();
            if (opponentCm.CanAttack(kingX, kingY))
            {
                return true;
            }
        }

        return false;
    }

    public List<GameObject> GetAllPieces(string player)
    {
        List<GameObject> pieces = new List<GameObject>();
        GameObject[] playerPieces = player == "white" ? playerWhite : playerBlack;

        foreach (GameObject piece in playerPieces)
        {
            if (piece != null)
            {
                pieces.Add(piece);
            }
        }

        return pieces;
    }

    public bool IsPositionUnderAttack(int x, int y, string player)
    {
        string opponent = player == "white" ? "black" : "white";
        List<GameObject> opponentPieces = GetAllPieces(opponent);

        foreach (GameObject piece in opponentPieces)
        {
            Chessman chessman = piece.GetComponent<Chessman>();
            if (chessman != null && chessman.AttacksPosition(x, y))
            {
                return true;
            }
        }

        return false;
    }
}