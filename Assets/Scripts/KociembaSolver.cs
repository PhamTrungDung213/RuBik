using System;
using System.Text;

namespace Kociemba
{
    public static class ArrayHelper
    {
        public static T[][] CreateJagged<T>(int d1, int d2)
        {
            T[][] arr = new T[d1][];
            for (int i = 0; i < d1; i++) arr[i] = new T[d2];
            return arr;
        }

        public static bool ArrayEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length) return false;
            for (int i = 0; i < a.Length; i++) if (a[i] != b[i]) return false;
            return true;
        }
    }

// ========================================================


public class Util {
    //Moves
    public static byte Ux1 = 0;
    public static byte Ux2 = 1;
    public static byte Ux3 = 2;
    public static byte Rx1 = 3;
    public static byte Rx2 = 4;
    public static byte Rx3 = 5;
    public static byte Fx1 = 6;
    public static byte Fx2 = 7;
    public static byte Fx3 = 8;
    public static byte Dx1 = 9;
    public static byte Dx2 = 10;
    public static byte Dx3 = 11;
    public static byte Lx1 = 12;
    public static byte Lx2 = 13;
    public static byte Lx3 = 14;
    public static byte Bx1 = 15;
    public static byte Bx2 = 16;
    public static byte Bx3 = 17;

    //Facelets
    public static byte U1 = 0;
    public static byte U2 = 1;
    public static byte U3 = 2;
    public static byte U4 = 3;
    public static byte U5 = 4;
    public static byte U6 = 5;
    public static byte U7 = 6;
    public static byte U8 = 7;
    public static byte U9 = 8;
    public static byte R1 = 9;
    public static byte R2 = 10;
    public static byte R3 = 11;
    public static byte R4 = 12;
    public static byte R5 = 13;
    public static byte R6 = 14;
    public static byte R7 = 15;
    public static byte R8 = 16;
    public static byte R9 = 17;
    public static byte F1 = 18;
    public static byte F2 = 19;
    public static byte F3 = 20;
    public static byte F4 = 21;
    public static byte F5 = 22;
    public static byte F6 = 23;
    public static byte F7 = 24;
    public static byte F8 = 25;
    public static byte F9 = 26;
    public static byte D1 = 27;
    public static byte D2 = 28;
    public static byte D3 = 29;
    public static byte D4 = 30;
    public static byte D5 = 31;
    public static byte D6 = 32;
    public static byte D7 = 33;
    public static byte D8 = 34;
    public static byte D9 = 35;
    public static byte L1 = 36;
    public static byte L2 = 37;
    public static byte L3 = 38;
    public static byte L4 = 39;
    public static byte L5 = 40;
    public static byte L6 = 41;
    public static byte L7 = 42;
    public static byte L8 = 43;
    public static byte L9 = 44;
    public static byte B1 = 45;
    public static byte B2 = 46;
    public static byte B3 = 47;
    public static byte B4 = 48;
    public static byte B5 = 49;
    public static byte B6 = 50;
    public static byte B7 = 51;
    public static byte B8 = 52;
    public static byte B9 = 53;

    //Colors
    public static byte U = 0;
    public static byte R = 1;
    public static byte F = 2;
    public static byte D = 3;
    public static byte L = 4;
    public static byte B = 5;

    public static byte[][] cornerFacelet = new byte[][] {
        new byte[] { U9, R1, F3 }, new byte[] { U7, F1, L3 }, new byte[] { U1, L1, B3 }, new byte[] { U3, B1, R3 },
        new byte[] { D3, F9, R7 }, new byte[] { D1, L9, F7 }, new byte[] { D7, B9, L7 }, new byte[] { D9, R9, B7 }
    };
    public static byte[][] edgeFacelet = new byte[][] {
        new byte[] { U6, R2 }, new byte[] { U8, F2 }, new byte[] { U4, L2 }, new byte[] { U2, B2 }, new byte[] { D6, R8 }, new byte[] { D2, F8 },
        new byte[] { D4, L8 }, new byte[] { D8, B8 }, new byte[] { F6, R4 }, new byte[] { F4, L6 }, new byte[] { B6, L4 }, new byte[] { B4, R6 }
    };

    public static int[][] Cnk = ArrayHelper.CreateJagged<int>(13, 13);
    public static string[] move2str = {
        "U ", "U2", "U'", "R ", "R2", "R'", "F ", "F2", "F'",
        "D ", "D2", "D'", "L ", "L2", "L'", "B ", "B2", "B'"
    };
    public static int[] ud2std = {Ux1, Ux2, Ux3, Rx2, Fx2, Dx1, Dx2, Dx3, Lx2, Bx2, Rx1, Rx3, Fx1, Fx3, Lx1, Lx3, Bx1, Bx3};
    public static int[] std2ud = new int[18];
    public static int[] ckmv2bit = new int[11];

    public class Solution {
        public int length = 0;
        public int depth1 = 0;
        public int verbose = 0;
        public int urfIdx = 0;
        public int[] moves = new int[31];

        public Solution() {}

        public void setArgs(int verbose, int urfIdx, int depth1) {
            this.verbose = verbose;
            this.urfIdx = urfIdx;
            this.depth1 = depth1;
        }

        public void appendSolMove(int curMove) {
            if (length == 0) {
                moves[length++] = curMove;
                return;
            }
            int axisCur = curMove / 3;
            int axisLast = moves[length - 1] / 3;
            if (axisCur == axisLast) {
                int pow = (curMove % 3 + moves[length - 1] % 3 + 1) % 4;
                if (pow == 3) {
                    length--;
                } else {
                    moves[length - 1] = axisCur * 3 + pow;
                }
                return;
            }
            if (length > 1
                    && axisCur % 3 == axisLast % 3
                    && axisCur == moves[length - 2] / 3) {
                int pow = (curMove % 3 + moves[length - 2] % 3 + 1) % 4;
                if (pow == 3) {
                    moves[length - 2] = moves[length - 1];
                    length--;
                } else {
                    moves[length - 2] = axisCur * 3 + pow;
                }
                return;
            }
            moves[length++] = curMove;
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            int urf = (verbose & Search.INVERSE_SOLUTION) != 0 ? (urfIdx + 3) % 6 : urfIdx;
            if (urf < 3) {
                for (int s = 0; s < length; s++) {
                    if ((verbose & Search.USE_SEPARATOR) != 0 && s == depth1) {
                        sb.Append(".  ");
                    }
                    sb.Append(move2str[CubieCube.urfMove[urf][moves[s]]]).Append(' ');
                }
            } else {
                for (int s = length - 1; s >= 0; s--) {
                    sb.Append(move2str[CubieCube.urfMove[urf][moves[s]]]).Append(' ');
                    if ((verbose & Search.USE_SEPARATOR) != 0 && s == depth1) {
                        sb.Append(".  ");
                    }
                }
            }
            if ((verbose & Search.APPEND_LENGTH) != 0) {
                sb.Append("(").Append(length).Append("f)");
            }
            return sb.ToString();
        }
    }

    public static void toCubieCube(byte[] f, CubieCube ccRet) {
        byte ori;
        for (int i = 0; i < 8; i++) {
            ccRet.ca[i] = 0;
        }
        for (int i = 0; i < 12; i++) {
            ccRet.ea[i] = 0;
        }
        byte col1, col2;
        for (byte i = 0; i < 8; i++) {
            for (ori = 0; ori < 3; ori++){
                if (f[cornerFacelet[i][ori]] == U || f[cornerFacelet[i][ori]] == D)
                    break;
            }
            col1 = f[cornerFacelet[i][(ori + 1) % 3]];
            col2 = f[cornerFacelet[i][(ori + 2) % 3]];
            for (byte j = 0; j < 8; j++) {
                if (col1 == cornerFacelet[j][1] / 9 && col2 == cornerFacelet[j][2] / 9) {
                    ccRet.ca[i] = (byte) (ori % 3 << 3 | j);
                    break;
                }
            }
        }
        for (byte i = 0; i < 12; i++) {
            for (byte j = 0; j < 12; j++) {
                if (f[edgeFacelet[i][0]] == edgeFacelet[j][0] / 9
                        && f[edgeFacelet[i][1]] == edgeFacelet[j][1] / 9) {
                    ccRet.ea[i] = (byte) (j << 1);
                    break;
                }
                if (f[edgeFacelet[i][0]] == edgeFacelet[j][1] / 9
                        && f[edgeFacelet[i][1]] == edgeFacelet[j][0] / 9) {
                    ccRet.ea[i] = (byte) (j << 1 | 1);
                    break;
                }
            }
        }
    }

    public static string toFaceCube(CubieCube cc) {
        char[] f = new char[54];
        char[] ts = new char[] {'U', 'R', 'F', 'D', 'L', 'B'};
        for (int i = 0; i < 54; i++) {
            f[i] = ts[i / 9];
        }
        for (byte c = 0; c < 8; c++) {
            int j = cc.ca[c] & 0x7;
            int ori = cc.ca[c] >> 3;
            for (byte n = 0; n < 3; n++) {
                f[cornerFacelet[c][(n + ori) % 3]] = ts[cornerFacelet[j][n] / 9];
            }
        }
        for (byte e = 0; e < 12; e++) {
            int j = cc.ea[e] >> 1;
            int ori = cc.ea[e] & 1;
            for (byte n = 0; n < 2; n++) {
                f[edgeFacelet[e][(n + ori) % 2]] = ts[edgeFacelet[j][n] / 9];
            }
        }
        return new string(f);
    }

    public static int getNParity(int idx, int n) {
        int p = 0;
        for (int i = n - 2; i >= 0; i--) {
            p ^= idx % (n - i);
            idx /= (n - i);
        }
        return p & 1;
    }

    public static byte setVal(int val0, int val, bool isEdge) {
        return (byte) (isEdge ? (val << 1 | val0 & 1) : (val | val0 & ~7));
    }

    public static int getVal(int val0, bool isEdge) {
        return isEdge ? val0 >> 1 : val0 & 7;
    }

