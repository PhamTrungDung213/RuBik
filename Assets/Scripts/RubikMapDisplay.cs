using UnityEngine;
using UnityEngine.UI;

public class RubikMapDisplay : MonoBehaviour
{
    public GameObject rubik;
    public RubikLayerRotate layerRotate;

    [Header("Keo cac Panel (0-8) cua tung mat vao day")]
    public GameObject[] faceUp    = new GameObject[9];
    public GameObject[] faceFront = new GameObject[9];
    public GameObject[] faceDown  = new GameObject[9];
    public GameObject[] faceLeft  = new GameObject[9];
    public GameObject[] faceRight = new GameObject[9];
    public GameObject[] faceBack  = new GameObject[9];

    void LateUpdate()
    {
        if (layerRotate != null && layerRotate.rotating) return;

        ScanFace(Vector3.up,      faceUp);
        ScanFace(Vector3.forward, faceFront);
        ScanFace(Vector3.down,    faceDown);
        ScanFace(Vector3.left,    faceLeft);
        ScanFace(Vector3.right,   faceRight);
        ScanFace(Vector3.back,    faceBack);
    }

    void ScanFace(Vector3 direction, GameObject[] cells)
    {
        Vector3 worldDir = rubik.transform.TransformDirection(direction);
        Vector3 origin   = rubik.transform.position + worldDir * 2f;

        Vector3 right, up;
        GetAxes(worldDir, out right, out up);

        int i = 0;
        for (int row = 1; row >= -1; row--)
        {
            for (int col = -1; col <= 1; col++)
            {
                Vector3 rayOrigin = origin + right * col + up * row;
                Ray ray = new Ray(rayOrigin, -worldDir);

                if (Physics.Raycast(ray, out RaycastHit hit, 5f))
                {
                    Renderer rend  = hit.collider.GetComponent<Renderer>();
                    Image    panel = cells[i].GetComponent<Image>();
                    if (rend != null && panel != null)
                        panel.color = rend.material.color;
                }
                i++;
            }
        }
    }

    void GetAxes(Vector3 dir, out Vector3 right, out Vector3 up)
    {
        dir = dir.normalized;
        if (Mathf.Abs(Vector3.Dot(dir, Vector3.up)) < 0.9f)
        {
            right = Vector3.Cross(Vector3.up, dir).normalized;
            up    = Vector3.Cross(dir, right).normalized;
        }
        else
        {
            right = Vector3.Cross(Vector3.forward, dir).normalized;
            up    = Vector3.Cross(dir, right).normalized;
        }
    }
}
