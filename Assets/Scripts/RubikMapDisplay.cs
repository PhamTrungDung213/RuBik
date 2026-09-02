using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RubikMapDisplay : MonoBehaviour
{
    public GameObject rubik;
    public RubikLayerRotate layerRotate;

    [Header("Keo 9 Panel cua tung mat vao day")]
    public GameObject[] faceUp    = new GameObject[9];
    public GameObject[] faceFront = new GameObject[9];
    public GameObject[] faceDown  = new GameObject[9];
    public GameObject[] faceLeft  = new GameObject[9];
    public GameObject[] faceRight = new GameObject[9];
    public GameObject[] faceBack  = new GameObject[9];

    float spacing = 1f;

    void Start()
    {
        ComputeSpacing();

        // Tu dong sap xep 9 panel cua tung mat theo thu tu chuan:
        // Hang tren (Trai, Giua, Phai) -> Hang giua (Trai, Giua, Phai) -> Hang duoi (Trai, Giua, Phai)
        // Bat ke nguoi dung keo tha trong Inspector theo thu tu nao.
        SortGrid(faceUp);
        SortGrid(faceFront);
        SortGrid(faceDown);
        SortGrid(faceLeft);
        SortGrid(faceRight);
        SortGrid(faceBack);
    }

    void SortGrid(GameObject[] panels)
    {
        if (panels == null || panels.Length != 9) return;
        for (int i = 0; i < 9; i++)
        {
            if (panels[i] == null) return;
        }

        // Sap xep theo Y giam dan (tren xuong duoi), roi theo X tang dan (trai sang phai)
        var sorted = panels
            .Select(p => new { obj = p, rt = p.GetComponent<RectTransform>() })
            .OrderByDescending(p => Mathf.Round(p.rt != null ? p.rt.anchoredPosition.y : 0f))
            .ThenBy(p => Mathf.Round(p.rt != null ? p.rt.anchoredPosition.x : 0f))
            .Select(p => p.obj)
            .ToArray();

        for (int i = 0; i < 9; i++)
        {
            panels[i] = sorted[i];
        }
    }

    void ComputeSpacing()
    {
        var xs = new List<float>();
        foreach (Transform child in rubik.transform)
        {
            Vector3 lp = rubik.transform.InverseTransformPoint(child.position);
            xs.Add(lp.x);
        }
        xs.Sort();

        float minDist = float.MaxValue;
        for (int i = 1; i < xs.Count; i++)
        {
            float d = xs[i] - xs[i - 1];
            if (d > 0.01f) minDist = Mathf.Min(minDist, d);
        }
        if (minDist < float.MaxValue) spacing = minDist;
    }

    void LateUpdate()
    {
        if (layerRotate != null && layerRotate.rotating) return;

        // Quet 6 mat theo he toa do ban trai phang 2D (Net) chuan xac 100%
        ScanFace(Vector3.up,       Vector3.right,    Vector3.forward, faceUp);
        ScanFace(Vector3.down,     Vector3.right,   -Vector3.forward, faceDown);
        ScanFace(-Vector3.forward, Vector3.right,    Vector3.up,      faceFront);
        ScanFace(Vector3.forward, -Vector3.right,    Vector3.up,      faceBack);
        ScanFace(-Vector3.right,  -Vector3.forward,  Vector3.up,      faceLeft);
        ScanFace(Vector3.right,    Vector3.forward,  Vector3.up,      faceRight);
    }

    void ScanFace(Vector3 localNormal, Vector3 localRight, Vector3 localUp, GameObject[] cells)
    {
        float sp = spacing;
        Vector3 worldNormal = rubik.transform.TransformDirection(localNormal);

        for (int row = 1; row >= -1; row--)
        {
            for (int col = -1; col <= 1; col++)
            {
                // Index chuan theo Row-Major: 
                // row=1 (Top): 0, 1, 2
                // row=0 (Mid): 3, 4, 5
                // row=-1(Bot): 6, 7, 8
                int i = (1 - row) * 3 + (col + 1);

                Vector3 localOrigin = localNormal * (sp * 2f)
                                    + localRight   * (col * sp)
                                    + localUp      * (row * sp);
                Vector3 worldOrigin = rubik.transform.TransformPoint(localOrigin);

                Ray ray = new Ray(worldOrigin, -worldNormal);
                float rayDist = sp * 4f * rubik.transform.lossyScale.x;

                if (Physics.Raycast(ray, out RaycastHit hit, rayDist))
                {
                    Color c = ReadColor(hit, worldNormal);
                    var panel = cells[i]?.GetComponent<Image>();
                    if (panel != null) panel.color = c;
                }
            }
        }
    }

    Color ReadColor(RaycastHit hit, Vector3 worldNormal)
    {
        // 1. Tim root cubelet (la con truc tiep cua GameObject rubik)
        Transform cube = hit.collider.transform;
        while (cube != null && cube.parent != null && cube.parent != rubik.transform)
        {
            cube = cube.parent;
        }
        if (cube == null || cube.parent == null) cube = hit.collider.transform;

        // 2. Tim sticker mat nao dang huong ve worldNormal nhat
        Renderer bestRenderer = null;
        float maxDot = 0.5f;

        foreach (Transform child in cube)
        {
            Renderer r = child.GetComponent<Renderer>();
            if (r == null || r.sharedMaterial == null) continue;

            Vector3 faceDir = (child.position - cube.position).normalized;
            float dot = Vector3.Dot(faceDir, worldNormal);
            if (dot > maxDot)
            {
                maxDot = dot;
                bestRenderer = r;
            }
        }

        if (bestRenderer != null && bestRenderer.sharedMaterial != null)
        {
            return bestRenderer.sharedMaterial.color;
        }

        // Fallback
        Renderer rend = hit.collider.GetComponent<Renderer>();
        if (rend != null && rend.sharedMaterial != null)
            return rend.sharedMaterial.color;

        return Color.grey;
    }
}