    public static void setNPerm(byte[] arr, int idx, int n, bool isEdge) {
        long val = unchecked((long)0xFEDCBA9876543210L);
        long extract = 0;
        for (int p = 2; p <= n; p++) {
            extract = extract << 4 | idx % p;
            idx /= p;
        }
        for (int i = 0; i < n - 1; i++) {
            int v = ((int) extract & 0xf) << 2;
            extract >>= 4;
            arr[i] = setVal(arr[i], (int) (val >> v & 0xf), isEdge);
            long m = (1L << v) - 1;
            val = val & m | val >> 4 & ~m;
        }
        arr[n - 1] = setVal(arr[n - 1], (int) (val & 0xf), isEdge);
    }

    public static int getNPerm(byte[] arr, int n, bool isEdge) {
        int idx = 0;
        long val = unchecked((long)0xFEDCBA9876543210L);
        for (int i = 0; i < n - 1; i++) {
            int v = getVal(arr[i], isEdge) << 2;
            idx = (n - i) * idx + (int) (val >> v & 0xf);
            val -= unchecked((long)0x1111111111111110L) << v;
        }
        return idx;
    }

    public static int getComb(byte[] arr, int mask, bool isEdge) {
        int end = arr.Length - 1;
        int idxC = 0, r = 4;
        for (int i = end; i >= 0; i--) {
            int perm = getVal(arr[i], isEdge);
            if ((perm & 0xc) == mask) {
                idxC += Cnk[i][r--];
            }
        }
        return idxC;
    }

    public static void setComb(byte[] arr, int idxC, int mask, bool isEdge) {
        int end = arr.Length - 1;
        int r = 4, fill = end;
        for (int i = end; i >= 0; i--) {
            if (idxC >= Cnk[i][r]) {
                idxC -= Cnk[i][r--];
                arr[i] = setVal(arr[i], r | mask, isEdge);
            } else {
                if ((fill & 0xc) == mask) {
                    fill -= 4;
                }
                arr[i] = setVal(arr[i], fill--, isEdge);
            }
        }
    }

    static Util() {
        for (int i = 0; i < 18; i++) {
            std2ud[ud2std[i]] = i;
        }
        for (int i = 0; i < 10; i++) {
            int ix = ud2std[i] / 3;
            ckmv2bit[i] = 0;
            for (int j = 0; j < 10; j++) {
                int jx = ud2std[j] / 3;
                ckmv2bit[i] |= ((ix == jx) || ((ix % 3 == jx % 3) && (ix >= jx)) ? 1 : 0) << j;
            }
        }
        ckmv2bit[10] = 0;
        for (int i = 0; i < 13; i++) {
            Cnk[i][0] = Cnk[i][i] = 1;
            for (int j = 1; j < i; j++) {
                Cnk[i][j] = Cnk[i - 1][j - 1] + Cnk[i - 1][j];
            }
        }
    }
}

// ========================================================




public class CubieCube {

    /**
     * 16 symmetries generated by S_F2, S_U4 and S_LR2
     */
    public static CubieCube[] CubeSym = new CubieCube[16];

    /**
     * 18 move cubes
     */
    public static CubieCube[] moveCube = new CubieCube[18];

    public static long[] moveCubeSym = new long[18];
    public static int[] firstMoveSym = new int[48];

    public static int[][] SymMult = ArrayHelper.CreateJagged<int>(16, 16);
    public static int[][] SymMultInv = ArrayHelper.CreateJagged<int>(16, 16);
    public static int[][] SymMove = ArrayHelper.CreateJagged<int>(16, 18);
    public static int[] Sym8Move = new int[8 * 18];
    public static int[][] SymMoveUD = ArrayHelper.CreateJagged<int>(16, 18);

    /**
     * ClassIndexToRepresentantArrays
     */
    public static char[] FlipS2R = new char[CoordCube.N_FLIP_SYM];
    public static char[] TwistS2R = new char[CoordCube.N_TWIST_SYM];
    public static char[] EPermS2R = new char[CoordCube.N_PERM_SYM];
    public static byte[] Perm2CombP = new byte[CoordCube.N_PERM_SYM];
    public static char[] PermInvEdgeSym = new char[CoordCube.N_PERM_SYM];
    public static byte[] MPermInv = new byte[CoordCube.N_MPERM];

    /**
     * Notice that Edge Perm Coordnate and Corner Perm Coordnate are the same symmetry structure.
     * So their ClassIndexToRepresentantArray are the same.
     * And when x is RawEdgePermCoordnate, y*16+k is SymEdgePermCoordnate, y*16+(k^e2c[k]) will
     * be the SymCornerPermCoordnate of the State whose RawCornerPermCoordnate is x.
     */
    // static byte[] e2c = {0, 0, 0, 0, 1, 3, 1, 3, 1, 3, 1, 3, 0, 0, 0, 0};
    public static int SYM_E2C_MAGIC = 0x00DDDD00;
    public static int ESym2CSym(int idx) {
        return idx ^ (SYM_E2C_MAGIC >> ((idx & 0xf) << 1) & 3);
    }

    /**
     * Raw-Coordnate to Sym-Coordnate, only for speeding up initializaion.
     */
    public static char[] FlipR2S = new char[CoordCube.N_FLIP];
    public static char[] TwistR2S = new char[CoordCube.N_TWIST];
    public static char[] EPermR2S = new char[CoordCube.N_PERM];
    public static char[] FlipS2RF = Search.USE_TWIST_FLIP_PRUN ? new char[CoordCube.N_FLIP_SYM * 8] : null;

    /**
     *
     */
    public static char[] SymStateTwist;// = new char[CoordCube.N_TWIST_SYM];
    public static char[] SymStateFlip;// = new char[CoordCube.N_FLIP_SYM];
    public static char[] SymStatePerm;// = new char[CoordCube.N_PERM_SYM];

    public static CubieCube urf1 = new CubieCube(2531, 1373, 67026819, 1367);
    public static CubieCube urf2 = new CubieCube(2089, 1906, 322752913, 2040);
    public static byte[][] urfMove = new byte[][] {
        new byte[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17},
        new byte[] {6, 7, 8, 0, 1, 2, 3, 4, 5, 15, 16, 17, 9, 10, 11, 12, 13, 14},
        new byte[] {3, 4, 5, 6, 7, 8, 0, 1, 2, 12, 13, 14, 15, 16, 17, 9, 10, 11},
        new byte[] {2, 1, 0, 5, 4, 3, 8, 7, 6, 11, 10, 9, 14, 13, 12, 17, 16, 15},
        new byte[] {8, 7, 6, 2, 1, 0, 5, 4, 3, 17, 16, 15, 11, 10, 9, 14, 13, 12},
        new byte[] {5, 4, 3, 8, 7, 6, 2, 1, 0, 14, 13, 12, 17, 16, 15, 11, 10, 9}
    };

    public byte[] ca = new byte[] {0, 1, 2, 3, 4, 5, 6, 7};
    public byte[] ea = new byte[] {0, 2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22};
    public CubieCube temps = null;

    public CubieCube() {
    }

    public CubieCube(int cperm, int twist, int eperm, int flip) {
        this.setCPerm(cperm);
        this.setTwist(twist);
        Util.setNPerm(ea, eperm, 12, true);
        this.setFlip(flip);
    }

    public CubieCube(CubieCube c) {
        copy(c);
    }

    public void copy(CubieCube c) {
        for (int i = 0; i < 8; i++) {
            this.ca[i] = c.ca[i];
        }
        for (int i = 0; i < 12; i++) {
            this.ea[i] = c.ea[i];
        }
    }

    public void invCubieCube() {
        if (temps == null) {
            temps = new CubieCube();
        }
        for (byte edge = 0; edge < 12; edge++) {
            temps.ea[ea[edge] >> 1] = (byte) (edge << 1 | ea[edge] & 1);
        }
        for (byte corn = 0; corn < 8; corn++) {
            temps.ca[ca[corn] & 0x7] = (byte) (corn | 0x20 >> (ca[corn] >> 3) & 0x18);
        }
        copy(temps);
    }

    /**
     * prod = a * b, Corner Only.
     */
    public static void CornMult(CubieCube a, CubieCube b, CubieCube prod) {
        for (int corn = 0; corn < 8; corn++) {
            int oriA = a.ca[b.ca[corn] & 7] >> 3;
            int oriB = b.ca[corn] >> 3;
            prod.ca[corn] = (byte) (a.ca[b.ca[corn] & 7] & 7 | (oriA + oriB) % 3 << 3);
        }
    }

    /**
     * prod = a * b, Corner Only. With mirrored cases considered
     */
    public static void CornMultFull(CubieCube a, CubieCube b, CubieCube prod) {
        for (int corn = 0; corn < 8; corn++) {
            int oriA = a.ca[b.ca[corn] & 7] >> 3;
            int oriB = b.ca[corn] >> 3;
            int ori = oriA + ((oriA < 3) ? oriB : 6 - oriB);
            ori = ori % 3 + ((oriA < 3) == (oriB < 3) ? 0 : 3);
            prod.ca[corn] = (byte) (a.ca[b.ca[corn] & 7] & 7 | ori << 3);
        }
    }

    /**
     * prod = a * b, Edge Only.
     */
    public static void EdgeMult(CubieCube a, CubieCube b, CubieCube prod) {
        for (int ed = 0; ed < 12; ed++) {
            prod.ea[ed] = (byte) (a.ea[b.ea[ed] >> 1] ^ (b.ea[ed] & 1));
        }
    }

    /**
     * b = S_idx^-1 * a * S_idx, Corner Only.
     */
    public static void CornConjugate(CubieCube a, int idx, CubieCube b) {
        CubieCube sinv = CubeSym[SymMultInv[0][idx]];
        CubieCube s = CubeSym[idx];
        for (int corn = 0; corn < 8; corn++) {
            int oriA = sinv.ca[a.ca[s.ca[corn] & 7] & 7] >> 3;
            int oriB = a.ca[s.ca[corn] & 7] >> 3;
            int ori = (oriA < 3) ? oriB : (3 - oriB) % 3;
            b.ca[corn] = (byte) (sinv.ca[a.ca[s.ca[corn] & 7] & 7] & 7 | ori << 3);
        }
    }

