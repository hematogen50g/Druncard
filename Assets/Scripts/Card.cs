using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Card
{
    public override string ToString()
    {
        return RankDescriptor[Rank] + " of " + Suit.ToString();
    }
    public static string[] RankDescriptor = { "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Jack", "Queen", "King", "Ace" };
    public int Rank { get; set; }
    public int Index { get; set; }
    public Suit Suit { get; private set; }    
    public Vector3 DeckPosition { get; private set; }
    public bool FaceUp { get; private set; } = false;
    
    public GameObject GO { get; private set; }   
    private Sprite faceSprite;
    private Sprite bSprite;   
    public void FlipFaceDownInstantly()
    {        
        Img.sprite = bSprite;
        FaceUp = false;
    }
    public void FlipFaceUpInstantly()
    {
        Img.sprite = faceSprite;
        FaceUp = true;
    }
    public void FlipInstantly()
    {
        if (FaceUp)
            FlipFaceDownInstantly();
        else
            FlipFaceUpInstantly();
    }

    public Image Img { get; private set; }
    public Card(int rank, Suit s, GameObject go, Sprite spr, Sprite backSprite)
    {        
        Rank = rank;
        Suit = s;
        GO = go;
        Img = go.GetComponent<Image>();

        bSprite = backSprite;
        faceSprite = spr;
        Img.sprite = bSprite;
               
        GO.name = ToString();
       
    }
    public void ResetTablePosition(float xShift)
    {
        GO.transform.localPosition = new Vector3(xShift,0,0);        
    }
    public void SetFace(Sprite s)
    {
        faceSprite = s;
    }
    public void SetBackground(Sprite s)
    {
        bSprite = s;
    }
    
    

    public void ResetStats()
    {
        t = f = 0;
        halfFlipped = false;
        FlipFaceDownInstantly();
        GO.transform.localScale = Vector3.one;
    }

    #region Moving And  Flipping

    private float flipSpeed = 1;
    
    /// <summary>
    /// move amount
    /// </summary>
    private float t = 0;

    /// <summary>
    /// flip amount
    /// </summary>
    private float f  = 0;

    private bool halfFlipped  = false;

    public delegate void FlippedCardCallback();

    public delegate void MovedCardCallback(Card c);

    private FlippedCardCallback flippedCardCallback;
    private MovedCardCallback movedCardCallback;

    //Updates flip continuously and calls back when finished
    public void UpdateFlip()
    {       
            //reverse card scaling.
        if (!halfFlipped)
            GO.transform.localScale = new Vector3(1 - f, 1, 1);
        else
            GO.transform.localScale = new Vector3(f, 1, 1);

        f += Time.deltaTime * flipSpeed;
        
        if(f >= 1)
        {
            f = 0;
            if (!halfFlipped)
            {
                halfFlipped = true;
                //flip instantly just changes card current sprite to opposite side sprite
                FlipInstantly();
            }
            else
            {               
                // here card is already flipped. invoke callback method.
                GO.transform.localScale = Vector3.one;
                halfFlipped = false;
                flippedCardCallback.Invoke();               
            }
        }       
    }
    

    //this method is called externally to initiate flipping process
    public void Flip(float speed, FlippedCardCallback callback)
    {
        flipSpeed = speed;
        flippedCardCallback = callback;
    }
    private Vector3 posA, posB;
    private float moveSpeed;
    /// <summary>
    /// Initiate moving card from A position to B position with speed.
    /// </summary>
    /// <param name="positionA"></param>
    /// <param name="positionB"></param>
    /// <param name="speed"></param>
    /// <param name="callback"> Is called when moving is done</param>
    public void Move(Vector3 positionA, Vector3 positionB, float speed)
    {        
        posA = positionA;
        posB = positionB;
        moveSpeed = speed;        
        doneMoving = false;        
    }
    /// <summary>
    /// Move card from it`s current position to new position with speed
    /// </summary>
    /// <param name="position"></param>
    /// <param name="speed"></param>
    /// <param name="callback"></param>
    public void Move(Vector3 position, float speed, MovedCardCallback callback)
    {
        Move(GO.transform.position, position, speed);       
        movedCardCallback = callback;       
    }
    
    private bool doneMoving;
    float acc;
    float accFactor = 4;
    public void UpdateMove()
    {
        if (!doneMoving)
        {
            //print(Mathf.Sin(0.5f).ToString() +" "+ Mathf.Sin(0.0f).ToString()+" "+ Mathf.Sin(Mathf.PI/2).ToString());
            //acc = Mathf.Sin(t* Mathf.PI / 2);
            acc = Mathf.Abs(0.5f-t)*accFactor;
            float del = 1;
            if (t > 0.8f)
            {
                acc /= 4;
                del = 5;
            }
            

            t += Time.deltaTime * (moveSpeed/del + acc);
            GO.transform.position = Vector3.Lerp(posA, posB, t);

            if (t >= 1)
            {
                doneMoving = true;
                t = 0;
                movedCardCallback.Invoke(this);
            }            
        }        
    }
    #endregion
}

