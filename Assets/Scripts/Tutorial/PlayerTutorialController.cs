using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerTutorialController : MonoBehaviour
{
    [SerializeField] private Rigidbody _player;
    [SerializeField] private GameObject _shot;
    [SerializeField] private Transform _shotSpawn;

    [SerializeField] private float _speed;
    [SerializeField] private float _xMin, _xMax, _zMin, _zMax;
    [SerializeField] private float _tilt;
    [SerializeField] private float _fireRate;

    private KeyCode _fireKey;
    private Vector3 _mobileOffset;
    private Vector3 _startingPoint;

    private float GetHorizontalMove => Input.GetAxis("Horizontal");
    private float GetVerticalMove => Input.GetAxis("Vertical");
    private float _nextFire;
    private int _leftTouch = 99; // No active touch

    void Start()
    {         
        _fireKey = SessionManager.GetFireKey();
    }

    void Update()
    {   
        if(!TutorialManager.StageMovePlayer && !TutorialManager.StageFire)
        {
            return;
        }

        //Player movement
        if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {   
            MovePlayer(VectorManager.NewVector3(x: GetHorizontalMove, z: GetVerticalMove));

            if(!TutorialManager.StageFire)
            {
                return;
            }

            if (Input.GetKey(_fireKey) && !EventSystem.current.IsPointerOverGameObject())
            {
                Shoot();
            }
        }
        else if(Application.platform == RuntimePlatform.Android)
        {   
            int i = 0;
            while(i < Input.touchCount)
            {   
                Touch t = Input.GetTouch(i);
                var touchPos = GetWorldTouchPosition(t.position) * -1;

                if(t.phase == TouchPhase.Began)
                {
                    if(t.position.x > Screen.width / 2 && !IsTouchOverUI(t.fingerId))
                    {
                        //Click on right side of screen
                        if(!TutorialManager.StageFire)
                        {
                            return;
                        }

                        Shoot();
                    }
                    else
                    {
                        _leftTouch = t.fingerId;
                        _startingPoint = touchPos;
                    }
                }
                else if(t.phase == TouchPhase.Moved && _leftTouch == t.fingerId)
                {
                    _mobileOffset = _startingPoint - touchPos;
                    Vector3 direction = Vector3.ClampMagnitude(new Vector3(_mobileOffset.x, 0, _mobileOffset.z), 1.0f);
                
                    MovePlayer(direction);
                }
                else if(t.phase == TouchPhase.Ended && _leftTouch == t.fingerId)
                {
                    _mobileOffset = Vector3.zero;
                    _leftTouch = 99;
                    MovePlayer(Vector3.zero);
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
    }

    private void MovePlayer(Vector3 direction)
    {
        _player.linearVelocity = VectorManager.NewVector3(
            x: direction.x,  
            z: direction.z
        ) * _speed;
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

    private bool IsTouchOverUI(int fingerId)
    {
        if(!TutorialManager.StageFire)
        {
            return false;
        }

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
}