    /**
     * b = S_idx^-1 * a * S_idx, Edge Only.
     */
    public static void EdgeConjugate(CubieCube a, int idx, CubieCube b) {
        CubieCube sinv = CubeSym[SymMultInv[0][idx]];
        CubieCube s = CubeSym[idx];
        for (int ed = 0; ed < 12; ed++) {
            b.ea[ed] = (byte) (sinv.ea[a.ea[s.ea[ed] >> 1] >> 1] ^ (a.ea[s.ea[ed] >> 1] & 1) ^ (s.ea[ed] & 1));
        }
    }

    public static int getPermSymInv(int idx, int sym, bool isCorner) {
        int idxi = PermInvEdgeSym[idx];
        if (isCorner) {
            idxi = ESym2CSym(idxi);
        }
        return idxi & 0xfff0 | SymMult[idxi & 0xf][sym];
    }

    public static int getSkipMoves(long ssym) {
        int ret = 0;
        for (int i = 1; (ssym >>= 1) != 0; i++) {
            if ((ssym & 1) == 1) {
                ret |= firstMoveSym[i];
            }
        }
        return ret;
    }

    /**
     * this = S_urf^-1 * this * S_urf.
     */
    public void URFConjugate() {
        if (temps == null) {
            temps = new CubieCube();
        }
        CornMult(urf2, this, temps);
        CornMult(temps, urf1, this);
        EdgeMult(urf2, this, temps);
        EdgeMult(temps, urf1, this);
    }

    // ********************************************* Get and set coordinates *********************************************
    // XSym : Symmetry Coordnate of X. MUST be called after initialization of ClassIndexToRepresentantArrays.

    // ++++++++++++++++++++ Phase 1 Coordnates ++++++++++++++++++++
    // Flip : Orientation of 12 Edges. Raw[0, 2048) Sym[0, 336 * 8)
    // Twist : Orientation of 8 Corners. Raw[0, 2187) Sym[0, 324 * 8)
    // UDSlice : Positions of the 4 UDSlice edges, the order is ignored. [0, 495)

    public int getFlip() {
        int idx = 0;
        for (int i = 0; i < 11; i++) {
            idx = idx << 1 | ea[i] & 1;
        }
        return idx;
    }

    public void setFlip(int idx) {
        int parity = 0, val;
        for (int i = 10; i >= 0; i--, idx >>= 1) {
            parity ^= (val = idx & 1);
            ea[i] = (byte) (ea[i] & ~1 | val);
        }
        ea[11] = (byte) (ea[11] & ~1 | parity);
    }

    public int getFlipSym() {
        return FlipR2S[getFlip()];
    }

    public int getTwist() {
        int idx = 0;
        for (int i = 0; i < 7; i++) {
            idx += (idx << 1) + (ca[i] >> 3);
        }
        return idx;
    }

    public void setTwist(int idx) {
        int twst = 15, val;
        for (int i = 6; i >= 0; i--, idx /= 3) {
            twst -= (val = idx % 3);
            ca[i] = (byte) (ca[i] & 0x7 | val << 3);
        }
        ca[7] = (byte) (ca[7] & 0x7 | (twst % 3) << 3);
    }

    public int getTwistSym() {
        return TwistR2S[getTwist()];
    }

    public int getUDSlice() {
        return 494 - Util.getComb(ea, 8, true);
    }

    public void setUDSlice(int idx) {
        Util.setComb(ea, 494 - idx, 8, true);
    }

    // ++++++++++++++++++++ Phase 2 Coordnates ++++++++++++++++++++
    // EPerm : Permutations of 8 UD Edges. Raw[0, 40320) Sym[0, 2187 * 16)
    // Cperm : Permutations of 8 Corners. Raw[0, 40320) Sym[0, 2187 * 16)
    // MPerm : Permutations of 4 UDSlice Edges. [0, 24)

    public int getCPerm() {
        return Util.getNPerm(ca, 8, false);
    }

    public void setCPerm(int idx) {
        Util.setNPerm(ca, idx, 8, false);
    }

    public int getCPermSym() {
        return ESym2CSym(EPermR2S[getCPerm()]);
    }

    public int getEPerm() {
        return Util.getNPerm(ea, 8, true);
    }

    public void setEPerm(int idx) {
        Util.setNPerm(ea, idx, 8, true);
    }

    public int getEPermSym() {
        return EPermR2S[getEPerm()];
    }

    public int getMPerm() {
        return Util.getNPerm(ea, 12, true) % 24;
    }

    public void setMPerm(int idx) {
        Util.setNPerm(ea, idx, 12, true);
    }

    public int getCComb() {
        return Util.getComb(ca, 0, false);
    }

    public void setCComb(int idx) {
        Util.setComb(ca, idx, 0, false);
    }

    /**
     * Check a cubiecube for solvability. Return the error code.
     * 0: Cube is solvable
     * -2: Not all 12 edges exist exactly once
     * -3: Flip error: One edge has to be flipped
     * -4: Not all corners exist exactly once
     * -5: Twist error: One corner has to be twisted
     * -6: Parity error: Two corners or two edges have to be exchanged
     */
    public int verify() {
        int sum = 0;
        int edgeMask = 0;
        for (int e = 0; e < 12; e++) {
            edgeMask |= 1 << (ea[e] >> 1);
            sum ^= ea[e] & 1;
        }
        if (edgeMask != 0xfff) {
            return -2;// missing edges
        }
        if (sum != 0) {
            return -3;
        }
        int cornMask = 0;
        sum = 0;
        for (int c = 0; c < 8; c++) {
            cornMask |= 1 << (ca[c] & 7);
            sum += ca[c] >> 3;
        }
        if (cornMask != 0xff) {
            return -4;// missing corners
        }
        if (sum % 3 != 0) {
            return -5;// twisted corner
        }
        if ((Util.getNParity(Util.getNPerm(ea, 12, true), 12) ^ Util.getNParity(getCPerm(), 8)) != 0) {
            return -6;// parity error
        }
        return 0;// cube ok
    }

    public long selfSymmetry() {
        CubieCube c = new CubieCube(this);
        CubieCube d = new CubieCube();
        int cperm = c.getCPermSym() >> 4;
        long sym = 0L;
        for (int urfInv = 0; urfInv < 6; urfInv++) {
            int cpermx = c.getCPermSym() >> 4;
            if (cperm == cpermx) {
                for (int i = 0; i < 16; i++) {
                    CornConjugate(c, SymMultInv[0][i], d);
                    if (ArrayHelper.ArrayEquals(d.ca, ca)) {
                        EdgeConjugate(c, SymMultInv[0][i], d);
                        if (ArrayHelper.ArrayEquals(d.ea, ea)) {
                            sym |= 1L << Math.Min(urfInv << 4 | i, 48);
                        }
                    }
                }
            }
            c.URFConjugate();
            if (urfInv % 3 == 2) {
                c.invCubieCube();
            }
        }
        return sym;
    }

    // ********************************************* Initialization functions *********************************************

    public static void initMove() {
        moveCube[0] = new CubieCube(15120, 0, 119750400, 0);
        moveCube[3] = new CubieCube(21021, 1494, 323403417, 0);
        moveCube[6] = new CubieCube(8064, 1236, 29441808, 550);
        moveCube[9] = new CubieCube(9, 0, 5880, 0);
        moveCube[12] = new CubieCube(1230, 412, 2949660, 0);
        moveCube[15] = new CubieCube(224, 137, 328552, 137);
        for (int a = 0; a < 18; a += 3) {
            for (int p = 0; p < 2; p++) {
                moveCube[a + p + 1] = new CubieCube();
                EdgeMult(moveCube[a + p], moveCube[a], moveCube[a + p + 1]);
                CornMult(moveCube[a + p], moveCube[a], moveCube[a + p + 1]);
            }
        }
    }

