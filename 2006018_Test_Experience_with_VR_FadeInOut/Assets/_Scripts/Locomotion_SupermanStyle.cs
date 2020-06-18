using System.Collections;
using System.Collections.Generic;
// using Steam_VR_InteractionSystem;
using UnityEngine;
using Valve.VR;

public class Locomotion_SupermanStyle : MonoBehaviour
{
    public bool isLocomotionActive = true;

    [Space(10)]

    [Header("Locomotion settings")]
    [Tooltip("Y Offset from HMD (in Meters!)")]
    public float yHmdOffset = -0.2F;
    private Vector3 Center;
    [Tooltip("Treshold to overcome before moving")]
    public float Deadzone = 0.02F;
    [Tooltip("How fast is the movement?")]
    public float speedmultiplier = 0.02F;
    private float multiplyForward = 1.2F;

    [Space(5)]
    [Tooltip("Haptic response")]
    public bool hapticFeedback;    
    //[Range(0, 2000)]
    //public ushort pulsePower = 000;
    [Range(0, 1)]
    public float pulseDuration=0.1f;
    [Range(1, 320)]
    public float pulseFrequency=160;
    [Range(0.1f, 0.5f)]
    public float pulseAmplitude=0.2f;

    [Space(10)]
    [Header("Action Sets")]
    [Tooltip("What button activates movement?")]
    public SteamVR_Action_Boolean MoveAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Teleport");
    public SteamVR_Action_Pose ControllerPose = SteamVR_Input.GetAction<SteamVR_Action_Pose>("Pose");
    public SteamVR_Action_Vibration HapticAction = SteamVR_Input.GetAction<SteamVR_Action_Vibration>("Haptic");
    private bool rMoveButtonPressed = false;
    private bool lMoveButtonPressed = false;

    [Space(10)]
    [Header("Horizontal Movement")]
    [Tooltip("Is something pulling you down?")]
    public bool GravityOn = false;
    public KeyCode GravitySwitchKey = KeyCode.G;
    [Tooltip("When you don't move, you fall with this velocity")]
    public float GravityStrength = -0.05F;
    private RaycastHit hit;
    [Tooltip("Can you fly?")]
    public bool HorizontalOnly = false;
    [Tooltip("Is there a minimum Y.Position?")]
    public bool isMinimumY = false;
    [Tooltip("Never go below this absolute Y.Position")]
    public float MinimumY = 0;

    [Space(10)]
    [Header("LineRenderer")]
    public Gradient gradient;
    public float LineWidth = 0.02F;
    private LineRenderer lr;

    [Space(10)]
    [Header("SteamVRObjects")]
    public GameObject LeftHand;
    public GameObject RightHand;
    public Transform VRCamera;

    // Use this for initialization
    // private LineRenderer lr;


