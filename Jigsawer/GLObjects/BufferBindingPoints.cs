

using Jigsawer.Debug;
using Jigsawer.Helpers;

using OpenTK.Graphics.OpenGL4;

namespace Jigsawer.GLObjects;

public static class BufferBindingPoints {
    private static readonly int uboPointsCount = GetMaxUBOPoints();
    private static readonly byte[] uboUseCounts = new byte[uboPointsCount];
    private static readonly int[] pointToUBO = new int[uboPointsCount];

    private static int GetMaxUBOPoints() {
        GL.GetInteger(GetPName.MaxUniformBufferBindings, out int maxPoints);
        Logger.LogDebug("Max UBO binding points: " + maxPoints);
        return maxPoints;
    }

    public static int GrabUBOPoint() {
        var (index, _) = uboUseCounts.GetMinElement();

        ++uboUseCounts[index];

        return index;
    }

    public static void ReturnUBOPoint(int point) {
        --uboUseCounts[point];
    }

    public static void BindUBO(int point, int ubo) {
        if (pointToUBO[point] != ubo) {
            pointToUBO[point] = ubo;
            GL.BindBufferBase(BufferRangeTarget.UniformBuffer, point, ubo);
        }
    }

    public static void UnbindAllUBOs() {
        Span<int> span = pointToUBO;

        for (int i = 0; i < span.Length; ++i) {
            if (span[i] != 0) {
                GL.BindBufferBase(BufferRangeTarget.UniformBuffer, i, 0);
            }
        }

        span.Clear();

        Array.Clear(uboUseCounts);
    }
}
