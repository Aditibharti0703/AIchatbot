# AI Chatbot - Architecture Documentation

## 🏗️ Architecture Overview

This enterprise-grade .NET Core Web API follows **Clean Architecture** principles with a layered approach that ensures separation of concerns, testability, and maintainability.

### Layer Sequence
```
Controller ➝ Services ➝ Broker ➝ Handler (CQRS) ➝ Repository ➝ Entity
```

## 📋 Implementation Details

### 1. **Models Layer** (Foundation)

#### Entities
- **User.cs** - User authentication and profile data
- **FAQ.cs** - Knowledge base entries with categories and tags
- **ChatSession.cs** - Chat conversation sessions
- **ChatMessage.cs** - Individual chat messages with AI analysis

#### DTOs
- **AuthDTOs.cs** - Login/Register requests and responses
- **ChatDTOs.cs** - Chat requests, responses, and session data

#### Responses
- **ApiResponse.cs** - Standardized API response wrapper

### 2. **Data Layer** (Persistence)

#### ApplicationDbContext
- Entity Framework Core configuration
- Relationship mappings
- Seed data for FAQs
- Database indexes for performance

#### Repository Pattern
- Generic repository interface
- Entity Framework implementation
- Async/await patterns throughout

### 3. **Interfaces Layer** (Contracts)

#### Service Interfaces
- **IAuthService** - Authentication operations
- **IChatService** - Chat processing and FAQ management

#### Repository Interfaces
- **IRepository<T>** - Generic data access operations

#### Broker Interfaces
- **IAIBroker** - External AI service integration

#### Handler Interfaces (CQRS)
- **ICommand** / **ICommandHandler** - Write operations
- **IQuery** / **IQueryHandler** - Read operations

### 4. **Services Layer** (Business Logic)

#### AuthService
- JWT token generation and validation
- Password hashing (SHA256)
- User registration and login
- Token-based authentication

#### ChatService
- Message processing with AI integration
- FAQ matching and response generation
- Chat session management
- Context-aware conversations

### 5. **Brokers Layer** (External Services)

#### AIBroker
- Simulated OpenAI GPT integration
- Intent recognition and analysis
- Confidence scoring
- Response generation
- FAQ relevance matching

### 6. **Handlers Layer** (CQRS Implementation)

#### Commands
- **ProcessChatMessageCommand** - Handle chat message processing
- **ProcessChatMessageCommandHandler** - Execute chat logic

#### Queries
- **GetFAQsQuery** - Retrieve FAQ data
- **GetFAQsQueryHandler** - Execute FAQ retrieval

### 7. **Controllers Layer** (API Endpoints)

#### AuthController
- User registration and login
- JWT token validation
- Password management

#### ChatController
- Chat message processing
- Session management
- FAQ browsing
- CQRS pattern implementation

### 8. **Middleware Layer** (Cross-cutting Concerns)

#### ErrorHandlingMiddleware
- Centralized exception handling
- Standardized error responses
- Logging integration

## 🔧 CQRS Implementation

### Manual CQRS (No MediatR)

The project implements CQRS manually without third-party libraries:

```csharp
// Command Pattern
public class ProcessChatMessageCommand : ICommand
{
    public ChatRequest Request { get; set; }
    public int UserId { get; set; }
}

public class ProcessChatMessageCommandHandler : ICommandHandler<ProcessChatMessageCommand, ChatResponse>
{
    public async Task<ChatResponse> HandleAsync(ProcessChatMessageCommand command)
    {
        // Business logic implementation
    }
}

// Query Pattern
public class GetFAQsQuery : IQuery<IEnumerable<FAQDto>>
{
    public string? Category { get; set; }
}

public class GetFAQsQueryHandler : IQueryHandler<GetFAQsQuery, IEnumerable<FAQDto>>
{
    public async Task<IEnumerable<FAQDto>> HandleAsync(GetFAQsQuery query)
    {
        // Data retrieval logic
    }
}
```

### Benefits of Manual CQRS
- **No External Dependencies** - Self-contained implementation
- **Full Control** - Customizable behavior
- **Performance** - Direct method calls
- **Simplicity** - Easy to understand and maintain

## 🧠 AI Integration Strategy

### Simulated AI Broker
The `AIBroker` simulates OpenAI GPT API integration:

1. **Intent Recognition** - Keyword-based intent detection
2. **Confidence Scoring** - Algorithmic confidence calculation
3. **FAQ Matching** - Semantic similarity with Jaccard algorithm
4. **Response Generation** - Context-aware response creation
5. **Suggestion Generation** - Related question suggestions

### Intent Categories
- `order_tracking` - Order status and tracking
- `return_policy` - Returns and refunds
- `delivery_time` - Shipping and delivery
- `payment_methods` - Payment options
- `order_modification` - Cancellations and changes

## 🔐 Security Implementation

### JWT Authentication
- **Token Generation** - Secure JWT token creation
- **Token Validation** - Comprehensive token verification
- **Password Hashing** - SHA256 password security
- **Authorization** - Role-based access control ready

### Security Features
- HTTPS redirection
- Input validation
- Error handling without information leakage
- Secure configuration management

## 📊 Database Design

### Entity Relationships
```
User (1) ←→ (Many) ChatSession (1) ←→ (Many) ChatMessage
FAQ (1) ←→ (Many) ChatMessage (via RelatedFAQId)
```

### Database Features
- **Code-First Approach** - Entity Framework migrations
- **Seed Data** - Pre-populated FAQ entries
- **Indexes** - Performance optimization
- **Foreign Keys** - Referential integrity

## 🚀 Performance Optimizations

### Async/Await Patterns
- All database operations are asynchronous
- External service calls are non-blocking
- Proper async method signatures

### Database Optimizations
- Connection pooling
- Efficient queries with includes
- Proper indexing strategy

### Caching Strategy
- FAQ data can be cached (future enhancement)
- Session data optimization
- Response compression ready

## 🧪 Testing Strategy

### Unit Testing Ready
- Dependency injection enables easy mocking
- Interface-based design for testability
- Separated concerns for isolated testing

### Integration Testing
- Entity Framework in-memory database
- HTTP client mocking for AI broker
- End-to-end API testing

## 📈 Scalability Considerations

### Horizontal Scaling
- Stateless API design
- Database connection pooling
- External service abstraction

### Vertical Scaling
- Async operations for better resource utilization
- Efficient memory management
- Optimized database queries

## 🔮 Future Enhancements

### Immediate Improvements
1. **Real OpenAI Integration** - Replace simulated AI with actual API
2. **Redis Caching** - Add caching layer for FAQs
3. **WebSocket Support** - Real-time chat capabilities
4. **Advanced Analytics** - Usage tracking and insights

### Long-term Features
1. **Multi-language Support** - Internationalization
2. **Voice Integration** - Speech-to-text capabilities
3. **Machine Learning** - Custom model training
4. **Mobile SDK** - Native mobile app support

## 🛠️ Development Workflow

### Code Organization
- **Feature-based Structure** - Related code grouped together
- **Interface Segregation** - Clean contract definitions
- **Dependency Inversion** - High-level modules independent of low-level details

### Best Practices
- **SOLID Principles** - Clean, maintainable code
- **DRY Principle** - No code duplication
- **KISS Principle** - Simple, understandable solutions
- **YAGNI Principle** - Only implement what's needed

## 📝 API Design Principles

### RESTful Design
- Resource-based URLs
- Proper HTTP methods
- Consistent response formats
- Comprehensive error handling

### Documentation
- Swagger/OpenAPI integration
- Detailed endpoint documentation
- Request/response examples
- Authentication instructions

## 🔍 Monitoring and Logging

### Structured Logging
- Serilog integration ready
- Request/response logging
- Error tracking and alerting
- Performance monitoring

### Health Checks
- Database connectivity
- External service availability
- Application health status
- Dependency monitoring

---

## 🎯 Key Achievements

✅ **Clean Architecture** - Proper separation of concerns
✅ **CQRS Pattern** - Manual implementation without MediatR
✅ **JWT Authentication** - Secure user management
✅ **AI Integration** - Simulated but extensible AI broker
✅ **Entity Framework** - Code-first database approach
✅ **Async/Await** - Full asynchronous programming
✅ **Error Handling** - Centralized exception management
✅ **API Documentation** - Complete Swagger integration
✅ **Dependency Injection** - Proper IoC configuration
✅ **Testing Ready** - Mockable and testable design

This architecture provides a solid foundation for an enterprise-grade chatbot API that can scale, maintain, and extend as business requirements evolve. 