    public override string ToString() {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < 8; i++) {
            sb.Append("|" + (ca[i] & 7) + " " + (ca[i] >> 3));
        }
        sb.Append("\n");
        for (int i = 0; i < 12; i++) {
            sb.Append("|" + (ea[i] >> 1) + " " + (ea[i] & 1));
        }
        return sb.ToString();
    }

    public static void initSym() {
        CubieCube c = new CubieCube();
        CubieCube d = new CubieCube();
        CubieCube t;

        CubieCube f2 = new CubieCube(28783, 0, 259268407, 0);
        CubieCube u4 = new CubieCube(15138, 0, 119765538, 7);
        CubieCube lr2 = new CubieCube(5167, 0, 83473207, 0);
        for (int i = 0; i < 8; i++) {
            lr2.ca[i] |= 3 << 3;
        }

        for (int i = 0; i < 16; i++) {
            CubeSym[i] = new CubieCube(c);
            CornMultFull(c, u4, d);
            EdgeMult(c, u4, d);
            t = d;  d = c;  c = t;
            if (i % 4 == 3) {
                CornMultFull(c, lr2, d);
                EdgeMult(c, lr2, d);
                t = d;  d = c;  c = t;
            }
            if (i % 8 == 7) {
                CornMultFull(c, f2, d);
                EdgeMult(c, f2, d);
                t = d;  d = c;  c = t;
            }
        }
        for (int i = 0; i < 16; i++) {
            for (int j = 0; j < 16; j++) {
                CornMultFull(CubeSym[i], CubeSym[j], c);
                for (int k = 0; k < 16; k++) {
                    if (ArrayHelper.ArrayEquals(CubeSym[k].ca, c.ca)) {
                        SymMult[i][j] = k; // SymMult[i][j] = (k ^ i ^ j ^ (0x14ab4 >> j & i << 1 & 2)));
                        SymMultInv[k][j] = i; // i * j = k => k * j^-1 = i
                        break;
                    }
                }
            }
        }
        for (int j = 0; j < 18; j++) {
            for (int s = 0; s < 16; s++) {
                CornConjugate(moveCube[j], SymMultInv[0][s], c);
                for (int m = 0; m < 18; m++) {
                    if (ArrayHelper.ArrayEquals(moveCube[m].ca, c.ca)) {
                        SymMove[s][j] = m;
                        SymMoveUD[s][Util.std2ud[j]] = Util.std2ud[m];
                        break;
                    }
                }
                if (s % 2 == 0) {
                    Sym8Move[j << 3 | s >> 1] = SymMove[s][j];
                }
            }
        }

        for (int i = 0; i < 18; i++) {
            moveCubeSym[i] = moveCube[i].selfSymmetry();
            int j = i;
            for (int s = 0; s < 48; s++) {
                if (SymMove[s % 16][j] < i) {
                    firstMoveSym[s] |= 1 << i;
                }
                if (s % 16 == 15) {
                    j = urfMove[2][j];
                }
            }
        }
    }

    public static int initSym2Raw(int N_RAW, char[] Sym2Raw, char[] Raw2Sym, char[] SymState, int coord) {
        int N_RAW_HALF = (N_RAW + 1) / 2;
        CubieCube c = new CubieCube();
        CubieCube d = new CubieCube();
        int count = 0, idx = 0;
        int sym_inc = coord >= 2 ? 1 : 2;
        bool isEdge = coord != 1;

        for (int i = 0; i < N_RAW; i++) {
            if (Raw2Sym[i] != 0) {
                continue;
            }
            switch (coord) {
            case 0: c.setFlip(i); break;
            case 1: c.setTwist(i); break;
            case 2: c.setEPerm(i); break;
            }
            for (int s = 0; s < 16; s += sym_inc) {
                if (isEdge) {
                    EdgeConjugate(c, s, d);
                } else {
                    CornConjugate(c, s, d);
                }
                switch (coord) {
                case 0: idx = d.getFlip();
                    break;
                case 1: idx = d.getTwist();
                    break;
                case 2: idx = d.getEPerm();
                    break;
                }
                if (coord == 0 && Search.USE_TWIST_FLIP_PRUN) {
                    FlipS2RF[count << 3 | s >> 1] = (char) idx;
                }
                if (idx == i) {
                    SymState[count] = (char)(SymState[count] | (1 << (s / sym_inc)));
                }
                int symIdx = (count << 4 | s) / sym_inc;
                Raw2Sym[idx] = (char) symIdx;
            }
            Sym2Raw[count++] = (char) i;
        }
        return count;
    }

    public static void initFlipSym2Raw() {
        initSym2Raw(CoordCube.N_FLIP, FlipS2R, FlipR2S,
                    SymStateFlip = new char[CoordCube.N_FLIP_SYM], 0);
    }

    public static void initTwistSym2Raw() {
        initSym2Raw(CoordCube.N_TWIST, TwistS2R, TwistR2S,
                    SymStateTwist = new char[CoordCube.N_TWIST_SYM], 1);
    }

    public static void initPermSym2Raw() {
        initSym2Raw(CoordCube.N_PERM, EPermS2R, EPermR2S,
                    SymStatePerm = new char[CoordCube.N_PERM_SYM], 2);
        CubieCube cc = new CubieCube();
        for (int i = 0; i < CoordCube.N_PERM_SYM; i++) {
            cc.setEPerm(EPermS2R[i]);
            Perm2CombP[i] = (byte) (Util.getComb(cc.ea, 0, true) + (Search.USE_COMBP_PRUN ? Util.getNParity(EPermS2R[i], 8) * 70 : 0));
            cc.invCubieCube();
            PermInvEdgeSym[i] = (char) cc.getEPermSym();
        }
        for (int i = 0; i < CoordCube.N_MPERM; i++) {
            cc.setMPerm(i);
            cc.invCubieCube();
            MPermInv[i] = (byte) cc.getMPerm();
        }
    }

    static CubieCube() {
        CubieCube.initMove();
        CubieCube.initSym();
    }
}

// ========================================================


public class CoordCube {
    public static int N_MOVES = 18;
    public static int N_MOVES2 = 10;

    public static int N_SLICE = 495;
    public static int N_TWIST = 2187;
    public static int N_TWIST_SYM = 324;
    public static int N_FLIP = 2048;
    public static int N_FLIP_SYM = 336;
    public static int N_PERM = 40320;
    public static int N_PERM_SYM = 2768;
    public static int N_MPERM = 24;
    public static int N_COMB = Search.USE_COMBP_PRUN ? 140 : 70;
    public static int P2_PARITY_MOVE = Search.USE_COMBP_PRUN ? 0xA5 : 0;

    //XMove = Move Table
    //XPrun = Pruning Table
    //XConj = Conjugate Table

    //phase1
    public static char[][] UDSliceMove = ArrayHelper.CreateJagged<char>(N_SLICE, N_MOVES);
    public static char[][] TwistMove = ArrayHelper.CreateJagged<char>(N_TWIST_SYM, N_MOVES);
    public static char[][] FlipMove = ArrayHelper.CreateJagged<char>(N_FLIP_SYM, N_MOVES);
    public static char[][] UDSliceConj = ArrayHelper.CreateJagged<char>(N_SLICE, 8);
    public static int[] UDSliceTwistPrun = new int[N_SLICE * N_TWIST_SYM / 8 + 1];
    public static int[] UDSliceFlipPrun = new int[N_SLICE * N_FLIP_SYM / 8 + 1];
    public static int[] TwistFlipPrun = Search.USE_TWIST_FLIP_PRUN ? new int[N_FLIP * N_TWIST_SYM / 8 + 1] : null;

    //phase2
    public static char[][] CPermMove = ArrayHelper.CreateJagged<char>(N_PERM_SYM, N_MOVES2);
    public static char[][] EPermMove = ArrayHelper.CreateJagged<char>(N_PERM_SYM, N_MOVES2);
    public static char[][] MPermMove = ArrayHelper.CreateJagged<char>(N_MPERM, N_MOVES2);
    public static char[][] MPermConj = ArrayHelper.CreateJagged<char>(N_MPERM, 16);
    public static char[][] CCombPMove;// = ArrayHelper.CreateJagged<char>(N_COMB, N_MOVES2);
    public static char[][] CCombPConj = ArrayHelper.CreateJagged<char>(N_COMB, 16);
    public static int[] MCPermPrun = new int[N_MPERM * N_PERM_SYM / 8 + 1];
    public static int[] EPermCCombPPrun = new int[N_COMB * N_PERM_SYM / 8 + 1];

    /**
     *  0: not initialized, 1: partially initialized, 2: finished
     */
    public static int initLevel = 0;

    public static void init(bool fullInit) {
        if (initLevel == 2 || initLevel == 1 && !fullInit) {
            return;
        }
        if (initLevel == 0) {
            CubieCube.initPermSym2Raw();
            initCPermMove();
            initEPermMove();
            initMPermMoveConj();
            initCombPMoveConj();

            CubieCube.initFlipSym2Raw();
            CubieCube.initTwistSym2Raw();
            initFlipMove();
            initTwistMove();
            initUDSliceMoveConj();
        }
        initMCPermPrun(fullInit);
        initPermCombPPrun(fullInit);
        initSliceTwistPrun(fullInit);
        initSliceFlipPrun(fullInit);
        if (Search.USE_TWIST_FLIP_PRUN) {
            initTwistFlipPrun(fullInit);
        }
        initLevel = fullInit ? 2 : 1;
    }

    public static void setPruning(int[] table, int index, int value) {
        table[index >> 3] ^= value << (index << 2); // index << 2 <=> (index & 7) << 2
    }

    public static int getPruning(int[] table, int index) {
        return table[index >> 3] >> (index << 2) & 0xf; // index << 2 <=> (index & 7) << 2
    }

    public static void initUDSliceMoveConj() {
        CubieCube c = new CubieCube();
        CubieCube d = new CubieCube();
        for (int i = 0; i < N_SLICE; i++) {
            c.setUDSlice(i);
            for (int j = 0; j < N_MOVES; j += 3) {
                CubieCube.EdgeMult(c, CubieCube.moveCube[j], d);
                UDSliceMove[i][j] = (char) d.getUDSlice();
            }
            for (int j = 0; j < 16; j += 2) {
                CubieCube.EdgeConjugate(c, CubieCube.SymMultInv[0][j], d);
                UDSliceConj[i][j >> 1] = (char) d.getUDSlice();
            }
        }
        for (int i = 0; i < N_SLICE; i++) {
            for (int j = 0; j < N_MOVES; j += 3) {
                int udslice = UDSliceMove[i][j];
                for (int k = 1; k < 3; k++) {
                    udslice = UDSliceMove[udslice][j];
                    UDSliceMove[i][j + k] = (char) udslice;
                }
            }
        }
    }

    public static void initFlipMove() {
        CubieCube c = new CubieCube();
        CubieCube d = new CubieCube();
        for (int i = 0; i < N_FLIP_SYM; i++) {
            c.setFlip(CubieCube.FlipS2R[i]);
            for (int j = 0; j < N_MOVES; j++) {
                CubieCube.EdgeMult(c, CubieCube.moveCube[j], d);
                FlipMove[i][j] = (char) d.getFlipSym();
            }
        }
    }

    public static void initTwistMove() {
        CubieCube c = new CubieCube();
        CubieCube d = new CubieCube();
        for (int i = 0; i < N_TWIST_SYM; i++) {
            c.setTwist(CubieCube.TwistS2R[i]);
            for (int j = 0; j < N_MOVES; j++) {
                CubieCube.CornMult(c, CubieCube.moveCube[j], d);
                TwistMove[i][j] = (char) d.getTwistSym();
            }
        }
    }

    public static void initCPermMove() {
        CubieCube c = new CubieCube();
        CubieCube d = new CubieCube();
        for (int i = 0; i < N_PERM_SYM; i++) {
            c.setCPerm(CubieCube.EPermS2R[i]);
            for (int j = 0; j < N_MOVES2; j++) {
                CubieCube.CornMult(c, CubieCube.moveCube[Util.ud2std[j]], d);
                CPermMove[i][j] = (char) d.getCPermSym();
            }
        }
    }

