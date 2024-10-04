using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    [SerializeField] private LineRenderer rubber;
    [SerializeField] private Transform firstPoint;
    [SerializeField] private Transform secondPoint;

    [Header("Inscribed")]
    public GameObject projectilePrefab;
    public float velocityMult = 10f;
    public GameObject projLinePrefab;

    [Header("Dynamic")]
    public GameObject launchPoint;
    public Vector3 launchPos;
    public GameObject projectile;
    public bool aimingMode;

    [Header("Sound")]
    public AudioClip releaseSound;     // Sound to play when releasing the projectile
    private AudioSource audioSource;

    [Header("Camera Shake")]
    public CameraShake cameraShake;

    void Start(){
        rubber.positionCount = 3;
        rubber.SetPosition(0, firstPoint.position);
        rubber.SetPosition(2, secondPoint.position);

        Vector3 middlePoint = (firstPoint.position + secondPoint.position) / 2; // Midpoint between first and second points
        rubber.SetPosition(1, middlePoint);

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null) {
            Debug.LogError("No AudioSource found on the Slingshot object!");
        }
    }

    void Awake(){
        Transform launchPointTrans = transform.Find("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive(false);
        launchPos = launchPointTrans.position;
    }
    void OnMouseEnter(){
        print("Slingshot:OnMouseEnter()");
        launchPoint.SetActive(true);
    }
    void OnMouseExit(){
        print("Slingshot:OnMouseExit()");
        launchPoint.SetActive(false);
    }

    void OnMouseDown(){
        //player has pressed mouse button while over slingshot
        aimingMode = true;
        projectile = Instantiate(projectilePrefab) as GameObject;
        projectile.transform.position = launchPos;
        projectile.GetComponent<Rigidbody>().isKinematic = true;
    }
    void Update(){
        //if not in aim mode, return
        if(!aimingMode) return;

        //get current pos in 2d screen coords
        Vector3 mousePos2d = Input.mousePosition;
        float zDistanceFromCamera = Camera.main.WorldToScreenPoint(launchPos).z;
        mousePos2d.z = zDistanceFromCamera; // Set the z-distance based on the world position of the launch point

        // Convert the mouse position to 3D world coordinates
        Vector3 mousePos3d = Camera.main.ScreenToWorldPoint(mousePos2d);

        // Find the delta from the launch position to the mouse position in 3D
        Vector3 mouseDelta = mousePos3d - launchPos;

        // Limit the mouse delta to the radius of the slingshot's sphere collider
        float maxMagnitude = this.GetComponent<SphereCollider>().radius;
        if (mouseDelta.magnitude > maxMagnitude) {
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }

        // Move the projectile to the new position
        Vector3 projPos = launchPos + mouseDelta;
        projectile.transform.position = projPos;

        // Update the rubber band's middle point to follow the projectile
        rubber.SetPosition(1, projectile.transform.position);

        // Check if the mouse button has been released
        if (Input.GetMouseButtonUp(0)) {
            aimingMode = false;
            Rigidbody projRB = projectile.GetComponent<Rigidbody>();
            projRB.isKinematic = false;
            projRB.collisionDetectionMode = CollisionDetectionMode.Continuous;
            projRB.velocity = -mouseDelta * velocityMult;

            PlayReleaseSound();
            StartCoroutine(cameraShake.Shake(0.5f, 0.5f));

            // Camera and projectile interaction (optional)
            FollowCam.SWITCH_VIEW(FollowCam.eView.slingshot);
            FollowCam.POI = projectile;
            Instantiate<GameObject>(projLinePrefab, projectile.transform);
            projectile = null;

            // Notify mission manager that a shot has been fired
            MissionDemolishon.SHOT_FIRED();
        }
    }
    Vector3 GetMousePositionInWorld(){
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z += Camera.main.transform.position.z;
        Vector3 mousePositionInWorld = Camera.main.ScreenToWorldPoint(mousePosition);
        mousePositionInWorld.z -= Camera.main.transform.position.z;
        return mousePositionInWorld - transform.position;
    }
    void PlayReleaseSound(){
        if (releaseSound != null && audioSource != null){
            audioSource.PlayOneShot(releaseSound);
        } else {
            Debug.LogError("No release sound assigned or no AudioSource component found.");
        }
    }
}
