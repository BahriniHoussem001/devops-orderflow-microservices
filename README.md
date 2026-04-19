# DevOps OrderFlow Microservices

Mini projet DevOps basé sur deux microservices ASP.NET Core :

- OrderService.Api
- NotificationService.Api

## Architecture

- OrderService enregistre les commandes dans PostgreSQL
- OrderService publie un message dans RabbitMQ
- NotificationService consomme le message RabbitMQ
- NotificationService enregistre la notification dans PostgreSQL

## Technologies utilisées

- ASP.NET Core Web API
- PostgreSQL
- RabbitMQ
- Docker
- Docker Compose

## Lancement local avec Docker Compose

```bash
docker compose up --build
