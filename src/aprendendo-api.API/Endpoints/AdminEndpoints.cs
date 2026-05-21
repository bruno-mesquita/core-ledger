namespace aprendendo_api.API.Endpoints;

public static class AdminEndpoints
{
    public static void MapAdminEndpoints(this WebApplication app)
    {
        app.MapGet("/admin/test", () => new { message = "Admin access confirmed." })
            .RequireAuthorization(p => p.RequireRole("Admin"));
    }
}
