using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour {

    public bool isSelected;
    //used for testing matches
    private bool matchFound;
    public  bool skipMove;

    public bool isDraggingAbove;
    public bool isDraggingBelow;
    public bool isDraggingLeft;
    public bool isDraggingRight;
  
    public int column;
    public int row;
    public int targetX; //make local
    public int targetY; //make local

    public Vector2 startPos;
    public Vector2 currPos;
    public Vector2 prevPos;

    private Vector2 offset;

    private Vector2 currMousePos;
    public Vector2 MouseToPrint;
    //private Vector2 mousePosAdjusted;

    private Board board;
    //private GameObject otherDot;

    public float swipeAngle = 0;

    //used for  offseting the position of the dot and when it will move.
    public float indOff = 0.5f;
    public float followSpeed = 0.0014f;

    public Dot dAbove;
    public Dot dBelow;
    public Dot dLeft;
    public Dot dRight;


    public Dot dPrevAbove;
    public Dot dPrevBelow;
    public Dot dPrevLeft;
    public Dot dPrevRight;

    //Dot dPrev; // used specifically for dragged dots

    public IDotState currState;

    [HideInInspector] public Idle idleState;
    [HideInInspector] public Selected selState;
    [HideInInspector] public MoveUp upState;
    [HideInInspector] public MoveDown downState;
    [HideInInspector] public MoveLeft leftState;
    [HideInInspector] public MoveRight rightState;

    [HideInInspector] public DragUp dragUpState;
    [HideInInspector] public DragDown dragDownState;
    [HideInInspector] public DragLeft dragLeftState;
    [HideInInspector] public DragRight dragRightState;

    [HideInInspector] public CheckMatch checkState;
    [HideInInspector] public Fall fallState;
    [HideInInspector] public SnapBack snapState;


    private void Awake()
    {
        idleState = new Idle();
        selState = new Selected();
        upState = new MoveUp();
        downState = new MoveDown();
        leftState = new MoveLeft();
        rightState = new MoveRight();


        dragUpState = new DragUp();
        dragDownState = new DragDown();
        dragLeftState = new DragLeft();
        dragRightState = new DragRight();


        checkState = new CheckMatch();
        fallState = new Fall();
        snapState = new SnapBack();

        currState = idleState;
    }

    // Use this for initialization
    void Start()
    {
        board = FindObjectOfType<Board>();
        offset = board.transform.position;


        targetX = (int)transform.position.x - (int)offset.x;
        targetY = (int)transform.position.y - (int)offset.y;

        startPos = transform.position - new Vector3(offset.x, offset.y, 0.0f);
        currPos = startPos;
        row = targetY;
        column = targetX;
        isSelected = false;

        isDraggingAbove = false;
        isDraggingBelow = false;
        isDraggingLeft  = false;
        isDraggingRight = false;
        followSpeed = 0.25f;
        // dotBelow = null;
        dAbove = null;
        dBelow = null;
        dLeft = null;
        dRight = null;

        dPrevAbove = null;
        dPrevBelow = null;
        dPrevLeft = null;
        dPrevRight = null;
        MouseToPrint = new Vector2(0, 0);
    }

    void Update()
    {
        currState.Action(this);
        currState = currState.GetNextState(this);
        gameObject.name = "[ " + column + ", " + row + "]";
    }
    public void Kill()
    {
        Destroy(board.allDots[column, row]);
    }
    //-----------------PASSIVE STATES-------------------------------------------------------------------------

    public void IdleAction()
    {
        targetX = column;
        targetY = row;

        Vector2 TempPos = new Vector2(targetX + offset.x, transform.position.y);
        if (Mathf.Abs(targetX - (transform.position.x - offset.x)) > .1f)
        {
            //Move towards curr
            transform.position = Vector2.Lerp(transform.position, TempPos, .4f);
        }
        else
        {
            //Set Position         
            transform.position = TempPos;
            currPos = TempPos - offset;
            startPos = currPos;
            board.allDots[column, row] = this.gameObject;
        }


        TempPos = new Vector2(transform.position.x, targetY + offset.y);
        if (Mathf.Abs(targetY - (transform.position.y - offset.y)) > .1f)
        {
            //Move towards curr         
            transform.position = Vector2.Lerp(transform.position, TempPos, .4f);
        }
        else
        {
            //Set Position
            transform.position = TempPos;
            currPos = TempPos - offset;
            startPos = currPos;
            board.allDots[column, row] = this.gameObject;
        }

    }
    //--------------FALLING---------------------------------
    public void FallAction()
    {
        targetX = column;
        targetY = row - 1;
        board.allDots[column, row] = null;
        Vector2 TempPos = new Vector2(transform.position.x, targetY + offset.y);
        if (Mathf.Abs(targetY - (transform.position.y - offset.y)) > .1f)
        {
            //Move towards curr   
            transform.position = Vector2.Lerp(transform.position, TempPos, .4f);
        }
        else
        {
            //If you were at the top spawn a new one at the top
            if (row >= board.height - 1) board.CreateDotAtTop(column);
            //Set Position
            transform.position = TempPos;
            currPos = TempPos - offset;
            startPos = currPos;
            row = targetY;
            board.allDots[column, row] = this.gameObject;



        }
    }
    public bool CheckForFalling()
    {
        bool isFalling = false;
        if (row > 0 && board.allDots[column, row - 1] == null) isFalling = true;

        return isFalling;
    }
    //---------------SNAP BACK------------------------------
    public void SnapBackAction()
    {
        //If your actual position vector != your starting position vector, fix that shit
        Vector2 posAdjusted = (Vector2)transform.position - offset;
        if (Mathf.Abs((posAdjusted.x) - startPos.x) > .1f)
        {
            MoveNextGridSpaceX(startPos);
            transform.position = new Vector2(startPos.x, startPos.y) + offset;
        }

        else if (Mathf.Abs((posAdjusted.y) - startPos.y) > .1f)
        {
            MoveNextGridSpaceY(startPos);
            transform.position = new Vector2(startPos.x, startPos.y) + offset;

        }
    }
    public bool CheckIfAtStartPosition()
    {
        bool atStart = false;

        if (Mathf.Abs(startPos.x - (transform.position.x - offset.x)) < .2f &&
            Mathf.Abs(startPos.y - (transform.position.y - offset.y)) < .2f)
        {
            atStart = true;
            transform.position = startPos + offset;
        }
        return atStart;
    }

    //-----------------SELECTION-------------------------------------------------------------------------
    public void SelectedAction()
    {
        currMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 mouseAdjusted = currMousePos - offset;
        Vector2 DotPos = (Vector2)transform.position - offset;

        mouseAdjusted.x = Mathf.Clamp(mouseAdjusted.x, 0.0f, board.width - 1);
        mouseAdjusted.x = Mathf.Clamp(mouseAdjusted.x, DotPos.x - followSpeed, DotPos.x + followSpeed);

        mouseAdjusted.y = Mathf.Clamp(mouseAdjusted.y, 0.0f, board.height - 1);
        mouseAdjusted.y = Mathf.Clamp(mouseAdjusted.y, DotPos.y - followSpeed, DotPos.y + followSpeed);

        //mousePosAdjusted = currMousePos - offset;
        if (Mathf.Abs((currMousePos.x - offset.x) - startPos.x) > .1f)
        {
            MoveNextGridSpaceX(mouseAdjusted);
            transform.position = new Vector2(currMousePos.x, startPos.y + offset.y);
            //CheckMyDragAbove(column, row);
            //CheckMyDragBelow(column, row);

        }

        else if (Mathf.Abs((currMousePos.y - offset.y) - startPos.y) > .1f)
        {
            MoveNextGridSpaceY(mouseAdjusted);
            transform.position = new Vector2(startPos.x + offset.x, currMousePos.y);
        }
    }
    private void OnMouseDown()
    {
        isSelected = true;
    }
    private void OnMouseUp()
    {
        //Dont use this, it fucks with the ordering of bool switching
    }

    public bool GetSelected()
    {
        if (Input.GetMouseButtonUp(0))
        {
            isSelected = false;
        }
        return isSelected;
    }
    public void SetDeselected()
    {
        isSelected = false;
    }

    //--------------------DRAG STATES-----------------------------------------------------------
    public bool GetRightDrag()
    {
        bool result = true;
        if ((transform.position.x - offset.x) - 0.0f < startPos.x)
        {
            result = false;

        }
        return result;
    }
    public bool GetLeftDrag()
    {
        bool result = true;
        if ((transform.position.x - offset.x) + 0.0f > startPos.x)
        {
            result = false;
        }

        return result;
    }
    public bool GetUpDrag()
    {
        bool result = true;
        if ((transform.position.y - offset.y) - 0.0f < startPos.y)
        {
            result = false;
        }
        return result;
    }
    public bool GetDownDrag()
    {
        bool result = true;
        if ((transform.position.y - offset.y) + 0.0f > startPos.y)
        {
            result = false;
        }
        return result;
    }


    //---------------------Detach Methods-------------------------------------------------

    public void DetachFromPrev()
    {
        Debug.Log("Detached Prev");
        if (dPrevBelow != null)
        {
            dPrevBelow.SetAboveDot(null);
        }
        if (dPrevAbove != null)
        {
            dPrevAbove.SetBelowDot(null);
        }
        if (dPrevLeft != null)
        {
            dPrevLeft.SetRightDot(null);
        }
        if (dPrevRight != null)
        {
            dPrevRight.SetLeftDot(null);
        }
        dPrevBelow = null;
        dPrevAbove = null;
        dPrevLeft = null;
        dPrevRight = null;
    }
    public void DetachFromAbove()
    {
        //Debug.Log("Detached Above");
        if (dAbove != null)
        {
            dAbove.SetPrevBelow(null);
        }
        dAbove = null;
    }
    public void DetachFromBelow()
    {
        //Debug.Log("Detached Above");
        if (dBelow != null)
        {
            dBelow.SetPrevAbove(null);
        }
        dBelow = null;
    }
    public void DetachFromLeft()
    {
        //Debug.Log("Detached Above");
        if (dLeft != null)
        {
            dLeft.SetPrevRight(null);
        }
        dLeft = null;
    }
    public void DetachFromRight()
    {
        //Debug.Log("Detached Above");
        if (dRight != null)
        {
            dRight.SetPrevLeft(null);
        }
        dRight = null;
    }


    //---------------------GETTING DIRECTION OF MOVEMENT-------------------------------------------------
    public Vector2 GetDir()
    {
        currMousePos -= offset;
        Vector2 move = new Vector2(0, 0);
        if (Mathf.Abs((currMousePos.x) - startPos.x) > .3f)
        {
            move = new Vector2((currMousePos.x - startPos.x), 0);
        }
        else if (Mathf.Abs(currMousePos.y  - startPos.y) > .3f)
        {
            move = new Vector2(0, currMousePos.y - startPos.y);
        }
        currMousePos += offset;
        return move;
    }
    public Vector2 GetDirX()
    {
        Vector2 DotPos = (Vector2)transform.position - offset;

        Vector2 move = new Vector2(0, 0);
        if (Mathf.Abs(DotPos.x - startPos.x) > .1f)
        {
            move = new Vector2((DotPos.x - startPos.x), 0);
        }
        return move;
    }
    public Vector2 GetDirY()
    {

        Vector2 DotPos = (Vector2)transform.position - offset;

        Vector2 move = new Vector2(0, 0);
        if (Mathf.Abs(DotPos.y - startPos.y) > .1f)
        {
            move = new Vector2(0, DotPos.y - startPos.y);
        }
        return move;
    }

    //----------------------MOVING STATES-----------------------------------------------------------------
    public void LeftAction()
    {
        currMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mouseAdjusted = currMousePos - offset;
        Vector2 DotPos = (Vector2)transform.position - offset;

        mouseAdjusted.x = Mathf.Clamp(mouseAdjusted.x, 0.0f, board.width-1);
        mouseAdjusted.x = Mathf.Clamp(mouseAdjusted.x, DotPos.x - followSpeed, DotPos.x + followSpeed);
        MoveNextGridSpaceX(mouseAdjusted);
        if (mouseAdjusted.x < currPos.x)
        {
            if (dAbove == null) CheckMyDragAbove(column, row);
            if (dBelow == null) CheckMyDragBelow(column, row);
        }
    }
    public void RightAction()
    {
        currMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mouseAdjusted = currMousePos - offset;
        Vector2 DotPos = (Vector2)transform.position - offset;

        mouseAdjusted.x = Mathf.Clamp(mouseAdjusted.x, 0.0f, board.width-1);
        mouseAdjusted.x = Mathf.Clamp(mouseAdjusted.x, DotPos.x - followSpeed, DotPos.x + followSpeed);
        MoveNextGridSpaceX(mouseAdjusted);
        if (mouseAdjusted.x > currPos.x)
        {
            if (dAbove == null) CheckMyDragAbove(column, row);
            if (dBelow == null) CheckMyDragBelow(column, row);
        }
    }
    public void UpAction()
    {
        currMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mouseAdjusted = currMousePos - offset;
        Vector2 DotPos = (Vector2)transform.position - offset;

        mouseAdjusted.y = Mathf.Clamp(mouseAdjusted.y, 0.0f, board.height - 1);
        mouseAdjusted.y = Mathf.Clamp(mouseAdjusted.y, DotPos.y - followSpeed, DotPos.y + followSpeed);
        MoveNextGridSpaceY(mouseAdjusted);
        if (mouseAdjusted.y > currPos.y)
        {
            if (dLeft  == null) CheckMyDragLeft(column, row);
            if (dRight == null) CheckMyDragRight(column, row);
        }
    }
    public void DownAction()
    {
        currMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mouseAdjusted = currMousePos - offset;
        Vector2 DotPos = (Vector2)transform.position - offset;

        //mouseAdjusted.y = Mathf.Clamp(mouseAdjusted.y, 0.0f, board.height - 1);
        mouseAdjusted.y = Mathf.Clamp(mouseAdjusted.y, 0.0f, board.height - 1);
        mouseAdjusted.y = Mathf.Clamp(mouseAdjusted.y, DotPos.y - followSpeed, DotPos.y + followSpeed);
        MoveNextGridSpaceY(mouseAdjusted);
        if (mouseAdjusted.y < currPos.y)
        {
            if (dLeft  == null) CheckMyDragLeft(column, row);
            if (dRight == null) CheckMyDragRight(column, row);
        }
    }

    //Can switch this out for some vector math, using trig for this is just wasteful
    void CalculateAngle(Vector2 firstTouchPos, Vector2 finalTouchPos)
    {
        swipeAngle = Mathf.Atan2(finalTouchPos.y - firstTouchPos.y,
                                    finalTouchPos.x - firstTouchPos.x) * 180 / Mathf.PI;
        // Debug.Log(swipeAngle);
        MovePieces();
    }
    //Actually updates the columns
    void MovePieces()
    {
        GameObject otherDot;
        //RIGHT SWIPE
        if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1)
        {

            otherDot = board.allDots[column + 1, row];
            if (otherDot != gameObject)
            {
                board.allDots[column + 1, row] = gameObject;
                board.allDots[column, row] = otherDot;

                otherDot.GetComponent<Dot>().column -= 1;
                column += 1;
            }
            else skipMove = true;
            //if (otherDot != gameObject) otherDot.GetComponent<Dot>().column -= 1;
            //else skipMove = true;
            //if(column < board.width - 1) column += 1;
        }
        //UP SWIPE
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1)
        {
            otherDot = board.allDots[column, row + 1];
            if (otherDot != gameObject)
            {
                board.allDots[column, row + 1] = gameObject;
                board.allDots[column, row] = otherDot;

                otherDot.GetComponent<Dot>().row -= 1;
                row += 1;
            }
            else skipMove = true;

            //otherDot = board.allDots[column, row + 1];
            //otherDot.GetComponent<Dot>().row -= 1;
            //row += 1;
        }
        //LEFT SWIPE
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            otherDot = board.allDots[column - 1, row];
            if (otherDot != gameObject)
            {
                board.allDots[column - 1, row] = gameObject;
                board.allDots[column, row] = otherDot;

                otherDot.GetComponent<Dot>().column += 1;
                column -= 1;
            }
            else skipMove = true;
            //otherDot = board.allDots[column - 1, row];
            //if (otherDot != gameObject) otherDot.GetComponent<Dot>().column += 1;
            //else skipMove = true;
            //if (column > 0) column -= 1;
        }
        //DOWN SWIPE
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            otherDot = board.allDots[column, row - 1];
            if (otherDot != gameObject)
            {
                board.allDots[column, row - 1] = gameObject;
                board.allDots[column, row] = otherDot;

                otherDot.GetComponent<Dot>().row += 1;
                row -= 1;
            }
            else skipMove = true;

            //otherDot = board.allDots[column, row - 1];
            //otherDot.GetComponent<Dot>().row += 1;
            //row -= 1;
        }
        else skipMove = true;

    }
    void MoveNextGridSpaceX(Vector2 MousePos)
    {
        int MouseInd = (int)(MousePos.x + indOff);
        //Check to see if in new gridspace
        if (Mathf.Abs(MouseInd - (int)currPos.x) > .1f)
        {
            //Vector2 finalTouchPos = new Vector2(MousePos.x, 0.0f);
            Vector2 finalTouchPos = new Vector2(MouseInd, 0.0f);
            Vector2 firstTouchPos = new Vector2(currPos.x, 0.0f);

            transform.position = new Vector2(MousePos.x, startPos.y) + offset;

            while (MouseInd != column && !skipMove)
            {
                CalculateAngle(firstTouchPos, finalTouchPos);
            }
            if (skipMove) Debug.Log("Misfire");
            skipMove = false;
            //Update current position marker, don't forget your offset from the center
            currPos = new Vector2(MouseInd, currPos.y);
            board.allDots[(int)currPos.x, (int)currPos.y] = this.gameObject;
        }

        transform.position = new Vector2(MousePos.x, startPos.y) + offset;
        MouseToPrint = MousePos;

    }
    void MoveNextGridSpaceY(Vector2 MousePos)
    {
        int MouseInd = (int)(MousePos.y + indOff);

        //Check to see if in new gridspace
        if (Mathf.Abs(MouseInd - (int)currPos.y) > .1f)
        {
            Vector2 finalTouchPos = new Vector2(0.0f, MouseInd);
            Vector2 firstTouchPos = new Vector2(0.0f, currPos.y);

            while (MouseInd != row && !skipMove)
            {
           
                CalculateAngle(firstTouchPos, finalTouchPos);
            }
            if (skipMove) Debug.Log("Misfire");
            skipMove = false;

            currPos = new Vector2(currPos.x, MouseInd);
            board.allDots[(int)currPos.x, (int)currPos.y] = this.gameObject;
        }

        transform.position = new Vector2(startPos.x, MousePos.y) + offset;
        MouseToPrint = MousePos;
    }


    //--------------------------------MATCHING STATE--------------------------------------------------------------
    public void CheckAction()
    {
        matchFound = false;
        
        bool horz = MatchHorizontally(column, row);
        bool vert = MatchVertically(column, row);

        bool colH = ColumnMatchHorizontally(column);
        bool rwV  = RowMatchVertically(row);

        bool col = ColumnMatch(column);
        bool rw  = RowMatch(row);

        matchFound = (horz || vert || colH || rwV || col || rw);
    }
    public bool CheckForFoundMatch()
    {
        return matchFound;
    }
    //------------------------------MATCH SINGLE TARGET-------------------------------
    public bool MatchHorizontally(int col, int rw)
    {
        bool matchExists = false;

        int leftmost = col;
        int rightmost = col;

        GameObject currDot;
        currDot = board.allDots[col, rw];
        if (currDot != null)
        {
            GameObject testDot;
            //check left-----------------------------------------------------------------------
            if (col > 0)
            {            
                testDot = board.allDots[leftmost - 1, rw];
                while (testDot != null && leftmost > 0 && testDot.tag == currDot.tag)
                {
                    leftmost -= 1;
                    if (leftmost > 0) testDot = board.allDots[leftmost - 1, rw];  
                }
            }

            //check right-------------------------------------------------------------------------
            if (col < board.width - 1)
            {
                testDot = board.allDots[rightmost + 1, rw];
                while (testDot != null && rightmost < board.width - 1 && testDot.tag == currDot.tag)
                {
                    rightmost += 1;
                    if (rightmost < board.width - 1) testDot = board.allDots[rightmost + 1, rw];
                }
            }
            //If 3 or more are in a rw that count the match---------------------------------------------
            int hCount = rightmost - leftmost;
            if (hCount >= 2)
            {
                //Debug.Log("Match on horizontal");
                board.DestroyRow(leftmost, rightmost, rw);
                matchExists = true;
            }
        }
        return matchExists;
    }

    public bool MatchVertically(int col, int rw)
    {
        bool matchExists = false;

        int upmost      = rw;
        int downmost    = rw;

        GameObject currDot;
        currDot = board.allDots[col, rw];
        if (currDot != null)
        {
            GameObject testDot;
            //check down-----------------------------------------------------------------------
            if (rw > 0)
            {
                testDot = board.allDots[col, downmost - 1];
                while (testDot != null && downmost > 0 && testDot.tag == currDot.tag)
                {
                    downmost -= 1;
                    if (downmost > 0) testDot = board.allDots[col, downmost - 1];
                }
            }

            //check up-------------------------------------------------------------------------
            if (rw < board.height - 1)
            {
                testDot = board.allDots[col, upmost + 1];
                while (testDot != null && upmost < board.height - 1 && testDot.tag == currDot.tag)
                {
                    upmost += 1;
                    if (upmost < board.height - 1) testDot = board.allDots[col, upmost + 1];
                }
            }
            //If more than three in a col record match
            int vCount = upmost - downmost;
            if (vCount >= 2)
            {
                //Debug.Log("Match on vertical");
                board.DestroyColumn(downmost, upmost, col);
                matchExists = true;
            }
        }
        return matchExists;

    }
    //--------------MATCH GROUP
    bool ColumnMatchHorizontally(int col)
    {
        bool matchExists = false;

        for (int i = 0; i < board.height; i++)
        {
            if(MatchHorizontally(col, i)) matchExists = true;
        }

        return matchExists;
    }
    bool RowMatchVertically(int rw)
    {
        bool matchExists = false;

        for (int i = 0; i < board.width; i++)
        {
            if (MatchVertically(i, rw)) matchExists = true;
        }

        return matchExists;
    }

    //Matches Horizontally in a column
    bool RowMatch(int rw)
    {
        bool matchExists = false;

        for (int i = 0; i < board.height; i+=2)
        {
            if (MatchHorizontally(i, rw)) matchExists = true;
        }
        return matchExists;
    }
    //Matches vertically in a column
    bool ColumnMatch(int col)
    {
        bool matchExists = false;

        for (int i = 0; i < board.height; i+=2)
        {
            if (MatchVertically(col, i)) matchExists = true;
        }

        return matchExists;
    }

    //--------------------------------DRAG STATES----------------------------------------------
    //Check if the above and below are matching the selected, if so, drag those fuckers
    private void CheckMyDragAbove(int col, int rw)
    {
        GameObject currDot;// = board.allDots[col, rw];
        Dot cDot;// = currDot.GetComponent<Dot>();
        currDot = gameObject;
        cDot = this;
        Vector2 above = new Vector2(col,  rw + 1.0f);
        
        GameObject testDot;
        if (above.y < board.height)
        {
            testDot = board.allDots[(int)above.x, (int)above.y];
            Dot tDot = testDot.GetComponent<Dot>();

            if (testDot.tag == currDot.tag && !tDot.isSelected)
            {
                if ((cDot.currState == cDot.leftState ||
                    cDot.currState == cDot.dragLeftState))
                {
                    tDot.SetDragLeftState();
                }
                else if ((cDot.currState == cDot.rightState ||
                         cDot.currState == cDot.dragRightState) )
                    tDot.SetDragRightState();

                else if (cDot.currState == cDot.selState)
                    tDot.SetDragRightState();

                tDot.CheckMyDragAbove((int)above.x, (int)above.y);
                tDot.SetPrevBelow(cDot);
                cDot.SetAboveDot(tDot);              
            }            
        }
        
    }
    private void CheckMyDragBelow(int col, int rw)
    {
        GameObject currDot;// = board.allDots[col, rw];
        Dot cDot;// = currDot.GetComponent<Dot>();
        currDot = gameObject;
        cDot = this;
        Vector2 below = new Vector2(col, rw - 1.0f);

        GameObject testDot;
        if ((int)below.y >= 0)
        {
            testDot = board.allDots[(int)below.x, (int)below.y];
            Dot tDot = testDot.GetComponent<Dot>();
            if (testDot.tag == currDot.tag && !tDot.isSelected)
            {
                if (    cDot.currState == cDot.leftState ||
                        cDot.currState == cDot.dragLeftState)
                        tDot.SetDragLeftState();

                else if (   cDot.currState == cDot.rightState ||
                            cDot.currState == cDot.dragRightState)
                            tDot.SetDragRightState();

                else if (   cDot.currState == cDot.selState)
                            tDot.SetDragRightState();

                tDot.CheckMyDragBelow((int)below.x, (int)below.y);
                tDot.SetPrevAbove(cDot);
                cDot.SetBelowDot(tDot);
            }
        }
    }
    private void CheckMyDragLeft(int col, int rw)
    {
        GameObject currDot;// = board.allDots[col, rw];
        Dot cDot;// = currDot.GetComponent<Dot>();
        currDot = gameObject;
        cDot = this;
        Vector2 left = new Vector2(col - 1.0f, rw);

        GameObject testDot;
        if ((int)left.x >= 0)
        {
            testDot = board.allDots[(int)left.x, (int)left.y];
            Dot tDot = testDot.GetComponent<Dot>();
            if (testDot.tag == currDot.tag && !tDot.isSelected)
            {
                if (cDot.currState == cDot.upState ||
                    cDot.currState == cDot.dragUpState)
                    tDot.SetDragUpState();

                else if (cDot.currState == cDot.downState ||
                         cDot.currState == cDot.dragDownState)
                    tDot.SetDragDownState();

                else if (cDot.currState == cDot.selState)
                    tDot.SetDragUpState();

                tDot.CheckMyDragLeft((int)left.x, (int)left.y);
                tDot.SetPrevRight(cDot);
                cDot.SetLeftDot(tDot);
            }

        }
    }
    private void CheckMyDragRight(int col, int rw)
    {
        GameObject currDot;// = board.allDots[col, rw];
        Dot cDot;// = currDot.GetComponent<Dot>();
        currDot = gameObject;
        cDot = this;
        Vector2 right = new Vector2(col + 1.0f, rw);

        GameObject testDot;
        if (right.x < board.width)
        {
            testDot = board.allDots[(int)right.x, (int)right.y];
            Dot tDot = testDot.GetComponent<Dot>();
            if (testDot.tag == currDot.tag && !tDot.isSelected)
            {
                if (cDot.currState == cDot.downState ||
                    cDot.currState == cDot.dragDownState)
                    tDot.SetDragDownState();

                else if (cDot.currState == cDot.upState ||
                         cDot.currState == cDot.dragUpState)
                    tDot.SetDragUpState();

                else if (cDot.currState == cDot.selState)
                    tDot.SetDragUpState();

                //testDot.transform.position =
                //    currDot.transform.position + new Vector3(1, 0, 0);
                tDot.CheckMyDragRight((int)right.x, (int)right.y);
                tDot.SetPrevLeft(cDot);
                cDot.SetRightDot(tDot);
               // cDot.isDraggingRight = true;
            }
            //else if (testDot.tag != currDot.tag)
            //{
            //    cDot.isDraggingRight = false;
            //}

        }
    }


    public void SetDragLeftState()
    {
        currState = dragLeftState;
        isSelected = true;
    }
    public void SetDragRightState()
    {
        currState = dragRightState;
        isSelected = true;
    }
    public void SetDragUpState()
    {
        currState = dragUpState;
        isSelected = true;
    }
    public void SetDragDownState()
    {
        currState = dragDownState;
        isSelected = true;
    }
    //--------------------------------SET DRAG REFERENCES----------------------------------
    public void NullOutPrevDot()
    {
        //Debug.Log("Null out Prev dot");
        dPrevBelow = null;
        dPrevAbove = null;
        dPrevLeft  = null;
        dPrevRight = null;
    }
    public void NullOutMyDot()
    {
        //Debug.Log("Null out all dot");
        dBelow = null;
        dAbove = null;
        dLeft = null;
        dRight = null;
    }

    public void SetAboveDot(Dot pDot)
    {
        //Debug.Log("Setting Above dot");
        dAbove = pDot;
    }
    public void SetBelowDot(Dot pDot)
    {
        dBelow = pDot;
    }
    public void SetLeftDot(Dot pDot)
    {
        //Debug.Log("Setting Above dot");
        dLeft = pDot;
    }
    public void SetRightDot(Dot pDot)
    {
        dRight = pDot;
    }


    public void SetPrevAbove(Dot pDot)
    {
        dPrevAbove = pDot;
    }
    public void SetPrevBelow(Dot pDot)
    {
        dPrevBelow = pDot;
    }
    public void SetPrevLeft(Dot pDot)
    {
        dPrevLeft = pDot;
    }
    public void SetPrevRight(Dot pDot)
    {
        dPrevRight = pDot;
    }


}
