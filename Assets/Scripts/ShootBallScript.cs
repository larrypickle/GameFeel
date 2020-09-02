using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootBallScript : MonoBehaviour
{
    private const KeyCode resetBallButton = KeyCode.R;
    private const KeyCode stopRotationButton = KeyCode.DownArrow;
    private const KeyCode rotateCCWButton = KeyCode.LeftArrow;
    private const KeyCode rotateCCButton = KeyCode.RightArrow;
    private const KeyCode shootButton = KeyCode.Space;

    private const float INITIAL_VOLUME = 0.01f;
    private const float POWER_ACCELERATION = 10.0f;
    private const float MIN_POWER = 0.0f;
    private const float MAX_POWER = 20.0f;
    private const float MAX_FORCE_ANGLE = 2.0f;
    private const float MIN_FORCE_ANGLE = -2.0f;
    private const float ANGULAR_ACCELERATION = 2.0f;
    private const float VOLUME_FACTOR = 0.1f;
    private const float PITCH_FACTOR = 0.1f;

    private float initialAudiencePitch = 0.0f;
    private float forceAngle = 0.0f;
    private float forceMagnitude = 0.0f;
    private float currentPosition = 0.0f;
    private float myCurrentPower = 0.0f;

    private static int myCurrentScore = 0;
    private int myHighScore = 0;

    private static bool canShoot = true;
    private bool isPowerAscending = true;
    private bool isLastKeyCCW = false;

    private Vector3 respawnLocation;

    private Transform arrowPivotTransform;
    private Vector3 arrowPivotRotation;
    private Rigidbody2D myRigidBody;
    private SpriteRenderer arrowRenderer;

    private static Text scoreText;
    private static Text highScoreText;

    [SerializeField] Slider powerSlider;
    [SerializeField] GameObject arrowPivotObject;
    [SerializeField] GameObject arrowObject;
    [SerializeField] GameObject scoreTriggerObject;
    [SerializeField] GameObject scoreTextObject;
    [SerializeField] GameObject highScoreTextObject;
    [SerializeField] GameObject ballPrefab;
    [SerializeField] GameObject pointPrefab;
    [SerializeField] GameObject hoopObject;
    [SerializeField] AudioSource audienceClappingSound;
    [SerializeField] AudioSource demonSound;
    [SerializeField] AudioSource owOneSound;
    [SerializeField] AudioSource owTwoSound;
    [SerializeField] AudioSource owThreeSound;
    [SerializeField] AudioSource owFourSound;

    private GameObject[] points;
    private const int MAX_POINTS = 20;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Hoop"))
        {
            return;
        }

        int random = UnityEngine.Random.Range(1, 5);

        if (random == 1)
        {
            owOneSound.Play();
        } else if(random == 2)
        {
            owTwoSound.Play();
        } else if(random == 3)
        {
            owThreeSound.Play();
        } else
        {
            owFourSound.Play();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if(collision.gameObject != scoreTriggerObject)
        {
            return;
        }

        ScreenFX.didScore = true;
        myCurrentScore += 1;
        audienceClappingSound.volume = INITIAL_VOLUME + (myCurrentScore - 1) * VOLUME_FACTOR;
        audienceClappingSound.pitch = initialAudiencePitch + (myCurrentScore * PITCH_FACTOR);
        audienceClappingSound.Play();

        demonSound.pitch = UnityEngine.Random.Range(-1.0f, 1.0f);

        if (myCurrentScore == 5)
        {
            demonSound.Play();
        }

        if(myHighScore <= myCurrentScore)
        {
            myHighScore = myCurrentScore;
        }

        SetTexts();
        Debug.Log("Entered trigger enter!");
    }

    public static void ResetTexts()
    {
        scoreText.text = "Score: 0";
        myCurrentScore = 0;
    }

    private void SetTexts()
    {
        scoreText.text = "Score: " + myCurrentScore;
        highScoreText.text = "High Score: " + myHighScore;
    }
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

    private Vector2 GetPointPosition(float time)
    {
        Vector2 normalizedDirection = DegreeToVector2(arrowPivotObject.transform.rotation.eulerAngles.z);
        Vector2 initialPosition = (Vector2)transform.position;

        Vector2 pointPosition = initialPosition + (normalizedDirection * myCurrentPower * time);
        pointPosition += (0.5f * Physics2D.gravity * time * time);

        return pointPosition;
    }
    
    // Start is called before the first frame update
    void Start() {
        arrowPivotRotation = arrowPivotObject.transform.rotation.eulerAngles;
        myRigidBody = gameObject.GetComponent<Rigidbody2D>();
        myRigidBody.isKinematic = true;
        respawnLocation = gameObject.transform.position;
        powerSlider.minValue = MIN_POWER;
        powerSlider.maxValue = MAX_POWER;
        arrowRenderer = arrowObject.GetComponent<SpriteRenderer>();

        initialAudiencePitch = audienceClappingSound.pitch;
        
        points = new GameObject[MAX_POINTS];

        for(int x = 0; x < MAX_POINTS; x++)
        {
            points[x] = Instantiate(pointPrefab, transform.position, Quaternion.identity);
        }

        scoreText = scoreTextObject.GetComponent<Text>();
        highScoreText = highScoreTextObject.GetComponent<Text>();
    }

    private void ResetHoopPosition()
    {
        float xLocation = UnityEngine.Random.Range(-4.5f, 4.5f);
        hoopObject.transform.position = new Vector3(xLocation, 0.0f, 0.0f);
    }

    // Update is called once per frame
    void Update() {

        if(Input.GetKeyDown(resetBallButton))
        {
            gameObject.transform.position = respawnLocation;
            myRigidBody.velocity = Vector2.zero;
            myRigidBody.isKinematic = true;
            arrowRenderer.enabled = true;
            powerSlider.enabled = true;
            ScreenFX.didScore = false;
            audienceClappingSound.Stop();
            ResetHoopPosition();
            return;
        }

        if(myCurrentPower > MAX_POWER)
        {
            myCurrentPower = MAX_POWER;
        } else if(myCurrentPower < MIN_POWER)
        {
            myCurrentPower = MIN_POWER;
        }

        powerSlider.value = myCurrentPower;

        if (Input.GetKey(shootButton)) {

            if (myCurrentPower == MAX_POWER) {
                isPowerAscending = false;
            } else if (myCurrentPower == MIN_POWER) {
                isPowerAscending = true;
            }

            if (isPowerAscending) {
                myCurrentPower += POWER_ACCELERATION * Time.deltaTime;
            } else
            {
                myCurrentPower -= POWER_ACCELERATION * Time.deltaTime;
            }

            for (int x = 0; x < MAX_POINTS; x++)
            {
                points[x].SetActive(true);
                points[x].transform.position = GetPointPosition(x * 0.1f);
            }
        }

        if(Input.GetKeyUp(shootButton))
        {
            myRigidBody.isKinematic = false;
            Vector2 velocity = DegreeToVector2(arrowPivotObject.transform.rotation.eulerAngles.z);
            velocity *= myCurrentPower;
            myRigidBody.velocity = velocity;
            myCurrentPower = 0.0f;
            powerSlider.value = MIN_POWER;
            powerSlider.enabled = false;
            arrowRenderer.enabled = false;

            for(int x = 0; x < MAX_POINTS; x++)
            {
                points[x].SetActive(false);
            }

            return;
        }

        if(Input.GetKey(rotateCCButton))
        {
            if(isLastKeyCCW)
            {
                forceAngle = 0.0f;
            }
            isLastKeyCCW = false;

            if (forceAngle > MIN_FORCE_ANGLE)
            {
                forceAngle -= ANGULAR_ACCELERATION * Time.deltaTime;
            }
        }

        if(Input.GetKey(rotateCCWButton))
        {
            if(!isLastKeyCCW)
            {
                forceAngle = 0.0f;
            }
            isLastKeyCCW = true;

            if (forceAngle < MAX_FORCE_ANGLE)
            {
                forceAngle += ANGULAR_ACCELERATION * Time.deltaTime;
            }
        }

        if(Input.GetKeyDown(stopRotationButton))
        {
            forceAngle = 0.0f;
        }

        arrowPivotObject.transform.Rotate(0.0f, 0.0f, forceAngle);

    }
}