    public static void initEPermMove() {
        CubieCube c = new CubieCube();
        CubieCube d = new CubieCube();
        for (int i = 0; i < N_PERM_SYM; i++) {
            c.setEPerm(CubieCube.EPermS2R[i]);
            for (int j = 0; j < N_MOVES2; j++) {
                CubieCube.EdgeMult(c, CubieCube.moveCube[Util.ud2std[j]], d);
                EPermMove[i][j] = (char) d.getEPermSym();
            }
        }
    }

    public static void initMPermMoveConj() {
        CubieCube c = new CubieCube();
        CubieCube d = new CubieCube();
        for (int i = 0; i < N_MPERM; i++) {
            c.setMPerm(i);
            for (int j = 0; j < N_MOVES2; j++) {
                CubieCube.EdgeMult(c, CubieCube.moveCube[Util.ud2std[j]], d);
                MPermMove[i][j] = (char) d.getMPerm();
            }
            for (int j = 0; j < 16; j++) {
                CubieCube.EdgeConjugate(c, CubieCube.SymMultInv[0][j], d);
                MPermConj[i][j] = (char) d.getMPerm();
            }
        }
    }

    public static void initCombPMoveConj() {
        CubieCube c = new CubieCube();
        CubieCube d = new CubieCube();
        CCombPMove = ArrayHelper.CreateJagged<char>(N_COMB, N_MOVES2);
        for (int i = 0; i < N_COMB; i++) {
            c.setCComb(i % 70);
            for (int j = 0; j < N_MOVES2; j++) {
                CubieCube.CornMult(c, CubieCube.moveCube[Util.ud2std[j]], d);
                CCombPMove[i][j] = (char) (d.getCComb() + 70 * ((P2_PARITY_MOVE >> j & 1) ^ (i / 70)));
            }
            for (int j = 0; j < 16; j++) {
                CubieCube.CornConjugate(c, CubieCube.SymMultInv[0][j], d);
                CCombPConj[i][j] = (char) (d.getCComb() + 70 * (i / 70));
            }
        }
    }

    public static bool hasZero(int val) {
        return ((val - 0x11111111) & ~val & 0x88888888) != 0;
    }

    //          |   4 bits  |   4 bits  |   4 bits  |  2 bits | 1b |  1b |   4 bits  |
    //PrunFlag: | MIN_DEPTH | MAX_DEPTH | INV_DEPTH | Padding | P2 | E2C | SYM_SHIFT |
    public static void initRawSymPrun(int[] PrunTable,
                               char[][] RawMove, char[][] RawConj,
                               char[][] SymMove, char[] SymState,
                               int PrunFlag, bool fullInit) {

        int SYM_SHIFT = PrunFlag & 0xf;
        int SYM_E2C_MAGIC = ((PrunFlag >> 4) & 1) == 1 ? CubieCube.SYM_E2C_MAGIC : 0x00000000;
        bool IS_PHASE2 = ((PrunFlag >> 5) & 1) == 1;
        int INV_DEPTH = PrunFlag >> 8 & 0xf;
        int MAX_DEPTH = PrunFlag >> 12 & 0xf;
        int MIN_DEPTH = PrunFlag >> 16 & 0xf;
        int SEARCH_DEPTH = fullInit ? MAX_DEPTH : MIN_DEPTH;

        int SYM_MASK = (1 << SYM_SHIFT) - 1;
        bool ISTFP = RawMove == null;
        int N_RAW = ISTFP ? N_FLIP : RawMove.Length;
        int N_SIZE = N_RAW * SymMove.Length;
        int N_MOVES = IS_PHASE2 ? 10 : 18;
        int NEXT_AXIS_MAGIC = N_MOVES == 10 ? 0x42 : 0x92492;

        int depth = getPruning(PrunTable, N_SIZE) - 1;
        int done = 0;

        // long tt = System.nanoTime();

        if (depth == -1) {
            for (int i = 0; i < N_SIZE / 8 + 1; i++) {
                PrunTable[i] = 0x11111111;
            }
            setPruning(PrunTable, 0, 0 ^ 1);
            depth = 0;
            done = 1;
        }

        while (depth < SEARCH_DEPTH) {
            int mask = (depth + 1) * 0x11111111 ^ unchecked((int)0xffffffff);
            for (int i = 0; i < PrunTable.Length; i++) {
                int vPrun = PrunTable[i] ^ mask;
                vPrun &= vPrun >> 1;
                PrunTable[i] += vPrun & (vPrun >> 2) & 0x11111111;
            }

            bool inv = depth > INV_DEPTH;
            int select = inv ? (depth + 2) : depth;
            int selArrMask = select * 0x11111111;
            int check = inv ? depth : (depth + 2);
            depth++;
            int xorVal = depth ^ (depth + 1);
            int val = 0;
            for (int i = 0; i < N_SIZE; i++, val >>= 4) {
                if ((i & 7) == 0) {
                    val = PrunTable[i >> 3];
                    if (!hasZero(val ^ selArrMask)) {
                        i += 7;
                        continue;
                    }
                }
                if ((val & 0xf) != select) {
                    continue;
                }
                int raw = i % N_RAW;
                int sym = i / N_RAW;
                int flip = 0, fsym = 0;
                if (ISTFP) {
                    flip = CubieCube.FlipR2S[raw];
                    fsym = flip & 7;
                    flip >>= 3;
                }

                for (int m = 0; m < N_MOVES; m++) {
                    int symx = SymMove[sym][m];
                    int rawx;
                    if (ISTFP) {
                        rawx = CubieCube.FlipS2RF[
                                   FlipMove[flip][CubieCube.Sym8Move[m << 3 | fsym]] ^
                                   fsym ^ (symx & SYM_MASK)];
                    } else {
                        rawx = RawConj[RawMove[raw][m]][symx & SYM_MASK];

                    }
                    symx >>= SYM_SHIFT;
                    int idx = symx * N_RAW + rawx;
                    int prun = getPruning(PrunTable, idx);
                    if (prun != check) {
                        if (prun < depth - 1) {
                            m += NEXT_AXIS_MAGIC >> m & 3;
                        }
                        continue;
                    }
                    done++;
                    if (inv) {
                        setPruning(PrunTable, i, xorVal);
                        break;
                    }
                    setPruning(PrunTable, idx, xorVal);
                    for (int j = 1, symState = SymState[symx]; (symState >>= 1) != 0; j++) {
                        if ((symState & 1) != 1) {
                            continue;
                        }
                        int idxx = symx * N_RAW;
                        if (ISTFP) {
                            idxx += CubieCube.FlipS2RF[CubieCube.FlipR2S[rawx] ^ j];
                        } else {
                            idxx += RawConj[rawx][j ^ (SYM_E2C_MAGIC >> (j << 1) & 3)];
                        }
                        if (getPruning(PrunTable, idxx) == check) {
                            setPruning(PrunTable, idxx, xorVal);
                            done++;
                        }
                    }
                }
            }
            // System.out.println(string.format("%2d%10d%10f", depth, done, (System.nanoTime() - tt) / 1e6d));
        }
    }

    public static void initTwistFlipPrun(bool fullInit) {
        initRawSymPrun(
            TwistFlipPrun,
            null, null,
            TwistMove, CubieCube.SymStateTwist, 0x19603,
            fullInit
        );
    }

    public static void initSliceTwistPrun(bool fullInit) {
        initRawSymPrun(
            UDSliceTwistPrun,
            UDSliceMove, UDSliceConj,
            TwistMove, CubieCube.SymStateTwist, 0x69603,
            fullInit
        );
    }

    public static void initSliceFlipPrun(bool fullInit) {
        initRawSymPrun(
            UDSliceFlipPrun,
            UDSliceMove, UDSliceConj,
            FlipMove, CubieCube.SymStateFlip, 0x69603,
            fullInit
        );
    }

    public static void initMCPermPrun(bool fullInit) {
        initRawSymPrun(
            MCPermPrun,
            MPermMove, MPermConj,
            CPermMove, CubieCube.SymStatePerm, 0x8ea34,
            fullInit
        );
    }

    public static void initPermCombPPrun(bool fullInit) {
        initRawSymPrun(
            EPermCCombPPrun,
            CCombPMove, CCombPConj,
            EPermMove, CubieCube.SymStatePerm, 0x7d824,
            fullInit
        );
    }


    int twist;
    int tsym;
    int flip;
    int fsym;
    int slice;
    public int prun;

    int twistc;
    int flipc;

    public CoordCube() { }

    public void set(CoordCube node) {
        this.twist = node.twist;
        this.tsym = node.tsym;
        this.flip = node.flip;
        this.fsym = node.fsym;
        this.slice = node.slice;
        this.prun = node.prun;

        if (Search.USE_CONJ_PRUN) {
            this.twistc = node.twistc;
            this.flipc = node.flipc;
        }
    }

    public void calcPruning(bool isPhase1) {
        prun = Math.Max(
                   Math.Max(
                       getPruning(UDSliceTwistPrun,
                                  twist * N_SLICE + UDSliceConj[slice][tsym]),
                       getPruning(UDSliceFlipPrun,
                                  flip * N_SLICE + UDSliceConj[slice][fsym])),
                   Math.Max(
                       Search.USE_CONJ_PRUN ? getPruning(TwistFlipPrun,
                               (twistc >> 3) << 11 | CubieCube.FlipS2RF[flipc ^ (twistc & 7)]) : 0,
                       Search.USE_TWIST_FLIP_PRUN ? getPruning(TwistFlipPrun,
                               twist << 11 | CubieCube.FlipS2RF[flip << 3 | (fsym ^ tsym)]) : 0));
    }

