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

    private float left=0, right = 0, up=0.51f, down=0;
    private Vector3 position;
    public GameObject block;
    private BuildField field;

    private void Start()
    {
        field = GameObject.FindGameObjectWithTag("field").GetComponent<BuildField>();
        position = this.transform.position;
        Invoke("move", 0.2f);
    }
    
    private void Update() {

        position = this.transform.position;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            changePosition(0.51f, 0, 0, 0);
            //transform.rotation = Quaternion.AngleAxis(180, Vector3.forward);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            changePosition(0, 0.51f, 0, 0);
            //transform.rotation = Quaternion.AngleAxis(0, Vector3.forward);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            changePosition(0, 0, 0.51f, 0);
            //transform.rotation = Quaternion.AngleAxis(90, Vector3.forward);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            changePosition(0, 0, 0, 0.51f);
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
                                    changePosition(0, 0.51f, 0, 0);
                                    //transform.rotation = Quaternion.AngleAxis(0, Vector3.forward);
                                } else {//lefy
                                    changePosition(0.51f, 0, 0, 0);
                                    //transform.rotation = Quaternion.AngleAxis(180, Vector3.forward);
                                }
                            }

                            if (swipeType.y != 0.0f) {
                                if (swipeType.y > 0.0f) {//up
                                    changePosition(0, 0, 0.51f, 0);
                                    //transform.rotation = Quaternion.AngleAxis(90, Vector3.forward);
                                } else {//down
                                    changePosition(0, 0, 0, 0.51f);
                                    //transform.rotation = Quaternion.AngleAxis(270, Vector3.forward);
                                }
                            }

                        }

                        break;
                }
            }
        }
    }

    private void move()
    {
        field.setField((int)(Math.Ceiling(position.x / 0.51f) + 12 - 1), (int)(Math.Ceiling(position.y / 0.51f) + 8 - 1), 2);
        if (position.x - left > -6 && position.x + right < 6 && position.y + up < 3.6f && position.y - down > -3.6f) {
            position.x -= left;
            position.x += right;
            position.y += up;
            position.y -= down;
            position.z = -1;
        }
        this.transform.position = position;
        field.updateTryingBridge((int)(Math.Ceiling(position.x / 0.51f) + 12 - 1), (int)(Math.Ceiling(position.y / 0.51f) + 8 - 1));
        field.setField((int)(Math.Ceiling(position.x / 0.51f) + 12 - 1), (int)(Math.Ceiling(position.y / 0.51f) + 8 - 1), 3);
        Invoke("move", 0.2f);
    }

    private void changePosition(float left, float right, float up, float down) {
        this.left = left;
        this.right = right;
        this.up = up;
        this.down = down;
    }
}