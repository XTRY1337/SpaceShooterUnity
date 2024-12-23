using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;

using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class PlayerController : MonoBehaviour
{
    [Header("-- Game --")]
    [SerializeField] private GameObject _shot;
    [SerializeField] private Transform _shotSpawn;
    [SerializeField] private float _speed;
    [SerializeField] private float _xMin, _xMax, _zMin, _zMax;
    [SerializeField] private float _tilt;
    [SerializeField] private float _fireRate;
    private bool _isWindows;
    private int _screenWidth;

    [Header("-- Limit line --")]
    public LineRenderer warningLine;
    public Material warningMaterial;
    private bool _limitLine;

    [Header("-- Screen touch --")]
    private Vector3 _startingPoint;
    private int _leftTouch = 99; // No active touch
    private static Vector3 _mobileMovement;
    public static Vector3 MobileMovement => _mobileMovement;
    
    [Header("-- Player --")]
    [SerializeField] private Rigidbody _player;
    [SerializeField] private Material[] _playerMaterials;
    [SerializeField] private Renderer _playerRenderer;
    private KeyCode _fireKey;
    private float _nextFire;
    private int _movePlayerOption;
    public static float GetHorizontalMove => Input.GetAxis("Horizontal");
    public static float GetVerticalMove => Input.GetAxis("Vertical");

    [Header("-- Joystick --")]
    [SerializeField] private Vector2 _joystickSize = new Vector2(300, 300);
    [SerializeField] private FloatingJoystick _joystick;
    private Finger _movementFinger;
    private Vector2 _movementAmount;
    private bool _joystickFlag;

    void Start()
    {
        _playerRenderer.material = _playerMaterials[SessionManager.GetSkin()];
        _limitLine = SessionManager.GetLimiteLine();       
        _movePlayerOption = SessionManager.GetPlayerMovementControlOption();
        _joystickFlag = SessionManager.GetJoystick();
        _fireKey = SessionManager.GetFireKey();
        _isWindows = Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor;
        _screenWidth = Screen.width;
    }

    void Update()
    {   
        //Player movement
        if(_isWindows)
        {   
            Vector3 playerMovement = VectorManager.NewVector3(x: GetHorizontalMove, z: GetVerticalMove);
            MovePlayer(playerMovement);

            if (Input.GetKey(_fireKey) && 
                !EventSystem.current.IsPointerOverGameObject() &&
                !GameManager.IsGamePaused)
            {
                Shoot();
            }
        }
        else
        {   
            int i = 0;
            int touchCount = Input.touchCount;
            while(i < touchCount)
            {
                UnityEngine.Touch touch = Input.GetTouch(i);
                var touchPos = GetWorldTouchPosition(touch.position) * -1;

                if(touch.phase == TouchPhase.Began && !GameManager.IsGamePaused)
                {
                    switch(_movePlayerOption)
                    {
                        case 0:
                            if(touch.position.x > _screenWidth / 2 && !IsTouchOverUI(touch.fingerId))
                            {
                                Shoot();
                            }
                            else
                            {
                                _leftTouch = touch.fingerId;
                                _startingPoint = touchPos;
                            }
                            break;
                        case 1:
                            if (touch.position.x <= _screenWidth / 2 && !IsTouchOverUI(touch.fingerId))
                            {
                                Shoot();
                            }
                            else
                            {
                                _leftTouch = touch.fingerId;
                                _startingPoint = touchPos;
                            }
                            break;
                        default:
                            break;
                    }
                }
                else if(touch.phase == TouchPhase.Moved && _leftTouch == touch.fingerId)
                {
                    if(_joystickFlag)
                    {
                        _mobileMovement = VectorManager.NewVector3(x: _movementAmount.x, z: _movementAmount.y);
                        MovePlayer(_mobileMovement);
                    }
                    else
                    {
                        _mobileMovement = _startingPoint - touchPos;
                        Vector3 direction = Vector3.ClampMagnitude(new Vector3(_mobileMovement.x, 0, _mobileMovement.z), 1.0f);
                        MovePlayer(direction);
                    }
                }
                else if(touch.phase == TouchPhase.Ended && _leftTouch == touch.fingerId)
                {
                    _mobileMovement = Vector3.zero;
                    MovePlayer(Vector3.zero);
                    
                    _leftTouch = 99;
                }

                i++;
            }
        }

        //Player zone limit
        _player.position = VectorManager.NewVector3(
            x: Math.Clamp(_player.position.x, _xMin, _xMax), 
            z: Math.Clamp(_player.position.z, _zMin, _zMax)
        );

        //Player rotation
        _player.rotation = Quaternion.Euler(0, 0, _player.linearVelocity.x * _tilt);

        //Limite line
        if(_limitLine)
        {
            LimiteLineHandle();
        }
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
        if(_joystickFlag  && !GameManager.IsGamePaused)
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
        if(_joystickFlag  && !GameManager.IsGamePaused)
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

        if(_joystickFlag && _movementFinger is not null && !GameManager.IsGamePaused)
        {
            switch(_movePlayerOption)
            {
                case 0:
                    if(touchedFinger.screenPosition.x < Screen.width / 2f)
                    {
                        ShowJoystick(touchedFinger);
                    }
                    break;
                case 1:
                    if(touchedFinger.screenPosition.x > Screen.width / 2f)
                    {
                        ShowJoystick(touchedFinger);
                    }
                    break;
                default:
                    break;
            }
        }
    }

    private void MovePlayer(Vector3 direction)
    {
        _player.linearVelocity = VectorManager.NewVector3(
            x: direction.x,  
            z: direction.z
        ) * _speed * GameManager.GameSpeed;
    }

    public void Shoot()
    {
        if(Time.time > _nextFire)
        {
            _nextFire = Time.time + _fireRate;
            Instantiate(_shot, _shotSpawn.position, _shotSpawn.rotation);
            AudioManager.Instance.PlaySoundEffect("Shot");
        }
    }

    private Vector2 ClampStartPosition(Vector2 startPosition)
    {
        if (startPosition.x < _joystickSize.x / 2)
        {
            startPosition.x = _joystickSize.x / 2;
        }

        if (startPosition.y < _joystickSize.y / 2)
        {
            startPosition.y = _joystickSize.y / 2;
        }
        else if (startPosition.y > Screen.height - _joystickSize.y / 2)
        {
            startPosition.y = Screen.height - _joystickSize.y / 2;
        }

        return startPosition;
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

    private void ShowJoystick(Finger touchedFinger)
    {
        _movementFinger = touchedFinger;
        _movementAmount = Vector2.zero;
        _joystick.gameObject.SetActive(true);
        _joystick.RectTransform.sizeDelta = _joystickSize;
        _joystick.RectTransform.transform.position = ClampStartPosition(touchedFinger.screenPosition);
    }

    private bool IsTouchOverUI(int fingerId)
    {
        //Check UI Click on android
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            pointerId = fingerId,
            position = Input.GetTouch(fingerId).position
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0;
    }

    private void LimiteLineHandle()
    {
        float fadeFactor = Mathf.Clamp01(_player.position.z - 2.9f);

        if (_player.position.z >= 2.8)
        {
            warningLine.enabled = true;

            Color lineColor = warningMaterial.color;
            lineColor.a = fadeFactor;
            warningMaterial.color = lineColor;

            warningLine.SetPosition(0, new Vector3(-3000, 0, _zMax + 1));
            warningLine.SetPosition(1, new Vector3(3000, 0, _zMax + 1));
        }
        else
        {
            warningLine.enabled = false;
        }
    }
}