    public bool setWithPrun(CubieCube cc, int depth) {
        twist = cc.getTwistSym();
        flip = cc.getFlipSym();
        tsym = twist & 7;
        twist = twist >> 3;

        prun = Search.USE_TWIST_FLIP_PRUN ? getPruning(TwistFlipPrun,
                twist << 11 | CubieCube.FlipS2RF[flip ^ tsym]) : 0;
        if (prun > depth) {
            return false;
        }

        fsym = flip & 7;
        flip = flip >> 3;

        slice = cc.getUDSlice();
        prun = Math.Max(prun, Math.Max(
                            getPruning(UDSliceTwistPrun,
                                       twist * N_SLICE + UDSliceConj[slice][tsym]),
                            getPruning(UDSliceFlipPrun,
                                       flip * N_SLICE + UDSliceConj[slice][fsym])));
        if (prun > depth) {
            return false;
        }

        if (Search.USE_CONJ_PRUN) {
            CubieCube pc = new CubieCube();
            CubieCube.CornConjugate(cc, 1, pc);
            CubieCube.EdgeConjugate(cc, 1, pc);
            twistc = pc.getTwistSym();
            flipc = pc.getFlipSym();
            prun = Math.Max(prun,
                            getPruning(TwistFlipPrun,
                                       (twistc >> 3) << 11 | CubieCube.FlipS2RF[flipc ^ (twistc & 7)]));
        }

        return prun <= depth;
    }

    /**
     * @return pruning value
     */
    public int doMovePrun(CoordCube cc, int m, bool isPhase1) {
        slice = UDSliceMove[cc.slice][m];

        flip = FlipMove[cc.flip][CubieCube.Sym8Move[m << 3 | cc.fsym]];
        fsym = (flip & 7) ^ cc.fsym;
        flip >>= 3;

        twist = TwistMove[cc.twist][CubieCube.Sym8Move[m << 3 | cc.tsym]];
        tsym = (twist & 7) ^ cc.tsym;
        twist >>= 3;

        prun = Math.Max(
                   Math.Max(
                       getPruning(UDSliceTwistPrun,
                                  twist * N_SLICE + UDSliceConj[slice][tsym]),
                       getPruning(UDSliceFlipPrun,
                                  flip * N_SLICE + UDSliceConj[slice][fsym])),
                   Search.USE_TWIST_FLIP_PRUN ? getPruning(TwistFlipPrun,
                           twist << 11 | CubieCube.FlipS2RF[flip << 3 | (fsym ^ tsym)]) : 0);
        return prun;
    }

    public int doMovePrunConj(CoordCube cc, int m) {
        m = CubieCube.SymMove[3][m];
        flipc = FlipMove[cc.flipc >> 3][CubieCube.Sym8Move[m << 3 | cc.flipc & 7]] ^ (cc.flipc & 7);
        twistc = TwistMove[cc.twistc >> 3][CubieCube.Sym8Move[m << 3 | cc.twistc & 7]] ^ (cc.twistc & 7);
        return getPruning(TwistFlipPrun,
                          (twistc >> 3) << 11 | CubieCube.FlipS2RF[flipc ^ (twistc & 7)]);
    }
}

// ========================================================
/**
    Copyright (C) 2015  Shuang Chen

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */


/**
 * Rubik's Cube Solver.<br>
 * A much faster and smaller implemention of Two-Phase Algorithm.<br>
 * Symmetry is used to reduce memory used.<br>
 * Total Memory used is about 1MB.<br>
 * @author Shuang Chen
 */
public class Search {

    public static bool USE_TWIST_FLIP_PRUN = true;

    //Options for research purpose.
    public static int MAX_PRE_MOVES = 20;
    public static bool TRY_INVERSE = true;
    public static bool TRY_THREE_AXES = true;

    public static bool USE_COMBP_PRUN = USE_TWIST_FLIP_PRUN;
    public static bool USE_CONJ_PRUN = USE_TWIST_FLIP_PRUN;
    public static int MIN_P1LENGTH_PRE = 7;
    public static int MAX_DEPTH2 = 12;

    public static bool inited = false;

    public int[] move = new int[31];

    public CoordCube[] nodeUD = new CoordCube[21];
    public CoordCube[] nodeRL = new CoordCube[21];
    public CoordCube[] nodeFB = new CoordCube[21];

    public long selfSym;
    public int conjMask;
    public int urfIdx;
    public int length1;
    public int depth1;
    public int maxDep2;
    public int solLen;
    public Util.Solution curSol;
    public long probe;
    public long probeMax;
    public long probeMin;
    public int verbose;
    public int valid1;
    public bool allowShorter = false;
    public CubieCube cc = new CubieCube();
    public CubieCube[] urfCubieCube = new CubieCube[6];
    public CoordCube[] urfCoordCube = new CoordCube[6];
    public CubieCube[] phase1Cubie = new CubieCube[21];

    CubieCube[] preMoveCubes = new CubieCube[MAX_PRE_MOVES + 1];
    int[] preMoves = new int[MAX_PRE_MOVES];
    int preMoveLen = 0;
    int maxPreMoves = 0;

    public bool isRec = false;

    /**
     *     Verbose_Mask determines if a " . " separates the phase1 and phase2 parts of the solver string like in F' R B R L2 F .
     *     U2 U D for example.<br>
     */
    public static int USE_SEPARATOR = 0x1;

    /**
     *     Verbose_Mask determines if the solution will be inversed to a scramble/state generator.
     */
    public static int INVERSE_SOLUTION = 0x2;

    /**
     *     Verbose_Mask determines if a tag such as "(21f)" will be appended to the curSol.
     */
    public static int APPEND_LENGTH = 0x4;

    /**
     *     Verbose_Mask determines if guaranteeing the solution to be optimal.
     */
    public static int OPTIMAL_SOLUTION = 0x8;


    public Search() {
        for (int i = 0; i < 21; i++) {
            nodeUD[i] = new CoordCube();
            nodeRL[i] = new CoordCube();
            nodeFB[i] = new CoordCube();
            phase1Cubie[i] = new CubieCube();
        }
        for (int i = 0; i < 6; i++) {
            urfCubieCube[i] = new CubieCube();
            urfCoordCube[i] = new CoordCube();
        }
        for (int i = 0; i < MAX_PRE_MOVES; i++) {
            preMoveCubes[i + 1] = new CubieCube();
        }
    }

    /**
     * Computes the solver string for a given cube.
     *
     * @param facelets
     *      is the cube definition string format.<br>
     * The names of the facelet positions of the cube:
     * <pre>
     *             |************|
     *             |*U1**U2**U3*|
     *             |************|
     *             |*U4**U5**U6*|
     *             |************|
     *             |*U7**U8**U9*|
     *             |************|
     * ************|************|************|************|
     * *L1**L2**L3*|*F1**F2**F3*|*R1**R2**R3*|*B1**B2**B3*|
     * ************|************|************|************|
     * *L4**L5**L6*|*F4**F5**F6*|*R4**R5**R6*|*B4**B5**B6*|
     * ************|************|************|************|
     * *L7**L8**L9*|*F7**F8**F9*|*R7**R8**R9*|*B7**B8**B9*|
     * ************|************|************|************|
     *             |************|
     *             |*D1**D2**D3*|
     *             |************|
     *             |*D4**D5**D6*|
     *             |************|
     *             |*D7**D8**D9*|
     *             |************|
     * </pre>
     * A cube definition string "UBL..." means for example: In position U1 we have the U-color, in position U2 we have the
     * B-color, in position U3 we have the L color etc. For example, the "super flip" state is represented as <br>
     * <pre>UBULURUFURURFRBRDRFUFLFRFDFDFDLDRDBDLULBLFLDLBUBRBLBDB</pre>
     * and the state generated by "F U' F2 D' B U R' F' L D' R' U' L U B' D2 R' F U2 D2" can be represented as <br>
     * <pre>FBLLURRFBUUFBRFDDFUULLFRDDLRFBLDRFBLUUBFLBDDBUURRBLDDR</pre>
     * You can also use {@link cs.min2phase.Tools#fromScramble(java.lang.string s)} to convert the scramble string to the
     * cube definition string.
     *
     * @param maxDepth
     *      defines the maximal allowed maneuver length. For random cubes, a maxDepth of 21 usually will return a
     *      solution in less than 0.02 seconds on average. With a maxDepth of 20 it takes about 0.1 seconds on average to find a
     *      solution, but it may take much longer for specific cubes.
     *
     * @param probeMax
     *      defines the maximum number of the probes of phase 2. If it does not return with a solution, it returns with
     *      an error code.
     *
     * @param probeMin
     *      defines the minimum number of the probes of phase 2. So, if a solution is found within given probes, the
     *      computing will continue to find shorter solution(s). Btw, if probeMin > probeMax, probeMin will be set to probeMax.
     *
     * @param verbose
     *      determins the format of the solution(s). see USE_SEPARATOR, INVERSE_SOLUTION, APPEND_LENGTH, OPTIMAL_SOLUTION
     *
     * @return The solution string or an error code:<br>
     *      Error 1: There is not exactly one facelet of each colour<br>
     *      Error 2: Not all 12 edges exist exactly once<br>
     *      Error 3: Flip error: One edge has to be flipped<br>
     *      Error 4: Not all corners exist exactly once<br>
     *      Error 5: Twist error: One corner has to be twisted<br>
     *      Error 6: Parity error: Two corners or two edges have to be exchanged<br>
     *      Error 7: No solution exists for the given maxDepth<br>
     *      Error 8: Probe limit exceeded, no solution within given probMax
     */
    public  string solution(string facelets, int maxDepth, long probeMax, long probeMin, int verbose) {
        int check = verify(facelets);
        if (check != 0) {
            return "Error " + Math.Abs(check);
        }
        this.solLen = maxDepth + 1;
        this.probe = 0;
        this.probeMax = probeMax;
        this.probeMin = Math.Min(probeMin, probeMax);
        this.verbose = verbose;
        this.curSol = null;
        this.isRec = false;

        CoordCube.init(false);
        initSearch();

        return (verbose & OPTIMAL_SOLUTION) == 0 ? search() : searchopt();
    }

