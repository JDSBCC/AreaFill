using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{

    //public
    public float RotationPeriod = 0.3f;
    public GameObject CubeWall;

    //private
    private Vector3 Scale;

    private bool IsRotated = false;
    float DirectionX = 0;
    float DirectionZ = 0;

    float StartAngleRad = 0;
    Vector3 StartPos;
    float RotationTime = 0;
    float Radius = 1;
    Quaternion FromRotation;
    Quaternion ToRotation;

    bool isEmpty = false;

    private void Start()
    {
        Scale = transform.lossyScale;
    }

    private void Update()
    {
        Movement();

    }

    private void FixedUpdate()
    {
        if (IsRotated)
        {
            RotationTime += Time.fixedDeltaTime;
            float ratio = Mathf.Lerp(0, 1, RotationTime / RotationPeriod);

            float thetaRad = Mathf.Lerp(0, Mathf.PI / 2f, ratio);
            float distanceX = -DirectionX * Radius * (Mathf.Cos(StartAngleRad) - Mathf.Cos(StartAngleRad + thetaRad));
            float distanceY = Radius * (Mathf.Sin(StartAngleRad + thetaRad) - Mathf.Sin(StartAngleRad));
            float distanceZ = DirectionZ * Radius * (Mathf.Cos(StartAngleRad) - Mathf.Cos(StartAngleRad + thetaRad));
            transform.position = new Vector3(StartPos.x + distanceX, StartPos.y + distanceY, StartPos.z + distanceZ);

            transform.rotation = Quaternion.Lerp(FromRotation, ToRotation, ratio);
            if (ratio == 1)
            {
                isEmpty = true;
                IsRotated = false;
                DirectionX = 0;
                DirectionZ = 0;
                RotationTime = 0;
            }
        }
        if (isEmpty)
        {
            Instantiate(CubeWall, StartPos, Quaternion.identity);
            isEmpty = !isEmpty;
        }
    }

    private void LateUpdate()
    {
        
    }

    private void CreateTrail()
    {

    }

    private void Movement()
    {
        //test without?
        float x = 0;
        float z = 0;

        x = Input.GetAxisRaw("Horizontal");
        if (x == 0)
        {
            z = Input.GetAxisRaw("Vertical");
        }

        if ((x != 0 || z != 0) && !IsRotated)
        {
            DirectionX = -x;
            DirectionZ = z;
            StartPos = transform.position;
            FromRotation = transform.rotation;
            transform.Rotate(DirectionZ * 90, 0, DirectionX * 90, Space.World);
            ToRotation = transform.rotation;
            transform.rotation = FromRotation;
            SetRadius();
            RotationTime = 0;       
            IsRotated = true;
        }
    }

    private void SetRadius()
    {
        Vector3 dirVec = new Vector3(0, 0, 0);
        Vector3 nomVec = Vector3.up;

        if (DirectionX != 0)
        {
            dirVec = Vector3.right;
        }
        else if (DirectionZ != 0)
        {
            dirVec = Vector3.forward;
        }

        var vecRightDir = Mathf.Abs(Vector3.Dot(transform.right, dirVec));
        var vecUpDir = Mathf.Abs(Vector3.Dot(transform.up, dirVec));
        var vecForwardDir = Mathf.Abs(Vector3.Dot(transform.forward, dirVec));

        var vecRightNomVec = Mathf.Abs(Vector3.Dot(transform.right, nomVec));
        var vecUpNomVec= Mathf.Abs(Vector3.Dot(transform.up, nomVec));
        var vecForwardNomVec = Mathf.Abs(Vector3.Dot(transform.forward, nomVec));

        if (vecRightDir > 0.99)
        {
            if (vecUpNomVec > 0.99)
            {
                Radius = Mathf.Sqrt(Mathf.Pow(Scale.x / 2f, 2f) + Mathf.Pow(Scale.y / 2f, 2f));
                StartAngleRad = Mathf.Atan2(Scale.y, Scale.x);
            }
            else if (vecForwardNomVec > 0.99)
            {
                Radius = Mathf.Sqrt(Mathf.Pow(Scale.x / 2f, 2f) + Mathf.Pow(Scale.z / 2f, 2f));
                StartAngleRad = Mathf.Atan2(Scale.z, Scale.x);
            }
        }
        else if (vecUpDir > 0.99)
        {
            if (vecRightNomVec > 0.99)
            {
                Radius = Mathf.Sqrt(Mathf.Pow(Scale.y / 2f, 2f) + Mathf.Pow(Scale.x / 2f, 2f));
                StartAngleRad = Mathf.Atan2(Scale.x, Scale.y);
            }
            else if (vecForwardNomVec > 0.99)
            {
                Radius = Mathf.Sqrt(Mathf.Pow(Scale.y / 2f, 2f) + Mathf.Pow(Scale.z / 2f, 2f));
                StartAngleRad = Mathf.Atan2(Scale.z, Scale.y);
            }
        }
        else if (vecForwardDir > 0.99)
        {
            if (vecRightNomVec > 0.99)
            {
                Radius = Mathf.Sqrt(Mathf.Pow(Scale.z / 2f, 2f) + Mathf.Pow(Scale.x / 2f, 2f));
                StartAngleRad = Mathf.Atan2(Scale.x, Scale.z);
            }
            else if (vecUpNomVec > 0.99)
            {
                Radius = Mathf.Sqrt(Mathf.Pow(Scale.z / 2f, 2f) + Mathf.Pow(Scale.y / 2f, 2f));
                StartAngleRad = Mathf.Atan2(Scale.y, Scale.z);
            }
        }
    }
}
