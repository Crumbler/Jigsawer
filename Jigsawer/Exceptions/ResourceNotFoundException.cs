
namespace Jigsawer.Exceptions;
public sealed class ResourceNotFoundException : Exception {
    public ResourceNotFoundException(string resourceName) : 
        base($"Resource {resourceName} not found.") { }
}
