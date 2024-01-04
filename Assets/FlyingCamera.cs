using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingCamera : MonoBehaviour
{
    public Camera cam;
    public float Speed;
    public KeyCode keyCode;
    // Update is called once per frame
    Vector3 forward = Vector3.zero;
    private void OnDrawGizmos() {
        Gizmos.DrawLine(transform.position, transform.position + forward * 3);
    }
    void Update()
    {
        
        if (Input.GetMouseButton(1)) {

            forward = new Vector3(Mathf.Sin((transform.eulerAngles.y + 90) * Mathf.Deg2Rad), 0, Mathf.Cos((transform.eulerAngles.y + 90) * Mathf.Deg2Rad)).normalized ;
            Vector3 right = new Vector3(Mathf.Cos(transform.eulerAngles.x * Mathf.Deg2Rad), Mathf.Sin(transform.eulerAngles.y * Mathf.Deg2Rad), Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad));
            //transform.position += ((forward * Input.GetAxis("Vertical")) + (right * Input.GetAxis("Horizontal"))) * Speed * Time.deltaTime;
            transform.position += (forward * Input.GetAxis("Horizontal")) * Speed * Time.deltaTime;
            transform.position += (transform.forward * Input.GetAxis("Vertical")) * Speed * Time.deltaTime;
            Debug.Log(forward);
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            transform.Rotate(Vector3.up * 1 * mouseX);
            transform.Rotate(Vector3.left * 1 * mouseY);
        }
        //if (Input.GetMouseButton(2)) {
        //    cam.transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0));
        //    //transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0));
        //}
        // cam.transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y") * 2, Input.GetAxis("Mouse X") * 2, 0));
        if (Input.GetMouseButtonDown(0)) {
            
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit)) {
                if (hit.transform.tag == "Terrain") {
                    Debug.Log(hit.point);
                    Marching marching = hit.transform.GetComponent<Marching>();
                    for (int x = -1; x <= 1; x++) {
                        for (int y = -1; y <= 1; y++) {
                            for (int z = -1; z <= 1; z++) {
                                Vector3 pos = new Vector3(x, y, z);
                                marching.PlaceTerrain(hit.point + pos);
                            }
                        }
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(2)) {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1f));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) {
                if (hit.transform.tag == "Terrain") {
                   // Debug.Log("Terrain Clicked");
                    hit.transform.GetComponent<Marching>().RemoveTerrain(hit.point);
                }
            }
        }
    }
}
