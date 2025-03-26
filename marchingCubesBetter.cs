using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class marchingCubesBetter : MonoBehaviour
{
    // Start is called before the first frame update
    public Mesh m;
    MeshFilter mf;
    public float[,,] points;
    public int[,,] verticeReference;
    int[][] triangleLookupTable = new int[256][];
    public int dimensions = 10;
    public float radius = 3f;

    public int TestPattern = 0;

    Vector3[] localTransformations = new Vector3[12]
    {
        new Vector3(2, 0, 1 ),
        new Vector3(2, 1, 2 ),
        new Vector3(2, 2, 1 ),
        new Vector3(2, 1, 0 ),
        new Vector3(0, 0, 1 ),
        new Vector3(0, 1, 2 ),
        new Vector3(0, 2, 1 ),
        new Vector3(0, 1, 0 ),
        new Vector3(1, 0, 0 ),
        new Vector3(1, 0, 2 ),
        new Vector3(1, 2, 2 ),
        new Vector3(1, 2, 0 )

    };


    void initialiseLookupTable()
    {
        triangleLookupTable[00] = new int[] { -1 };
        triangleLookupTable[01] = new int[] { 0, 3, 8, -1 };
        triangleLookupTable[02] = new int[] { 0, 9, 1, -1 };
        triangleLookupTable[03] = new int[] { 3, 8, 1, 1, 8, 9, -1 };
        triangleLookupTable[04] = new int[] { 2, 11, 3, -1 };
        triangleLookupTable[05] = new int[] { 8, 0, 11, 11, 0, 2, -1 };
        triangleLookupTable[06] = new int[] { 3, 2, 11, 1, 0, 9, -1 };
        triangleLookupTable[07] = new int[] { 11, 1, 2, 11, 9, 1, 11, 8, 9, -1 };
        triangleLookupTable[08] = new int[] { 1, 10, 2, -1 };
        triangleLookupTable[09] = new int[] { 0, 3, 8, 2, 1, 10, -1 };
        triangleLookupTable[10] = new int[] { 10, 2, 9, 9, 2, 0, -1 };
        triangleLookupTable[11] = new int[] { 8, 2, 3, 8, 10, 2, 8, 9, 10, -1 };
        triangleLookupTable[12] = new int[] { 11, 3, 10, 10, 3, 1, -1 };
        triangleLookupTable[13] = new int[] { 10, 0, 1, 10, 8, 0, 10, 11, 8, -1 };
        triangleLookupTable[14] = new int[] { 9, 3, 0, 9, 11, 3, 9, 10, 11, -1 };
        triangleLookupTable[15] = new int[] { 8, 9, 11, 11, 9, 10, -1 };
        triangleLookupTable[16] = new int[] { 4, 8, 7, -1 };
        triangleLookupTable[17] = new int[] { 7, 4, 3, 3, 4, 0, -1 };
        triangleLookupTable[18] = new int[] { 4, 8, 7, 0, 9, 1, -1 };
        triangleLookupTable[19] = new int[] { 1, 4, 9, 1, 7, 4, 1, 3, 7, -1 };
        triangleLookupTable[20] = new int[] { 8, 7, 4, 11, 3, 2, -1 };
        triangleLookupTable[21] = new int[] { 4, 11, 7, 4, 2, 11, 4, 0, 2, -1 };
        triangleLookupTable[22] = new int[] { 0, 9, 1, 8, 7, 4, 11, 3, 2, -1 };
        triangleLookupTable[23] = new int[] { 7, 4, 11, 11, 4, 2, 2, 4, 9, 2, 9, 1, -1 };
        triangleLookupTable[24] = new int[] { 4, 8, 7, 2, 1, 10, -1 };
        triangleLookupTable[25] = new int[] { 7, 4, 3, 3, 4, 0, 10, 2, 1, -1 };
        triangleLookupTable[26] = new int[] { 10, 2, 9, 9, 2, 0, 7, 4, 8, -1 };
        triangleLookupTable[27] = new int[] { 10, 2, 3, 10, 3, 4, 3, 7, 4, 9, 10, 4, -1 };
        triangleLookupTable[28] = new int[] { 1, 10, 3, 3, 10, 11, 4, 8, 7, -1 };
        triangleLookupTable[29] = new int[] { 10, 11, 1, 11, 7, 4, 1, 11, 4, 1, 4, 0, -1 };
        triangleLookupTable[30] = new int[] { 7, 4, 8, 9, 3, 0, 9, 11, 3, 9, 10, 11, -1 };
        triangleLookupTable[31] = new int[] { 7, 4, 11, 4, 9, 11, 9, 10, 11, -1 };
        triangleLookupTable[32] = new int[] { 9, 4, 5, -1 };
        triangleLookupTable[33] = new int[] { 9, 4, 5, 8, 0, 3, -1 };
        triangleLookupTable[34] = new int[] { 4, 5, 0, 0, 5, 1, -1 };
        triangleLookupTable[35] = new int[] { 5, 8, 4, 5, 3, 8, 5, 1, 3, -1 };
        triangleLookupTable[36] = new int[] { 9, 4, 5, 11, 3, 2, -1 };
        triangleLookupTable[37] = new int[] { 2, 11, 0, 0, 11, 8, 5, 9, 4, -1 };
        triangleLookupTable[38] = new int[] { 4, 5, 0, 0, 5, 1, 11, 3, 2, -1 };
        triangleLookupTable[39] = new int[] { 5, 1, 4, 1, 2, 11, 4, 1, 11, 4, 11, 8, -1 };
        triangleLookupTable[40] = new int[] { 1, 10, 2, 5, 9, 4, -1 };
        triangleLookupTable[41] = new int[] { 9, 4, 5, 0, 3, 8, 2, 1, 10, -1 };
        triangleLookupTable[42] = new int[] { 2, 5, 10, 2, 4, 5, 2, 0, 4, -1 };
        triangleLookupTable[43] = new int[] { 10, 2, 5, 5, 2, 4, 4, 2, 3, 4, 3, 8, -1 };
        triangleLookupTable[44] = new int[] { 11, 3, 10, 10, 3, 1, 4, 5, 9, -1 };
        triangleLookupTable[45] = new int[] { 4, 5, 9, 10, 0, 1, 10, 8, 0, 10, 11, 8, -1 };
        triangleLookupTable[46] = new int[] { 11, 3, 0, 11, 0, 5, 0, 4, 5, 10, 11, 5, -1 };
        triangleLookupTable[47] = new int[] { 4, 5, 8, 5, 10, 8, 10, 11, 8, -1 };
        triangleLookupTable[48] = new int[] { 8, 7, 9, 9, 7, 5, -1 };
        triangleLookupTable[49] = new int[] { 3, 9, 0, 3, 5, 9, 3, 7, 5, -1 };
        triangleLookupTable[50] = new int[] { 7, 0, 8, 7, 1, 0, 7, 5, 1, -1 };
        triangleLookupTable[51] = new int[] { 7, 5, 3, 3, 5, 1, -1 };
        triangleLookupTable[52] = new int[] { 5, 9, 7, 7, 9, 8, 2, 11, 3, -1 };
        triangleLookupTable[53] = new int[] { 2, 11, 7, 2, 7, 9, 7, 5, 9, 0, 2, 9, -1 };
        triangleLookupTable[54] = new int[] { 2, 11, 3, 7, 0, 8, 7, 1, 0, 7, 5, 1, -1 };
        triangleLookupTable[55] = new int[] { 2, 11, 1, 11, 7, 1, 7, 5, 1, -1 };
        triangleLookupTable[56] = new int[] { 8, 7, 9, 9, 7, 5, 2, 1, 10, -1 };
        triangleLookupTable[57] = new int[] { 10, 2, 1, 3, 9, 0, 3, 5, 9, 3, 7, 5, -1 };
        triangleLookupTable[58] = new int[] { 7, 5, 8, 5, 10, 2, 8, 5, 2, 8, 2, 0, -1 };
        triangleLookupTable[59] = new int[] { 10, 2, 5, 2, 3, 5, 3, 7, 5, -1 };
        triangleLookupTable[60] = new int[] { 8, 7, 5, 8, 5, 9, 11, 3, 10, 3, 1, 10, -1 };
        triangleLookupTable[61] = new int[] { 5, 11, 7, 10, 11, 5, 1, 9, 0, -1 };
        triangleLookupTable[62] = new int[] { 11, 5, 10, 7, 5, 11, 8, 3, 0, -1 };
        triangleLookupTable[63] = new int[] { 5, 11, 7, 10, 11, 5, -1 };
        triangleLookupTable[64] = new int[] { 6, 7, 11, -1 };
        triangleLookupTable[65] = new int[] { 7, 11, 6, 3, 8, 0, -1 };
        triangleLookupTable[66] = new int[] { 6, 7, 11, 0, 9, 1, -1 };
        triangleLookupTable[67] = new int[] { 9, 1, 8, 8, 1, 3, 6, 7, 11, -1 };
        triangleLookupTable[68] = new int[] { 3, 2, 7, 7, 2, 6, -1 };
        triangleLookupTable[69] = new int[] { 6, 7, 2, 2, 7, 3, 9, 1, 0, -1 };
        triangleLookupTable[70] = new int[] { 0, 7, 8, 0, 6, 7, 0, 2, 6, -1 };
        triangleLookupTable[71] = new int[] { 6, 7, 8, 6, 8, 1, 8, 9, 1, 2, 6, 1, -1 };
        triangleLookupTable[72] = new int[] { 11, 6, 7, 10, 2, 1, -1 };
        triangleLookupTable[73] = new int[] { 3, 8, 0, 11, 6, 7, 10, 2, 1, -1 };
        triangleLookupTable[74] = new int[] { 0, 9, 2, 2, 9, 10, 7, 11, 6, -1 };
        triangleLookupTable[75] = new int[] { 6, 7, 11, 8, 2, 3, 8, 10, 2, 8, 9, 10, -1 };
        triangleLookupTable[76] = new int[] { 7, 10, 6, 7, 1, 10, 7, 3, 1, -1 };
        triangleLookupTable[77] = new int[] { 8, 0, 7, 7, 0, 6, 6, 0, 1, 6, 1, 10, -1 };
        triangleLookupTable[78] = new int[] { 7, 3, 6, 3, 0, 9, 6, 3, 9, 6, 9, 10, -1 };
        triangleLookupTable[79] = new int[] { 6, 7, 10, 7, 8, 10, 8, 9, 10, -1 };
        triangleLookupTable[80] = new int[] { 11, 6, 8, 8, 6, 4, -1 };
        triangleLookupTable[81] = new int[] { 6, 3, 11, 6, 0, 3, 6, 4, 0, -1 };
        triangleLookupTable[82] = new int[] { 11, 6, 8, 8, 6, 4, 1, 0, 9, -1 };
        triangleLookupTable[83] = new int[] { 1, 3, 9, 3, 11, 6, 9, 3, 6, 9, 6, 4, -1 };
        triangleLookupTable[84] = new int[] { 2, 8, 3, 2, 4, 8, 2, 6, 4, -1 };
        triangleLookupTable[85] = new int[] { 4, 0, 6, 6, 0, 2, -1 };
        triangleLookupTable[86] = new int[] { 9, 1, 0, 2, 8, 3, 2, 4, 8, 2, 6, 4, -1 };
        triangleLookupTable[87] = new int[] { 9, 1, 4, 1, 2, 4, 2, 6, 4, -1 };
        triangleLookupTable[88] = new int[] { 4, 8, 6, 6, 8, 11, 1, 10, 2, -1 };
        triangleLookupTable[89] = new int[] { 1, 10, 2, 6, 3, 11, 6, 0, 3, 6, 4, 0, -1 };
        triangleLookupTable[90] = new int[] { 11, 6, 4, 11, 4, 8, 10, 2, 9, 2, 0, 9, -1 };
        triangleLookupTable[91] = new int[] { 10, 4, 9, 6, 4, 10, 11, 2, 3, -1 };
        triangleLookupTable[92] = new int[] { 4, 8, 3, 4, 3, 10, 3, 1, 10, 6, 4, 10, -1 };
        triangleLookupTable[93] = new int[] { 1, 10, 0, 10, 6, 0, 6, 4, 0, -1 };
        triangleLookupTable[94] = new int[] { 4, 10, 6, 9, 10, 4, 0, 8, 3, -1 };
        triangleLookupTable[95] = new int[] { 4, 10, 6, 9, 10, 4, -1 };
        triangleLookupTable[96] = new int[] { 6, 7, 11, 4, 5, 9, -1 };
        triangleLookupTable[97] = new int[] { 4, 5, 9, 7, 11, 6, 3, 8, 0, -1 };
        triangleLookupTable[98] = new int[] { 1, 0, 5, 5, 0, 4, 11, 6, 7, -1 };
        triangleLookupTable[99] = new int[] { 11, 6, 7, 5, 8, 4, 5, 3, 8, 5, 1, 3, -1 };
        triangleLookupTable[100] = new int[] { 3, 2, 7, 7, 2, 6, 9, 4, 5, -1 };
        triangleLookupTable[101] = new int[] { 5, 9, 4, 0, 7, 8, 0, 6, 7, 0, 2, 6, -1 };
        triangleLookupTable[102] = new int[] { 3, 2, 6, 3, 6, 7, 1, 0, 5, 0, 4, 5, -1 };
        triangleLookupTable[103] = new int[] { 6, 1, 2, 5, 1, 6, 4, 7, 8, -1 };
        triangleLookupTable[104] = new int[] { 10, 2, 1, 6, 7, 11, 4, 5, 9, -1 };
        triangleLookupTable[105] = new int[] { 0, 3, 8, 4, 5, 9, 11, 6, 7, 10, 2, 1, -1 };
        triangleLookupTable[106] = new int[] { 7, 11, 6, 2, 5, 10, 2, 4, 5, 2, 0, 4, -1 };
        triangleLookupTable[107] = new int[] { 8, 4, 7, 5, 10, 6, 3, 11, 2, -1 };
        triangleLookupTable[108] = new int[] { 9, 4, 5, 7, 10, 6, 7, 1, 10, 7, 3, 1, -1 };
        triangleLookupTable[109] = new int[] { 10, 6, 5, 7, 8, 4, 1, 9, 0, -1 };
        triangleLookupTable[110] = new int[] { 4, 3, 0, 7, 3, 4, 6, 5, 10, -1 };
        triangleLookupTable[111] = new int[] { 10, 6, 5, 8, 4, 7, -1 };
        triangleLookupTable[112] = new int[] { 9, 6, 5, 9, 11, 6, 9, 8, 11, -1 };
        triangleLookupTable[113] = new int[] { 11, 6, 3, 3, 6, 0, 0, 6, 5, 0, 5, 9, -1 };
        triangleLookupTable[114] = new int[] { 11, 6, 5, 11, 5, 0, 5, 1, 0, 8, 11, 0, -1 };
        triangleLookupTable[115] = new int[] { 11, 6, 3, 6, 5, 3, 5, 1, 3, -1 };
        triangleLookupTable[116] = new int[] { 9, 8, 5, 8, 3, 2, 5, 8, 2, 5, 2, 6, -1 };
        triangleLookupTable[117] = new int[] { 5, 9, 6, 9, 0, 6, 0, 2, 6, -1 };
        triangleLookupTable[118] = new int[] { 1, 6, 5, 2, 6, 1, 3, 0, 8, -1 };
        triangleLookupTable[119] = new int[] { 1, 6, 5, 2, 6, 1, -1 };
        triangleLookupTable[120] = new int[] { 2, 1, 10, 9, 6, 5, 9, 11, 6, 9, 8, 11, -1 };
        triangleLookupTable[121] = new int[] { 9, 0, 1, 3, 11, 2, 5, 10, 6, -1 };
        triangleLookupTable[122] = new int[] { 11, 0, 8, 2, 0, 11, 10, 6, 5, -1 };
        triangleLookupTable[123] = new int[] { 3, 11, 2, 5, 10, 6, -1 };
        triangleLookupTable[124] = new int[] { 1, 8, 3, 9, 8, 1, 5, 10, 6, -1 };
        triangleLookupTable[125] = new int[] { 6, 5, 10, 0, 1, 9, -1 };
        triangleLookupTable[126] = new int[] { 8, 3, 0, 5, 10, 6, -1 };
        triangleLookupTable[127] = new int[] { 6, 5, 10, -1 };
        triangleLookupTable[128] = new int[] { 10, 5, 6, -1 };
        triangleLookupTable[129] = new int[] { 0, 3, 8, 6, 10, 5, -1 };
        triangleLookupTable[130] = new int[] { 10, 5, 6, 9, 1, 0, -1 };
        triangleLookupTable[131] = new int[] { 3, 8, 1, 1, 8, 9, 6, 10, 5, -1 };
        triangleLookupTable[132] = new int[] { 2, 11, 3, 6, 10, 5, -1 };
        triangleLookupTable[133] = new int[] { 8, 0, 11, 11, 0, 2, 5, 6, 10, -1 };
        triangleLookupTable[134] = new int[] { 1, 0, 9, 2, 11, 3, 6, 10, 5, -1 };
        triangleLookupTable[135] = new int[] { 5, 6, 10, 11, 1, 2, 11, 9, 1, 11, 8, 9, -1 };
        triangleLookupTable[136] = new int[] { 5, 6, 1, 1, 6, 2, -1 };
        triangleLookupTable[137] = new int[] { 5, 6, 1, 1, 6, 2, 8, 0, 3, -1 };
        triangleLookupTable[138] = new int[] { 6, 9, 5, 6, 0, 9, 6, 2, 0, -1 };
        triangleLookupTable[139] = new int[] { 6, 2, 5, 2, 3, 8, 5, 2, 8, 5, 8, 9, -1 };
        triangleLookupTable[140] = new int[] { 3, 6, 11, 3, 5, 6, 3, 1, 5, -1 };
        triangleLookupTable[141] = new int[] { 8, 0, 1, 8, 1, 6, 1, 5, 6, 11, 8, 6, -1 };
        triangleLookupTable[142] = new int[] { 11, 3, 6, 6, 3, 5, 5, 3, 0, 5, 0, 9, -1 };
        triangleLookupTable[143] = new int[] { 5, 6, 9, 6, 11, 9, 11, 8, 9, -1 };
        triangleLookupTable[144] = new int[] { 5, 6, 10, 7, 4, 8, -1 };
        triangleLookupTable[145] = new int[] { 0, 3, 4, 4, 3, 7, 10, 5, 6, -1 };
        triangleLookupTable[146] = new int[] { 5, 6, 10, 4, 8, 7, 0, 9, 1, -1 };
        triangleLookupTable[147] = new int[] { 6, 10, 5, 1, 4, 9, 1, 7, 4, 1, 3, 7, -1 };
        triangleLookupTable[148] = new int[] { 7, 4, 8, 6, 10, 5, 2, 11, 3, -1 };
        triangleLookupTable[149] = new int[] { 10, 5, 6, 4, 11, 7, 4, 2, 11, 4, 0, 2, -1 };
        triangleLookupTable[150] = new int[] { 4, 8, 7, 6, 10, 5, 3, 2, 11, 1, 0, 9, -1 };
        triangleLookupTable[151] = new int[] { 1, 2, 10, 11, 7, 6, 9, 5, 4, -1 };
        triangleLookupTable[152] = new int[] { 2, 1, 6, 6, 1, 5, 8, 7, 4, -1 };
        triangleLookupTable[153] = new int[] { 0, 3, 7, 0, 7, 4, 2, 1, 6, 1, 5, 6, -1 };
        triangleLookupTable[154] = new int[] { 8, 7, 4, 6, 9, 5, 6, 0, 9, 6, 2, 0, -1 };
        triangleLookupTable[155] = new int[] { 7, 2, 3, 6, 2, 7, 5, 4, 9, -1 };
        triangleLookupTable[156] = new int[] { 4, 8, 7, 3, 6, 11, 3, 5, 6, 3, 1, 5, -1 };
        triangleLookupTable[157] = new int[] { 5, 0, 1, 4, 0, 5, 7, 6, 11, -1 };
        triangleLookupTable[158] = new int[] { 9, 5, 4, 6, 11, 7, 0, 8, 3, -1 };
        triangleLookupTable[159] = new int[] { 11, 7, 6, 9, 5, 4, -1 };
        triangleLookupTable[160] = new int[] { 6, 10, 4, 4, 10, 9, -1 };
        triangleLookupTable[161] = new int[] { 6, 10, 4, 4, 10, 9, 3, 8, 0, -1 };
        triangleLookupTable[162] = new int[] { 0, 10, 1, 0, 6, 10, 0, 4, 6, -1 };
        triangleLookupTable[163] = new int[] { 6, 10, 1, 6, 1, 8, 1, 3, 8, 4, 6, 8, -1 };
        triangleLookupTable[164] = new int[] { 9, 4, 10, 10, 4, 6, 3, 2, 11, -1 };
        triangleLookupTable[165] = new int[] { 2, 11, 8, 2, 8, 0, 6, 10, 4, 10, 9, 4, -1 };
        triangleLookupTable[166] = new int[] { 11, 3, 2, 0, 10, 1, 0, 6, 10, 0, 4, 6, -1 };
        triangleLookupTable[167] = new int[] { 6, 8, 4, 11, 8, 6, 2, 10, 1, -1 };
        triangleLookupTable[168] = new int[] { 4, 1, 9, 4, 2, 1, 4, 6, 2, -1 };
        triangleLookupTable[169] = new int[] { 3, 8, 0, 4, 1, 9, 4, 2, 1, 4, 6, 2, -1 };
        triangleLookupTable[170] = new int[] { 6, 2, 4, 4, 2, 0, -1 };
        triangleLookupTable[171] = new int[] { 3, 8, 2, 8, 4, 2, 4, 6, 2, -1 };
        triangleLookupTable[172] = new int[] { 4, 6, 9, 6, 11, 3, 9, 6, 3, 9, 3, 1, -1 };
        triangleLookupTable[173] = new int[] { 8, 6, 11, 4, 6, 8, 9, 0, 1, -1 };
        triangleLookupTable[174] = new int[] { 11, 3, 6, 3, 0, 6, 0, 4, 6, -1 };
        triangleLookupTable[175] = new int[] { 8, 6, 11, 4, 6, 8, -1 };
        triangleLookupTable[176] = new int[] { 10, 7, 6, 10, 8, 7, 10, 9, 8, -1 };
        triangleLookupTable[177] = new int[] { 3, 7, 0, 7, 6, 10, 0, 7, 10, 0, 10, 9, -1 };
        triangleLookupTable[178] = new int[] { 6, 10, 7, 7, 10, 8, 8, 10, 1, 8, 1, 0, -1 };
        triangleLookupTable[179] = new int[] { 6, 10, 7, 10, 1, 7, 1, 3, 7, -1 };
        triangleLookupTable[180] = new int[] { 3, 2, 11, 10, 7, 6, 10, 8, 7, 10, 9, 8, -1 };
        triangleLookupTable[181] = new int[] { 2, 9, 0, 10, 9, 2, 6, 11, 7, -1 };
        triangleLookupTable[182] = new int[] { 0, 8, 3, 7, 6, 11, 1, 2, 10, -1 };
        triangleLookupTable[183] = new int[] { 7, 6, 11, 1, 2, 10, -1 };
        triangleLookupTable[184] = new int[] { 2, 1, 9, 2, 9, 7, 9, 8, 7, 6, 2, 7, -1 };
        triangleLookupTable[185] = new int[] { 2, 7, 6, 3, 7, 2, 0, 1, 9, -1 };
        triangleLookupTable[186] = new int[] { 8, 7, 0, 7, 6, 0, 6, 2, 0, -1 };
        triangleLookupTable[187] = new int[] { 7, 2, 3, 6, 2, 7, -1 };
        triangleLookupTable[188] = new int[] { 8, 1, 9, 3, 1, 8, 11, 7, 6, -1 };
        triangleLookupTable[189] = new int[] { 11, 7, 6, 1, 9, 0, -1 };
        triangleLookupTable[190] = new int[] { 6, 11, 7, 0, 8, 3, -1 };
        triangleLookupTable[191] = new int[] { 11, 7, 6, -1 };
        triangleLookupTable[192] = new int[] { 7, 11, 5, 5, 11, 10, -1 };
        triangleLookupTable[193] = new int[] { 10, 5, 11, 11, 5, 7, 0, 3, 8, -1 };
        triangleLookupTable[194] = new int[] { 7, 11, 5, 5, 11, 10, 0, 9, 1, -1 };
        triangleLookupTable[195] = new int[] { 7, 11, 10, 7, 10, 5, 3, 8, 1, 8, 9, 1, -1 };
        triangleLookupTable[196] = new int[] { 5, 2, 10, 5, 3, 2, 5, 7, 3, -1 };
        triangleLookupTable[197] = new int[] { 5, 7, 10, 7, 8, 0, 10, 7, 0, 10, 0, 2, -1 };
        triangleLookupTable[198] = new int[] { 0, 9, 1, 5, 2, 10, 5, 3, 2, 5, 7, 3, -1 };
        triangleLookupTable[199] = new int[] { 9, 7, 8, 5, 7, 9, 10, 1, 2, -1 };
        triangleLookupTable[200] = new int[] { 1, 11, 2, 1, 7, 11, 1, 5, 7, -1 };
        triangleLookupTable[201] = new int[] { 8, 0, 3, 1, 11, 2, 1, 7, 11, 1, 5, 7, -1 };
        triangleLookupTable[202] = new int[] { 7, 11, 2, 7, 2, 9, 2, 0, 9, 5, 7, 9, -1 };
        triangleLookupTable[203] = new int[] { 7, 9, 5, 8, 9, 7, 3, 11, 2, -1 };
        triangleLookupTable[204] = new int[] { 3, 1, 7, 7, 1, 5, -1 };
        triangleLookupTable[205] = new int[] { 8, 0, 7, 0, 1, 7, 1, 5, 7, -1 };
        triangleLookupTable[206] = new int[] { 0, 9, 3, 9, 5, 3, 5, 7, 3, -1 };
        triangleLookupTable[207] = new int[] { 9, 7, 8, 5, 7, 9, -1 };
        triangleLookupTable[208] = new int[] { 8, 5, 4, 8, 10, 5, 8, 11, 10, -1 };
        triangleLookupTable[209] = new int[] { 0, 3, 11, 0, 11, 5, 11, 10, 5, 4, 0, 5, -1 };
        triangleLookupTable[210] = new int[] { 1, 0, 9, 8, 5, 4, 8, 10, 5, 8, 11, 10, -1 };
        triangleLookupTable[211] = new int[] { 10, 3, 11, 1, 3, 10, 9, 5, 4, -1 };
        triangleLookupTable[212] = new int[] { 3, 2, 8, 8, 2, 4, 4, 2, 10, 4, 10, 5, -1 };
        triangleLookupTable[213] = new int[] { 10, 5, 2, 5, 4, 2, 4, 0, 2, -1 };
        triangleLookupTable[214] = new int[] { 5, 4, 9, 8, 3, 0, 10, 1, 2, -1 };
        triangleLookupTable[215] = new int[] { 2, 10, 1, 4, 9, 5, -1 };
        triangleLookupTable[216] = new int[] { 8, 11, 4, 11, 2, 1, 4, 11, 1, 4, 1, 5, -1 };
        triangleLookupTable[217] = new int[] { 0, 5, 4, 1, 5, 0, 2, 3, 11, -1 };
        triangleLookupTable[218] = new int[] { 0, 11, 2, 8, 11, 0, 4, 9, 5, -1 };
        triangleLookupTable[219] = new int[] { 5, 4, 9, 2, 3, 11, -1 };
        triangleLookupTable[220] = new int[] { 4, 8, 5, 8, 3, 5, 3, 1, 5, -1 };
        triangleLookupTable[221] = new int[] { 0, 5, 4, 1, 5, 0, -1 };
        triangleLookupTable[222] = new int[] { 5, 4, 9, 3, 0, 8, -1 };
        triangleLookupTable[223] = new int[] { 5, 4, 9, -1 };
        triangleLookupTable[224] = new int[] { 11, 4, 7, 11, 9, 4, 11, 10, 9, -1 };
        triangleLookupTable[225] = new int[] { 0, 3, 8, 11, 4, 7, 11, 9, 4, 11, 10, 9, -1 };
        triangleLookupTable[226] = new int[] { 11, 10, 7, 10, 1, 0, 7, 10, 0, 7, 0, 4, -1 };
        triangleLookupTable[227] = new int[] { 3, 10, 1, 11, 10, 3, 7, 8, 4, -1 };
        triangleLookupTable[228] = new int[] { 3, 2, 10, 3, 10, 4, 10, 9, 4, 7, 3, 4, -1 };
        triangleLookupTable[229] = new int[] { 9, 2, 10, 0, 2, 9, 8, 4, 7, -1 };
        triangleLookupTable[230] = new int[] { 3, 4, 7, 0, 4, 3, 1, 2, 10, -1 };
        triangleLookupTable[231] = new int[] { 7, 8, 4, 10, 1, 2, -1 };
        triangleLookupTable[232] = new int[] { 7, 11, 4, 4, 11, 9, 9, 11, 2, 9, 2, 1, -1 };
        triangleLookupTable[233] = new int[] { 1, 9, 0, 4, 7, 8, 2, 3, 11, -1 };
        triangleLookupTable[234] = new int[] { 7, 11, 4, 11, 2, 4, 2, 0, 4, -1 };
        triangleLookupTable[235] = new int[] { 4, 7, 8, 2, 3, 11, -1 };
        triangleLookupTable[236] = new int[] { 9, 4, 1, 4, 7, 1, 7, 3, 1, -1 };
        triangleLookupTable[237] = new int[] { 7, 8, 4, 1, 9, 0, -1 };
        triangleLookupTable[238] = new int[] { 3, 4, 7, 0, 4, 3, -1 };
        triangleLookupTable[239] = new int[] { 7, 8, 4, -1 };
        triangleLookupTable[240] = new int[] { 11, 10, 8, 8, 10, 9, -1 };
        triangleLookupTable[241] = new int[] { 0, 3, 9, 3, 11, 9, 11, 10, 9, -1 };
        triangleLookupTable[242] = new int[] { 1, 0, 10, 0, 8, 10, 8, 11, 10, -1 };
        triangleLookupTable[243] = new int[] { 10, 3, 11, 1, 3, 10, -1 };
        triangleLookupTable[244] = new int[] { 3, 2, 8, 2, 10, 8, 10, 9, 8, -1 };
        triangleLookupTable[245] = new int[] { 9, 2, 10, 0, 2, 9, -1 };
        triangleLookupTable[246] = new int[] { 8, 3, 0, 10, 1, 2, -1 };
        triangleLookupTable[247] = new int[] { 2, 10, 1, -1 };
        triangleLookupTable[248] = new int[] { 2, 1, 11, 1, 9, 11, 9, 8, 11, -1 };
        triangleLookupTable[249] = new int[] { 11, 2, 3, 9, 0, 1, -1 };
        triangleLookupTable[250] = new int[] { 11, 0, 8, 2, 0, 11, -1 };
        triangleLookupTable[251] = new int[] { 3, 11, 2, -1 };
        triangleLookupTable[252] = new int[] { 1, 8, 3, 9, 8, 1, -1 };
        triangleLookupTable[253] = new int[] { 1, 9, 0, -1 };
        triangleLookupTable[254] = new int[] { 8, 3, 0, -1 };
        triangleLookupTable[255] = new int[] { -1 };
    }


    // Start is called before the first frame update
    void Start()
    {


        m = new Mesh();
        mf = GetComponent<MeshFilter>();
        mf.mesh = m;

        initialiseLookupTable();
        newPointList(20, 20, 20);



    }

    void newPointList(int x, int y, int z)
    {
        float Offset = Random.Range(0, 10000);

        points = new float[x, y, z];

        for (int xI = 0; xI < x; xI++)
        {
            for (int yI = 0; yI < y; yI++)
            {
                for (int zI = 0; zI < z; zI++)
                {
                    float dX = x / 2 - xI;
                    float dY = y / 2 - yI;
                    float dZ = z / 2 - zI;
                    float distance = Mathf.Sqrt(dX * dX + dY * dY + dZ * dZ);
                    if (distance >= radius)
                    {
                        points[xI, yI, zI] = 1f;
                    } else
                    {
                        points[xI, yI, zI] = 0f;
                    }
                    //Debug.Log(points[xI, yI, zI]);
                }
            }
        }

        verticeReference = new int[x * 2 - 1, y * 2 - 1, z * 2 - 1];
        /*
         *		We need to reference the edges instead of the corners. Below shows storing just corners
		 *			* ----- *
		 *			|		|
		 *			|		|
		 *			|		|
		 *			* ----- *
		 *		= 4 vertices
		 *		Storing edges as well
		 *			* --*-- *
		 *			|		|
		 *			*		*
		 *			|		|
		 *			* --*-- *
		 *		= 8 vertices
		 *		so twice as many points, - 1 because we dont store an edge for the last coordinate.
		 */
        updateMesh();
    }


    void updateMesh()
    {
        List<Vector3> verticeList = new List<Vector3>();
        List<int> triangleList = new List<int>();
        //m.vertices = new Vector3[] { };
        //m.triangles = new int[] { };

        int width = points.GetLength(0);
        int height = points.GetLength(1);
        int depth = points.GetLength(2);

        int verticeWidth = verticeReference.GetLength(0);
        int verticeHeight = verticeReference.GetLength(1);
        int verticeDepth = verticeReference.GetLength(2);
        int index = 0;
        for (int x = 0; x < verticeWidth; x++)
        {
            for (int y = 0; y < verticeHeight; y++)
            {
                for (int z = 0; z < verticeDepth; z++)
                {
                    verticeList.Add(new Vector3(x, y, z));

                    verticeReference[x, y, z] = index;

                    index++;
                } // Currently stores 27 points per square (i think), where we should only really need 12 - but optimising might be weird
            }
        }
        Vector3[] nearbyVertices = new Vector3[12];
        for (int x = 0; x < width - 1; x++)
        {
            for (int y = 0; y < height - 1; y++)
            {
                for (int z = 0; z < depth - 1; z++)
                {
                    int tX = 2 * x;
                    int tY = 2 * y;
                    int tZ = 2 * z;
                    // Need to define the point pattern
                    int pattern = 0;
                    pattern = pattern + (int)Mathf.Round(points[x + 1, y, z]);   // 00000001
                    pattern = pattern + ((int)Mathf.Round(points[x + 1, y, z + 1]) * 2);   // 00000010
                    pattern = pattern + ((int)Mathf.Round(points[x + 1, y + 1, z]) * 4);   // 00000100
                    pattern = pattern + ((int)Mathf.Round(points[x + 1, y + 1, z + 1]) * 8);   // 00001000
                    pattern = pattern + ((int)Mathf.Round(points[x, y, z]) * 16);  // 00010000
                    pattern = pattern + ((int)Mathf.Round(points[x, y, z + 1]) * 32);  // 00100000
                    pattern = pattern + ((int)Mathf.Round(points[x, y + 1, z]) * 64);  // 01000000
                    pattern = pattern + ((int)Mathf.Round(points[x, y + 1, z + 1]) * 128); // 00000100
                    //pattern = TestPattern;



                    int[] trianglePattern = triangleLookupTable[pattern];
                    System.Array.Reverse(trianglePattern);
                    
                    //Debug.Log(trianglePattern);
                    
                    foreach (int vertexReference in trianglePattern)
                    {
                        if (!(vertexReference == -1))
                        {
                            Vector3 localVertex = new Vector3(tX, tY, tZ) + localTransformations[vertexReference];

                            triangleList.Add(verticeReference[(int)localVertex.x, (int)localVertex.y, (int)localVertex.z]);

                        }
                        
                        
                    }
                    if ((trianglePattern.Length - 1) % 3 != 0)
                    {
                        Debug.Log("Bad pattern length");
                    }



                }
            }
        }

        Debug.Log(verticeList.Count);
        Debug.Log(triangleList.Max());
        

        m.vertices = verticeList.ToArray();
        m.triangles = triangleList.ToArray();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            newPointList(dimensions, dimensions, dimensions);
            updateMesh();
        }

    }
}
