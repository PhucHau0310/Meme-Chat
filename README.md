# MemeChat - Real-time Chat Application

A real-time chat application with support for private messaging, group chats, and file sharing built with .NET and React.

## Features

-   **Authentication** - Secure user authentication system
-   **Real-time Communication** - Using SignalR for instant messaging
-   **File Sharing** - Support for various file types:
    -   Images
    -   Videos
    -   Documents
-   **Group Chat** - Create and manage group conversations
-   **Private Chat** - One-on-one private messaging
-   **Friend System** - Add and manage friends
-   **Notifications** - Real-time notifications for new messages and friend requests

## Tech Stack

### Backend (MemeChatAPI)

-   ASP.NET Core 9.0
-   Entity Framework Core
-   SignalR
-   SQL Server
-   Docker support

### Frontend (MemeChatWeb)

-   React
-   TypeScript
-   (Additional frontend details can be added)

## Project Structure

```
├── MemeChatAPI/           # Backend API
│   ├── Controllers/       # API endpoints
│   ├── Services/         # Business logic
│   ├── Models/          # Data models
│   ├── Data/           # Database context
│   └── Hubs/           # SignalR hubs
└── MemeChatWeb/         # Frontend application
    └── src/            # Source code
```

## Getting Started

### Prerequisites

-   .NET 9.0 SDK
-   Docker (optional)
-   SQL Server

### Running the API

```bash
cd MemeChatAPI
dotnet restore
dotnet run
```

### Using Docker

```bash
docker compose up
```

## API Documentation

The API documentation is available through Swagger UI when running in development mode at:

```
https://localhost:8081/swagger
```

## Database Schema

Key entities include:

-   Users
-   Messages
-   Conversations (Private/Group)
-   Message Attachments
-   Notifications
-   Friend Relationships

## License

This project is licensed under the MIT License - see the LICENSE file for details.
