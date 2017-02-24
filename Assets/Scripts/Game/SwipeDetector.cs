using System;
using System.Collections.Generic;
using UnityEngine;

public class SwipeDetector : MonoBehaviour
{
    private float fingerStartTime = 0.0f;
    private Vector2 fingerStartPos = Vector2.zero;
    private bool isSwipe = false;
    private float minSwipeDist = 50.0f;
    private float maxSwipeTime = 0.5f;

    private float left=0, right = 0, up=0, down=0;
    private Vector3 position;
    private BuildField field;

    private void Start()
    {
        field = GameObject.FindGameObjectWithTag("field").GetComponent<BuildField>();
        position = this.transform.position;
        Invoke("Move", 0.2f);
    }
    
    private void Update() {

        position = this.transform.position;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangePosition(0.51f, 0, 0, 0);
            //transform.rotation = Quaternion.AngleAxis(180, Vector3.forward);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangePosition(0, 0.51f, 0, 0);
            //transform.rotation = Quaternion.AngleAxis(0, Vector3.forward);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ChangePosition(0, 0, 0.51f, 0);
            //transform.rotation = Quaternion.AngleAxis(90, Vector3.forward);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ChangePosition(0, 0, 0, 0.51f);
            //transform.rotation = Quaternion.AngleAxis(270, Vector3.forward);
        }

        if (Input.touchCount > 0) {
            foreach (Touch touch in Input.touches) {
                switch (touch.phase){
                    case TouchPhase.Began:
                        /* this is a new touch */
                        isSwipe = true;
                        fingerStartTime = Time.time;
                        fingerStartPos = touch.position;
                        break;
                    case TouchPhase.Canceled:
                        /* The touch is being canceled */
                        isSwipe = false;
                        break;
                    case TouchPhase.Ended:

                        float gestureTime = Time.time - fingerStartTime;
                        float gestureDist = (touch.position - fingerStartPos).magnitude;

                        if (isSwipe && gestureTime < maxSwipeTime && gestureDist > minSwipeDist){
                            Vector2 direction = touch.position - fingerStartPos;
                            Vector2 swipeType = Vector2.zero;

                            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) {// the swipe is horizontal:
                                swipeType = Vector2.right * Mathf.Sign(direction.x);
                            } else { // the swipe is vertical:
                                swipeType = Vector2.up * Mathf.Sign(direction.y);
                            }

                            if (swipeType.x != 0.0f) {
                                if (swipeType.x > 0.0f) {//right
                                    ChangePosition(0, 0.51f, 0, 0);
                                    //transform.rotation = Quaternion.AngleAxis(0, Vector3.forward);
                                } else {//left
                                    ChangePosition(0.51f, 0, 0, 0);
                                    //transform.rotation = Quaternion.AngleAxis(180, Vector3.forward);
                                }
                            }

                            if (swipeType.y != 0.0f) {
                                if (swipeType.y > 0.0f) {//up
                                    ChangePosition(0, 0, 0.51f, 0);
                                    //transform.rotation = Quaternion.AngleAxis(90, Vector3.forward);
                                } else {//down
                                    ChangePosition(0, 0, 0, 0.51f);
                                    //transform.rotation = Quaternion.AngleAxis(270, Vector3.forward);
                                }
                            }

                        }

                        break;
                }
            }
        }
    }

    private void Move()
    {
        field.SetField((int)(Math.Round(position.x / 0.51f) - field.GetOrigX()), (int)(Math.Round(position.y / 0.51f) - field.GetOrigY()), FieldState.TEMP_BLOCK);
        if (position.x - left >= field.GetOrigX() * 0.51f && position.x + right < (field.GetSizeX()+ field.GetOrigX()) * 0.51f && position.y + up < (field.GetSizeY()+ field.GetOrigY()) * 0.51f && position.y - down >= field.GetOrigY() * 0.51f) {
            position.x -= left;
            position.x += right;
            position.y += up;
            position.y -= down;
            position.z = -1;
        }
        this.transform.position = position;
        field.UpdateTryingBridge((int)(Math.Round(position.x / 0.51f) - field.GetOrigX()), (int)(Math.Round(position.y / 0.51f) - field.GetOrigY()));
        field.SetField((int)(Math.Round(position.x / 0.51f) - field.GetOrigX()), (int)(Math.Round(position.y / 0.51f) - field.GetOrigY()), FieldState.HERO);
        field.Draw();
        Invoke("Move", 0.15f);
    }

    private void ChangePosition(float left, float right, float up, float down) {
        this.left = left;
        this.right = right;
        this.up = up;
        this.down = down;
    }
}