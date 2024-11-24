using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessman : MonoBehaviour
{
    // References to objects in our Unity Scene
    public GameObject controller;
    public GameObject movePlate;

    // Piece movement tracking
    private bool hasMoved = false;
    private bool justMovedTwoSquares = false;
    private int lastMoveFromY = -1;

    // Position for this Chesspiece on the Board
    private int xBoard = -1;
    private int yBoard = -1;

    // Variable for keeping track of the player it belongs to: "black" or "white"
    private string player;

    // References to all the possible Sprites that this Chesspiece could be
    public Sprite black_queen, black_knight, black_bishop, black_king, black_rook, black_pawn;
    public Sprite white_queen, white_knight, white_bishop, white_king, white_rook, white_pawn;

    private Game game; // Declare the game variable

    public void Activate()
    {
        // Get the game controller
        controller = GameObject.FindGameObjectWithTag("GameController");

        // Set initial transform based on board position
        SetCoords();

        // Assign correct sprite and player based on the piece's name
        switch (this.name)
        {
            case "black_queen": this.GetComponent<SpriteRenderer>().sprite = black_queen; player = "black"; break;
            case "black_knight": this.GetComponent<SpriteRenderer>().sprite = black_knight; player = "black"; break;
            case "black_bishop": this.GetComponent<SpriteRenderer>().sprite = black_bishop; player = "black"; break;
            case "black_king": this.GetComponent<SpriteRenderer>().sprite = black_king; player = "black"; break;
            case "black_rook": this.GetComponent<SpriteRenderer>().sprite = black_rook; player = "black"; break;
            case "black_pawn": this.GetComponent<SpriteRenderer>().sprite = black_pawn; player = "black"; break;
            case "white_queen": this.GetComponent<SpriteRenderer>().sprite = white_queen; player = "white"; break;
            case "white_knight": this.GetComponent<SpriteRenderer>().sprite = white_knight; player = "white"; break;
            case "white_bishop": this.GetComponent<SpriteRenderer>().sprite = white_bishop; player = "white"; break;
            case "white_king": this.GetComponent<SpriteRenderer>().sprite = white_king; player = "white"; break;
            case "white_rook": this.GetComponent<SpriteRenderer>().sprite = white_rook; player = "white"; break;
            case "white_pawn": this.GetComponent<SpriteRenderer>().sprite = white_pawn; player = "white"; break;
        }
    }

    public void SetCoords()
    {
        // Convert board position to Unity coordinates
        float x = xBoard * 0.66f + -2.3f;
        float y = yBoard * 0.66f + -2.3f;

        // Update piece position
        this.transform.position = new Vector3(x, y, -1.0f);
    }

    public int GetXBoard() => xBoard;
    public int GetYBoard() => yBoard;
    public string GetPlayer() => player;

    public void SetXBoard(int x) => xBoard = x;
    public void SetYBoard(int y) => yBoard = y;

    private void OnMouseUp()
    {
        if (!controller.GetComponent<Game>().IsGameOver() && 
            controller.GetComponent <Game>().GetCurrentPlayer() == player)
        {
            // Clear existing move plates
            DestroyMovePlates();

            // Generate new move plates
            InitiateMovePlates();
        }
    }

    public void DestroyMovePlates()
    {
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        for (int i = 0; i < movePlates.Length; i++)
        {
            Destroy(movePlates[i]);
        }
    }

    public void InitiateMovePlates()
    {
        switch (this.name)
        {
            case "black_queen":
            case "white_queen":
                LineMovePlate(1, 0);
                LineMovePlate(0, 1);
                LineMovePlate(1, 1);
                LineMovePlate(-1, 0);
                LineMovePlate(0, -1);
                LineMovePlate(-1, -1);
                LineMovePlate(-1, 1);
                LineMovePlate(1, -1);
                break;
            case "black_knight":
            case "white_knight":
                LMovePlate();
                break;
            case "black_bishop":
            case "white_bishop":
                LineMovePlate(1, 1);
                LineMovePlate(1, -1);
                LineMovePlate(-1, 1);
                LineMovePlate(-1, -1);
                break;
            case "black_king":
            case "white_king":
                SurroundMovePlate();
                break;
            case "black_rook":
            case "white_rook":
                LineMovePlate(1, 0);
                LineMovePlate(0, 1);
                LineMovePlate(-1, 0);
                LineMovePlate(0, -1);
                break;
            case "black_pawn":
                PawnMovePlate(xBoard, yBoard - 1);
                break;
            case "white_pawn":
                PawnMovePlate(xBoard, yBoard + 1);
                break;
        }
    }

    public void LineMovePlate(int xIncrement, int yIncrement)
    {
        Game sc = controller.GetComponent<Game>();

        int x = xBoard + xIncrement;
        int y = yBoard + yIncrement;

        while (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y) == null)
        {
            MovePlateSpawn(x, y);
            x += xIncrement;
            y += yIncrement;
        }

        if (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y).GetComponent<Chessman>().player != player)
        {
            MovePlateAttackSpawn(x, y);
        }
    }

    public void LMovePlate()
    {
        PointMovePlate(xBoard + 1, yBoard + 2);
        PointMovePlate(xBoard - 1, yBoard + 2);
        PointMovePlate(xBoard + 2, yBoard + 1);
        PointMovePlate(xBoard + 2, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard - 2);
        PointMovePlate(xBoard - 1, yBoard - 2);
        PointMovePlate(xBoard - 2, yBoard + 1);
        PointMovePlate(xBoard - 2, yBoard - 1);
    }

    public void SurroundMovePlate()
    {
        PointMovePlate(xBoard, yBoard + 1);
        PointMovePlate(xBoard, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard);
        PointMovePlate(xBoard - 1, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard + 1);
        PointMovePlate(xBoard + 1, yBoard);
        PointMovePlate(xBoard + 1, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard + 1);
    }

    public void PawnMovePlate(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();
        
        // Forward movement
        if (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y) == null)
        {
            MovePlateSpawn(x, y);
            
            // First move - can move 2 squares if path is clear
            if (!hasMoved)
            {
                int twoSquaresY = player == "black" ? yBoard - 2 : yBoard + 2;
                if (sc.PositionOnBoard(x, twoSquaresY) && sc.GetPosition(x, twoSquaresY) == null)
                {
                    MovePlateSpawn(x, twoSquaresY);
                }
            }
        }

        // Diagonal captures
        int[] captureX = { x - 1, x + 1 };
        foreach (int capX in captureX)
        {
            if (sc.PositionOnBoard(capX, y))
            {
                GameObject piece = sc.GetPosition(capX, y);
                if (piece != null && piece.GetComponent<Chessman>().player != player)
                {
                    MovePlateAttackSpawn(capX, y);
                }
            }
        }

        // En Passant
        if ((player == "white" && yBoard == 4) || (player == "black" && yBoard == 3))
        {
            foreach (int capX in captureX)
            {
                if (sc.PositionOnBoard(capX, yBoard))
                {
                    GameObject adjacentPiece = sc.GetPosition(capX, yBoard);
                    if (adjacentPiece != null && 
                        adjacentPiece.name.EndsWith("pawn") &&
                        adjacentPiece.GetComponent<Chessman>().justMovedTwoSquares && 
                        adjacentPiece.GetComponent<Chessman>().player != player)
                    {
                        // Create special en passant move plate
                        MovePlateEnPassantSpawn(capX, y, adjacentPiece);
                    }
                }
            }
        }
    }

    public void PointMovePlate(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();
        if (sc.PositionOnBoard(x, y))
        {
            GameObject cp = sc.GetPosition(x, y);

            if (cp == null)
            {
                MovePlateSpawn(x, y);
            }
            else if (cp.GetComponent<Chessman>().player != player)
            {
                MovePlateAttackSpawn(x, y);
            }
        }
    }

    public void MovePlateSpawn(int matrixX, int matrixY)
    {
        float x = matrixX * 0.66f + -2.3f;
        float y = matrixY * 0.66f + -2.3f;

        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);
        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(matrixX, matrixY);
    }

    public void MovePlateAttackSpawn(int matrixX, int matrixY)
    {
        float x = matrixX * 0.66f + -2.3f;
        float y = matrixY * 0.66f + -2.3f;

        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);
        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.attack = true;
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(matrixX, matrixY);
    }

    public void MovePlateEnPassantSpawn(int matrixX, int matrixY, GameObject pieceToCapture)
    {
        float x = matrixX * 0.66f + -2.3f;
        float y = matrixY * 0.66f + -2.3f;

        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);
        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.attack = true;
        mpScript.enPassant = true;
        mpScript.pieceToCapture = pieceToCapture;
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(matrixX, matrixY);
    }

    // Called after a piece moves
    public void OnPieceMoved(int oldY)
    {
        lastMoveFromY = oldY;
        hasMoved = true;
        justMovedTwoSquares = Mathf.Abs(oldY - yBoard) == 2;
    }

    // Called at the start of each turn
    public void OnTurnStart()
    {
        justMovedTwoSquares = false;
    }

    // Called when a pawn reaches the opposite end of the board
    public void PromotePawn()
    {
        if ((player == "white" && yBoard == 7) || (player == "black" && yBoard == 0))
        {
            // Change to queen (you could add UI to let player choose the piece)
            this.name = player + "_queen";
            this.GetComponent<SpriteRenderer>().sprite = 
                player == "white" ? white_queen : black_queen;
        }
    }

    public bool AttacksPosition(int x, int y)
    {
        // Example logic for a few types of pieces
        
        // For a rook (can move horizontally or vertically)
        if (this.name.Contains("rook"))
        {
            return x == xBoard || y == yBoard;
        }

        // For a bishop (can move diagonally)
        if (this.name.Contains("bishop"))
        {
 return Mathf.Abs(x - xBoard) == Mathf.Abs(y - yBoard);
        }

        // For a queen (can move both like a rook and a bishop)
        if (this.name.Contains("queen"))
        {
            return x == xBoard || y == yBoard || Mathf.Abs(x - xBoard) == Mathf.Abs(y - yBoard);
        }

        // For a knight (moves in an "L" shape)
        if (this.name.Contains("knight"))
        {
            int dx = Mathf.Abs(x - xBoard);
            int dy = Mathf.Abs(y - yBoard);
            return (dx == 2 && dy == 1) || (dx == 1 && dy == 2);
        }

        // For a pawn (can attack diagonally one step forward)
        if (this.name.Contains("pawn"))
        {
            int direction = player == "white" ? 1 : -1; // White pawns move upwards, black pawns move downwards
            return Mathf.Abs(x - xBoard) == 1 && (y - yBoard == direction);
        }

        // For a king (can move one square in any direction)
        if (this.name.Contains("king"))
        {
            return Mathf.Abs(x - xBoard) <= 1 && Mathf.Abs(y - yBoard) <= 1;
        }

        return false; // Default: if no attack logic, return false
    }

    public bool IsKingInCheck(int kingX, int kingY)
    {
        Game sc = controller.GetComponent<Game>();
        // Iterate over all opponent's pieces to check if they can attack the king
        foreach (var piece in sc.GetAllPieces(player == "white" ? "black" : "white"))
        {
            if (piece.GetComponent<Chessman>().AttacksPosition(kingX, kingY))
            {
                return true; // King is in check
            }
        }
        return false; // King is not in check
    }

    // Check if the king can escape check (i.e., move to a safe square)
    public bool CanKingEscapeCheck(int kingX, int kingY)
    {
        Game sc = controller.GetComponent<Game>();
        // Try all possible moves for the king
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                int newX = kingX + dx;
                int newY = kingY + dy;
                if (sc.PositionOnBoard(newX, newY) && !sc.IsPositionUnderAttack(newX, newY, player))
                {
                    return true; // There's a valid escape move for the king
                }
            }
        }
        return false; // No escape move available for the king
    }

    // Check if it's checkmate (the king is in check and cannot escape)
    public bool IsCheckmate(int kingX, int kingY)
    {
        if (IsKingInCheck(kingX, kingY) && !CanKingEscapeCheck(kingX, kingY))
        {
            return true; // It's checkmate
        }
        return false; // It's not checkmate
    }

    // Check if a piece can block the check (move between the attacking piece and the king)
    public bool CanPieceBlockCheck(int kingX, int kingY)
    {
        Game sc = controller.GetComponent<Game>();
        // Iterate through pieces to see if any can block the check
        foreach (var piece in sc.GetAllPieces(player))
        {
            Chessman chessPiece = piece.GetComponent<Chessman>();
            if (chessPiece.CanBlockCheck(kingX, kingY))
            {
                return true; // A piece can block the check
            }
        }
        return false; // No pieces can block the check
    }

    // Check if a piece can block the check (based on piece type and movement)
    public bool CanBlockCheck(int kingX, int kingY)
    {
        // Each piece will have different blocking logic, depending on its type
        Game sc = controller.GetComponent<Game>();
        if (this.name.Contains("rook") || this.name.Contains("queen"))
        {
            // For rook/queen, can move in straight lines to block
            if (xBoard == kingX)
            {
                // Check vertically
                for (int y = Mathf.Min(kingY, yBoard) + 1; y < Mathf.Max(kingY, yBoard); y++)
                {
                    if (sc.GetPosition(xBoard, y) != null) return false;
                }
                return true;
            }
            if (yBoard == kingY)
            {
                // Check horizontally
                for (int x = Mathf.Min( kingX, xBoard) + 1; x < Mathf.Max(kingX, xBoard); x++)
                {
                    if (sc.GetPosition(x, yBoard) != null) return false;
                }
                return true;
            }
        }

        if (this.name.Contains("bishop") || this.name.Contains("queen"))
        {
            // For bishop/queen, can move diagonally to block
            if (Mathf.Abs(kingX - xBoard) == Mathf.Abs(kingY - yBoard))
            {
                int xDirection = (kingX > xBoard) ? 1 : -1;
                int yDirection = (kingY > yBoard) ? 1 : -1;

                for (int i = 1; i < Mathf.Abs(kingX - xBoard); i++)
                {
                    if (sc.GetPosition(xBoard + i * xDirection, yBoard + i * yDirection) != null) return false;
                }
                return true;
            }
        }

        return false; // Can't block check
    }

    // Check if a piece can capture the attacker (the piece attacking the king)
    public bool CanPieceCaptureAttacker(int attackerX, int attackerY)
    {
        Game sc = controller.GetComponent<Game>();
        foreach (var piece in sc.GetAllPieces(player))
        {
            Chessman chessPiece = piece.GetComponent<Chessman>();
            if (chessPiece.AttacksPosition(attackerX, attackerY))
            {
                return true; // A piece can capture the attacker
            }
        }
        return false; // No piece can capture the attacker
    }

    public bool CanAttack(int targetX, int targetY)
    {
        // Example logic for pieces' attack range
        if (this.name.Contains("rook"))
        {
            return targetX == xBoard || targetY == yBoard;
        }
        if (this.name.Contains("bishop"))
        {
            return Mathf.Abs(targetX - xBoard) == Mathf.Abs(targetY - yBoard);
        }
        if (this.name.Contains("queen"))
        {
            return targetX == xBoard || targetY == yBoard || Mathf.Abs(targetX - xBoard) == Mathf.Abs(targetY - yBoard);
        }
        if (this.name.Contains("knight"))
        {
            int dx = Mathf.Abs(targetX - xBoard);
            int dy = Mathf.Abs(targetY - yBoard);
            return (dx == 2 && dy == 1) || (dx == 1 && dy == 2);
        }
        if (this.name.Contains("pawn"))
        {
            int direction = player == "white" ? 1 : -1;
            return Mathf.Abs(targetX - xBoard) == 1 && (targetY - yBoard == direction);
        }
        if (this.name.Contains("king"))
        {
            return Mathf.Abs(targetX - xBoard) <= 1 && Mathf.Abs(targetY - yBoard) <= 1;
        }
        return false;
    }

    void Start()
    {
        game = FindObjectOfType<Game>();  // Reference to the Game instance
    }
}