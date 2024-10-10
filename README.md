# .NET 8 App with Full-Text Search using Elasticsearch

This project demonstrates a .NET 8 application that integrates with **Elasticsearch** to implement full-text search functionality. The solution is containerized using Docker and Docker Compose and includes a .NET 8 API and an Elasticsearch instance.

## Prerequisites

Before running this project, ensure you have the following installed on your machine:

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://docs.docker.com/compose/install/)

## Project Structure

- **Dockerfile**: Defines the steps to build the Docker image for the .NET 8 application.
- **docker-compose.yml**: Defines the services to run, including the .NET app and Elasticsearch.
- **YourApp.csproj**: Project file for the .NET application.

## Full-Text Search with Elasticsearch

Full-text search is implemented using Elasticsearch in this project. The app uses Elasticsearch's powerful querying capabilities to provide search functionality across multiple fields, such as product names and descriptions. Elasticsearch is used to index and search product data efficiently.

