# FluentCMS Infrastructure Repositories Entity Framework SQL Server

This package implements SQL Server-specific Entity Framework repositories for FluentCMS Infrastructure.

## Key Features

- SQL Server data access

## Installation

` ` `bash

dotnet add package FluentCMS.Infrastructure.Repositories.EntityFramework.SqlServer

` ` `

## Basic Usage Example

` ` `csharp

services.AddFluentCmsDbContext(options => options.UseSqlServer(connectionString));

` ` `

## Dependencies

SQL Server EF provider

## License

MIT License

