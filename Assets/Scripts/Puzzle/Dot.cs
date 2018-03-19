using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour {

    private bool isSelected;
    public int column;
    public int row;

    public int targetX;
    public int targetY;

    public Vector2 startPos;
    private Vector2 currMousePos;

    private Board board;
    private GameObject otherDot;
    private Vector2 firstTouchPos;
    private Vector2 finalTouchPos;
    public Vector2 TempPos;
    private Vector2 currPos;
    public Vector2 offset;
    public float swipeAngle = 0;

	// Use this for initialization
	void Start ()
    {
        board = FindObjectOfType<Board>();
        offset = board.transform.position;


        targetX = (int)transform.position.x - (int)offset.x;
        targetY = (int)transform.position.y - (int)offset.y;

        startPos = transform.position;
        currPos  = startPos;
        row    = targetY;
        column = targetX;
        isSelected = false;
        //currPos = new Vector2(currX, currY);
    }
	
	// Update is called once per frame
	//void Update ()
 //   {
 //       targetX = column;
 //       targetY = row;

 //       if (Mathf.Abs(targetX - (transform.position.x - offset.x)) > .1f)
 //       {
 //           //Move towards curr
 //           TempPos = new Vector2( targetX + offset.x, transform.position.y);
 //           transform.position = Vector2.Lerp(transform.position, TempPos, .4f);
 //       }
 //       else
 //       {
 //           //Set Position
 //           TempPos = new Vector2(targetX + offset.x, transform.position.y);
 //           transform.position = TempPos;
 //           board.allDots[column, row] = this.gameObject;
 //       }

 //       if (Mathf.Abs(targetY - (transform.position.y - offset.y)) > .1f)
 //       {
 //           //Move towards curr
 //           TempPos = new Vector2(transform.position.x, targetY + offset.y);
 //           transform.position = Vector2.Lerp(transform.position, TempPos, .4f);
 //       }
 //       else
 //       {
 //           //Set Position
 //           TempPos = new Vector2(transform.position.x, targetY + offset.y);
 //           transform.position = TempPos;
 //           board.allDots[column, row] = this.gameObject;
 //       }


 //   }

 //   private void OnMouseDown()
 //   {
 //       firstTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
 //       Debug.Log(firstTouchPos);
 //   }
 //   private void OnMouseUp()
 //   {
 //       finalTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
 //       CalculateAngle();
 //   }

    void CalculateAngle()
    {
        swipeAngle = Mathf.Atan2(finalTouchPos.y - firstTouchPos.y,
                                    finalTouchPos.x - firstTouchPos.x) * 180 / Mathf.PI;
       // Debug.Log(swipeAngle);
        MovePieces();
    }
    void MovePieces()
    {
        //RIGHT SWIPE
        if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width)
        {
            otherDot = board.allDots[column + 1, row];
            otherDot.GetComponent<Dot>().column -= 1;
            column += 1;              
        }
        //UP SWIPE
       else  if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height)
        {
            otherDot = board.allDots[column, row+1];
            otherDot.GetComponent<Dot>().row -= 1;
            row += 1;
        }
        //LEFT SWIPE
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            otherDot = board.allDots[column - 1, row];
            otherDot.GetComponent<Dot>().column += 1;
            column -= 1;
        }
        //DOWN SWIPE
       else  if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            otherDot = board.allDots[column, row -1 ];
            otherDot.GetComponent<Dot>().row += 1;
            row -= 1;
        }

    }

    void MoveNextGridSpaceX()
    {
        //Check to see if in new gridspace
        if (Mathf.Abs((int)currMousePos.x - (int)currPos.x) > .1f)
        {
            //int newPosX = currMousePos.x;
            finalTouchPos = new Vector2(currMousePos.x, 0.0f);
            firstTouchPos = new Vector2(currPos.x, 0.0f);
            CalculateAngle();
            currPos = new Vector2((int)currMousePos.x, currPos.y);
            board.allDots[(int)currPos.x, (int)currPos.y] = this.gameObject;
        }
    }
    void MoveNextGridSpaceY()
    {
        //Check to see if in new gridspace
        if (Mathf.Abs((int)currMousePos.y - (int)currPos.y) > .1f)
        {
            //int newPosX = currMousePos.x;
            finalTouchPos = new Vector2(0.0f, currMousePos.y);
            firstTouchPos = new Vector2(0.0f, currPos.y);
            CalculateAngle();
            currPos = new Vector2(currPos.x, (int)currMousePos.y);

            board.allDots[(int)currPos.x, (int)currPos.y] = this.gameObject;
        }
    }


    void Update()
    {


        if(isSelected)
        {
            currMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Mathf.Abs(currMousePos.x - startPos.x) > .1f)
            {
                MoveNextGridSpaceX();
                transform.position = new Vector2(currMousePos.x, startPos.y);
            }

            else if (Mathf.Abs(currMousePos.y - startPos.y) > .1f)
            {
                MoveNextGridSpaceY();
                transform.position = new Vector2(startPos.x, currMousePos.y);
            }
            else
            {
               // transform.position = currMousePos;
            }
        }
        else
        {
            targetX = column;
            targetY = row;

            if (Mathf.Abs(targetX - (transform.position.x - offset.x)) > .1f)
            {
                //Move towards curr
                TempPos = new Vector2(targetX + offset.x, transform.position.y);
                transform.position = Vector2.Lerp(transform.position, TempPos, .4f);
            }
            else
            {
                //Set Position
                TempPos = new Vector2(targetX + offset.x, transform.position.y);
                transform.position = TempPos;
                currPos = TempPos;
                startPos = TempPos;
                board.allDots[column, row] = this.gameObject;
            }

            if (Mathf.Abs(targetY - (transform.position.y - offset.y)) > .1f)
            {
                //Move towards curr
                TempPos = new Vector2(transform.position.x, targetY + offset.y);
                transform.position = Vector2.Lerp(transform.position, TempPos, .4f);
            }
            else
            {
                //Set Position
                TempPos = new Vector2(transform.position.x, targetY + offset.y);
                transform.position = TempPos;
                currPos = TempPos;
                startPos = TempPos;
                board.allDots[column, row] = this.gameObject;
            }



        }
        //targetX = column;
        //targetY = row;

        //if (Mathf.Abs(targetX - (transform.position.x - offset.x)) > .1f)
        //{
        //    //Move towards curr
        //    TempPos = new Vector2(targetX + offset.x, transform.position.y);
        //    transform.position = Vector2.Lerp(transform.position, TempPos, .4f);
        //}
        //else
        //{
        //    //Set Position
        //    TempPos = new Vector2(targetX + offset.x, transform.position.y);
        //    transform.position = TempPos;
        //    board.allDots[column, row] = this.gameObject;
        //}

        //if (Mathf.Abs(targetY - (transform.position.y - offset.y)) > .1f)
        //{
        //    //Move towards curr
        //    TempPos = new Vector2(transform.position.x, targetY + offset.y);
        //    transform.position = Vector2.Lerp(transform.position, TempPos, .4f);
        //}
        //else
        //{
        //    //Set Position
        //    TempPos = new Vector2(transform.position.x, targetY + offset.y);
        //    transform.position = TempPos;
        //    board.allDots[column, row] = this.gameObject;
        //}


    }
    private void OnMouseDown()
    {
        firstTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        isSelected = true;
        //Debug.Log(firstTouchPos);
    }
    private void OnMouseUp()
    {
        isSelected = false;
        startPos = transform.position;
        //finalTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //CalculateAngle();
    }





}
