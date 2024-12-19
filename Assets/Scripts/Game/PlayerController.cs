using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class PlayerController : MonoBehaviour
{
    public Material[] materials;
    public static Renderer objectRenderer;
    public static MeshRenderer objectMeshRenderer;
    public static GameObject player;
    public LineRenderer warningLine;
    public Material warningMaterial;
    public Rigidbody rb;
    public GameObject shot;
    public Transform shotSpawn;
    public float speed;
    public float xMin, xMax, zMin, zMax;
    public float tilt;
    public float fireRate;
    private float nextFire;
    internal float getHorizontalMove => Input.GetAxis("Horizontal");
    internal float getVerticalMove => Input.GetAxis("Vertical");
    private static bool _limitLine;
    public Camera gameCamera;
    private Vector3 startingPoint;
    private int leftTouch = 99;
    internal Vector3 mobileOffset;
    private int _movePlayerOption;
    private bool _joystickFlag;
    
    [SerializeField] private Vector2 _joystickSize = new Vector2(300,300);
    [SerializeField] private FloatingJoystickMy _joystick;
    private Finger _movementFinger;
    private Vector2 _movementAmount;

    internal FloatingJoystickMy getJoystick => _joystick;

    private KeyCode _fireKey;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        objectRenderer.material = materials[SessionManager.GetSkin()];
        
        objectMeshRenderer = GetComponent<MeshRenderer>();

        player = GameObject.Find("Player");

        _limitLine = SessionManager.GetLimiteLine();       

        _movePlayerOption = SessionManager.GetPlayerMovementControlOption();
        _joystickFlag = SessionManager.GetJoystick();
        _fireKey = SessionManager.GetFireKey();
    }

    void Update()
    {   
        //Player movement
        if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {   
            MovePlayer(VectorCreator.SetVector3(x: getHorizontalMove, z: getVerticalMove));

            if (Input.GetKey(_fireKey) && 
                !EventSystem.current.IsPointerOverGameObject() &&
                !GameController.gamePaused)
            {
                Shoot();
            }
        }
        else if(Application.platform == RuntimePlatform.Android)
        {   
            int i = 0;
            while(i < Input.touchCount)
            {
                UnityEngine.Touch t = Input.GetTouch(i);
                var touchPos = GetWorldTouchPosition(t.position) * -1;

                if(t.phase == TouchPhase.Began && 
                   (!GameController.gamePaused || SessionManager.GetFirstPlay()))
                {
                    if(_movePlayerOption == 0)
                    {   
                        if(t.position.x > Screen.width / 2 && !IsTouchOverUI(t.fingerId))
                        {
                            //Click on right side of screen
                            if(!GameController.gamePaused)
                                Shoot();
                        }
                        else
                        {
                            leftTouch = t.fingerId;
                            startingPoint = touchPos;
                        }
                    }
                    else
                    {
                        if (t.position.x <= Screen.width / 2 && !IsTouchOverUI(t.fingerId))
                        {
                            //Click on left side of screen
                            if(!GameController.gamePaused)
                                Shoot();
                        }
                        else
                        {
                            leftTouch = t.fingerId;
                            startingPoint = touchPos;
                        }
                    }
                }
                else if(t.phase == TouchPhase.Moved && leftTouch == t.fingerId)
                {
                    if(_joystickFlag)
                    {
                        mobileOffset.x = _movementAmount.x;
                        mobileOffset.y = 0;
                        mobileOffset.z= _movementAmount.y;
                        MovePlayer(VectorCreator.SetVector3(
                            x: _movementAmount.x,  
                            z: _movementAmount.y
                        ));
                    }
                    else
                    {
                        mobileOffset = startingPoint - touchPos;
                        Vector3 direction = Vector3.ClampMagnitude(new Vector3(mobileOffset.x, 0, mobileOffset.z), 1.0f);
                    
                        MovePlayer(direction);
                    }
                }
                else if(t.phase == TouchPhase.Ended && leftTouch == t.fingerId)
                {
                    mobileOffset = Vector3.zero;
                    MovePlayer(Vector3.zero);
                    
                    leftTouch = 99;
                }
                i++;
            }
        }

        //Player zone limit
        rb.position = VectorCreator.SetVector3(
            x: Math.Clamp(rb.position.x, xMin, xMax), 
            z: Math.Clamp(rb.position.z, zMin, zMax)
        );

        if(_limitLine)
        {
            float fadeFactor = Mathf.Clamp01(rb.position.z - 2.9f);

            if (rb.position.z >= 2.8)
            {
                warningLine.enabled = true;

                Color lineColor = warningMaterial.color;
                lineColor.a = fadeFactor;
                warningMaterial.color = lineColor;

                warningLine.SetPosition(0, new Vector3(-3000, 0, zMax + 1));
                warningLine.SetPosition(1, new Vector3(3000, 0, zMax + 1));
            }
            else
            {
                warningLine.enabled = false;
            }
        }

        //Player rotation
        rb.rotation = Quaternion.Euler(0, 0, rb.linearVelocity.x * tilt);
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        ETouch.Touch.onFingerDown += HandleFingerDown;
        ETouch.Touch.onFingerUp += HandleLoseFinger;  
        ETouch.Touch.onFingerMove += HandleFingerMove; 
    }

    private void OnDisable()
    {
        ETouch.Touch.onFingerDown -= HandleFingerDown;
        ETouch.Touch.onFingerUp -= HandleLoseFinger;  
        ETouch.Touch.onFingerMove -= HandleFingerMove;  
        EnhancedTouchSupport.Disable();
    }

    private void HandleFingerMove(Finger movedFinger)
    {
        if(_joystickFlag  && !GameController.gamePaused)
        {
            if(_movementFinger == movedFinger)
            {
                Vector2 knobPosition;
                float maxMovement = _joystickSize.x / 2f;
                ETouch.Touch currentTouch = movedFinger.currentTouch;

                // Convert screenPosition to canvas space
                Vector2 touchPosition = currentTouch.screenPosition;
                Vector2 joystickCenter = _joystick.RectTransform.position;

                // Calculate the distance beetween touch and joystick center
                Vector2 direction = touchPosition - joystickCenter;
                float distance = direction.magnitude;

                //Max range
                if (distance > maxMovement)
                {
                    direction.Normalize();
                    knobPosition = direction * maxMovement;
                }
                else
                {
                    knobPosition = direction;
                }

                _joystick.Knob.anchoredPosition = knobPosition;
                _movementAmount = knobPosition / maxMovement;
            }
        }
    }

    private void HandleLoseFinger(Finger lostFinger)
    {
        if(_joystickFlag  && !GameController.gamePaused)
        {
            if(lostFinger == _movementFinger)
            {
                ClearJoystick();
            }
        }
    }

    internal void ClearJoystick()
    {
        _movementFinger = null;
        _joystick.Knob.anchoredPosition = Vector2.zero;
        _joystick.gameObject.SetActive(false);
        _movementAmount = Vector2.zero;
    }

    private void HandleFingerDown(Finger touchedFinger)
    {   
        if (IsTouchOverUI(touchedFinger.index))
            return;

        if(_joystickFlag  && !GameController.gamePaused)
        {
            if(_movePlayerOption == 0)
            {
                if(_movementFinger == null && touchedFinger.screenPosition.x < Screen.width / 2f)
                {
                    _movementFinger = touchedFinger;
                    _movementAmount = Vector2.zero;
                    _joystick.gameObject.SetActive(true);
                    _joystick.RectTransform.sizeDelta = _joystickSize;
                    _joystick.RectTransform.transform.position = ClampStartPosition(touchedFinger.screenPosition);
                }
            }
            else
            {
                if(_movementFinger == null && touchedFinger.screenPosition.x > Screen.width / 2f)
                {
                    _movementFinger = touchedFinger;
                    _movementAmount = Vector2.zero;
                    _joystick.gameObject.SetActive(true);
                    _joystick.RectTransform.sizeDelta = _joystickSize;
                    _joystick.RectTransform.transform.position = ClampStartPosition(touchedFinger.screenPosition);
                }
            }
        }
    }

    private void MovePlayer(Vector3 direction)
    {
        rb.linearVelocity = VectorCreator.SetVector3(
            x: direction.x,  
            z: direction.z
        ) * speed * GameController.gameSpeed;
    }

    public void Shoot()
    {
        if(Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
            AudioManager.instance.PlaySoundEffect("Shot");
        }
    }

    private Vector2 ClampStartPosition(Vector2 StartPosition)
    {
        if (StartPosition.x < _joystickSize.x / 2)
        {
            StartPosition.x = _joystickSize.x / 2;
        }

        if (StartPosition.y < _joystickSize.y / 2)
        {
            StartPosition.y = _joystickSize.y / 2;
        }
        else if (StartPosition.y > Screen.height - _joystickSize.y / 2)
        {
            StartPosition.y = Screen.height - _joystickSize.y / 2;
        }

        return StartPosition;
    }

    private Vector3 GetWorldTouchPosition(Vector2 screenPosition)
    {
        // Convert touch x-y into x-z word
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            return hitInfo.point;
        }
        else
        {
            Vector3 defaultPlane = new Vector3(screenPosition.x, 0, screenPosition.y);
            return Camera.main.ScreenToWorldPoint(defaultPlane);
        }
    }

    private bool IsTouchOverUI(int fingerId) //Check UI Click on android
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            pointerId = fingerId,
            position = Input.GetTouch(fingerId).position
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0;
    }
}
