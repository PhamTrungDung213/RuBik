using UnityEngine;

public class RotateRubik : MonoBehaviour
{
    Vector2 firstPressPos;
    Vector2 secondPressPos;
    Vector2 currentSwipe;
    Quaternion rotation;
    Quaternion previousPosition;
    public float speedDrag;
    public GameObject rubik;
    void Start()
    {
        rotation = rubik.transform.rotation;
    }
    void Update()
    {
        Swipe();
        Drag();
        rubik.transform.rotation = Quaternion.Lerp(rubik.transform.rotation, rotation, Time.deltaTime * 5);
    }
    void Swipe()
    {
        if (Input.GetMouseButtonDown(1))
        {
            //Lay vi tri dau tien cua tro chuot khi nhan (2d)
            firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            
        }
        if (Input.GetMouseButtonUp(1))
        {
            //Lay vi tri cuoi cung cua tro chuot khi tha (2d)
            secondPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            //Tinh vector swipe tu 2 den 1
            currentSwipe = new Vector2(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);
            if (currentSwipe.magnitude < 10f) return; //tranh click
            //Chuan hoa vector
            currentSwipe.Normalize();
            if(LeftSwipe(currentSwipe))
            {
                rotation = Quaternion.Euler(0, 90, 0) * rotation;
            }
            else if(RightSwipe(currentSwipe))
            {
                rotation = Quaternion.Euler(0, -90, 0) * rotation;
            }
            else if(UpLeftSwipe(currentSwipe))
            {
                rotation = Quaternion.Euler(90, 0, 0) * rotation;
            }
            else if(UpRightSwipe(currentSwipe))
            {
                rotation = Quaternion.Euler(0, 0, -90) * rotation;
            }
            else if(DownLeftSwipe(currentSwipe))
            {
                rotation = Quaternion.Euler(0, 0, 90) * rotation;
            }
            else if(DownRightSwipe(currentSwipe))
            {
                rotation = Quaternion.Euler(-90, 0, 0) * rotation;
            }
        }
            
    }
    
    bool LeftSwipe(Vector2 swipe)
    {
        return swipe.x < 0 && swipe.y > -0.5f && swipe.y < 0.5f;
    }
    bool RightSwipe(Vector2 swipe)
    {
        return swipe.x > 0 && swipe.y > -0.5f && swipe.y < 0.5f;
    }
    bool UpLeftSwipe(Vector2 swipe)
    {
        return swipe.x < 0 && swipe.y > 0f;
    }
    bool UpRightSwipe(Vector2 swipe)
    {
        return swipe.x > 0 && swipe.y > 0f;
    }
    bool DownLeftSwipe(Vector2 swipe)
    {
        return swipe.x < 0 && swipe.y < 0f;
    }
    bool DownRightSwipe(Vector2 swipe)
    {
        return swipe.x > 0 && swipe.y < 0f;
    }
    
    void Drag()
    {
        if (Input.GetMouseButtonDown(2))
        {
            previousPosition = rotation;
        }
        if (Input.GetMouseButton(2)) 
        {
            float diChuyenNgang = Input.GetAxis("Mouse X") * speedDrag;
            float diChuyenDoc = Input.GetAxis("Mouse Y") * speedDrag;

            rotation = Quaternion.Euler(diChuyenDoc, -diChuyenNgang, 0) * rotation;
        }
        if (Input.GetMouseButtonUp(2))
        {
            rotation = previousPosition;
        }
    }
}