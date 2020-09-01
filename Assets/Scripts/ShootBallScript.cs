using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootBallScript : MonoBehaviour
{
    private const KeyCode rotateCCWButton = KeyCode.LeftArrow;
    private const KeyCode rotateCCButton = KeyCode.RightArrow;
    private const KeyCode shootButton = KeyCode.Space;
    
    private const float POWER_ACCELERATION = 50.0f;
    private const float MIN_POWER = 0.0f;
    private const float MAX_POWER = 100.0f;
    private const float ANGULAR_ACCELERATION = 5.0f;

    private float forceAngle = 0.0f;
    private float forceMagnitude = 0.0f;
    private float currentPosition = 0.0f;
    private float myCurrentPower = 0.0f;

    private static bool canShoot = true;
    private bool isPowerAscending = true;
    private Transform arrowPivotTransform;
    private Vector3 arrowPivotRotation;

    [SerializeField] Slider powerSlider;
    [SerializeField] GameObject arrowPivotObject;
    [SerializeField] GameObject triggerObject;

    public static Vector2 RadianToVector2(float radian)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    public static Vector2 DegreeToVector2(float degree)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad);
    }

    public static void ShouldShoot(bool bCanShoot)
    {
        canShoot = bCanShoot;
    }
    
    // Start is called before the first frame update
    void Start() {
        arrowPivotRotation = arrowPivotObject.transform.rotation.eulerAngles;
    }

    // Update is called once per frame
    void Update() {

        bool shotBall = false;

        if(Input.GetKey(shootButton)) {

            if(myCurrentPower == MAX_POWER)
            {
                isPowerAscending = false;
            } else if(myCurrentPower == MIN_POWER)
            {
                isPowerAscending = true;
            }

            if(isPowerAscending) {

            } else
            {

            }

        }

        if(Input.GetKey(rotateCCButton))
        {
            forceAngle -= ANGULAR_ACCELERATION;
        }

        if(Input.GetKey(rotateCCWButton))
        {
            forceAngle += ANGULAR_ACCELERATION;
        }

        arrowPivotObject.transform.Rotate(0.0f, 0.0f, forceAngle);

        if(shotBall) {
            canShoot = false;
        }
        
    }
}
