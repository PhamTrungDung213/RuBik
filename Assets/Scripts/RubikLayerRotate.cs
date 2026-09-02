using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubikLayerRotate : MonoBehaviour
{
    public GameObject rubik;
    public float speed = 300f;

    [HideInInspector] public bool rotating = false;
    [HideInInspector] public bool recordMoves = true;
    public System.Action<Vector3, float, float> OnMoveComplete;

    Vector2 mouseStart;
    Transform hitCube;
    Vector3 localHitNormal;

    bool dragStarted;
    Vector3 liveDragAxis;
    float liveDragLayer;
    List<Transform> liveDragCubes;
    float liveDragAngle;
    Vector2 liveDragScreenDir;
    const float DRAG_TO_90 = 120f;

    void Update()
    {
        if (rotating) return;
        HandleKeyboard();
        HandleMouse();
    }

    void HandleKeyboard()
    {
        bool shift = Input.GetKey(KeyCode.LeftShift);
        float dir = shift ? -1f : 1f;

        if (Input.GetKeyDown(KeyCode.U)) StartCoroutine(Rotate(Vector3.up,       1f,  90f * dir));
        if (Input.GetKeyDown(KeyCode.D)) StartCoroutine(Rotate(Vector3.up,      -1f, -90f * dir));
        if (Input.GetKeyDown(KeyCode.R)) StartCoroutine(Rotate(Vector3.right,    1f,  90f * dir));
        if (Input.GetKeyDown(KeyCode.L)) StartCoroutine(Rotate(Vector3.right,   -1f, -90f * dir));
        if (Input.GetKeyDown(KeyCode.F)) StartCoroutine(Rotate(Vector3.forward, -1f,  90f * dir));
        if (Input.GetKeyDown(KeyCode.B)) StartCoroutine(Rotate(Vector3.forward,  1f, -90f * dir));
    }

    void HandleMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Tim cubelet con truc tiep cua rubik
                Transform cube = hit.collider.transform;
                while (cube != null && cube.parent != null && cube.parent != rubik.transform)
                {
                    cube = cube.parent;
                }
                if (cube == null || cube.parent != rubik.transform) cube = hit.collider.transform;

                hitCube = cube;
                localHitNormal = SnapToAxis(rubik.transform.InverseTransformDirection(hit.normal));
                mouseStart = Input.mousePosition;
                dragStarted = false;
            }
        }

        if (Input.GetMouseButton(0) && hitCube != null)
        {
            Vector2 drag = (Vector2)Input.mousePosition - mouseStart;

            if (!dragStarted && drag.magnitude > 15f)
            {
                dragStarted = true;
                SetupLiveDrag(drag);
            }

            if (dragStarted && liveDragCubes != null)
            {
                ApplyLiveDrag(drag);
            }
        }

        if (Input.GetMouseButtonUp(0) && hitCube != null)
        {
            if (dragStarted && liveDragCubes != null)
            {
                StartCoroutine(SnapDrag());
            }
            else
            {
                hitCube = null;
            }
        }
    }

    void SetupLiveDrag(Vector2 drag)
    {
        Vector3 A1, A2;
        GetAxes(localHitNormal, out A1, out A2);

        Camera cam = Camera.main;
        Vector3 worldCenter = rubik.transform.position;
        Vector3 worldCubePos = hitCube.position;
        Vector2 scrCubePos = cam.WorldToScreenPoint(worldCubePos);

        Vector3 worldA1 = rubik.transform.TransformDirection(A1);
        Vector3 worldA2 = rubik.transform.TransformDirection(A2);

        Vector2 sA1 = ((Vector2)cam.WorldToScreenPoint(worldCubePos + worldA1 * 0.5f) - scrCubePos).normalized;
        Vector2 sA2 = ((Vector2)cam.WorldToScreenPoint(worldCubePos + worldA2 * 0.5f) - scrCubePos).normalized;

        float dotA1 = Vector2.Dot(drag.normalized, sA1);
        float dotA2 = Vector2.Dot(drag.normalized, sA2);
        Vector3 localCubePos = rubik.transform.InverseTransformPoint(hitCube.position);

        // --- Chon truc xoay (Chuan hoa tong quat cho tat ca 6 mat bao gom U va D) ---
        // Vuot theo truc tren mat nao thi se xoay lop quanh truc vuong goc con lai
        if (Mathf.Abs(dotA1) >= Mathf.Abs(dotA2))
        {
            liveDragAxis  = A2;
            liveDragLayer = Mathf.Round(Vector3.Dot(localCubePos, A2));
        }
        else
        {
            liveDragAxis  = A1;
            liveDragLayer = Mathf.Round(Vector3.Dot(localCubePos, A1));
        }

        // --- Tinh vector tangent tren man hinh ---
        Vector3 worldRotAxis = rubik.transform.TransformDirection(liveDragAxis);
        Vector3 hitVec       = worldCubePos - worldCenter;
        Vector3 tangent      = Vector3.Cross(worldRotAxis, hitVec);

        // Neu hit trung truc xoay (tam vien nam tren truc), dung phap tuyen mat de tinh huong
        if (tangent.sqrMagnitude < 0.001f)
        {
            Vector3 worldNormal = rubik.transform.TransformDirection(localHitNormal);
            tangent = Vector3.Cross(worldRotAxis, worldNormal);
        }

        Vector2 screenTangent = ((Vector2)cam.WorldToScreenPoint(worldCubePos + tangent * 0.5f) - scrCubePos).normalized;

        liveDragScreenDir = screenTangent;
        liveDragCubes = GetLayer(liveDragAxis, liveDragLayer);
        liveDragAngle = 0f;
    }

    void ApplyLiveDrag(Vector2 drag)
    {
        float projected   = Vector2.Dot(drag, liveDragScreenDir);
        float targetAngle = Mathf.Clamp(projected / DRAG_TO_90 * 90f, -90f, 90f);
        float delta       = targetAngle - liveDragAngle;

        if (Mathf.Abs(delta) > 0.01f)
        {
            Vector3 worldAxis = rubik.transform.TransformDirection(liveDragAxis);
            foreach (var t in liveDragCubes)
                t.RotateAround(rubik.transform.position, worldAxis, delta);
            liveDragAngle = targetAngle;
        }
    }

    IEnumerator SnapDrag()
    {
        rotating = true;
        float snapped   = Mathf.Round(liveDragAngle / 90f) * 90f;
        float remaining = snapped - liveDragAngle;
        Vector3 worldAxis = rubik.transform.TransformDirection(liveDragAxis);

        float done = 0f, abs = Mathf.Abs(remaining);
        while (done < abs - 0.01f)
        {
            float step = Mathf.Min(speed * Time.deltaTime, abs - done);
            foreach (var t in liveDragCubes)
                t.RotateAround(rubik.transform.position, worldAxis, Mathf.Sign(remaining) * step);
            done += step;
            yield return null;
        }

        foreach (var t in liveDragCubes)
        {
            SnapCubelet(t);
        }

        if (Mathf.Abs(snapped) > 1f && recordMoves)
            OnMoveComplete?.Invoke(liveDragAxis, liveDragLayer, snapped);

        hitCube = null; liveDragCubes = null; rotating = false;
    }

    public IEnumerator RotateLayer(Vector3 localAxis, float layerValue, float totalAngle)
    {
        yield return StartCoroutine(Rotate(localAxis, layerValue, totalAngle));
    }

    IEnumerator Rotate(Vector3 localAxis, float layerValue, float totalAngle)
    {
        rotating = true;
        List<Transform> layer = GetLayer(localAxis, layerValue);
        Vector3 worldCenter = rubik.transform.position;
        Vector3 worldAxis   = rubik.transform.TransformDirection(localAxis);

        float done = 0;
        while (done < Mathf.Abs(totalAngle))
        {
            float step = Mathf.Min(speed * Time.deltaTime, Mathf.Abs(totalAngle) - done);
            foreach (var t in layer)
                t.RotateAround(worldCenter, worldAxis, Mathf.Sign(totalAngle) * step);
            done += step;
            yield return null;
        }

        foreach (var t in layer)
        {
            SnapCubelet(t);
        }

        if (recordMoves) OnMoveComplete?.Invoke(localAxis, layerValue, totalAngle);
        rotating = false;
    }

    void SnapCubelet(Transform t)
    {
        Vector3 lp = rubik.transform.InverseTransformPoint(t.position);
        t.position = rubik.transform.TransformPoint(
            new Vector3(Mathf.Round(lp.x), Mathf.Round(lp.y), Mathf.Round(lp.z)));

        Vector3 lf = rubik.transform.InverseTransformDirection(t.forward).normalized;
        Vector3 lu = rubik.transform.InverseTransformDirection(t.up).normalized;

        lf = SnapToAxis(lf);
        lu = SnapToAxis(lu);

        Vector3 lr = Vector3.Cross(lu, lf);
        if (lr.sqrMagnitude < 0.1f)
        {
            lu = Mathf.Abs(lf.y) < 0.9f ? Vector3.up : Vector3.forward;
            lr = Vector3.Cross(lu, lf).normalized;
        }
        lu = Vector3.Cross(lf, lr).normalized;

        t.rotation = Quaternion.LookRotation(
            rubik.transform.TransformDirection(lf),
            rubik.transform.TransformDirection(lu)
        );
    }

    List<Transform> GetLayer(Vector3 localAxis, float value)
    {
        var result = new List<Transform>();
        foreach (Transform child in rubik.transform)
        {
            Vector3 lp = rubik.transform.InverseTransformPoint(child.position);
            if (Mathf.Abs(Vector3.Dot(lp, localAxis) - value) < 0.4f)
                result.Add(child);
        }
        return result;
    }

    void GetAxes(Vector3 normal, out Vector3 right, out Vector3 up)
    {
        normal = normal.normalized;
        if (Mathf.Abs(Vector3.Dot(normal, Vector3.up)) < 0.9f)
        {
            right = Vector3.Cross(Vector3.up, normal).normalized;
            up    = Vector3.Cross(normal, right).normalized;
        }
        else
        {
            right = Vector3.Cross(Vector3.forward, normal).normalized;
            up    = Vector3.Cross(normal, right).normalized;
        }
    }

    Vector3 SnapToAxis(Vector3 v)
    {
        float ax = Mathf.Abs(v.x), ay = Mathf.Abs(v.y), az = Mathf.Abs(v.z);
        if (ax >= ay && ax >= az) return new Vector3(Mathf.Sign(v.x), 0, 0);
        if (ay >= ax && ay >= az) return new Vector3(0, Mathf.Sign(v.y), 0);
        return new Vector3(0, 0, Mathf.Sign(v.z));
    }
}
