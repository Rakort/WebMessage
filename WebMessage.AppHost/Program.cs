

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");
var postgresdb = postgres.AddDatabase("postgresdb", "postgres");

var api = builder.AddProject<Projects.WebMessage_Api>("webmessage-api")
    .WithReference(postgresdb);

builder.AddProject<Projects.WebMessage_Writer>("webmessage-writer")
    .WithReference(api);

builder.AddProject<Projects.WebMessage_Observer>("webmessage-observer")
    .WithReference(api);

builder.AddProject<Projects.WebMessage_Reader>("webmessage-reader")
    .WithReference(api);

builder.Build().Run();
