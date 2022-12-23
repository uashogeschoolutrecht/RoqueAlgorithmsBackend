# Fake news
Repository for a study into the spread of fake news through algorithms

## Prerequisite

## How to get started

There are a few things to configure to get started.
1. Clone the repository and make it your own.
2. Fill in the `placeholder.solutionsettings.json` and change the name to `solutionsettings.json`.
3. Let EFCore create the database
   1. Add all migrations with:  
   `dotnet ef migrations add <name of migration> --context <name of the context>` in the command line <b>or</b>   
   `add-migration <name of migration> -Context <name of the context>` in the Package Manager console
   2. Update Database with:  
   `dotnet ef database update --context <name of the context>` in the command line <b>or</b>  
   `Update-Database` in the Package Manager console
### You can run the language model on your own
To do that:
1. In `FakeNewsFrontend` run in the command line `docker build . -t <name of image>`