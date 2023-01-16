using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RIT.RochesterLOS.Player
{
    /// <summary>
    /// Placed on the Main Camera game object, used to walk around the area
    /// </summary>
    public class PlayerControl : MonoBehaviour
    {
        [SerializeField] private float lookSpeed = 150f;

        float xRotation = 0;
        float X, Y;

        // Start is called before the first frame update
        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void FixedUpdate()
        {
            //Time.deltaTime enusres uniformity no matter frame rate
            transform.Rotate(new Vector3(Input.GetAxis("Mouse Y") * lookSpeed * Time.deltaTime,
               Input.GetAxis("Mouse X") * lookSpeed * Time.deltaTime, 0));
            X = transform.rotation.eulerAngles.x;
            Y = transform.rotation.eulerAngles.y;
            transform.rotation = Quaternion.Euler(X, Y, 0);
        }
    }
}