    public void initSearch() {
        conjMask = (TRY_INVERSE ? 0 : 0x38) | (TRY_THREE_AXES ? 0 : 0x36);
        selfSym = cc.selfSymmetry();
        conjMask |= (selfSym >> 16 & 0xffff) != 0 ? 0x12 : 0;
        conjMask |= (selfSym >> 32 & 0xffff) != 0 ? 0x24 : 0;
        conjMask |= (selfSym >> 48 & 0xffff) != 0 ? 0x38 : 0;
        selfSym &= 0xffffffffffffL;
        maxPreMoves = conjMask > 7 ? 0 : MAX_PRE_MOVES;

        for (int i = 0; i < 6; i++) {
            urfCubieCube[i].copy(cc);
            urfCoordCube[i].setWithPrun(urfCubieCube[i], 20);
            cc.URFConjugate();
            if (i % 3 == 2) {
                cc.invCubieCube();
            }
        }
    }

    public  string next(long probeMax, long probeMin, int verbose) {
        this.probe = 0;
        this.probeMax = probeMax;
        this.probeMin = Math.Min(probeMin, probeMax);
        this.curSol = null;
        this.isRec = (this.verbose & OPTIMAL_SOLUTION) == (verbose & OPTIMAL_SOLUTION);
        this.verbose = verbose;
        return (verbose & OPTIMAL_SOLUTION) == 0 ? search() : searchopt();
    }

    public static bool isInited() {
        return inited;
    }

    public long numberOfProbes() {
        return probe;
    }

    public int length() {
        return solLen;
    }

    public  static void init() {
        CoordCube.init(true);
        inited = true;
    }

    public int verify(string facelets) {
        int count = 0x000000;
        byte[] f = new byte[54];
        try {
            string center = new string(
                new char[] {
                    facelets[Util.U5],
                    facelets[Util.R5],
                    facelets[Util.F5],
                    facelets[Util.D5],
                    facelets[Util.L5],
                    facelets[Util.B5]
                }
            );
            for (int i = 0; i < 54; i++) {
                f[i] = (byte) center.IndexOf(facelets[i]);
                if (f[i] == -1) {
                    return -1;
                }
                count += 1 << (f[i] << 2);
            }
        } catch (Exception e) {
            return -1;
        }
        if (count != 0x999999) {
            return -1;
        }
        Util.toCubieCube(f, cc);
        return cc.verify();
    }

    public int phase1PreMoves(int maxl, int lm, CubieCube cc, int ssym) {
        preMoveLen = maxPreMoves - maxl;
        if (isRec ? depth1 == length1 - preMoveLen
                : (preMoveLen == 0 || (0x36FB7 >> lm & 1) == 0)) {
            depth1 = length1 - preMoveLen;
            phase1Cubie[0] = cc;
            allowShorter = depth1 == MIN_P1LENGTH_PRE && preMoveLen != 0;

            if (nodeUD[depth1 + 1].setWithPrun(cc, depth1)
                    && phase1(nodeUD[depth1 + 1], ssym, depth1, -1) == 0) {
                return 0;
            }
        }

        if (maxl == 0 || preMoveLen + MIN_P1LENGTH_PRE >= length1) {
            return 1;
        }

        int skipMoves = CubieCube.getSkipMoves(ssym);
        if (maxl == 1 || preMoveLen + 1 + MIN_P1LENGTH_PRE >= length1) { //last pre move
            skipMoves |= 0x36FB7; // 11 0110 1111 1011 0111
        }

        lm = lm / 3 * 3;
        for (int m = 0; m < 18; m++) {
            if (m == lm || m == lm - 9 || m == lm + 9) {
                m += 2;
                continue;
            }
            if (isRec && m != preMoves[maxPreMoves - maxl] || (skipMoves & 1 << m) != 0) {
                continue;
            }
            CubieCube.CornMult(CubieCube.moveCube[m], cc, preMoveCubes[maxl]);
            CubieCube.EdgeMult(CubieCube.moveCube[m], cc, preMoveCubes[maxl]);
            preMoves[maxPreMoves - maxl] = m;
            int ret = phase1PreMoves(maxl - 1, m, preMoveCubes[maxl], ssym & (int) CubieCube.moveCubeSym[m]);
            if (ret == 0) {
                return 0;
            }
        }
        return 1;
    }

    public string search() {
        for (length1 = isRec ? length1 : 0; length1 < solLen; length1++) {
            maxDep2 = Math.Min(MAX_DEPTH2, solLen - length1 - 1);
            for (urfIdx = isRec ? urfIdx : 0; urfIdx < 6; urfIdx++) {
                if ((conjMask & 1 << urfIdx) != 0) {
                    continue;
                }
                if (phase1PreMoves(maxPreMoves, -30, urfCubieCube[urfIdx], (int) (selfSym & 0xffff)) == 0) {
                    return curSol == null ? "Error 8" : curSol.ToString();
                }
            }
        }
        return curSol == null ? "Error 7" : curSol.ToString();
    }

    /**
     * @return
     *      0: Found or Probe limit exceeded
     *      1: at least 1 + maxDep2 moves away, Try next power
     *      2: at least 2 + maxDep2 moves away, Try next axis
     */
    public int initPhase2Pre() {
        isRec = false;
        if (probe >= (curSol == null ? probeMax : probeMin)) {
            return 0;
        }
        ++probe;

        for (int i = valid1; i < depth1; i++) {
            CubieCube.CornMult(phase1Cubie[i], CubieCube.moveCube[move[i]], phase1Cubie[i + 1]);
            CubieCube.EdgeMult(phase1Cubie[i], CubieCube.moveCube[move[i]], phase1Cubie[i + 1]);
        }
        valid1 = depth1;

        int p2corn = phase1Cubie[depth1].getCPermSym();
        int p2csym = p2corn & 0xf;
        p2corn >>= 4;
        int p2edge = phase1Cubie[depth1].getEPermSym();
        int p2esym = p2edge & 0xf;
        p2edge >>= 4;
        int p2mid = phase1Cubie[depth1].getMPerm();
        int edgei = CubieCube.getPermSymInv(p2edge, p2esym, false);
        int corni = CubieCube.getPermSymInv(p2corn, p2csym, true);

        int lastMove = depth1 == 0 ? -1 : move[depth1 - 1];
        int lastPre = preMoveLen == 0 ? -1 : preMoves[preMoveLen - 1];

        int ret = 0;
        int p2switchMax = (preMoveLen == 0 ? 1 : 2) * (depth1 == 0 ? 1 : 2);
        for (int p2switch = 0, p2switchMask = (1 << p2switchMax) - 1;
                p2switch < p2switchMax; p2switch++) {
            // 0 normal; 1 lastmove; 2 lastmove + premove; 3 premove
            if ((p2switchMask >> p2switch & 1) != 0) {
                p2switchMask &= ~(1 << p2switch);
                ret = initPhase2(p2corn, p2csym, p2edge, p2esym, p2mid, edgei, corni);
                if (ret == 0 || ret > 2) {
                    break;
                } else if (ret == 2) {
                    p2switchMask &= 0x4 << p2switch; // 0->2; 1=>3; 2=>N/A
                }
            }
            if (p2switchMask == 0) {
                break;
            }
            if ((p2switch & 1) == 0 && depth1 > 0) {
                int m = Util.std2ud[lastMove / 3 * 3 + 1];
                move[depth1 - 1] = Util.ud2std[m] * 2 - move[depth1 - 1];

                p2mid = CoordCube.MPermMove[p2mid][m];
                p2corn = CoordCube.CPermMove[p2corn][CubieCube.SymMoveUD[p2csym][m]];
                p2csym = CubieCube.SymMult[p2corn & 0xf][p2csym];
                p2corn >>= 4;
                p2edge = CoordCube.EPermMove[p2edge][CubieCube.SymMoveUD[p2esym][m]];
                p2esym = CubieCube.SymMult[p2edge & 0xf][p2esym];
                p2edge >>= 4;
                corni = CubieCube.getPermSymInv(p2corn, p2csym, true);
                edgei = CubieCube.getPermSymInv(p2edge, p2esym, false);
            } else if (preMoveLen > 0) {
                int m = Util.std2ud[lastPre / 3 * 3 + 1];
                preMoves[preMoveLen - 1] = Util.ud2std[m] * 2 - preMoves[preMoveLen - 1];

                p2mid = CubieCube.MPermInv[CoordCube.MPermMove[CubieCube.MPermInv[p2mid]][m]];
                p2corn = CoordCube.CPermMove[corni >> 4][CubieCube.SymMoveUD[corni & 0xf][m]];
                corni = p2corn & ~0xf | CubieCube.SymMult[p2corn & 0xf][corni & 0xf];
                p2corn = CubieCube.getPermSymInv(corni >> 4, corni & 0xf, true);
                p2csym = p2corn & 0xf;
                p2corn >>= 4;
                p2edge = CoordCube.EPermMove[edgei >> 4][CubieCube.SymMoveUD[edgei & 0xf][m]];
                edgei = p2edge & ~0xf | CubieCube.SymMult[p2edge & 0xf][edgei & 0xf];
                p2edge = CubieCube.getPermSymInv(edgei >> 4, edgei & 0xf, false);
                p2esym = p2edge & 0xf;
                p2edge >>= 4;
            }
        }
        if (depth1 > 0) {
            move[depth1 - 1] = lastMove;
        }
        if (preMoveLen > 0) {
            preMoves[preMoveLen - 1] = lastPre;
        }
        return ret == 0 ? 0 : 2;
    }

