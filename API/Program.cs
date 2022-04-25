using Application.Activities;

var builder = WebApplication.CreateBuilder(args);

//add services to container
builder.Services.AddControllers(opt =>
{
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    opt.Filters.Add(new AuthorizeFilter(policy));
})
    .AddFluentValidation(config =>
{
    config.RegisterValidatorsFromAssemblyContaining<Create>();
});
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

// Configure the httprequest pipeline
var app = builder.Build();
// app.UseMiddleware<ExceptionMiddleware>();

// app.UseXContentTypeOptions();
// app.UserReferrerPolicy(opt => opt.NoReferrer());
// app.UseXXssProtection(opt => opt.NoReferrer());
// app.UseXfo(opt => opt.Deny());
// app.UseStatusCodePages(opt => opt 
//     .BlockedAllMixedContent()
//     .FormActions(sbyte => s.Self()
//     .FrameAncestors(s => s.Self()))
//     );

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPIv5 v1"));
}
// else
// {
//     app.Use(async (context, next) =>
//     {
//         context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000");
//     });
// }

// app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chat");

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

using var scope = app.Services.CreateScope();

var services = scope.ServiceProvider;

try 
{
    var context = services.GetRequiredService<DataContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    await context.Database.MigrateAsync();
    await Seed.SeedData(context, userManager);
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during migration");
}

await app.RunAsync();