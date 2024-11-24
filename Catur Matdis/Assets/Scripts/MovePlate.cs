using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlate : MonoBehaviour
{
    public GameObject controller;
    
    GameObject reference = null;
    
    // Board positions
    int matrixX;
    int matrixY;
    
    // false: movement, true: attacking
    public bool attack = false;
    public bool enPassant = false;
    public GameObject pieceToCapture = null;

    public void Start()
    {
        if (controller == null)
        {
            controller = GameObject.FindGameObjectWithTag("GameController");
        }

        // Set color based on whether this is an attack move
        if (attack)
        {
            // Set to red with some transparency for attack moves
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.75f);
        }
        else
        {
            // Set to default blue with transparency for normal moves
            gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 1, 1, 0.75f);
        }
    }

    public void OnMouseUp()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        
        if (attack)
        {
            GameObject chessPiece = controller.GetComponent<Game>().GetPosition(matrixX, matrixY);
            
            if (enPassant && pieceToCapture != null)
            {
                chessPiece = pieceToCapture;
            }

            if (chessPiece != null)
            {
                Destroy(chessPiece);
            }
        }

        // Save the old position for pawn movement tracking
        int oldY = reference.GetComponent<Chessman>().GetYBoard();

        // Move reference chess piece to this position
        controller.GetComponent<Game>().SetPositionEmpty(
            reference.GetComponent<Chessman>().GetXBoard(),
            reference.GetComponent<Chessman>().GetYBoard());

        reference.GetComponent<Chessman>().SetXBoard(matrixX);
        reference.GetComponent<Chessman>().SetYBoard(matrixY);
        reference.GetComponent<Chessman>().SetCoords();

        controller.GetComponent<Game>().SetPosition(reference);

        // Update movement tracking for the piece that just moved
        reference.GetComponent<Chessman>().OnPieceMoved(oldY);

        // Check for pawn promotion
        if (reference.name.EndsWith("pawn"))
        {
            reference.GetComponent<Chessman>().PromotePawn();
        }

        // Get the Game component
        Game game = controller.GetComponent<Game>();
        
        // Reset all pieces' justMovedTwoSquares flag at the start of next turn
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                GameObject piece = game.GetPosition(x, y);
                if (piece != null)
                {
                    Chessman chessman = piece.GetComponent<Chessman>();
                    if (chessman != null)
                    {
                        chessman.OnTurnStart();
                    }
                }
            }
        }

        controller.GetComponent<Game>().NextTurn();

        reference.GetComponent<Chessman>().DestroyMovePlates();
    }

    public void SetCoords(int x, int y)
    {
        matrixX = x;
        matrixY = y;
    }

    public void SetReference(GameObject obj)
    {
        reference = obj;
    }

    public GameObject GetReference()
    {
        return reference;
    }
}