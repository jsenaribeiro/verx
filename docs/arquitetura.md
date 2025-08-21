# Arquitetura

## Padrões arquiteturais
- serviço RESTful
- arquitetura de microsserviço
- layers architecture (Clean Architecture)
- abordagem domain-driven design
- CQRS (apenas em nível estrutural)

## Definições arquiteturais
- Autenticacao com JWT Bearer
- ORM com EntityFramework

## Padrões de projeto
- dependency injection
- service locator
- repository + generic repository
- unitOfWork 
- service (DDD)
- entity (DDD)

## Princípios
- SOLID: SRP + OCP + LKV + ISP + DIP
- DKY: DRY + KISS + YAGNI

## Abordagens
- clean code
- domain-driven design
- behavior-driven development

## Proposições
- autenticação JWT Bearer e autorização RBAC
- CQRS em nível infraestrutural (desempenho)
- Event Sourcing (auditing historical data)
- integração com SonarQube (cobertura)

## Dependencias
- log: nlog
- dbs: SqlServer
- orm: Entity Framework
- med: MediatR

## Objetivos
- escalabilidade: CQRS (queue-ready), DI, REST (stateless), Docker, cache?
- resilência: SQL (ACID), health-check, circuit-break?
- segurança: autenticação, autorização, jwt-bearer, open-api, logs