#!/usr/bin/env bash
export ASPNETCORE_ENVIRONMENT=Development
dotnet ef database update --project Identity.Infrastructure --startup-project Identity.WebApi --context ApplicationDbContext