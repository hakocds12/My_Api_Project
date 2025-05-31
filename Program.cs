// ... other usings ...
using UserManagementAPI.Middleware; 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddLogging();
// No extra service registration needed for AuthenticationMiddleware itself,
// as IConfiguration and ILogger are already registered.


var app = builder.Build();

// Error handling middleware (FIRST)
app.UseErrorHandler();

// --- Add your authentication middleware (NEXT) ---
// Method 1: Using the extension method
app.UseAuthenticationMiddleware();
// Method 2: Directly using UseMiddleware
// app.UseMiddleware<AuthenticationMiddleware>();
// --- End Add ---


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // app.UseSwagger();
    // app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

// Your logging middleware (currently after auth, will be moved to last in Step 5)
app.UseRequestLogging();


app.MapControllers();

app.Run();


