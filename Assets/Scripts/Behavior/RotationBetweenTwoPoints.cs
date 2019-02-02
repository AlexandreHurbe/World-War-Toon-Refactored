using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationBetweenTwoPoints : MonoBehaviour
{
    public float deltaAngle;
    public float speed;

    private float maxAngle;
    private float minAngle;
    private bool mustRotate;
    private float y;

    // Start is called before the first frame update
    void Start()
    {
        mustRotate = false;
        maxAngle = this.transform.localEulerAngles.y + deltaAngle;
        Debug.Log(maxAngle);
        minAngle = this.transform.localEulerAngles.y - deltaAngle;
        Debug.Log(minAngle);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (this.transform.localEulerAngles.y >= maxAngle && !mustRotate)
        {
            Debug.Log(mustRotate);
            mustRotate = true;
        }
        else if (this.transform.localEulerAngles.y <= minAngle && mustRotate)
        {
            mustRotate = false;
        }


        if (mustRotate)
        {
            y = Mathf.Lerp(this.transform.localEulerAngles.y, minAngle, Time.deltaTime * speed); 
        }
        else
        {
            y = Mathf.Lerp(this.transform.localEulerAngles.y, maxAngle, Time.deltaTime * speed);
        }

        this.transform.localEulerAngles = new Vector3(this.transform.localEulerAngles.x, y, this.transform.localEulerAngles.z);


    }
}
