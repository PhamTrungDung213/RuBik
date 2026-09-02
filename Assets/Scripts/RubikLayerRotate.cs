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
        if (Input.GetKeyDown(KeyCode.M)) StartCoroutine(Rotate(Vector3.right,   0f, -90f * dir));
        if (Input.GetKeyDown(KeyCode.E)) StartCoroutine(Rotate(Vector3.up,      0f, -90f * dir));
        if (Input.GetKeyDown(KeyCode.S)) StartCoroutine(Rotate(Vector3.forward, 0f,  90f * dir));
    }

    void HandleMouse()
    {
        if (Input.GetMouseButtonDown(0) && !rotating)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform.IsChildOf(rubik.transform))
            {
                hitCube        = hit.transform;
                localHitNormal = SnapToAxis(rubik.transform.InverseTransformDirection(hit.normal));
                mouseStart     = Input.mousePosition;
                dragStarted    = false;
                liveDragAngle  = 0f;
                liveDragCubes  = null;
            }
        }

        if (hitCube != null && Input.GetMouseButton(0) && !rotating)
        {
            Vector2 drag = (Vector2)Input.mousePosition - mouseStart;
            if (!dragStarted && drag.magnitude > 10f)
            {
                SetupLiveDrag(drag);
                dragStarted = true;
            }
            if (dragStarted) ApplyLiveDrag(drag);
        }

        if (Input.GetMouseButtonUp(0) && hitCube != null)
        {
            if (dragStarted && liveDragCubes != null)
                StartCoroutine(SnapDrag());
            else
                hitCube = null;
            dragStarted = false;
        }
    }

    void SetupLiveDrag(Vector2 drag)
    {
        Vector3 A1, A2;
        GetAxes(localHitNormal, out A1, out A2);

        Camera cam = Camera.main;
        Vector3 center = rubik.transform.position;
        Vector2 scrCenter = cam.WorldToScreenPoint(center);

        Vector2 sA1 = ((Vector2)cam.WorldToScreenPoint(center + rubik.transform.TransformDirection(A1)) - scrCenter).normalized;
        Vector2 sA2 = ((Vector2)cam.WorldToScreenPoint(center + rubik.transform.TransformDirection(A2)) - scrCenter).normalized;

        float dotA1 = Vector2.Dot(drag.normalized, sA1);
        float dotA2 = Vector2.Dot(drag.normalized, sA2);
        Vector3 localCubePos = rubik.transform.InverseTransformPoint(hitCube.position);

        // --- Chon truc xoay ---
        bool isTopBottom = Mathf.Abs(localHitNormal.y) > 0.9f;
        if (isTopBottom)
        {
            // Mat top/bottom: luon xoay quanh truc Y (normal cua mat)
            liveDragAxis  = localHitNormal;
            liveDragLayer = Mathf.Round(Vector3.Dot(localCubePos, localHitNormal));
        }
        else if (Mathf.Abs(dotA1) >= Mathf.Abs(dotA2))
        {
            // Keo NGANG tren mat ben: xoay lop ngang (truc Y)
            liveDragAxis  = Vector3.up;
            liveDragLayer = Mathf.Round(localCubePos.y);
        }
        else
        {
            // Keo DOC tren mat ben: xoay cot (A1 = truc ngang trong mat)
            liveDragAxis  = A1;
            liveDragLayer = Mathf.Round(Vector3.Dot(localCubePos, A1));
        }

        // --- Tinh huong tangent (left-hand Unity convention) ---
        // Voi rotation duong quanh worldAxis, diem P di theo: Cross(P-center, worldAxis)
        // (Dao nguoc Cross so voi right-hand, vi Unity la left-hand)
        Vector3 worldRotAxis = rubik.transform.TransformDirection(liveDragAxis);
        Vector3 hitVec       = hitCube.position - center;
        Vector3 tangent      = Vector3.Cross(worldRotAxis, hitVec);

        Vector2 screenTangent = ((Vector2)cam.WorldToScreenPoint(center + tangent) - scrCenter).normalized;

        // Neu tangent gan zero (hit dung tren truc xoay), chon vec tuy chon
        if (screenTangent.sqrMagnitude < 0.001f)
        {
            Vector3 perp = (Mathf.Abs(worldRotAxis.x) < 0.9f)
                ? Vector3.Cross(Vector3.right, worldRotAxis).normalized
                : Vector3.Cross(Vector3.up, worldRotAxis).normalized;
            tangent = Vector3.Cross(worldRotAxis, perp);
            screenTangent = ((Vector2)cam.WorldToScreenPoint(center + tangent) - scrCenter).normalized;
        }

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
            Vector3 lp = rubik.transform.InverseTransformPoint(t.position);
            t.position = rubik.transform.TransformPoint(
                new Vector3(Mathf.Round(lp.x), Mathf.Round(lp.y), Mathf.Round(lp.z)));
            Vector3 le = t.localEulerAngles;
            t.localEulerAngles = new Vector3(
                Mathf.Round(le.x / 90f) * 90f,
                Mathf.Round(le.y / 90f) * 90f,
                Mathf.Round(le.z / 90f) * 90f);
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
            Vector3 lp = rubik.transform.InverseTransformPoint(t.position);
            t.position = rubik.transform.TransformPoint(
                new Vector3(Mathf.Round(lp.x), Mathf.Round(lp.y), Mathf.Round(lp.z)));
            Vector3 le = t.localEulerAngles;
            t.localEulerAngles = new Vector3(
                Mathf.Round(le.x / 90f) * 90f,
                Mathf.Round(le.y / 90f) * 90f,
                Mathf.Round(le.z / 90f) * 90f);
        }

        if (recordMoves) OnMoveComplete?.Invoke(localAxis, layerValue, totalAngle);
        rotating = false;
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

