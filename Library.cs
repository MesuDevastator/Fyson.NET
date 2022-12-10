namespace Fyson;

public static class Library
{
    public static void LoadLibrary(this string library)
    {
        // TODO: Implement library loading
        switch (library)
        {
            case "@math.flib":
                return;
            default:
                throw new FileNotFoundException("Library not found", library);
        }
    }
}