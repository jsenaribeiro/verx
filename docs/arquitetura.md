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

## Dependencias
- log: nlog
- dbs: SqlServer
- orm: Entity Framework
- med: MediatR

## Objetivos
- escalabilidade: containerização (kubernetes, docker), cache (response)
- resiliência: fallback (retry, cache), circuit-breaker, throtling
- segurança: validação, autenticação, autorização, logs, cors e exceptions
- padrões: SOA + MSA, DDD, CQRS(handlers), REST
- integracao: JSON, HTTP, CQRS (MediatR), Polly(resiliência), Redis
- atributos: escalabilidade, resiliência, segurança, desempenho

## Proposições
- CQRS em nível infraestrutural (desempenho)
- Event Sourcing (auditing historical data)
- integração com SonarQube (qualidade)

## Pendente 
- Redis 
- Load Tests
- Unit Tests
- FUnctional Tests