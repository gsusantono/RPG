using UnityEngine;

public class CameraCol : MonoBehaviour {

    private Transform rayCheckCam;
    public float minDistance = 1f;
    public float maxDistance = 4f;
    public float smooth = 50f;
    public LayerMask layerMask;

    private Vector3 vecDefault;
    private float distance;

    private void Start()
    {
        vecDefault = transform.localPosition.normalized;
        distance = transform.localPosition.magnitude;
        rayCheckCam = GetComponentInParent<Transform>();
    }
    private void LateUpdate()
    {
        CameraCollision();
    }
    void CameraCollision()
    {
        Vector3 desiredCamPos = transform.parent.TransformPoint(vecDefault * maxDistance);
        //  プレーヤからカメラ位置へのベクトル

        RaycastHit hit;
        //  RayCastを撃って、Layer＝＝”Ground”だったら、当たった位置にカメラを設定する
        if (Physics.Linecast(rayCheckCam.position, desiredCamPos, out hit, layerMask))
        {
            distance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
        }
        else
        {
            distance = maxDistance;
        }
        transform.localPosition = Vector3.Lerp(transform.localPosition,
            vecDefault * distance, Time.deltaTime * smooth);
    }
}
