# AI Chatbot - Enterprise .NET Core Web API

A comprehensive, enterprise-grade .NET Core Web API for an AI-powered chatbot that answers common customer queries using an internal FAQ database. Built with clean architecture principles and CQRS pattern.

## 🏗️ Architecture Overview

This project follows **Clean Architecture** principles with the following layer sequence:

```
Controller ➝ Services ➝ Broker ➝ Handler (CQRS) ➝ Repository ➝ Entity
```

### Key Features

- ✅ **Clean Architecture** - Separation of concerns with clear layer boundaries
- ✅ **JWT Authentication** - Secure user registration and login
- ✅ **CQRS Pattern** - Command Query Responsibility Segregation without MediatR
- ✅ **Entity Framework Core** - Code-first approach with SQLite database
- ✅ **AI Integration** - Simulated OpenAI GPT API integration
- ✅ **FAQ Management** - Intelligent FAQ matching and response generation
- ✅ **Chat Sessions** - Persistent chat history and session management
- ✅ **Error Handling** - Centralized exception handling middleware
- ✅ **Swagger Documentation** - Complete API documentation
- ✅ **Async/Await** - Full asynchronous programming patterns
- ✅ **Dependency Injection** - Proper IoC container configuration

## 📁 Project Structure

```
AIchatbot/
├── Controllers/                 # API Controllers
│   ├── AuthController.cs       # Authentication endpoints
│   └── ChatController.cs       # Chat and FAQ endpoints
├── Models/                     # Data Models
│   ├── Entities/              # Database entities
│   │   ├── User.cs
│   │   ├── FAQ.cs
│   │   ├── ChatSession.cs
│   │   └── ChatMessage.cs
│   ├── DTOs/                  # Data Transfer Objects
│   │   ├── AuthDTOs.cs
│   │   └── ChatDTOs.cs
│   └── Responses/             # API Response models
│       └── ApiResponse.cs
├── Interfaces/                 # Contract definitions
│   ├── Services/              # Service interfaces
│   ├── Repositories/          # Repository interfaces
│   ├── Brokers/               # External service interfaces
│   └── Handlers/              # CQRS handler interfaces
├── Services/                  # Business logic services
│   ├── AuthService.cs         # Authentication logic
│   └── ChatService.cs         # Chat processing logic
├── Brokers/                   # External service integrations
│   └── AIBroker.cs           # AI service integration
├── Handlers/                  # CQRS Command/Query handlers
│   ├── Commands/             # Command handlers
│   └── Queries/              # Query handlers
├── Data/                      # Data access layer
│   ├── Context/              # Entity Framework context
│   └── Repositories/         # Repository implementations
├── Middleware/                # Custom middleware
│   └── ErrorHandlingMiddleware.cs
└── Configuration/             # Configuration files
```

## 🚀 Getting Started

### Prerequisites

- .NET 8.0 SDK
- Visual Studio 2022 or VS Code
- SQLite (included with EF Core)

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd AIchatbot
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Update configuration**
   Edit `appsettings.json` and update the JWT secret:
   ```json
   {
     "Jwt": {
       "Secret": "your-super-secret-key-with-at-least-32-characters"
     }
   }
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

5. **Access Swagger UI**
   Navigate to `https://localhost:7000` to view the API documentation.

## 🔐 Authentication

### Register a new user
```http
POST /api/auth/register
Content-Type: application/json

{
  "username": "john_doe",
  "email": "john@example.com",
  "password": "password123",
  "confirmPassword": "password123",
  "firstName": "John",
  "lastName": "Doe"
}
```

### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "john@example.com",
  "password": "password123"
}
```

### Using JWT Token
Include the JWT token in the Authorization header:
```
Authorization: Bearer <your-jwt-token>
```

## 💬 Chat API Examples

### Send a message to the chatbot
```http
POST /api/chat/send
Authorization: Bearer <your-jwt-token>
Content-Type: application/json

