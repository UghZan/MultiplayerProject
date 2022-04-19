using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//Hides walls in front of camera and controls camera position
//Not the best way to do this, but it'll do for small scale\

//considering that this is clientside behaviour, no networking needed

public class CameraController : MonoBehaviour
{
    [Header("Camera Rotation Settings")]
    public int pos; //every 90 degrees
    public float rotationSpeed; //how fast should we rotate towards desired position

    [Header("Misc")]
    [SerializeField] Camera camera;
    [SerializeField] LayerMask wallMask; //wlls have "Wall" layer, we'll check only that

    Ray cameraRay;
    RaycastHit[] rayHits; // using nonalloc spherecast
    List<Transform> hiddenWalls;

    // Start is called before the first frame update
    void Start()
    {
        hiddenWalls = new List<Transform>();
        rayHits = new RaycastHit[2];
    }

    // Update is called once per frame
    void Update()
    {
        //take a ray from center of the screen
        cameraRay = camera.ViewportPointToRay(Vector2.one / 2);

        //get all walls in front of camera
        Physics.SphereCastNonAlloc(cameraRay, 1f, rayHits, 10, wallMask);
        foreach (RaycastHit hit in rayHits)
        {
            //considering that rayHits is a static array, we may not even have something hit on this index
            //so check it first
            if (hit.transform == null) continue;
            if (!hiddenWalls.Contains(hit.transform))
            {
                //if this is a new wall, add it into hidden walls array and stop it from rendering
                hiddenWalls.Add(hit.transform);
                hit.transform.GetComponent<MeshRenderer>().enabled = false;
            }
        }
        //now we need to check if we should update which walls should be hidden
        for (int i = 0; i < hiddenWalls.Count; i++)
        {
            bool stillHidden = false;
            //check if any of the walls that we still need to hide are in hiddenWalls
            foreach(RaycastHit hit in rayHits)
            {
                if (hit.transform == null) continue;

                if(hiddenWalls[i].Equals(hit.transform))
                {
                    stillHidden = true;
                    break;
                }
            }

            //if some walls shouldn't be hidden anymore, remove it from hidden walls and render it again
            //don't forget to update the index
            if(!stillHidden)
            {
                hiddenWalls[i].GetComponent<MeshRenderer>().enabled = true;
                hiddenWalls.RemoveAt(i);
                i--;
            }
        }
    }

    private void LateUpdate()
    {
        //rotate camera towards position
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(45 + pos * 45, Vector3.up), Time.deltaTime * rotationSpeed);
    }

    //fires upon pressing RMB/right button
    public void RotateCamera(InputAction.CallbackContext ctx)
    {
        //should fire once
        if (ctx.phase == InputActionPhase.Performed)
        {
            pos++;
        }
    }
}
