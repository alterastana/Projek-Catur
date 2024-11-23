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
    
    public void Activate(){
        controller = GameObject.FindGameObjectWithTag("GameController");

        SetCoords();

        switch (this.name){
            case "QueenB": this.GetComponent<SpriteRenderer>().sprite = QueenB; break;
            case "KnightB": this.GetComponent<SpriteRenderer>().sprite = KnightB; break;
            case "RookB": this.GetComponent<SpriteRenderer>().sprite = RookB; break;
            case "PawnB": this.GetComponent<SpriteRenderer>().sprite = PawnB; break;
            case "KingB": this.GetComponent<SpriteRenderer>().sprite = KingB; break;
            case "BishopB": this.GetComponent<SpriteRenderer>().sprite = BishopB; break;

            case "QueenW": this.GetComponent<SpriteRenderer>().sprite = QueenW; break;
            case "KnightW": this.GetComponent<SpriteRenderer>().sprite = KnightW; break;
            case "RookW": this.GetComponent<SpriteRenderer>().sprite = RookW; break;
            case "PawnW": this.GetComponent<SpriteRenderer>().sprite = PawnW; break;
            case "KingW": this.GetComponent<SpriteRenderer>().sprite = KingW; break;
            case "BishopW": this.GetComponent<SpriteRenderer>().sprite = BishopW; break;
        }   
    }
    public void SetCoords() {
        float x = xBoard;
        float y = yBoard;

        x *= 0.66f;
        y *= 0.66f;

        x += -2.3f;
        y += -2.3f;

        this.transform.position = new Vector3(x,y,-1.0f);
    }

    public int GetXBoard(){

        return xBoard;

    }

    public int GetYBoard(){
        return yBoard;
    }

    public void SetXBoard(int x){
        xBoard = x;
    }

    public void SetYBoard(int y){
        yBoard = y;
    }
}