    public int initPhase2(int p2corn, int p2csym, int p2edge, int p2esym, int p2mid, int edgei, int corni) {
        int prun = Math.Max(
                       CoordCube.getPruning(CoordCube.EPermCCombPPrun,
                                            (edgei >> 4) * CoordCube.N_COMB + CoordCube.CCombPConj[CubieCube.Perm2CombP[corni >> 4] & 0xff][CubieCube.SymMultInv[edgei & 0xf][corni & 0xf]]),
                       Math.Max(
                           CoordCube.getPruning(CoordCube.EPermCCombPPrun,
                                                p2edge * CoordCube.N_COMB + CoordCube.CCombPConj[CubieCube.Perm2CombP[p2corn] & 0xff][CubieCube.SymMultInv[p2esym][p2csym]]),
                           CoordCube.getPruning(CoordCube.MCPermPrun,
                                                p2corn * CoordCube.N_MPERM + CoordCube.MPermConj[p2mid][p2csym])));

        if (prun > maxDep2) {
            return prun - maxDep2;
        }

        int depth2;
        for (depth2 = maxDep2; depth2 >= prun; depth2--) {
            int ret = phase2(p2edge, p2esym, p2corn, p2csym, p2mid, depth2, depth1, 10);
            if (ret < 0) {
                break;
            }
            depth2 -= ret;
            solLen = 0;
            curSol = new Util.Solution();
            curSol.setArgs(verbose, urfIdx, depth1);
            for (int i = 0; i < depth1 + depth2; i++) {
                curSol.appendSolMove(move[i]);
            }
            for (int i = preMoveLen - 1; i >= 0; i--) {
                curSol.appendSolMove(preMoves[i]);
            }
            solLen = curSol.length;
        }

        if (depth2 != maxDep2) { //At least one solution has been found.
            maxDep2 = Math.Min(MAX_DEPTH2, solLen - length1 - 1);
            return probe >= probeMin ? 0 : 1;
        }
        return 1;
    }

    /**
     * @return
     *      0: Found or Probe limit exceeded
     *      1: Try Next Power
     *      2: Try Next Axis
     */
    public int phase1(CoordCube node, int ssym, int maxl, int lm) {
        if (node.prun == 0 && maxl < 5) {
            if (allowShorter || maxl == 0) {
                depth1 -= maxl;
                int ret = initPhase2Pre();
                depth1 += maxl;
                return ret;
            } else {
                return 1;
            }
        }

        int skipMoves = CubieCube.getSkipMoves(ssym);

        for (int axis = 0; axis < 18; axis += 3) {
            if (axis == lm || axis == lm - 9) {
                continue;
            }
            for (int power = 0; power < 3; power++) {
                int m = axis + power;

                if (isRec && m != move[depth1 - maxl]
                        || skipMoves != 0 && (skipMoves & 1 << m) != 0) {
                    continue;
                }

                int prun = nodeUD[maxl].doMovePrun(node, m, true);
                if (prun > maxl) {
                    break;
                } else if (prun == maxl) {
                    continue;
                }

                if (USE_CONJ_PRUN) {
                    prun = nodeUD[maxl].doMovePrunConj(node, m);
                    if (prun > maxl) {
                        break;
                    } else if (prun == maxl) {
                        continue;
                    }
                }

                move[depth1 - maxl] = m;
                valid1 = Math.Min(valid1, depth1 - maxl);
                int ret = phase1(nodeUD[maxl], ssym & (int) CubieCube.moveCubeSym[m], maxl - 1, axis);
                if (ret == 0) {
                    return 0;
                } else if (ret >= 2) {
                    break;
                }
            }
        }
        return 1;
    }

    public string searchopt() {
        int maxprun1 = 0;
        int maxprun2 = 0;
        for (int i = 0; i < 6; i++) {
            urfCoordCube[i].calcPruning(false);
            if (i < 3) {
                maxprun1 = Math.Max(maxprun1, urfCoordCube[i].prun);
            } else {
                maxprun2 = Math.Max(maxprun2, urfCoordCube[i].prun);
            }
        }
        urfIdx = maxprun2 > maxprun1 ? 3 : 0;
        phase1Cubie[0] = urfCubieCube[urfIdx];
        for (length1 = isRec ? length1 : 0; length1 < solLen; length1++) {
            CoordCube ud = urfCoordCube[0 + urfIdx];
            CoordCube rl = urfCoordCube[1 + urfIdx];
            CoordCube fb = urfCoordCube[2 + urfIdx];

            if (ud.prun <= length1 && rl.prun <= length1 && fb.prun <= length1
                    && phase1opt(ud, rl, fb, selfSym, length1, -1) == 0) {
                return curSol == null ? "Error 8" : curSol.ToString();
            }
        }
        return curSol == null ? "Error 7" : curSol.ToString();
    }

    /**
     * @return
     *      0: Found or Probe limit exceeded
     *      1: Try Next Power
     *      2: Try Next Axis
     */
    public int phase1opt(CoordCube ud, CoordCube rl, CoordCube fb, long ssym, int maxl, int lm) {
        if (ud.prun == 0 && rl.prun == 0 && fb.prun == 0 && maxl < 5) {
            maxDep2 = maxl;
            depth1 = length1 - maxl;
            return initPhase2Pre() == 0 ? 0 : 1;
        }

        int skipMoves = CubieCube.getSkipMoves(ssym);

        for (int axis = 0; axis < 18; axis += 3) {
            if (axis == lm || axis == lm - 9) {
                continue;
            }
            for (int power = 0; power < 3; power++) {
                int m = axis + power;

                if (isRec && m != move[length1 - maxl]
                        || skipMoves != 0 && (skipMoves & 1 << m) != 0) {
                    continue;
                }

                // UD Axis
                int prun_ud = Math.Max(nodeUD[maxl].doMovePrun(ud, m, false),
                                       USE_CONJ_PRUN ? nodeUD[maxl].doMovePrunConj(ud, m) : 0);
                if (prun_ud > maxl) {
                    break;
                } else if (prun_ud == maxl) {
                    continue;
                }

                // RL Axis
                m = CubieCube.urfMove[2][m];

                int prun_rl = Math.Max(nodeRL[maxl].doMovePrun(rl, m, false),
                                       USE_CONJ_PRUN ? nodeRL[maxl].doMovePrunConj(rl, m) : 0);
                if (prun_rl > maxl) {
                    break;
                } else if (prun_rl == maxl) {
                    continue;
                }

                // FB Axis
                m = CubieCube.urfMove[2][m];

                int prun_fb = Math.Max(nodeFB[maxl].doMovePrun(fb, m, false),
                                       USE_CONJ_PRUN ? nodeFB[maxl].doMovePrunConj(fb, m) : 0);
                if (prun_ud == prun_rl && prun_rl == prun_fb && prun_fb != 0) {
                    prun_fb++;
                }

                if (prun_fb > maxl) {
                    break;
                } else if (prun_fb == maxl) {
                    continue;
                }

                m = CubieCube.urfMove[2][m];

                move[length1 - maxl] = m;
                valid1 = Math.Min(valid1, length1 - maxl);
                int ret = phase1opt(nodeUD[maxl], nodeRL[maxl], nodeFB[maxl], ssym & CubieCube.moveCubeSym[m], maxl - 1, axis);
                if (ret == 0) {
                    return 0;
                }
            }
        }
        return 1;
    }

    //-1: no solution found
    // X: solution with X moves shorter than expectation. Hence, the length of the solution is  depth - X
    public int phase2(int edge, int esym, int corn, int csym, int mid, int maxl, int depth, int lm) {
        if (edge == 0 && corn == 0 && mid == 0) {
            return maxl;
        }
        int moveMask = Util.ckmv2bit[lm];
        for (int m = 0; m < 10; m++) {
            if ((moveMask >> m & 1) != 0) {
                m += 0x42 >> m & 3;
                continue;
            }
            int midx = CoordCube.MPermMove[mid][m];
            int cornx = CoordCube.CPermMove[corn][CubieCube.SymMoveUD[csym][m]];
            int csymx = CubieCube.SymMult[cornx & 0xf][csym];
            cornx >>= 4;
            int edgex = CoordCube.EPermMove[edge][CubieCube.SymMoveUD[esym][m]];
            int esymx = CubieCube.SymMult[edgex & 0xf][esym];
            edgex >>= 4;
            int edgei = CubieCube.getPermSymInv(edgex, esymx, false);
            int corni = CubieCube.getPermSymInv(cornx, csymx, true);

            int prun = CoordCube.getPruning(CoordCube.EPermCCombPPrun,
                                            (edgei >> 4) * CoordCube.N_COMB + CoordCube.CCombPConj[CubieCube.Perm2CombP[corni >> 4] & 0xff][CubieCube.SymMultInv[edgei & 0xf][corni & 0xf]]);
            if (prun > maxl + 1) {
                return maxl - prun + 1;
            } else if (prun >= maxl) {
                m += 0x42 >> m & 3 & (maxl - prun);
                continue;
            }
            prun = Math.Max(
                       CoordCube.getPruning(CoordCube.MCPermPrun,
                                            cornx * CoordCube.N_MPERM + CoordCube.MPermConj[midx][csymx]),
                       CoordCube.getPruning(CoordCube.EPermCCombPPrun,
                                            edgex * CoordCube.N_COMB + CoordCube.CCombPConj[CubieCube.Perm2CombP[cornx] & 0xff][CubieCube.SymMultInv[esymx][csymx]]));
            if (prun >= maxl) {
                m += 0x42 >> m & 3 & (maxl - prun);
                continue;
            }
            int ret = phase2(edgex, esymx, cornx, csymx, midx, maxl - 1, depth + 1, m);
            if (ret >= 0) {
                move[depth] = Util.ud2std[m];
                return ret;
            }
            if (ret < -2) {
                break;
            }
            if (ret < -1) {
                m += 0x42 >> m & 3;
            }
        }
        return -1;
    }
}


    public static class KociembaHelper
    {
        private static Search searchInstance;
        private static bool inited = false;
        private static readonly object lockObj = new object();

        public static void Init()
        {
            if (inited) return;
            lock (lockObj)
            {
                if (inited) return;
                searchInstance = new Search();
                CoordCube.init(true);
                inited = true;
            }
        }

        public static string Solve(string facelets, int maxDepth = 21, long probeMax = 100000)
        {
            if (!inited) Init();
            lock (lockObj)
            {
                return searchInstance.solution(facelets, maxDepth, probeMax, 0, 0);
            }
        }
    }
}
