# Test Task (Full Stack .NET + React)
Here is a recommendation list for a test task:

- You do not have to implement everything from proposed list, it is just to give you idea about the direction. Feel free to implement any other technology, pattern, or approach that will show how good you are in ASP.NET and React
- Make sure all your code is well-structured, refactored, and ready for the presentation
- Make sure you understand and can explain each line of the code, this is very important. It is better to have less code that you can explain rather than more code that you cannot
- Whatever topic you choose, you need to understand it really well, as you will be asked technical questions about it

## Technology List
- Use React (with TypeScript) for front-end (required)
- Use ASP.NET Minimal API for endpoints
- Connect to SQL or NoSQL databases (or both)
- Libraries:
  - MediatR
  - Tailwind
  - NSwag (generate a typed TypeScript API client for React)
  - SignalR (with @microsoft/signalr client)
  - A React UI library (e.g. Fluent UI React, MUI, or shadcn/ui)

Note: A big plus is to apply Domain-Driven Design, but do this only if you perfectly understand this topic and are ready for all questions

## Front-End (React) Proposals
- React with TypeScript (required)
- State management (Redux Toolkit, Zustand, or React Query/TanStack Query for server state)
- Client-side routing (React Router)
- Forms and validation (React Hook Form, Zod/Yup)
- Typed API client generated from the backend OpenAPI spec via NSwag
- Real-time UI updates using the SignalR JavaScript client
- Component-driven development, reusable components, and a clean folder structure
- Build tooling (Vite) and a clean dev/prod build pipeline

## Architecture Approaches Proposals
- Domain-Driven Design (DDD) - bounded contexts, aggregates, value objects, domain events
- Hexagonal Architecture (Ports and Adapters) - isolating core business logic
- Event-Driven Architecture - event sourcing, event streaming, domain events

## Microservices-Related Proposals
- Advanced distributed system design
- Event-driven architecture implementation with Kafka or RabbitMQ
- Advanced API Gateway configurations (e.g., rate limiting, authentication, and routing)
- Distributed caching (Redis)
- Resilience patterns (Polly for retry/circuit breaker)
- Background jobs and scheduled tasks (Hangfire, Quartz.NET)
- Apply event sourcing
- Implement outbox pattern
- Saga pattern for managing distributed transactions
- gRPC for high-performance communication
- Custom middleware development for cross-cutting concerns (e.g., logging, authentication, request validation)
- Service discovery and load balancing with tools like Consul or Eureka
- Database versioning and migrations using EF Core Migrations or Flyway
- Feature toggles and canary releases using libraries like FeatureToggle or LaunchDarkly
- Data consistency patterns like Two-Phase Commit or eventual consistency

## Other Advanced Proposals
- Unit and Integration Testing (xUnit, NUnit, Moq, TestContainers; Jest/Vitest + React Testing Library for the front-end)
- Performance optimization (async/await patterns, response compression, query optimization; React memoization, code splitting, lazy loading)
- API versioning
- Health checks and monitoring (Application Insights, Serilog)
- Validation using FluentValidation
- Object mapping (AutoMapper, Mapster)
- Repository pattern / Unit of Work
- Security best practices (OWASP guidelines, input sanitization, SQL injection prevention)