    void Start()
    {
        LineRenderer line = gameObject.AddComponent<LineRenderer>();
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        line.positionCount = 0;
        line.widthMultiplier = LineWidth;
        line.colorGradient = gradient;     
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {


        // Locomotion            
        if (isLocomotionActive && SteamVR.active)
        {    
           
                // VRCamera = ControllerPose.GetLocalPosition(SteamVR_Input_Sources.Camera);
                Center = new Vector3(VRCamera.transform.position.x, VRCamera.transform.position.y + yHmdOffset, VRCamera.transform.position.z);
                
                Vector3 rightDir = new Vector3(0, 0, 0);
                Vector3 leftDir = new Vector3(0, 0, 0);

                //RIGHT
                if (MoveAction.GetState(SteamVR_Input_Sources.RightHand))
                {
                    // rightDir = ControllerPose.GetLocalPosition(SteamVR_Input_Sources.RightHand) - Center;
                    rightDir = RightHand.transform.position - Center;

                    if ((rightDir.x < (Deadzone)) && (rightDir.x > (-Deadzone))) rightDir.x = 0;
                    else rightDir.x = rightDir.x - Deadzone;

                    if ((rightDir.y < (Deadzone)) && (rightDir.y > (-Deadzone))) rightDir.y = 0;
                    else rightDir.y = rightDir.y - Deadzone;

                    if ((rightDir.z < (Deadzone)) && (rightDir.z > (-Deadzone))) rightDir.z = 0;
                    else rightDir.z = rightDir.z - Deadzone;

                    rightDir = new Vector3(rightDir.x, rightDir.y, multiplyForward * rightDir.z);
                    rMoveButtonPressed = true;

                    if (hapticFeedback)
                    {
                        Pulse(SteamVR_Input_Sources.RightHand);
                    }
                   
                }
                else rMoveButtonPressed = false;

                //LEFT
                if (MoveAction.GetState(SteamVR_Input_Sources.LeftHand))
                {
                    // leftDir = ControllerPose.GetLocalPosition(SteamVR_Input_Sources.LeftHand) - Center;
                    leftDir = LeftHand.transform.position - Center;

                    if ((leftDir.x < (Deadzone)) && (leftDir.x > (-Deadzone))) leftDir.x = 0;
                    else leftDir.x = leftDir.x - Deadzone;

                    if ((rightDir.y < (Deadzone)) && (rightDir.y > (-Deadzone))) rightDir.y = 0;
                    else leftDir.y = leftDir.y - Deadzone;

                    if ((leftDir.z < (Deadzone)) && (leftDir.z > (-Deadzone))) leftDir.z = 0;
                    else leftDir.z = leftDir.z - Deadzone;

                    leftDir = new Vector3(leftDir.x, leftDir.y, multiplyForward * leftDir.z);
                    lMoveButtonPressed = true;

                    if (hapticFeedback)
                    {
                        Pulse(SteamVR_Input_Sources.LeftHand);
                    }

                }
                else lMoveButtonPressed = false;


               


                if (lMoveButtonPressed || rMoveButtonPressed)
                {
                    Vector3 dir = leftDir + rightDir;
                    //LineRenderer lr = GetComponent<LineRenderer>(); // BAD PERFORMANCE! and not needed
                    lr.positionCount = 2;
                    lr.SetPosition(0, Center);

                    if (lMoveButtonPressed && rMoveButtonPressed)
                    {
                        lr.SetPosition(1, Center + dir * 0.5f);
                    }

                    else lr.SetPosition(1, Center + dir);

                    transform.position += (dir * speedmultiplier);

                }


                else if (lr.positionCount>0)
                {                    
                    lr.positionCount = 0;
                }
                                                            

                // Gravity            
                if (Input.GetKey(GravitySwitchKey)) GravityOn = !GravityOn;
                if ((!rMoveButtonPressed && !lMoveButtonPressed) && GravityOn)
                {
                Gravity();
                }
                // ___________________


                if (isMinimumY && transform.position.y < MinimumY) transform.position = new Vector3(transform.position.x, MinimumY, transform.position.z);
                
                     
        }

    }

    private void Gravity()
    {

        // RayCast - Ist etwas unter mir?
        if (Physics.Raycast(VRCamera.transform.position, Vector3.down, out hit))
        {
            // the ray collided with something, you can interact
            // with the hit object now by using hit.collider.gameObject



            if (hit.distance > VRCamera.transform.localPosition.y + 0.1F) // höher als kopfhoch?
            {

                //Debug.Log("ich falle");
                transform.position += new Vector3(0, GravityStrength, 0);

            }

            else if (hit.distance < VRCamera.transform.localPosition.y - 0.1F) // tiefer als kopfhoch?
            {

                //Debug.Log("ich fliege hoch");

                transform.position += new Vector3(0, (-1 * GravityStrength), 0);

            }

            else // ungefähr kopfhoch
            {


            }



        }

        // Nichts unter mir? Aaaahhh....
        else
        {
            // nothing was below your gameObject
            transform.position += new Vector3(0, (-1 * GravityStrength), 0); // hochfliegen
        }
    }

private void Pulse (SteamVR_Input_Sources source)
    {
        HapticAction.Execute(0, pulseDuration, pulseFrequency, pulseAmplitude, source);
    }



    }
