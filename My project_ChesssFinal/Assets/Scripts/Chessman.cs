using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessman : MonoBehaviour
{
    public GameObject controller;
    public GameObject movePlate;

    private int xBoard = -1;
    private int yBoard = -1;
    private string player;

    private static GameObject[,] positions = new GameObject[8, 8];
    private List<GameObject> movePlates = new List<GameObject>();

    // Sprites for all types of pieces
    public Sprite QueenB, KnightB, RookB, PawnB, KingB, BishopB;
    public Sprite QueenW, KnightW, RookW, PawnW, KingW, BishopW;

    private void OnMouseUp()
    {
        DestroyMovePlates();
        InitiateMovePlates();
    }

    public void Activate()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        SetPieceSpriteAndPlayer();
        SetScaleToFitBoard();
        SetCoords();
        positions[xBoard, yBoard] = gameObject; // Update positions array
    }

    private void SetPieceSpriteAndPlayer()
    {
        switch (this.name)
        {
            case "QueenB": SetPiece(QueenB, "black"); break;
            case "KnightB": SetPiece(KnightB, "black"); break;
            case "RookB": SetPiece(RookB, "black"); break;
            case "PawnB": SetPiece(PawnB, "black"); break;
            case "KingB": SetPiece(KingB, "black"); break;
            case "BishopB": SetPiece(BishopB, "black"); break;
            case "QueenW": SetPiece(QueenW, "white"); break;
            case "KnightW": SetPiece(KnightW, "white"); break;
            case "RookW": SetPiece(RookW, "white"); break;
            case "PawnW": SetPiece(PawnW, "white"); break;
            case "KingW": SetPiece(KingW, "white"); break;
            case "BishopW": SetPiece(BishopW, "white"); break;
        }
    }

    private void SetPiece(Sprite sprite, string playerColor)
    {
        GetComponent<SpriteRenderer>().sprite = sprite;
        player = playerColor;
    }

    public void SetCoords()
    {
        Transform boardTransform = GameObject.FindGameObjectWithTag("GameController").transform;
        float boardWidth = boardTransform.localScale.x * 8f;
        float boardHeight = boardTransform.localScale.y * 8f;

        float squareSizeX = boardWidth / 8f;
        float squareSizeY = boardHeight / 8f;

        float offsetX = -boardWidth / 2f + squareSizeX / 2f;
        float offsetY = -boardHeight / 2f + squareSizeY / 2f;

        // Adjust vertical position for certain pieces
        if (player == "white" && this.name != "PawnW")
        {
            offsetY -= 0.1f;
        }

        if (xBoard == 0 && (this.name == "RookB" || this.name == "RookW"))
        {
            offsetX -= 0.02f;
        }

        float x = offsetX + (xBoard * squareSizeX);
        float y = offsetY + (yBoard * squareSizeY);

        transform.position = new Vector3(x, y, -1.0f);
    }

    public void InitiateMovePlates()
    {
        switch (this.name)
        {
            case "RookB":
            case "RookW":
                LineMovePlate(1, 0);    // Right
                LineMovePlate(-1, 0);   // Left
                LineMovePlate(0, 1);    // Up
                LineMovePlate(0, -1);   // Down
                break;

            case "BishopB":
            case "BishopW":
                LineMovePlate(1, 1);    // Up-Right
                LineMovePlate(1, -1);   // Down-Right
                LineMovePlate(-1, 1);   // Up-Left
                LineMovePlate(-1, -1);  // Down-Left
                break;

            case "KnightB":
            case "KnightW":
                LMovePlate();
                break;

            case "PawnB":
                PawnMovementForBlack();
                break;

            case "PawnW":
                PawnMovementForWhite();
                break;

            case "KingB":
            case "KingW":
                SurroundMovePlate();
                break;

            case "QueenB":
            case "QueenW":
                LineMovePlate(1, 0);    // Right
                LineMovePlate(-1, 0);   // Left
                LineMovePlate(0, 1);    // Up
                LineMovePlate(0, -1);   // Down
                LineMovePlate(1, 1);    // Up-Right
                LineMovePlate(1, -1);   // Down-Right
                LineMovePlate(-1, 1);   // Up-Left
                LineMovePlate(-1, -1);  // Down-Left
                break;
        }
    }

    public void PawnMovementForBlack()
    {
        // Move forward
        if (yBoard > 0 && GetPosition(xBoard, yBoard - 1) == null)
        {
            MovePlateSpawn(xBoard, yBoard - 1);
            // Double move from starting position
            if (yBoard == 6 && GetPosition(xBoard, yBoard - 2) == null)
            {
                MovePlateSpawn(xBoard, yBoard - 2);
            }
        }

        // Capture diagonally
        CheckDiagonalCapture(yBoard - 1);
    }

    public void PawnMovementForWhite()
    {
        // Move forward
        if (yBoard < 7 && GetPosition(xBoard, yBoard + 1) == null)
        {
            MovePlateSpawn(xBoard, yBoard + 1);
            // Double move from starting position
            if (yBoard == 1 && GetPosition(xBoard, yBoard + 2) == null)
            {
                MovePlateSpawn(xBoard, yBoard + 2);
            }
        }

        // Capture diagonally
        CheckDiagonalCapture(yBoard + 1);
    }

    private void CheckDiagonalCapture(int targetY)
    {
        if (PositionOnBoard(xBoard + 1, targetY) && GetPosition(xBoard + 1, targetY) != null &&
            GetPosition(xBoard + 1, targetY).GetComponent<Chessman>().player != player)
        {
            MovePlateAttackSpawn(xBoard + 1, targetY);
        }

        if (PositionOnBoard(xBoard - 1, targetY) && GetPosition(xBoard - 1, targetY) != null &&
            GetPosition(xBoard - 1, targetY).GetComponent<Chessman>().player != player)
        {
            MovePlateAttackSpawn(xBoard - 1, targetY);
        }
    }

    public void LineMovePlate(int xIncrement, int yIncrement)
    {
        int x = xBoard + xIncrement;
        int y = yBoard + yIncrement;

        while (PositionOnBoard(x, y) && GetPosition(x, y) == null)
        {
            MovePlateSpawn(x, y);
            x += xIncrement;
            y += yIncrement;
        }

        if (PositionOnBoard(x, y) && GetPosition(x, y) != null)
        {
            GameObject piece = GetPosition(x, y);
            if (piece.GetComponent<Chessman>().player != player)
            {
                MovePlateAttackSpawn(x, y);
            }
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
        PointMovePlate(xBoard - 1, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard);
        PointMovePlate(xBoard - 1, yBoard + 1);
        PointMovePlate(xBoard + 1, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard);
        PointMovePlate(xBoard + 1, yBoard + 1);
    }

    public void PointMovePlate(int x, int y)
    {
        if (PositionOnBoard(x, y))
        {
            GameObject piece = GetPosition(x, y);

            if (piece == null)
            {
                MovePlateSpawn(x, y);
            }
            else if (piece.GetComponent<Chessman>().player != player)
            {
                MovePlateAttackSpawn(x, y);
            }
        }
    }

    public void MovePlateSpawn(int matrixX, int matrixY)
    {
        Transform boardTransform = GameObject.FindGameObjectWithTag("GameController").transform;
        float boardWidth = boardTransform.localScale.x * 8f;
        float boardHeight = boardTransform.localScale.y * 8f;

        float squareSizeX = boardWidth / 8f;
        float squareSizeY = boardHeight / 8f;

        float offsetX = -boardWidth / 2f + squareSizeX / 2f;
        float offsetY = -boardHeight / 2f + squareSizeY / 2f;

        float x = offsetX + (matrixX * squareSizeX);
        float y = offsetY + (matrixY * squareSizeY);

        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);
        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(matrixX, matrixY);
        
        movePlates.Add(mp);
    }

    public void MovePlateAttackSpawn(int matrixX, int matrixY)
    {
        Transform boardTransform = GameObject.FindGameObjectWithTag("GameController").transform;
        float boardWidth = boardTransform.localScale.x * 8f;
        float boardHeight = boardTransform.localScale.y * 8f;

        float squareSizeX = boardWidth / 8f;
        float squareSizeY = boardHeight / 8f;

        float offsetX = -boardWidth / 2f + squareSizeX / 2f;
        float offsetY = -boardHeight / 2f + squareSizeY / 2f;

        float x = offsetX + (matrixX * squareSizeX);
        float y = offsetY + (matrixY * squareSizeY);

        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);
        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.attack = true;
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(matrixX, matrixY);
        
        movePlates.Add(mp);
    }

    public void DestroyMovePlates()
    {
        foreach (GameObject plate in movePlates)
        {
            if (plate != null)
            {
                Destroy(plate);
            }
        }
        movePlates.Clear();
    }

    public void SetXBoard(int x)
    {
        xBoard = x;
    }

    public void SetYBoard(int y)
    {
        yBoard = y;
    }

    public int GetXBoard()
    {
        return xBoard;
    }

    public int GetYBoard()
    {
        return yBoard;
    }

    public string GetPlayer()
    {
        return player;
    }

    public GameObject GetPosition(int x, int y)
    {
        if (PositionOnBoard(x, y))
        {
            return positions[x, y];
        }
        return null;
    }

    public bool PositionOnBoard(int x, int y)
    {
        return x >= 0 && y >= 0 && x < positions.GetLength(0) && y < positions.GetLength(1);
    }

    public void SetScaleToFitBoard()
    {
        Transform boardTransform = GameObject.FindGameObjectWithTag("GameController").transform;
        float boardWidth = boardTransform.localScale.x * 8f;
        float boardHeight = boardTransform.localScale.y * 8f;

        float squareSizeX = boardWidth / 8f;
        float squareSizeY = boardHeight / 8f;
        float squareSize = Mathf.Min(squareSizeX, squareSizeY);

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && spriteRenderer.sprite != null)
        {
            float pixelsPerUnit = 100f;
            float spriteWidth = spriteRenderer.sprite.rect.width / pixelsPerUnit;
            float spriteHeight = spriteRenderer.sprite.rect.height / pixelsPerUnit;
            float spriteMaxDimension = Mathf.Max(spriteWidth, spriteHeight);

            float scaleFactor = squareSize / spriteMaxDimension;
            transform.localScale = new Vector3(scaleFactor, scaleFactor, 1);
        }
    }

   private bool IsMoveSafe(int targetX, int targetY)
    {
        // Temporarily move the king to the target position
        Vector3 originalPosition = transform.position;
        transform.position = new Vector3(targetX, targetY, -1.0f);

        // Check if the new position is attacked by any opponent pieces
        foreach (Chessman piece in FindObjectsOfType<Chessman>())  // Changed from GameObject to Chessman
        {
            if (piece.GetPlayer() != player)  // Use GetPlayer() instead of direct access
            {
                piece.InitiateMovePlates(); // Now works because we're using Chessman type
                if (piece.GetMovePlates().Contains(new Vector2(targetX, targetY)))
                {
                    // The move is not safe
                    transform.position = originalPosition; // Restore original position
                    return false;
                }
            }
        }

        // Restore original position
        transform.position = originalPosition;
        return true; // The move is safe
    }

    public List<Vector2> GetMovePlates()
    {
        List<Vector2> positions = new List<Vector2>();
        foreach (GameObject plate in movePlates)
        {
            if (plate != null)
            {
                positions.Add(new Vector2(plate.transform.position.x, plate.transform.position.y));
            }
        }
        return positions;
    }
}