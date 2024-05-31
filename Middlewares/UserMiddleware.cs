using votesystem_csharp.Models;

class UserMiddleware
{
    public static async Task GetUser(HttpContext context, Func<Task> next)
    {
        context.Items["user"] = await User.GetUserByContext(context);
        await next.Invoke();
    }
}