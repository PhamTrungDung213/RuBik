using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RubikMapDisplay : MonoBehaviour
{
    public GameObject rubik;
    public RubikLayerRotate layerRotate;

    [Header("Keo 9 Panel cua tung mat vao day (0=top-left, 8=bot-right)")]
    public GameObject[] faceUp    = new GameObject[9];
    public GameObject[] faceFront = new GameObject[9];
    public GameObject[] faceDown  = new GameObject[9];
    public GameObject[] faceLeft  = new GameObject[9];
    public GameObject[] faceRight = new GameObject[9];
    public GameObject[] faceBack  = new GameObject[9];

    float spacing = 1f; // khoang cach giua cubelet trong LOCAL SPACE cua rubik

    void Start()
    {
        ComputeSpacing();
    }

    // Tinh khoang cach thuc te giua cac cubelet (de scan dung o)
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
        Debug.Log("[RubikMap] Cubelet spacing (local): " + spacing);
    }

    void LateUpdate()
    {
        if (layerRotate != null && layerRotate.rotating) return;

        // Moi mat dung truc LOCAL cua Rubik:
        //   localNormal = huong phap tuyen mat
        //   localRight  = huong tang col (trai->phai tren panel)
        //   localUp     = huong tang row (bot->top tren panel, row=1 la dong tren cung)
        //
        // Cross-map layout:
        //       [U]
        //  [L][F][R][B]
        //       [D]
        //
        // U: phai = +X, tren-panel = -Z (Front o day panel)
        // D: phai = +X, tren-panel = +Z (Back o day panel)
        // F: phai = +X, tren-panel = +Y
        // B: phai = -X (dao vi nhin tu sau), tren-panel = +Y
        // L: phai = -Z (Back o phai), tren-panel = +Y
        // R: phai = +Z (Front o phai), tren-panel = +Y

        ScanFace(Vector3.up,       Vector3.right,   -Vector3.forward, faceUp);
        ScanFace(Vector3.down,     Vector3.right,    Vector3.forward, faceDown);
        ScanFace(-Vector3.forward, Vector3.right,    Vector3.up,      faceFront);
        ScanFace(Vector3.forward, -Vector3.right,    Vector3.up,      faceBack);
        ScanFace(-Vector3.right,  -Vector3.forward,  Vector3.up,      faceLeft);
        ScanFace(Vector3.right,    Vector3.forward,  Vector3.up,      faceRight);
    }

    void ScanFace(Vector3 localNormal, Vector3 localRight, Vector3 localUp, GameObject[] cells)
    {
        float sp = spacing;
        // Tinh huong world (TransformDirection khong tinh scale, dung cho huong)
        Vector3 worldNormal = rubik.transform.TransformDirection(localNormal);
        int i = 0;
        for (int row = 1; row >= -1; row--)
        {
            for (int col = -1; col <= 1; col++)
            {
                // Tinh diem xuat phat trong LOCAL SPACE roi chuyen sang world
                // (TransformPoint tinh ca scale, nen dung cho vi tri)
                Vector3 localOrigin = localNormal * (sp * 2f)
                                    + localRight   * (col * sp)
                                    + localUp      * (row * sp);
                Vector3 worldOrigin = rubik.transform.TransformPoint(localOrigin);

                Ray ray = new Ray(worldOrigin, -worldNormal);
                // Khoang raycast = 4x spacing, chuyen sang world qua lossyScale
                float rayDist = sp * 4f * rubik.transform.lossyScale.x;

                if (Physics.Raycast(ray, out RaycastHit hit, rayDist))
                {
                    Color c = ReadColor(hit, worldNormal);
                    var panel = cells[i]?.GetComponent<Image>();
                    if (panel != null) panel.color = c;
                }
                i++;
            }
        }
    }

    // Doc mau cubelet: ho tro don material, da material (submesh), va child renderer
    Color ReadColor(RaycastHit hit, Vector3 worldNormal)
    {
        Renderer rend = hit.collider.GetComponent<Renderer>();

        if (rend != null)
        {
            if (rend.sharedMaterials.Length == 1)
                return rend.sharedMaterial != null ? rend.sharedMaterial.color : Color.grey;

            // Da material: tim submesh bi hit
            MeshFilter mf = hit.collider.GetComponent<MeshFilter>();
            if (mf != null && mf.sharedMesh != null)
            {
                int sub = GetSubMesh(mf.sharedMesh, hit.triangleIndex);
                sub = Mathf.Clamp(sub, 0, rend.sharedMaterials.Length - 1);
                if (rend.sharedMaterials[sub] != null)
                    return rend.sharedMaterials[sub].color;
            }
            return rend.sharedMaterial != null ? rend.sharedMaterial.color : Color.grey;
        }

        // Thu tim renderer trong child objects, chon cai mat huong gan nhat voi worldNormal
        Renderer best = null;
        float bestDot = 0.5f;
        foreach (Renderer r in hit.collider.GetComponentsInChildren<Renderer>())
        {
            foreach (Vector3 axis in new[] {
                r.transform.forward, -r.transform.forward,
                r.transform.up,      -r.transform.up,
                r.transform.right,   -r.transform.right })
            {
                float d = Vector3.Dot(axis, worldNormal);
                if (d > bestDot) { bestDot = d; best = r; }
            }
        }
        if (best != null && best.sharedMaterial != null) return best.sharedMaterial.color;

        return Color.grey;
    }

    static int GetSubMesh(Mesh mesh, int triangleIndex)
    {
        for (int s = 0; s < mesh.subMeshCount; s++)
        {
            var sub = mesh.GetSubMesh(s);
            int start = sub.indexStart / 3;
            int count = sub.indexCount  / 3;
            if (triangleIndex >= start && triangleIndex < start + count) return s;
        }
        return 0;
    }
}
