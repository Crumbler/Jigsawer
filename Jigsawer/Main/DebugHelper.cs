
using Jigsawer.Exceptions;

using OpenTK.Graphics.OpenGL4;

namespace Jigsawer.Main;

public static class DebugHelper {
    public static string OpenGLDebugSourceToString(DebugSource source) => source switch {
        DebugSource.DontCare => "Don't care",
        DebugSource.DebugSourceApi => "API",
        DebugSource.DebugSourceWindowSystem => "Window system",
        DebugSource.DebugSourceShaderCompiler => "Shader compiler",
        DebugSource.DebugSourceThirdParty => "Third party",
        DebugSource.DebugSourceApplication => "Application",
        DebugSource.DebugSourceOther => "Other",
        _ => throw new UnknownDebugSourceException(source)
    };

    public static string OpenGLDebugTypeToString(DebugType type) => type switch {
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

    public static string OpenGLDebugSeverityToString(DebugSeverity severity) => severity switch {
        DebugSeverity.DontCare => "Don't care",
        DebugSeverity.DebugSeverityNotification => "Notification",
        DebugSeverity.DebugSeverityHigh => "High",
        DebugSeverity.DebugSeverityMedium => "Medium",
        DebugSeverity.DebugSeverityLow => "Low",
        _ => throw new UnknownDebugSeverityException(severity)
    };
}
