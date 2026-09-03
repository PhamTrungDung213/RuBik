using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RubikFunction : MonoBehaviour
{
    public GameObject rubik;
    public RubikLayerRotate layerRotate;
    public Button solveButton;
    public Button shuffleButton;
    public float solveSpeed = 1000f;
    public float shuffleSpeed = 1000f;

    [Header("UI Hien Thi Buoc Giai (TMP hoac Text)")]
    public TMP_Text solutionTextTMP;
    public Text solutionTextLegacy;

    struct CubeletState { public Transform t; public Vector3 pos; public Quaternion rot; }
    List<CubeletState> initialStates = new List<CubeletState>();

    struct Move { public Vector3 axis; public float layer; public float angle; }
    List<Move> history = new List<Move>();

    void Start()
    {
        foreach (Transform child in rubik.transform)
            initialStates.Add(new CubeletState { t = child, pos = child.localPosition, rot = child.localRotation });

        layerRotate.OnMoveComplete += (axis, layer, angle) =>
            history.Add(new Move { axis = axis, layer = layer, angle = angle });

        solveButton.onClick.AddListener(OnSolve);
        shuffleButton.onClick.AddListener(OnShuffle);

        AutoFindSolutionText();
        SetSolutionText("Bấm TRỘN RUBIK hoặc GIẢI TỰ ĐỘNG!");

        // Khoi tao truoc bang Kociemba trong luong ngam (background task)
        Task.Run(() =>
        {
            try
            {
                Kociemba.KociembaHelper.Init();
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("Kociemba background init: " + ex.Message);
            }
        });
    }

    void SetSolutionText(string msg)
    {
        if (solutionTextTMP != null) solutionTextTMP.text = msg;
        if (solutionTextLegacy != null) solutionTextLegacy.text = msg;
    }

    void AutoFindSolutionText()
    {
        if (solutionTextTMP != null || solutionTextLegacy != null) return;

        // Tu dong tim kiem text phu hop trong Canvas neu chua keo tha thu cong
        var allTmp = FindObjectsByType<TMP_Text>(FindObjectsInactive.Include);
        foreach (var t in allTmp)
        {
            string n = t.name.ToLower();
            if (n.Contains("solution") || n.Contains("buoc") || n.Contains("formula") || n.Contains("step"))
            {
                solutionTextTMP = t;
                return;
            }
        }

        var allLegacy = FindObjectsByType<Text>(FindObjectsInactive.Include);
        foreach (var t in allLegacy)
        {
            string n = t.name.ToLower();
            if (n.Contains("solution") || n.Contains("buoc") || n.Contains("formula") || n.Contains("step"))
            {
                solutionTextLegacy = t;
                return;
            }
        }
    }

    void OnShuffle()
    {
        if (layerRotate.rotating) return;
        StartCoroutine(ShuffleRoutine());
    }

    void OnSolve()
    {
        if (layerRotate.rotating) return;
        StartCoroutine(SolveRoutine());
    }

    IEnumerator ShuffleRoutine()
    {
        history.Clear();
        SetSolutionText("Đang xáo trộn Rubik...");
        yield return null;

        Vector3[] axes = { Vector3.up, Vector3.right, Vector3.forward };
        float[] outerLayers = { -1f, 1f };
        float[] angles = { 90f, -90f };

        Vector3 lastAxis = Vector3.zero;

        float savedSpeed = layerRotate.speed;
        layerRotate.speed = shuffleSpeed;

        for (int i = 0; i < 20; i++)
        {
            Vector3 axis;
            do { axis = axes[Random.Range(0, axes.Length)]; }
            while (axis == lastAxis);
            lastAxis = axis;

            float layer = outerLayers[Random.Range(0, outerLayers.Length)];
            float angle = angles[Random.Range(0, angles.Length)];

            yield return StartCoroutine(layerRotate.RotateLayer(axis, layer, angle));
        }

        layerRotate.speed = savedSpeed;
        SetSolutionText("Đã xáo trộn xong. Hãy thử giải hoặc bấm GIẢI TỰ ĐỘNG!");
    }

    IEnumerator SolveRoutine()
    {
        SetSolutionText("Đang quét trạng thái 54 ô màu...");
        yield return null;

        // 1. Quet trang thai 54 o mau thuc te tren khoi 3D
        string facelets = GetCubeStateString();
        Debug.Log("Current cube facelets: " + facelets);

        // 2. Kiem tra xem Rubik da o trang thai hoan thanh chua
        if (IsSolved(facelets))
        {
            SetSolutionText("Rubik đã ở trạng thái hoàn thành!");
            Debug.Log("Rubik is already solved!");
            yield break;
        }

        SetSolutionText("Đang tính toán công thức Kociemba...");
        yield return null;

        // 3. Chay thuat toan Kociemba Two-Phase
        string solution = Kociemba.KociembaHelper.Solve(facelets);
        Debug.Log("Kociemba solution: " + solution);

        if (string.IsNullOrEmpty(solution) || solution.StartsWith("Error"))
        {
            SetSolutionText("Lỗi tìm bước giải: " + solution);
            Debug.LogWarning("Kociemba returned error (" + solution + "). Fallback to history undo if available.");

            if (history.Count > 0)
            {
                SetSolutionText("Đang hoàn tác theo lịch sử...");
                float fallbackSpeed = layerRotate.speed;
                layerRotate.speed = solveSpeed;
                layerRotate.recordMoves = false;

                for (int i = history.Count - 1; i >= 0; i--)
                {
                    Move m = history[i];
                    yield return StartCoroutine(layerRotate.RotateLayer(m.axis, m.layer, -m.angle));
                }

                layerRotate.speed = fallbackSpeed;
                layerRotate.recordMoves = true;
                history.Clear();
                SetSolutionText("Đã giải xong theo lịch sử!");
            }
            yield break;
        }

        // 4. Thuc thi cac buoc giai theo cong thuc Kociemba
        float savedSpeed = layerRotate.speed;
        layerRotate.speed = solveSpeed;
        layerRotate.recordMoves = false;

        string[] moves = solution.Trim().Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
        int totalSteps = moves.Length;

        for (int i = 0; i < totalSteps; i++)
        {
            string currentMove = moves[i];
            SetSolutionText($"Bước {i + 1}/{totalSteps}: [{currentMove}]\nCông thức: {solution}");
            yield return StartCoroutine(ExecuteMove(currentMove));
        }

        layerRotate.speed = savedSpeed;
        layerRotate.recordMoves = true;
        history.Clear();

        SetSolutionText($"Đã giải xong ({totalSteps} bước)!\n{solution}");
    }

    IEnumerator ExecuteMove(string move)
    {
        if (string.IsNullOrEmpty(move)) yield break;

        char face = move[0];
        int modifier = 1; // 1: 90 deg, 2: 180 deg, -1: -90 deg (prime)
        if (move.Length > 1)
        {
            if (move[1] == '2') modifier = 2;
            else if (move[1] == '\'') modifier = -1;
        }

        Vector3 axis = Vector3.zero;
        float layer = 0f;
        float baseAngle = 0f;

        switch (face)
        {
            case 'U':
                axis = Vector3.up;
                layer = 1f;
                baseAngle = 90f;
                break;
            case 'D':
                axis = Vector3.up;
                layer = -1f;
                baseAngle = -90f;
                break;
            case 'R':
                axis = Vector3.right;
                layer = 1f;
                baseAngle = 90f;
                break;
            case 'L':
                axis = Vector3.right;
                layer = -1f;
                baseAngle = -90f;
                break;
            case 'F':
                axis = Vector3.forward;
                layer = -1f;
                baseAngle = -90f;
                break;
            case 'B':
                axis = Vector3.forward;
                layer = 1f;
                baseAngle = 90f;
                break;
        }

        float totalAngle = baseAngle * modifier;
        yield return StartCoroutine(layerRotate.RotateLayer(axis, layer, totalAngle));
    }

    bool IsSolved(string facelets)
    {
        for (int i = 0; i < 6; i++)
        {
            char center = facelets[i * 9 + 4];
            for (int j = 0; j < 9; j++)
            {
                if (facelets[i * 9 + j] != center) return false;
            }
        }
        return true;
    }

    string GetCubeStateString()
    {
        // Quet 6 mat theo dung thu tu Kociemba: U, R, F, D, L, B
        Color[] faceU = ScanFaceColors(Vector3.up,       Vector3.right,    Vector3.forward);
        Color[] faceR = ScanFaceColors(Vector3.right,    Vector3.forward,  Vector3.up);
        Color[] faceF = ScanFaceColors(-Vector3.forward, Vector3.right,    Vector3.up);
        Color[] faceD = ScanFaceColors(Vector3.down,     Vector3.right,   -Vector3.forward);
        Color[] faceL = ScanFaceColors(-Vector3.right,  -Vector3.forward,  Vector3.up);
        Color[] faceB = ScanFaceColors(Vector3.forward, -Vector3.right,    Vector3.up);

        // Mau cua 6 vien tam (index 4 cua moi mat)
        Color centerU = faceU[4];
        Color centerR = faceR[4];
        Color centerF = faceF[4];
        Color centerD = faceD[4];
        Color centerL = faceL[4];
        Color centerB = faceB[4];

        Color[] centers = { centerU, centerR, centerF, centerD, centerL, centerB };
        char[] centerChars = { 'U', 'R', 'F', 'D', 'L', 'B' };

        StringBuilder sb = new StringBuilder(54);
        Color[][] allFaces = { faceU, faceR, faceF, faceD, faceL, faceB };

        foreach (var face in allFaces)
        {
            for (int i = 0; i < 9; i++)
            {
                sb.Append(FindClosestCenter(face[i], centers, centerChars));
            }
        }

        return sb.ToString();
    }

    char FindClosestCenter(Color c, Color[] centers, char[] chars)
    {
        float bestDist = float.MaxValue;
        char bestChar = 'U';

        for (int i = 0; i < 6; i++)
        {
            Color t = centers[i];
            float dist = (c.r - t.r) * (c.r - t.r)
                       + (c.g - t.g) * (c.g - t.g)
                       + (c.b - t.b) * (c.b - t.b);
            if (dist < bestDist)
            {
                bestDist = dist;
                bestChar = chars[i];
            }
        }
        return bestChar;
    }

    Color[] ScanFaceColors(Vector3 localNormal, Vector3 localRight, Vector3 localUp)
    {
        float sp = 1f;
        Vector3 worldNormal = rubik.transform.TransformDirection(localNormal);
        Color[] colors = new Color[9];

        for (int row = 1; row >= -1; row--)
        {
            for (int col = -1; col <= 1; col++)
            {
                int i = (1 - row) * 3 + (col + 1);

                Vector3 localOrigin = localNormal * (sp * 2f)
                                    + localRight   * (col * sp)
                                    + localUp      * (row * sp);
                Vector3 worldOrigin = rubik.transform.TransformPoint(localOrigin);

                Ray ray = new Ray(worldOrigin, -worldNormal);
                float rayDist = sp * 4f * rubik.transform.lossyScale.x;

                if (Physics.Raycast(ray, out RaycastHit hit, rayDist))
                {
                    colors[i] = ReadColor(hit, worldNormal);
                }
                else
                {
                    colors[i] = Color.grey;
                }
            }
        }
        return colors;
    }

    Color ReadColor(RaycastHit hit, Vector3 worldNormal)
    {
        Transform cube = hit.collider.transform;
        while (cube != null && cube.parent != null && cube.parent != rubik.transform)
        {
            cube = cube.parent;
        }
        if (cube == null || cube.parent == null) cube = hit.collider.transform;

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

        Renderer rend = hit.collider.GetComponent<Renderer>();
        if (rend != null && rend.sharedMaterial != null)
            return rend.sharedMaterial.color;

        return Color.grey;
    }

    void InstantReset()
    {
        foreach (var st in initialStates)
        {
            st.t.localPosition = st.pos;
            st.t.localRotation = st.rot;
        }
        SetSolutionText("Sẵn sàng.");
    }
}
