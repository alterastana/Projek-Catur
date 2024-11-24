using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlate : MonoBehaviour
{
    public GameObject controller;
    private GameObject reference = null;
    private int matrixX;
    private int matrixY;
    
    public bool attack = false;

    private void Start()
    {
        if (attack)
        {
            // Set warna merah untuk gerakan menyerang
            GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        }
    }

    private void OnMouseUp()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");

        // Handling captured piece
        if (attack)
        {
            GameObject capturedPiece = controller.GetComponent<Game>().GetPosition(matrixX, matrixY);
            if (capturedPiece != null)
            {
                Destroy(capturedPiece);
            }
        }

        // Update board positions
        controller.GetComponent<Game>().SetPositionEmpty(
            reference.GetComponent<Chessman>().GetXBoard(),
            reference.GetComponent<Chessman>().GetYBoard()
        );

        // Move the piece
        reference.GetComponent<Chessman>().SetXBoard(matrixX);
        reference.GetComponent<Chessman>().SetYBoard(matrixY);
        reference.GetComponent<Chessman>().SetCoords();

        // Update game state
        controller.GetComponent<Game>().SetPosition(reference);
        reference.GetComponent<Chessman>().DestroyMovePlates();
        
        // Next turn
        controller.GetComponent<Game>().NextTurn();
    }

    public void SetCoords(int x, int y)
    {
        matrixX = x;
        matrixY = y;

        // Get board dimensions
        Transform boardTransform = GameObject.FindGameObjectWithTag("GameController").transform;
        float boardWidth = boardTransform.localScale.x * 8f;
        float boardHeight = boardTransform.localScale.y * 8f;

        float squareSizeX = boardWidth / 8f;
        float squareSizeY = boardHeight / 8f;

        // Calculate offsets to center the plates
        float offsetX = -boardWidth / 2f + squareSizeX / 2f;
        float offsetY = -boardHeight / 2f + squareSizeY / 2f;

        // Calculate world position
        float xPos = offsetX + (x * squareSizeX);
        float yPos = offsetY + (y * squareSizeY);

        transform.position = new Vector3(xPos, yPos, -3.0f);
        
        // Scale the move plate to fit the board (adjusted scale)
        float plateSize = Mathf.Min(squareSizeX, squareSizeY) * 1.2f; // Slightly larger than the square
        transform.localScale = new Vector3(plateSize, plateSize, 1);
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
