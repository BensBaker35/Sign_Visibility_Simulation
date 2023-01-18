using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RIT.RochesterLOS.Player
{
    /// <summary>
    /// Placed on the Main Camera game object, used to walk around the area
    /// </summary>
   // [RequireComponent(typeof(Camera))]
    public class PlayerControl : MonoBehaviour
    {
        private const float LOOK_MAX = 360f;
        [SerializeField] private float lookSpeed = 150f;
        [SerializeField] private float lerpSpeed = 750f;
        [SerializeField] private float moveSpeed = 100f;

        private float xRotation = 0;
        private float X, Y;
        private bool lerpInProgress;

        private Camera playerCamera;
        

        // Start is called before the first frame update
        void Start()
        {
            //Cursor.lockState = CursorLockMode.Confined;
            //playerCamera = GetComponentInChildren<Camera>();
        }

        // Update is called once per frame
        void Update()
        {
            RotateCamera();

            if(Input.GetMouseButton(0)) //Left Click
            {
                Debug.Log("Left Mouse Click");
                var clickLocation = playerCamera.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;
                if(Physics.Raycast(clickLocation, out hit, Mathf.Infinity))
                {
                    var hitObject = hit.collider.gameObject;
                    Debug.Log($"Registerd hit: {hitObject.name}");
                    // if(!lerpInProgress )
                    // {
                    //     StartCoroutine(MoveToPosition(hit.point));
                    // }
                    
                    
                }
            }

            var inputVec = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            inputVec *= moveSpeed * Time.deltaTime;

            transform.position += inputVec;

        }

    /// <summary>
    /// Smoothly moves the character to the clicked position
    /// </summary>
    /// <param name="newPosition">The VALID position for the player to move to</param>
    /// <returns>IEnumerator for Coroutine</returns>
        private IEnumerator MoveToPosition(Vector3 newPosition)
        {
            lerpInProgress = true;
            var startTime = Time.time;
            Vector3 startPos = transform.position;
            var distanceToNext = Vector3.Distance(startPos, newPosition);
            while(true)
            {
                var distCmpl = (Time.time - startTime) * lerpSpeed;
                var complPerc = distCmpl / distanceToNext;
                transform.position = Vector3.Lerp(startPos, newPosition, complPerc);

                if(transform.position.Equals(newPosition))
                {
                    Debug.Log("Position Achieved");
                    break;
                }

                yield return new WaitForFixedUpdate();
            }
            lerpInProgress = false;            
        }

        /// <summary>
        /// Rotates the camera by following the mouse
        /// </summary>
        private void RotateCamera()
        {
            //Time.deltaTime enusres uniformity no matter frame rate
            transform.Rotate(new Vector3(Input.GetAxis("Mouse Y") * lookSpeed * Time.deltaTime,
               -Input.GetAxis("Mouse X") * lookSpeed * Time.deltaTime, 0));

            //Generalized human neck rotation
            X = Mathf.Clamp(transform.rotation.eulerAngles.x, -LOOK_MAX, LOOK_MAX);
            Y = Mathf.Clamp(transform.rotation.eulerAngles.y, -LOOK_MAX, LOOK_MAX);

            transform.rotation = Quaternion.Euler(X, Y, 0);
        }

        private void FixedUpdate()
        {

        }

        private void OnCollisionEnter(Collision other)
        {
            Debug.Log("Collision");
        }
    }
}
