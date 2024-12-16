using UnityEngine;

public class BackgroundMovement : MonoBehaviour
{
    private float yOffset = 0;
    private float xOffset = 0;
    private Material mat;
    public PlayerController playerController;
    public static int movementOption;
    public float parallaxSpeed;

    private bool _backgroundRealistic;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        _backgroundRealistic = SessionManager.GetRealistic();
    }

    void Update()
    {
        switch (movementOption)
        {   
            case 1: // = 1 move background during game to bottom
                if(!GameController.gameOver)
                {   
                    float playerHorizontalMovement = 0;
                    float playerVerticalMovement = 0;

                    if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
                    {
                        playerHorizontalMovement = playerController.getHorizontalMove;
                        playerVerticalMovement = playerController.getVerticalMove;
                    }
                    else if(Application.platform == RuntimePlatform.Android)
                    {
                        playerHorizontalMovement = playerController.mobileOffset.x;
                        playerVerticalMovement = playerController.mobileOffset.z;
                    }

                    yOffset += Time.deltaTime * GameController.gameSpeed / 10f;

                    if(_backgroundRealistic) //extra moves
                    {
                        if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
                        {
                            if(playerHorizontalMovement > 0)
                            {
                                //Ship to right, background to left
                                xOffset += 0.0001f * GameController.gameSpeed;
                            }
                            else if(playerHorizontalMovement < 0)
                            {
                                //Ship to left, background to righ
                                xOffset -= 0.0001f * GameController.gameSpeed;
                            }

                            if(playerVerticalMovement > 0)
                            {
                                //Ship up, background down more speed
                                yOffset += 0.00015f * GameController.gameSpeed;
                            }
                            else if(playerVerticalMovement < 0)
                            {
                                //Ship down, background down less speed
                                yOffset -= 0.0001f * GameController.gameSpeed;
                            }
                        }
                        else if(Application.platform == RuntimePlatform.Android)
                        {
                            if(playerHorizontalMovement > 0)
                            {
                                //Ship to right, background to left
                                xOffset += 0.0001f * GameController.gameSpeed * 3f;
                            }
                            else if(playerHorizontalMovement < 0)
                            {
                                //Ship to left, background to righ
                                xOffset -= 0.0001f * GameController.gameSpeed * 3f;
                            }

                            if(playerVerticalMovement > 0)
                            {
                                //Ship up, background down more speed
                                yOffset += 0.00015f * GameController.gameSpeed * 3f;
                            }
                            else if(playerVerticalMovement < 0)
                            {
                                //Ship down, background down less speed
                                yOffset -= 0.0001f * GameController.gameSpeed * 3f;
                            }
                        }
                    }
                }
                break;
            default: // = 0 move the map with mouse
                Vector3 mousePosition = Input.mousePosition;

                float xNormalized = mousePosition.x / Screen.width;
                float yNormalized = mousePosition.y / Screen.height;

                xOffset = xNormalized * parallaxSpeed;
                yOffset = yNormalized * parallaxSpeed;

                break;
        }

        mat.SetTextureOffset("_MainTex", VectorCreator.SetVector3(x: xOffset, y: yOffset));
    }
}
