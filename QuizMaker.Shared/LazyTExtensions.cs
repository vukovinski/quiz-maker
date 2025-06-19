namespace QuizMaker.Shared;

public static class LazyTExtensions
{
    public static T Activate<T>(this Lazy<T> lazy)
    {
        if (lazy == null) throw new ArgumentNullException(nameof(lazy));
        return lazy.Value;
    }
}
