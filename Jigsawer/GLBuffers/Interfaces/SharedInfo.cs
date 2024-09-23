
using OpenTK.Mathematics;

using System.Runtime.InteropServices;

namespace Jigsawer.GLBuffers.Interfaces;

// Size specified explicitly to account for an extra float after column2
[StructLayout(LayoutKind.Explicit, Size = 64)]
public struct SharedInfo {
    public const int TimeOffset = 0,
        TimeSize = sizeof(int);

    public const int ProjectionMatrixOffset = 16,
        ProjectionMatrixSize = sizeof(float) * 4 * 3;

    [FieldOffset(0)]
    public int time;

    [FieldOffset(16)]
    private Vector3 column0;

    [FieldOffset(16 + 16 * 1)]
    private Vector3 column1;

    [FieldOffset(16 + 16 * 2)]
    private Vector3 column2;

    public void SetProjectionMatrix(in Matrix3 mat) {
        column0 = mat.Column0;
        column1 = mat.Column1;
        column2 = mat.Column2;
    }
}