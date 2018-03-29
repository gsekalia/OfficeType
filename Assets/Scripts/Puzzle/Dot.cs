using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour {

    public bool isSelected;
    //used for testing matches
    private bool matchFound;

    //pointing to next dot.
    private Dot dAbove;


    public bool isDraggingAbove;
    private bool dragBelow;

    public int column;
    public int row;
    public int targetX; //make local
    public int targetY; //make local

    public Vector2 startPos;
    public Vector2 currPos;
    private Vector2 offset;

    private Vector2 currMousePos;
    private Vector2 mousePosAdjusted;

    private Board board;
    private GameObject otherDot;

    public float swipeAngle = 0;

    //used for  offseting the position of the dot and when it will move.
    public float indOff = 0.5f;


    //[HideInInspector]
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

        dAbove = null;
    }

    void Update()
    {
        currState.Action(this);
        currState = currState.GetNextState(this);
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

        mousePosAdjusted = currMousePos - offset;
        if (Mathf.Abs((currMousePos.x - offset.x) - startPos.x) > .1f)
        {
            MoveNextGridSpaceX(mousePosAdjusted);
            transform.position = new Vector2(currMousePos.x, startPos.y + offset.y);
            CheckMyDragAbove(column, row);
            
        }

        else if (Mathf.Abs((currMousePos.y - offset.y) - startPos.y) > .1f)
        {
            MoveNextGridSpaceY(mousePosAdjusted);
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

    public bool GetRightDrag()
    {
        if ((transform.position.x - offset.x) - 0.0f < startPos.x)
        {
            isSelected = false;
            isDraggingAbove = false;
        }
        return isSelected;
    }
    public bool GetLeftDrag()
    {

        if ((transform.position.x - offset.x) + 0.0f > startPos.x)
        {
            isSelected = false;
            isDraggingAbove = false;
        }
        return isSelected;
    }

    public bool GetSelected()
    {
        if (Input.GetMouseButtonUp(0))
        {
            isSelected = false;
            if (isDraggingAbove)
            {
                ClearSelected();
            }
            isDraggingAbove = false;
        }
        return isSelected;
    }
    public void ClearSelected()
    {

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
        // Set to local
        currMousePos -= offset;
        Vector2 move = new Vector2(0, 0);
        if (Mathf.Abs((currMousePos.x) - startPos.x) > .3f)
        {
            move = new Vector2((currMousePos.x - startPos.x), 0);
        }
        //Set Back to world
        currMousePos += offset;
        return move;
    }
    public Vector2 GetDirY()
    {
        // Set to local
        currMousePos -= offset;
        Vector2 move = new Vector2(0, 0);
        if (Mathf.Abs(currMousePos.y - startPos.y) > .5f)
        {
            move = new Vector2(0, currMousePos.y - startPos.y);
        }
        //Set Back to world
        currMousePos += offset;
        return move;
    }

    //----------------------MOVING STATES-----------------------------------------------------------------
    public void LeftAction()
    {
        currMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mouseAdjusted = currMousePos - offset;
        mouseAdjusted.x = Mathf.Clamp(mouseAdjusted.x, 0.0f, board.width-1);


        MoveNextGridSpaceX(mouseAdjusted);
        transform.position = new Vector2(mouseAdjusted.x + offset.x, startPos.y + offset.y);
        if (mouseAdjusted.x < currPos.x)
        {
            CheckMyDragAbove(column, row);
        }
        

    }
    public void RightAction()
    {
        currMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mouseAdjusted = currMousePos - offset;
        mouseAdjusted.x = Mathf.Clamp(mouseAdjusted.x, 0.0f, board.width-1);


        MoveNextGridSpaceX(mouseAdjusted);
        transform.position = new Vector2(mouseAdjusted.x + offset.x, startPos.y + offset.y);
        if (mouseAdjusted.x > currPos.x)
        {
            CheckMyDragAbove(column, row);

        }
    }
    public void UpAction()
    {
        currMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mouseAdjusted = currMousePos - offset;
        mouseAdjusted.y = Mathf.Clamp(mouseAdjusted.y, 0.0f, board.height - 1);


        MoveNextGridSpaceY(mouseAdjusted);
        transform.position = new Vector2(startPos.x + offset.x, mouseAdjusted.y + offset.y);
    }
    public void DownAction()
    {
        currMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mouseAdjusted = currMousePos - offset;
        mouseAdjusted.y = Mathf.Clamp(mouseAdjusted.y, 0.0f, board.height - 1);


        MoveNextGridSpaceY(mouseAdjusted);
        transform.position = new Vector2(startPos.x + offset.x, mouseAdjusted.y + offset.y);
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
        //RIGHT SWIPE
        if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1)
        {

            otherDot = board.allDots[column + 1, row];
            if (column >= board.width - 1)
            {
                Debug.Log("fuck");
            }
            otherDot.GetComponent<Dot>().column -= 1;
            column += 1;
        }
        //UP SWIPE
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1)
        {
            otherDot = board.allDots[column, row + 1];
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
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            otherDot = board.allDots[column, row - 1];
            otherDot.GetComponent<Dot>().row += 1;
            row -= 1;
        }

    }
    void MoveNextGridSpaceX(Vector2 MousePos)
    {
        int MouseInd = (int)(MousePos.x + indOff);
        //Check to see if in new gridspace
        if (Mathf.Abs(MouseInd - (int)currPos.x) > .1f)
        {
            Vector2 finalTouchPos = new Vector2(MousePos.x, 0.0f);
            Vector2 firstTouchPos = new Vector2(currPos.x, 0.0f);

            while (MouseInd != column)
            {

                CalculateAngle(firstTouchPos, finalTouchPos);
                //CheckMyDragAbove(column, row);
            }

            //Update current position marker, don't forget your offset from the center
            currPos = new Vector2(MouseInd, currPos.y);
            board.allDots[(int)currPos.x, (int)currPos.y] = this.gameObject;
        }

    }
    void MoveNextGridSpaceY(Vector2 MousePos)
    {
        int MouseInd = (int)(MousePos.y + indOff);

        //Check to see if in new gridspace
        if (Mathf.Abs(MouseInd - (int)currPos.y) > .1f)
        {
            Vector2 finalTouchPos = new Vector2(0.0f, MousePos.y);
            Vector2 firstTouchPos = new Vector2(0.0f, currPos.y);

            while (MouseInd != row)
            {
                CalculateAngle(firstTouchPos, finalTouchPos);
            }
            currPos = new Vector2(currPos.x, MouseInd);
            board.allDots[(int)currPos.x, (int)currPos.y] = this.gameObject;
        }
    }


    //----------------------MATCHING STATE--------------------------------------------------------------
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
    //----------------MATCH SINGLE TARGET-------------------------------
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

    //------------------DRAG STATES----------------------------------------------

    enum DRAG_DIR
    {   LEFT,
        RIGHT,
        UP,
        DOWN,
        NONE};
    //Check if the above and below are matching the selected, if so, drag those fuckers
    private void CheckMyDragAbove(int col, int rw)
    {     
        GameObject currDot = board.allDots[col, rw];
        Dot cDot = currDot.GetComponent<Dot>();
        Vector2 above = new Vector2(col,  rw + 1.0f);
        
        GameObject testDot;
        if (above.y < board.height)
        {
            testDot = board.allDots[(int)above.x, (int)above.y];
            if (testDot.tag == currDot.tag)
            {
                Dot tDot = testDot.GetComponent<Dot>();
                if (cDot.currState == cDot.leftState ||
                    cDot.currState == cDot.dragLeftState)
                    tDot.SetDragLeftState();
                
                else if (cDot.currState == cDot.rightState ||
                         cDot.currState == cDot.dragRightState)
                    tDot.SetDragRightState();

                else if (cDot.currState == cDot.selState)
                    tDot.SetDragRightState();

                tDot.CheckMyDragAbove((int)above.x, (int)above.y);
                //testDot.transform.position =
                //    new Vector2(currDot.transform.position.x,
                //    testDot.transform.position.y);

                cDot.isDraggingAbove = true;
            }
            else if(testDot.tag != currDot.tag)
            {
                cDot.isDraggingAbove = false;
            }
            
        }
        
    }
    private void CheckMyDragBelow(int col, int rw)
    {
        GameObject currDot = board.allDots[col, rw];
        Vector2 below = new Vector2(col, rw - 1.0f);

        GameObject testDot;
        if (below.y >= 0)
        {
            testDot = board.allDots[(int)below.x, (int)below.y];
            if (testDot.tag == currDot.tag)
            {
                testDot.GetComponent<Dot>().CheckMyDragBelow((int)below.x, (int)below.y);
            }
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



}