{
  "message": "How can I track my order?",
  "sessionId": "optional-session-id"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Operation completed successfully",
  "data": {
    "success": true,
    "message": "You can track your order by logging into your account and visiting the 'Order History' section, or by using the tracking number provided in your order confirmation email.",
    "sessionId": "session-guid",
    "intent": "order_tracking",
    "confidence": 0.85,
    "responseSource": "FAQ",
    "relatedFAQId": 1,
    "suggestions": ["Track my order", "Order status", "Delivery update"],
    "timestamp": "2024-01-15T10:30:00Z"
  },
  "timestamp": "2024-01-15T10:30:00Z"
}
```

### Get chat session history
```http
GET /api/chat/session/{sessionId}
Authorization: Bearer <your-jwt-token>
```

### Get all FAQs
```http
GET /api/chat/faqs
Authorization: Bearer <your-jwt-token>
```

### Get FAQs by category
```http
GET /api/chat/faqs?category=Order Tracking
Authorization: Bearer <your-jwt-token>
```

## 🧠 AI Integration

The chatbot uses a simulated AI broker that:

1. **Analyzes user intent** - Determines what the user is asking about
2. **Calculates confidence** - How sure the AI is about the intent
3. **Matches FAQs** - Finds relevant FAQ entries
4. **Generates responses** - Creates contextual responses
5. **Provides suggestions** - Offers related questions

### Intent Recognition

The AI recognizes these intents:
- `order_tracking` - Order status and tracking
- `return_policy` - Returns and refunds
- `delivery_time` - Shipping and delivery
- `payment_methods` - Payment options
- `order_modification` - Cancellations and changes

## 📊 Database Schema

### Users Table
- `Id` (Primary Key)
- `Username` (Unique)
- `Email` (Unique)
- `PasswordHash`
- `FirstName`
- `LastName`
- `CreatedAt`
- `LastLoginAt`
- `IsActive`

### FAQs Table
- `Id` (Primary Key)
- `Question`
- `Answer`
- `Category`
- `Tags`
- `Priority`
- `IsActive`
- `CreatedAt`
- `UpdatedAt`
- `ViewCount`

### ChatSessions Table
- `Id` (Primary Key)
- `UserId` (Foreign Key)
- `SessionId` (Unique)
- `StartedAt`
- `EndedAt`
- `IsActive`

### ChatMessages Table
- `Id` (Primary Key)
- `ChatSessionId` (Foreign Key)
- `Content`
- `IsFromUser`
- `Timestamp`
- `Intent`
- `Confidence`
- `ResponseSource`
- `RelatedFAQId` (Foreign Key)

## 🔧 CQRS Implementation

This project implements CQRS manually without MediatR:

### Commands (Write Operations)
```csharp
public class ProcessChatMessageCommand : ICommand
{
    public ChatRequest Request { get; set; }
    public int UserId { get; set; }
}

public class ProcessChatMessageCommandHandler : ICommandHandler<ProcessChatMessageCommand, ChatResponse>
{
    public async Task<ChatResponse> HandleAsync(ProcessChatMessageCommand command)
    {
        // Handle the command
    }
}
```

### Queries (Read Operations)
```csharp
public class GetFAQsQuery : IQuery<IEnumerable<FAQDto>>
{
    public string? Category { get; set; }
}

public class GetFAQsQueryHandler : IQueryHandler<GetFAQsQuery, IEnumerable<FAQDto>>
{
    public async Task<IEnumerable<FAQDto>> HandleAsync(GetFAQsQuery query)
    {
        // Handle the query
    }
}
```

## 🛡️ Security Features

- **JWT Authentication** - Secure token-based authentication
- **Password Hashing** - SHA256 password hashing
- **Input Validation** - Comprehensive request validation
- **Error Handling** - Centralized exception handling
- **CORS Configuration** - Cross-origin resource sharing
- **HTTPS Redirection** - Secure communication

## 📝 API Documentation

The API is fully documented with Swagger/OpenAPI. Access the interactive documentation at:
- **Swagger UI**: `https://localhost:7000`
- **OpenAPI JSON**: `https://localhost:7000/swagger/v1/swagger.json`

## 🧪 Testing

### Example API Tests

1. **Register a user**
2. **Login and get JWT token**
3. **Send chat messages**
4. **Retrieve chat history**
5. **Browse FAQs**

### Sample Test Data

The application includes seeded FAQ data:
- Order tracking information
- Return policy details
- Delivery time information
- Payment method options
- Order modification policies

## 🚀 Deployment

### Development
```bash
dotnet run
```

### Production
```bash
dotnet publish -c Release
dotnet run --environment Production
```

### Docker (Optional)
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["AIchatbot.csproj", "./"]
RUN dotnet restore "AIchatbot.csproj"
COPY . .
RUN dotnet build "AIchatbot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AIchatbot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AIchatbot.dll"]
```

## 📈 Performance Considerations

- **Async/Await** - All database and external service calls are asynchronous
- **Connection Pooling** - Entity Framework Core connection pooling
- **Caching** - FAQ data can be cached for better performance
- **Database Indexing** - Proper indexes on frequently queried columns
- **Response Compression** - Enable gzip compression for responses

## 🔮 Future Enhancements

- [ ] Real OpenAI API integration
- [ ] Redis caching for FAQs
- [ ] WebSocket support for real-time chat
- [ ] Multi-language support
- [ ] Advanced analytics and reporting
- [ ] Machine learning model training
- [ ] Voice integration
- [ ] Mobile app support

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🆘 Support

For support and questions:
- Create an issue in the repository
- Contact the development team
- Check the API documentation

---

**Built with ❤️ using .NET 8.0 and Clean Architecture principles** 