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
    public Sprite QueenB, KnightB, RookB, PawnB, KingB, BishopB;
    public Sprite QueenW, KnightW, RookW, PawnW, KingW, BishopW;

    // Mengaktifkan dan mengatur bidak pada papan
    public void Activate()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        switch (this.name)
        {
            case "QueenB": this.GetComponent<SpriteRenderer>().sprite = QueenB; player = "black"; break;
            case "KnightB": this.GetComponent<SpriteRenderer>().sprite = KnightB; player = "black"; break;
            case "RookB": this.GetComponent<SpriteRenderer>().sprite = RookB; player = "black"; break;
            case "PawnB": this.GetComponent<SpriteRenderer>().sprite = PawnB; player = "black"; break;
            case "KingB": this.GetComponent<SpriteRenderer>().sprite = KingB; player = "black"; break;
            case "BishopB": this.GetComponent<SpriteRenderer>().sprite = BishopB; player = "black"; break;
            case "QueenW": this.GetComponent<SpriteRenderer>().sprite = QueenW; player = "white"; break;
            case "KnightW": this.GetComponent<SpriteRenderer>().sprite = KnightW; player = "white"; break;
            case "RookW": this.GetComponent<SpriteRenderer>().sprite = RookW; player = "white"; break;
            case "PawnW": this.GetComponent<SpriteRenderer>().sprite = PawnW; player = "white"; break;
            case "KingW": this.GetComponent<SpriteRenderer>().sprite = KingW; player = "white"; break;
            case "BishopW": this.GetComponent<SpriteRenderer>().sprite = BishopW; player = "white"; break;
        }
        SetScaleToFitBoard();
        SetCoords();
    }

    // Mengatur koordinat pada papan
    public void SetCoords()
    {
        float squareSize = 0.585f;  // Ukuran persegi papan
        float offsetX = -2.34f;  // Offset X
        float offsetY = -2.34f;  // Offset Y

        // Menghitung posisi bidak berdasarkan ukuran kotak dan offset
        float x = (xBoard * squareSize) + offsetX + (squareSize / 2);
        float y = (yBoard * squareSize) + offsetY + (squareSize / 2);

        this.transform.position = new Vector3(x, y, -1.0f);
    }

    // Mendapatkan posisi X pada papan
    public int GetXBoard()
    {
        return xBoard;
    }

    // Mendapatkan posisi Y pada papan
    public int GetYBoard()
    {
        return yBoard;
    }

    // Menetapkan posisi X pada papan
    public void SetXBoard(int x)
    {
        xBoard = x;
    }

    // Menetapkan posisi Y pada papan
    public void SetYBoard(int y)
    {
        yBoard = y;
    }

    // Mengatur skala bidak agar sesuai dengan ukuran papan
    public void SetScaleToFitBoard()
    {
        float squareSize = 0.585f;

        // Menurunkan faktor skala untuk bidak agar tidak terlalu besar
        float desiredPieceScale = squareSize * 3.5f;  // Mengurangi dari 4.5f menjadi 3.5f

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && spriteRenderer.sprite != null)
        {
            float width = spriteRenderer.sprite.bounds.size.x;
            float height = spriteRenderer.sprite.bounds.size.y;
            float maxDimension = Mathf.Max(width, height);

            // Menurunkan faktor normalisasi skala untuk membuat bidak lebih kecil
            float normalizedScale = (desiredPieceScale / maxDimension) * 2.5f;  // Mengurangi dari 3.0f menjadi 2.5f
            transform.localScale = new Vector3(normalizedScale, normalizedScale, 1);
        }
    }
}
