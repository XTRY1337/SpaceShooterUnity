using UnityEngine;
using UnityEngine.EventSystems;

public class BackgroundMovement : MonoBehaviour
{
    private Material _backgroundMaterial;

    private float _parallaxSpeed = 0.03f;
    private float _yOffset;
    private float _xOffset;
    private float _gameSpeed;
    private float _platformMultiplier;
    private float _horizontalAdjustment;
    private float _verticalAdjustment;
    private bool _backgroundRealistic;

    public static int MovementOption;

    void Start()
    {
        _backgroundMaterial = GetComponent<Renderer>().material;
        _backgroundRealistic = SessionManager.GetRealistic();
        _gameSpeed = SessionManager.GetGameSpeed();
        _platformMultiplier = GameIntroduction.IsWindows ? 3f : 1f;
        _horizontalAdjustment = 0.0001f * _gameSpeed;
        _verticalAdjustment = 0.00015f * _gameSpeed;
    }

    void Update()
    {
        switch (MovementOption)
        {   
            case 1: // = 1 move background during game to bottom
                if(!GameManager.IsGameOver)
                {   
                    _yOffset += Time.deltaTime * _gameSpeed / 10f;

                    if(_backgroundRealistic) //extra moves
                    {   
                        float playerHorizontalMovement = 0;
                        float playerVerticalMovement = 0;

                        if(GameIntroduction.IsWindows)
                        {
                            playerHorizontalMovement = PlayerController.GetHorizontalMove;
                            playerVerticalMovement = PlayerController.GetVerticalMove;
                        }
                        else
                        {
                            playerHorizontalMovement = PlayerController.MobileMovement.x;
                            playerVerticalMovement = PlayerController.MobileMovement.z;
                        }

                        if (playerHorizontalMovement > 0)
                        {
                            _xOffset += _horizontalAdjustment * _platformMultiplier;
                        }
                        else if (playerHorizontalMovement < 0)
                        {
                            _xOffset -= _horizontalAdjustment * _platformMultiplier;
                        }

                        if (playerVerticalMovement > 0)
                        {
                            _yOffset += _verticalAdjustment * _platformMultiplier;
                        }
                        else if (playerVerticalMovement < 0)
                        {
                            _yOffset -= _horizontalAdjustment * _platformMultiplier;
                        }
                    }
                }
                
                break;
            case 2: // = 2 tutorial
                _yOffset += Time.deltaTime / 10f;

                break;
            default: // = 0 move the background with mouse
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    Vector3 mousePosition = Input.mousePosition;

                    float xNormalized = mousePosition.x / Screen.width;
                    float yNormalized = mousePosition.y / Screen.height;

                    _xOffset = xNormalized * _parallaxSpeed;
                    _yOffset = yNormalized * _parallaxSpeed;
                }

                break;
        }

        _backgroundMaterial.SetTextureOffset("_MainTex", VectorManager.NewVector3(x: _xOffset, y: _yOffset));
    }
}
