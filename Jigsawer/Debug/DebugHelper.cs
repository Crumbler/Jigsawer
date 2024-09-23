using Jigsawer.Exceptions;

using OpenTK.Graphics.OpenGL4;

using System.Runtime.InteropServices;

namespace Jigsawer.Debug;

public static class DebugHelper {
    private static string OpenGLDebugSourceToString(DebugSource source) => source switch {
        DebugSource.DontCare => "Don't care",
        DebugSource.DebugSourceApi => "API",
        DebugSource.DebugSourceWindowSystem => "Window system",
        DebugSource.DebugSourceShaderCompiler => "Shader compiler",
        DebugSource.DebugSourceThirdParty => "Third party",
        DebugSource.DebugSourceApplication => "Application",
        DebugSource.DebugSourceOther => "Other",
        _ => throw new UnknownDebugSourceException(source)
    };

    private static string OpenGLDebugTypeToString(DebugType type) => type switch {
        DebugType.DontCare => "Don't care",
        DebugType.DebugTypeError => "Error",
        DebugType.DebugTypeDeprecatedBehavior => "Deprecated behavior",
        DebugType.DebugTypeUndefinedBehavior => "Undefined behavior",
        DebugType.DebugTypePortability => "Portability",
        DebugType.DebugTypePerformance => "Performance",
        DebugType.DebugTypeOther => "Other",
        DebugType.DebugTypeMarker => "Marker",
        DebugType.DebugTypePushGroup => "Push group",
        DebugType.DebugTypePopGroup => "Pop group",
        _ => throw new UnknownDebugTypeException(type)
    };

    private static string OpenGLDebugSeverityToString(DebugSeverity severity) => severity switch {
        DebugSeverity.DontCare => "Don't care",
        DebugSeverity.DebugSeverityNotification => "Notification",
        DebugSeverity.DebugSeverityHigh => "High",
        DebugSeverity.DebugSeverityMedium => "Medium",
        DebugSeverity.DebugSeverityLow => "Low",
        _ => throw new UnknownDebugSeverityException(severity)
    };

    public static void InitDebugLogging() {
#if DEBUG
        GL.DebugMessageCallback(debugMessageDelegate, 0);
        GL.Enable(EnableCap.DebugOutput);
#endif
    }

#if DEBUG
    private static readonly DebugProc debugMessageDelegate = OnDebugMessage;

    /// <param name="source">Source of the debugging message.</param>
    /// <param name="type">Type of the debugging message.</param>
    /// <param name="id">ID associated with the message.</param>
    /// <param name="severity">Severity of the message.</param>
    /// <param name="length">Length of the string in pMessage</param>
    /// <param name="pMessage">Pointer to message string</param>
    /// <param name="pUserParam">User specified parameter</param>
    private static void OnDebugMessage(
        DebugSource source,
        DebugType type,
        int id,
        DebugSeverity severity,
        int length,
        nint pMessage,
        nint pUserParam) {
        string message = Marshal.PtrToStringAnsi(pMessage, length);
        string sourceString = OpenGLDebugSourceToString(source);
        string typeString = OpenGLDebugTypeToString(type);
        string severityString = OpenGLDebugSeverityToString(severity);

        Console.WriteLine($"[Severity = {severityString}, source = {sourceString}, type = {typeString}, id = {id}]\n{message}");
    }
#endif
}
